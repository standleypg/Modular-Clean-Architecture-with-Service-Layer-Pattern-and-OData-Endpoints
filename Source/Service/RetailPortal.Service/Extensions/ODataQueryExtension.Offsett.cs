using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using RetailPortal.Model.DTOs.Common;
using System.Globalization;
using System.Web;

namespace RetailPortal.Service.Extensions;

public static partial class ODataQueryExtension
{
    private static readonly ODataQuerySettings _defaultQuerySettings = new() {  };

    public static Task<ODataResponse<TDestination>> GetODataResponse<TEntity, TDestination>(
        this IQueryable<TEntity> queryable,
        HttpRequest request)
    {
        var options = request.GetODataQueryOptions<TEntity>();

        var cursor = request.Query["cursor"].FirstOrDefault();
        var useCursorPagination = !string.IsNullOrEmpty(cursor) || (!request.Query.ContainsKey("$skip") && request.Query.ContainsKey("$top"));

        if(useCursorPagination)
        {
            return GetCursorBasedResponse<TEntity, TDestination>(queryable, request, options, cursor);
        }

        return GetOffsetBasedResponse<TEntity, TDestination>(queryable, options);
    }

    private static Task<ODataResponse<TDestination>> GetOffsetBasedResponse<TEntity, TDestination>(
        IQueryable<TEntity> queryable,
        ODataQueryOptions<TEntity> options)
    {
        var appliedQuery = GetAppliedQuery(queryable, options);
        var value = appliedQuery.ProjectToType<TDestination>();
        var countQuery = GetCountQuery(queryable, options);
        int? count = countQuery?.Count();

        return Task.FromResult(new ODataResponse<TDestination>()
        {
            Value = value,
            Count = count?.ToString(CultureInfo.InvariantCulture),
            NextPage = GetNextPageUri(options, count)
        });
    }

    private static IQueryable GetAppliedQuery<T>(
        IQueryable<T> queryable,
        ODataQueryOptions<T> options
    )
    {
        return options.ApplyTo(
            queryable,
            _defaultQuerySettings
        );
    }

    private static IQueryable<T>? GetCountQuery<T>(
        IQueryable<T> queryable,
        ODataQueryOptions<T> queryOptions
    )
    {
        IQueryable<T>? countQuery = null;
        if (queryOptions.Count is { Value: true })
        {
            countQuery = queryOptions.ApplyTo(
                queryable,
                _defaultQuerySettings,
                AllowedQueryOptions.Skip | AllowedQueryOptions.Top
            ) as IQueryable<T>;
        }

        return countQuery;
    }

    private static string? GetNextPageUri<T>(ODataQueryOptions<T> options, int? count)
    {
        var skip = options.Skip?.Value ?? 0;
        var top = options.Top.Value;

        if (!count.HasValue || skip + top >= count)
        {
            return null;
        }

        var query = HttpUtility.ParseQueryString(options.Request.QueryString.ToString());
        query.Set("$skip", string.Format(CultureInfo.InvariantCulture, "{0}", skip + top));
        query.Set("$top", string.Format(CultureInfo.InvariantCulture, "{0}", top));

        var rawNextPage = $"{options.Request.Scheme}://{options.Request.Host}{options.Request.Path}?{query}";
        return new Uri(HttpUtility.UrlDecode(rawNextPage)).AbsoluteUri;
    }
}