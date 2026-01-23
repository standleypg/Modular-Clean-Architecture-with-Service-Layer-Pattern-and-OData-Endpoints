using System.Text.Json.Serialization;

namespace CodeGen.Configuration;

public class TypeScriptGenConfig
{
    [JsonPropertyName("assemblyName")]
    public string AssemblyName { get; set; } = string.Empty;

    [JsonPropertyName("outputPath")]
    public string OutputPath { get; set; } = string.Empty;

    [JsonPropertyName("outputFileName")]
    public string OutputFileName { get; set; } = "models.ts";

    [JsonPropertyName("baseDirectory")]
    public string? BaseDirectory { get; set; }

    [JsonPropertyName("includeStaticClasses")]
    public bool IncludeStaticClasses { get; set; } = false;

    [JsonPropertyName("namespaces")]
    public List<NamespaceConfig> Namespaces { get; set; } = new();
}

public class NamespaceConfig
{
    [JsonPropertyName("namespace")]
    public string Namespace { get; set; } = string.Empty;

    [JsonPropertyName("includeTypes")]
    public List<string>? IncludeTypes { get; set; }

    [JsonPropertyName("excludeTypes")]
    public List<string>? ExcludeTypes { get; set; }

    [JsonPropertyName("includeGenericTypes")]
    public List<string>? IncludeGenericTypes { get; set; }

    [JsonPropertyName("excludeGenericTypes")]
    public List<string>? ExcludeGenericTypes { get; set; }

    [JsonPropertyName("includeNested")]
    public bool IncludeNested { get; set; } = false;
}