using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using RetailPortal.Model.DTOs.Common;
using System.Globalization;
using System.Web;

namespace RetailPortal.Service.Extensions;

public static class QueryableExtensions
{
    private const int DefaultPageSize = 5;

    public static async Task<ODataResponse<T>> GetODataResponseAsync<T>(this IQueryable<T> queryable,  ODataQueryOptions<T>? options, CancellationToken cancellationToken = default)
    {
        if(IsInValidOptions(options))
        {
            // TODO: return correct odata response when error
            return new ODataResponse<T>();
        }

        // Apply $filter (but not $skip, $top yet)
        var data = ApplyODataQuery(queryable, options);

        // Get total count based on filtered data (before applying pagination)
        int? count = null;
        if (options.Count.Value)
        {
            count = await data.CountAsync(cancellationToken);
        }

        // Determine next page URL if $top and $skip are provided
        string? nextPage = null;
        if (count > 0)
        {
            nextPage = GetNextPageUri(options, data);
        }

        // Apply $skip and $top after filtering
        data = ApplyPaging(data, options);

        return new ODataResponse<T>
        {
            Value = await data.ToListAsync(cancellationToken),
            Count = count,
            NextPage = nextPage
        };
    }

    private static IQueryable<T> ApplyODataQuery<T>(IQueryable<T> data, ODataQueryOptions<T> options)
    {
        if (options.Filter != null)
        {
            data = options.Filter.ApplyTo(data, new ODataQuerySettings()) as IQueryable<T> ?? data;
        }

        // Use OrderBy carefully as it can cost performance
        // Why? Because it creates extra SQL query check if the sorting exists
        if (options.OrderBy != null)
        {
            data = options.OrderBy.ApplyTo(data, new ODataQuerySettings()) as IQueryable<T> ?? data;
        }

        return data;
    }

    private static IQueryable<T> ApplyPaging<T>(
        IQueryable<T> queryable,
        ODataQueryOptions<T>? options)
    {

        if (options?.Skip != null)
        {
            queryable = options.Skip.ApplyTo(queryable, new ODataQuerySettings()) ?? queryable;
        }

        if (options?.Top != null)
        {
            queryable = options.Top.ApplyTo(queryable, new ODataQuerySettings()) ?? queryable;
        }

        return queryable;
    }

    private static bool IsInValidOptions<T>(ODataQueryOptions<T> options)
    {
        return options.SelectExpand != null || options.RawValues.Select != null;
    }

    private static string? GetNextPageUri<T>(ODataQueryOptions<T> options, IQueryable<T> data)
    {
        var skip = options.Skip?.Value ?? 0;
        var top = options.Top?.Value ?? DefaultPageSize;

        if (!data.Skip(skip + top).Any()) return null;

        var query = HttpUtility.ParseQueryString(options.Request.QueryString.ToString());
        query.Set("$skip", string.Format(CultureInfo.InvariantCulture, "{0}", skip + top));
        query.Set("$top", string.Format(CultureInfo.InvariantCulture, "{0}", top));

        var rawNextPage = $"{options.Request.Scheme}://{options.Request.Host}{options.Request.Path}?{query}";
        return new Uri(HttpUtility.UrlDecode(rawNextPage)).AbsoluteUri.ToString(CultureInfo.InvariantCulture);
    }
}