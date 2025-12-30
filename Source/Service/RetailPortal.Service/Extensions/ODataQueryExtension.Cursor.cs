using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using RetailPortal.Model.DTOs.Common;
using System.Globalization;
using System.Text;
using System.Web;

namespace RetailPortal.Service.Extensions;

public static partial class ODataQueryExtension
{
    private static async Task<ODataResponse<TDestination>> GetCursorBasedResponse<TEntity, TDestination>(
        IQueryable<TEntity> queryable,
        HttpRequest request,
        ODataQueryOptions<TEntity> options,
        string? cursor)
    {
        var pageSize = options.Top.Value;

        IQueryable<TEntity>? queryableWithCursor = null;
        if (!string.IsNullOrEmpty(cursor))
        {
            var cursorId = DecodeCursor(cursor);
            queryableWithCursor = ApplyCursorFilter(queryable, cursorId);
        }


        var filteredQuery = ApplyFilters(queryableWithCursor ?? queryable, options);

        var value = await filteredQuery.ProjectToType<TDestination>().ToListAsync();

        var countQuery = GetCountQuery(queryable, options);
        int? count = countQuery != null ? await countQuery.CountAsync() : null;

        string? nextCursor = null;
        if (count.HasValue && value.Count == pageSize)
        {
            var lastItem = filteredQuery.Last();
            var lastId = GetIdFromEntity(lastItem);
            nextCursor = EncodeCursor(lastId);
        }

        return new ODataResponse<TDestination>()
        {
            Value = value,
            Count = count?.ToString(CultureInfo.InvariantCulture),
            NextPage = GetNextPageUriCursor(request, nextCursor, pageSize)
        };
    }

    private static string EncodeCursor(long id)
    {
        var idString = id.ToString(CultureInfo.InvariantCulture);
        var bytes = Encoding.UTF8.GetBytes(idString);
        return Convert.ToBase64String(bytes);
    }

    private static long DecodeCursor(string cursor)
    {
        var bytes = Convert.FromBase64String(cursor);
        var idString = Encoding.UTF8.GetString(bytes);
        return long.Parse(idString, CultureInfo.InvariantCulture);
    }

    private static IQueryable<T> ApplyCursorFilter<T>(IQueryable<T> queryable, long cursorId)
    {
        return queryable.Where(e => e != null && EF.Property<long>(e, "Id") > cursorId);
    }

    private static IQueryable<T> ApplyFilters<T>(
        IQueryable<T> queryable,
        ODataQueryOptions<T> options)
    {
        return options.ApplyTo(queryable, _defaultQuerySettings) as IQueryable<T> ?? queryable;
    }

    private static string? GetNextPageUriCursor(HttpRequest request, string? cursor, int pageSize)
    {
        if (cursor == null)
        {
            return null;
        }

        var query = HttpUtility.ParseQueryString(request.QueryString.ToString());
        query.Remove("$skip");
        query.Set("cursor", cursor);
        query.Set("$top", pageSize.ToString(CultureInfo.InvariantCulture));

        var rawNextPage = $"{request.Scheme}://{request.Host}{request.Path}?{query}";
        return new Uri(rawNextPage).AbsoluteUri;
    }

    private static long GetIdFromEntity<T>(T entity)
    {
        var t = typeof(T).Name;
        var idProperty = typeof(T).GetProperty("Id");

        if (idProperty == null || idProperty.PropertyType != typeof(long))
        {
            throw new InvalidOperationException($"Entity {typeof(T).Name} must have a 'Id' property of type long");
        }

        var value = idProperty.GetValue(entity);
        return (long)(value ?? 0L);
    }
}