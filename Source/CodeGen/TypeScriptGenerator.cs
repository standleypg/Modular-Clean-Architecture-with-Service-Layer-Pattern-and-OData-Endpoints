using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using TypeLitePlus;
using TypeLitePlus.TsModels;

namespace CodeGen;

public class TypeScriptGenerator
{
    private static TypeScriptGenConfig? _config;

    private static void LoadConfig()
    {
        string configPath = Path.Combine(AppContext.BaseDirectory, "typescriptgenconfig.json");
        if (File.Exists(configPath))
        {
            var json = File.ReadAllText(configPath);
            _config = JsonSerializer.Deserialize<TypeScriptGenConfig>(json);
            Console.WriteLine("Config loaded successfully.");
        }
        else
        {
            throw new FileNotFoundException("Config file not found.", configPath);
        }
    }

    public static void GenerateTypeScript()
    {
        LoadConfig();

        if (_config?.Namespaces == null || _config.Namespaces.Count == 0)
        {
            Console.WriteLine("No namespaces specified in config. Nothing to generate.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_config.OutputPath))
        {
            Console.WriteLine("Output path not specified in config.");
            return;
        }

        var sharedAssembly = Assembly.Load("RetailPortal.Model");

        var modelBuilder = new TsModelBuilder();

        foreach (var nsConfig in _config.Namespaces)
        {
            var g = sharedAssembly.GetTypes().ToList();
            var types = sharedAssembly.GetTypes()
                .Where(t => t.Namespace == nsConfig.Namespace && nsConfig.IncludeDtos.Any(dto =>
                    t.Name.StartsWith(dto, StringComparison.InvariantCultureIgnoreCase) ||
                    t.Name.EndsWith(dto, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            foreach (var type in types)
            {
                modelBuilder.Add(type);
            }
        }

        var model = modelBuilder.Build();
        var visitor = new Visitor();
        model.RunVisitor(visitor);

        var generator = new TsGenerator();
        generator.SetIdentifierFormatter(formatter => !string.IsNullOrEmpty(formatter.Name)
            ? char.ToLowerInvariant(formatter.Name[0]) + formatter.Name.Substring(1) // Convert to camelCase
            : formatter.Name);
        generator.SetMemberTypeFormatter((formatter, name) =>
            name == "System.iGuid" ? "string" : name
        );

        generator.SetModuleNameFormatter(formatter =>
        {
            foreach (var formatterClass in formatter.Members)
            {
                if (formatterClass.Name.StartsWith('i')) continue;
                if (formatterClass.Name == "T") continue; // enforce generic type name
                formatterClass.Name = "i" + formatterClass.Name;
            }

            return formatter.Name;
        });

        generator.SetTypeVisibilityFormatter((formatter, name) =>
        {
            if (name == "iGuid")
            {
                formatter.Module = null;
                formatter.Properties.Clear();
                formatter.Fields.Clear();
                formatter.Interfaces.Clear();
                return false;
            }

            return true;
        });

        var tsCode = generator.Generate(model, TsGeneratorOutput.Properties | TsGeneratorOutput.Enums);
        tsCode = Regex.Replace(tsCode, @"declare\s+namespace\s+System\s*{\s*interface\s+iGuid\s*{\s*}\s*}",
            string.Empty).Trim();
        tsCode = tsCode.Replace("export const enum", "export enum", StringComparison.InvariantCultureIgnoreCase);
        var baseDirectory = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;

        if (baseDirectory == null)
        {
            throw new InvalidOperationException("Failed to determine base directory.");
        }

        var outputPath = Path.GetFullPath(_config.OutputPath, baseDirectory);
        var outputDir = Path.GetDirectoryName(outputPath);
        if (outputDir != null) Directory.CreateDirectory(outputDir);

        try
        {
            if (outputDir != null)
            {
                string outputFile = Path.Combine(outputDir, _config.OutputFileName);
                File.WriteAllText(outputFile, tsCode);
                Console.WriteLine($"TypeScript models generated at {_config.OutputPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to generate TypeScript models: {ex.Message}");
        }
    }
}