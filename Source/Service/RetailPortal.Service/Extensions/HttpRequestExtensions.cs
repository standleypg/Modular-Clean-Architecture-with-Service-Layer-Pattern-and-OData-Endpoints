using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Collections.Concurrent;

namespace RetailPortal.Service.Extensions;

internal static class HttpRequestExtensions
{
    extension<T>(HttpRequest request)
    {
        public ODataQueryOptions<T> GetODataQueryOptions()
        {
            var path = request.ODataFeature().Path;
            var edmModel = typeof(T).Get(request.HttpContext.RequestServices);

            var queryContext = new ODataQueryContext(
                edmModel,
                typeof(T),
                path
            );

            // Include $count=false by default if not specified in the request
            var queryCollection = QueryHelpers.ParseQuery(request.QueryString.ToString());
            if (!queryCollection.ContainsKey("$count"))
            {
                var modifiedQuery = QueryHelpers.AddQueryString(request.QueryString.ToString(), "$count", "false");
                request.QueryString = new QueryString(modifiedQuery);
            }

            // Include $top=1000 by default if not specified in the request
            if (!queryCollection.ContainsKey("$top"))
            {
                var modifiedQuery = QueryHelpers.AddQueryString(request.QueryString.ToString(), "$top", "1000");
                request.QueryString = new QueryString(modifiedQuery);
            }

            // Create and validate the query options.
            return new ODataQueryOptions<T>(queryContext, request);
        }
    }

    private static readonly ConcurrentDictionary<string, IEdmModel> _modelCache = new();

    extension(Type entityType)
    {
        private IEdmModel Get(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(entityType);

            return _modelCache.GetOrAdd(entityType.FullName!, key =>
            {
                var assemblyResolver = serviceProvider.GetService<IAssemblyResolver>();
                var builder = new ODataConventionModelBuilder(assemblyResolver, isQueryCompositionMode: true);
                EntityTypeConfiguration entityTypeConfiguration = builder.AddEntityType(entityType);
                builder.AddEntitySet(entityType.Name, entityTypeConfiguration);
                var model = builder.GetEdmModel();
                return model;
            });
        }
    }
}