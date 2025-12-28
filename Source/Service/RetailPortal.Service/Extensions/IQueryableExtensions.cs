using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using RetailPortal.Model.DTOs.Common;
using System.Globalization;
using System.Web;

namespace RetailPortal.Service.Extensions;

public static class QueryableExtensions
{
    private static readonly ODataQuerySettings _defaultQuerySettings = new()
    {
        EnsureStableOrdering = false
    };

    public static Task<ODataResponse<T>> GetODataResponseAsync<T>(this IQueryable<T> queryable,
        HttpRequest request)
    {
        var options = request.GetODataQueryOptions<T>();

        var appliedQuery = GetAppliedQuery(queryable, options);
        var value = appliedQuery.Cast<T>().ToList();
        var countQuery = GetCountQuery(queryable, options);
        int? count = countQuery?.Count();

        return Task.FromResult(new ODataResponse<T>()
        {
            Value = value,
            Count = count?.ToString(CultureInfo.InvariantCulture),
            NextPage = GetNextPageUri(
                options,
                appliedQuery.Provider.CreateQuery(
                    appliedQuery.Expression
                        .RemoveODataSkipTop()
                    ) as IQueryable<T>)
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
                _defaultQuerySettings
            ) as IQueryable<T>;
        }

        return countQuery;
    }

    private static string? GetNextPageUri<T>(ODataQueryOptions<T> options, IQueryable<T>? data)
    {
        var skip = options.Skip?.Value ?? 0;
        var top = options.Top.Value;

        if (data != null && !data.Skip(skip + top).Any())
        {
            return null;
        }

        var query = HttpUtility.ParseQueryString(options.Request.QueryString.ToString());
        query.Set("$skip", string.Format(CultureInfo.InvariantCulture, "{0}", skip + top));
        query.Set("$top", string.Format(CultureInfo.InvariantCulture, "{0}", top));

        var rawNextPage = $"{options.Request.Scheme}://{options.Request.Host}{options.Request.Path}?{query}";
        return new Uri(HttpUtility.UrlDecode(rawNextPage)).AbsoluteUri.ToString(CultureInfo.InvariantCulture);
    }
}