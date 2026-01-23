using System.Text.Json;
using CodeGen.Configuration;

namespace CodeGen.Utilities;

/// <summary>
/// Handles loading and validating configuration files.
/// </summary>
public static class ConfigLoader
{
    /// <summary>
    /// Loads configuration from the specified JSON file.
    /// </summary>
    public static TypeScriptGenConfig LoadConfig(string? configPath = null)
    {
        configPath ??= Path.Combine(AppContext.BaseDirectory, "typescriptgenconfig.json");

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found at: {configPath}", configPath);
        }

        var json = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<TypeScriptGenConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        });

        if (config == null)
        {
            throw new InvalidOperationException("Failed to deserialize configuration file.");
        }

        ValidateConfig(config);
        return config;
    }

    /// <summary>
    /// Validates the configuration object.
    /// </summary>
    private static void ValidateConfig(TypeScriptGenConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.AssemblyName))
        {
            throw new InvalidOperationException("AssemblyName must be specified in configuration.");
        }

        if (string.IsNullOrWhiteSpace(config.OutputPath))
        {
            throw new InvalidOperationException("OutputPath must be specified in configuration.");
        }

        if (string.IsNullOrWhiteSpace(config.OutputFileName))
        {
            throw new InvalidOperationException("OutputFileName must be specified in configuration.");
        }

        if (config.Namespaces == null || config.Namespaces.Count == 0)
        {
            throw new InvalidOperationException("At least one namespace must be specified in configuration.");
        }

        foreach (var ns in config.Namespaces)
        {
            if (string.IsNullOrWhiteSpace(ns.Namespace))
            {
                throw new InvalidOperationException("Namespace name cannot be empty.");
            }
        }
    }
}