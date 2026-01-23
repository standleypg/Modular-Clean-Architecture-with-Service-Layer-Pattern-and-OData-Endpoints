using CodeGen.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using TypeFilter = CodeGen.Filters.TypeFilter;

namespace CodeGen.Discovery;

/// <summary>
/// Discovers types from assemblies based on configuration.
/// </summary>
public class TypeDiscovery(TypeScriptGenConfig config, ILogger? logger = null)
{
    private readonly TypeFilter _typeFilter = new(config, logger);

    /// <summary>
    /// Loads the specified assembly.
    /// </summary>
    public Assembly LoadAssembly(string assemblyName)
    {
        try
        {
            logger?.LogDebug("Loading assembly: {AssemblyName}", assemblyName);
            return Assembly.Load(assemblyName);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to load assembly '{assemblyName}'. Ensure the assembly is available and the name is correct.", ex);
        }
    }

    /// <summary>
    /// Discovers types from the assembly based on configuration.
    /// </summary>
    public List<Type> DiscoverTypes(Assembly assembly)
    {
        var discoveredTypes = new List<Type>();

        foreach (var nsConfig in config.Namespaces)
        {
            logger?.LogDebug("Scanning namespace: {Namespace}", nsConfig.Namespace);

            var types = assembly.GetTypes()
                .Where(t => TypeFilter.IsInTargetNamespace(t, nsConfig) &&
                           !t.IsNested &&
                           t.IsPublic &&
                           this._typeFilter.IsNotStaticClass(t) &&
                           this._typeFilter.IsTypeIncluded(t, nsConfig))
                .ToList();

            logger?.LogDebug("Found {Count} types in namespace {Namespace}", types.Count, nsConfig.Namespace);
            discoveredTypes.AddRange(types);
        }

        return discoveredTypes;
    }
}