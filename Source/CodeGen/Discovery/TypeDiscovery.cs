using CodeGen.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using TypeFilter = CodeGen.Filters.TypeFilter;

namespace CodeGen.Discovery;

public class TypeDiscovery(TypeScriptGenConfig config, ILogger logger)
{
    private readonly TypeFilter _typeFilter = new(config, logger);

    public Assembly LoadAssembly(string assemblyName)
    {
        try
        {
            logger.LogDebug("Loading assembly: {AssemblyName}", assemblyName);
            return Assembly.Load(assemblyName);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to load assembly '{assemblyName}'. Ensure the assembly is available and the name is correct.", ex);
        }
    }

    public List<Type> DiscoverTypes(Assembly assembly)
    {
        var discoveredTypes = new List<Type>();

        foreach (var nsConfig in config.Namespaces)
        {
            logger.LogDebug("Scanning namespace: {Namespace}", nsConfig.Namespace);

            var types = assembly.GetTypes()
                .Where(t => TypeFilter.IsInTargetNamespace(t, nsConfig) &&
                            t is { IsNested: false, IsPublic: true } &&
                            this._typeFilter.IsNotStaticClass(t) &&
                            this._typeFilter.IsTypeIncluded(t, nsConfig))
                .ToList();

            logger.LogDebug("Found {Count} types in namespace {Namespace}", types.Count, nsConfig.Namespace);
            discoveredTypes.AddRange(types);
        }

        return discoveredTypes;
    }
}