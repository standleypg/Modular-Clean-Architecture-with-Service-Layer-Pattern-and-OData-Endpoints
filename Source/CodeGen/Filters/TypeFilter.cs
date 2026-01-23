using CodeGen.Configuration;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace CodeGen.Filters;

/// <summary>
/// Filters types based on configuration rules.
/// </summary>
public class TypeFilter(TypeScriptGenConfig config, ILogger? logger = null)
{
    /// <summary>
    /// Checks if a type is in the target namespace.
    /// </summary>
    public static bool IsInTargetNamespace(Type type, NamespaceConfig nsConfig)
    {
        if (string.IsNullOrEmpty(type.Namespace))
            return false;

        if (nsConfig.IncludeNested)
        {
            // Match namespace or any nested namespace
            return type.Namespace == nsConfig.Namespace ||
                   type.Namespace.StartsWith(nsConfig.Namespace + ".", StringComparison.Ordinal);
        }

        // Exact namespace match only
        return type.Namespace == nsConfig.Namespace;
    }

    /// <summary>
    /// Checks if a type should be excluded because it's a static class.
    /// </summary>
    public bool IsNotStaticClass(Type type)
    {
        // If config allows static classes, don't filter them out
        if (config.IncludeStaticClasses)
            return true;

        // A static class in C# is abstract and sealed
        var isStaticClass = type.IsAbstract && type.IsSealed;

        if (isStaticClass)
        {
            logger?.LogDebug("Excluding static class: {TypeName}", type.Name);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Determines if a type should be included based on include/exclude filters.
    /// Filter order: includeTypes -> excludeTypes -> includeGenericTypes -> excludeGenericTypes
    /// </summary>
    public bool IsTypeIncluded(Type type, NamespaceConfig nsConfig)
    {
        // Step 1: Apply includeTypes filter (if specified)
        if (nsConfig.IncludeTypes != null && nsConfig.IncludeTypes.Count > 0)
        {
            if (!MatchesAnyPattern(type.Name, nsConfig.IncludeTypes))
            {
                logger?.LogTrace("Type {TypeName} excluded: doesn't match includeTypes", type.Name);
                return false;
            }
        }

        // Step 2: Apply excludeTypes filter
        if (nsConfig.ExcludeTypes != null && nsConfig.ExcludeTypes.Count > 0)
        {
            if (MatchesAnyPattern(type.Name, nsConfig.ExcludeTypes))
            {
                logger?.LogDebug("Type {TypeName} excluded: matches excludeTypes", type.Name);
                return false;
            }
        }

        // Step 3 & 4: Apply generic type filters (if type is generic)
        if (type.IsGenericType)
        {
            var genericBaseName = GetGenericBaseName(type);

            // Apply includeGenericTypes filter (if specified)
            if (nsConfig.IncludeGenericTypes != null && nsConfig.IncludeGenericTypes.Count > 0)
            {
                if (!MatchesAnyPattern(genericBaseName, nsConfig.IncludeGenericTypes))
                {
                    logger?.LogTrace("Generic type {TypeName} excluded: doesn't match includeGenericTypes", type.Name);
                    return false;
                }
            }

            // Apply excludeGenericTypes filter
            if (nsConfig.ExcludeGenericTypes != null && nsConfig.ExcludeGenericTypes.Count > 0)
            {
                if (MatchesAnyPattern(genericBaseName, nsConfig.ExcludeGenericTypes))
                {
                    logger?.LogDebug("Generic type {TypeName} excluded: matches excludeGenericTypes", type.Name);
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the base name of a generic type without the `n suffix.
    /// For example, "Result`2" becomes "Result".
    /// </summary>
    private static string GetGenericBaseName(Type type)
    {
        var name = type.Name;
        var backtickIndex = name.IndexOf('`', StringComparison.InvariantCultureIgnoreCase);
        return backtickIndex > 0 ? name[..backtickIndex] : name;
    }

    /// <summary>
    /// Checks if a name matches any of the patterns (prefix, suffix, or exact match).
    /// </summary>
    private static bool MatchesAnyPattern(string name, List<string> patterns)
    {
        return patterns.Any(pattern =>
            name.StartsWith(pattern, StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith(pattern, StringComparison.OrdinalIgnoreCase) ||
            name.Equals(pattern, StringComparison.OrdinalIgnoreCase));
    }
}