using CodeGen.Configuration;
using CodeGen.Discovery;
using CodeGen.Formatters;
using CodeGen.Processing;
using CodeGen.Utilities;
using Microsoft.Extensions.Logging;
using TypeLitePlus;

namespace CodeGen;

public class TypeScriptGenerator(TypeScriptGenConfig config, ILogger<TypeScriptGenerator> logger)
{
    private readonly TypeScriptGenConfig _config = config ?? throw new ArgumentNullException(nameof(config));
    private readonly TypeDiscovery _typeDiscovery = new(config, logger);
    private readonly TypeFormatter _typeFormatter = new(logger);
    private readonly PathResolver _pathResolver = new(config);

    public void Generate()
    {
        try
        {
            logger.LogInformation("Starting TypeScript generation...");

            var assembly = this._typeDiscovery.LoadAssembly(this._config.AssemblyName);
            var types = this._typeDiscovery.DiscoverTypes(assembly);

            if (types.Count == 0)
            {
                logger.LogWarning("No types found matching the configured patterns. Nothing to generate.");
                return;
            }

            logger.LogInformation("Found {TypeCount} types to generate", types.Count);

            var model = this.BuildTypeScriptModel(types);
            var tsCode = this.GenerateTypeScriptCode(model);
            var outputPath = this.WriteOutputFile(tsCode);

            logger.LogInformation("TypeScript models successfully generated at: {OutputPath}", outputPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate TypeScript models");
            throw;
        }
    }

    /// <summary>
    /// Builds the TypeScript model from discovered types.
    /// </summary>
    private TsModel BuildTypeScriptModel(List<Type> types)
    {
        var modelBuilder = new TsModelBuilder();

        foreach (var type in types)
        {
            logger.LogDebug("Adding type to model: {TypeName}", type.FullName);
            modelBuilder.Add(type);
        }

        return modelBuilder.Build();
    }

    /// <summary>
    /// Generates TypeScript code from the model.
    /// </summary>
    private string GenerateTypeScriptCode(TsModel model)
    {
        var generator = this.ConfigureGenerator();
        var tsCode = generator.Generate(model, TsGeneratorOutput.Properties | TsGeneratorOutput.Enums);

        // Post-processing for cleanup
        return CodePostProcessor.Process(tsCode);
    }

    /// <summary>
    /// Configures the TypeScript generator with custom formatters.
    /// </summary>
    private TsGenerator ConfigureGenerator()
    {
        // Disable const enums for better TypeScript compatibility
        var generator = new TsGenerator { GenerateConstEnums = false };

        // Set up formatters
        generator.SetIdentifierFormatter(IdentifierFormatter.FormatPropertyName);
        generator.SetMemberTypeFormatter((prop, typeName) => this._typeFormatter.FormatMemberType(prop, typeName));
        generator.SetModuleNameFormatter(IdentifierFormatter.FormatModuleName);
        generator.SetTypeVisibilityFormatter((_, typeName) => TypeVisibilityFormatter.IsTypeVisible(typeName));

        return generator;
    }

    /// <summary>
    /// Writes the generated TypeScript code to the output file.
    /// </summary>
    private string WriteOutputFile(string tsCode)
    {
        var outputPath = this._pathResolver.ResolveOutputPath();
        var outputDirectory = Path.GetDirectoryName(outputPath);

        if (string.IsNullOrEmpty(outputDirectory))
        {
            throw new InvalidOperationException("Could not determine output directory from path.");
        }

        Directory.CreateDirectory(outputDirectory);

        logger.LogDebug("Writing output to: {OutputPath}", outputPath);
        File.WriteAllText(outputPath, tsCode);

        return outputPath;
    }
}