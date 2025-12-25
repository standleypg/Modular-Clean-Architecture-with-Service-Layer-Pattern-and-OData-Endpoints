using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.UriParser;
using RetailPortal.Model.Db.Entities.Common.Base;

namespace RetailPortal.Unit;

public static class TestUtils
{
    public static ODataQueryOptions<T> ODataQueryOptionsUtils<T>(int top = 0, bool includeSelectQuery = false)
        where T : EntityBase
    {
        var edmModel = TestUtils.CreateEdmModel<T>();
        var queryContext = new ODataQueryContext(edmModel, typeof(T), new ODataPath());
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Method = "GET",
                Path = "/api/products",
                QueryString = new QueryString($"?$count=true&$top={top}{(includeSelectQuery ? "&$select=Id" : "")}")
            }
        };
        var httpRequest = httpContext.Request;
        var queryOptions = new ODataQueryOptions<T>(queryContext, httpRequest);
        return queryOptions;
    }

    private static IEdmModel CreateEdmModel<T>() where T : EntityBase
    {
        var builder = new ODataConventionModelBuilder();

        // Configure the entity set with key from the base Entity class
        var entityConfiguration = builder.EntitySet<T>(typeof(T).Name);

        // Explicitly configure the key
        entityConfiguration.EntityType.HasKey(e => e.Id);

        return builder.GetEdmModel();
    }
}