using CodeGen;
using CodeGen.Utilities;
using Microsoft.Extensions.Logging;

try
{
    // Optional: Set up logging
    using var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder
            .SetMinimumLevel(LogLevel.Debug);
    });

    var logger = loggerFactory.CreateLogger<TypeScriptGenerator>();

    // Load configuration
    var configPath = args.Length > 0 ? args[0] : null;
    var config = ConfigLoader.LoadConfig(configPath);

    Console.WriteLine("Configuration loaded successfully.");
    Console.WriteLine($"Assembly: {config.AssemblyName}");
    Console.WriteLine($"Output: {config.OutputPath}/{config.OutputFileName}");
    Console.WriteLine();

    // Generate TypeScript
    var generator = new TypeScriptGenerator(config, logger);
    generator.Generate();

    Console.WriteLine();
    Console.WriteLine("✓ TypeScript generation completed successfully!");
    return 0;
}
catch (FileNotFoundException ex)
{
    Console.Error.WriteLine($"ERROR: {ex.Message}");
    Console.Error.WriteLine();
    Console.Error.WriteLine("Usage: CodeGen [configPath]");
    Console.Error.WriteLine("  configPath: Optional path to typescriptgenconfig.json");
    return 1;
}
catch (InvalidOperationException ex)
{
    Console.Error.WriteLine($"CONFIGURATION ERROR: {ex.Message}");
    return 2;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"UNEXPECTED ERROR: {ex.Message}");
    Console.Error.WriteLine();
    Console.Error.WriteLine(ex.StackTrace);
    return 3;
}