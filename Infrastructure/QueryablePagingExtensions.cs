using HorsesForCourses.WebApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Infrastructure;

public static class QueryablePagingExtensions
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, PageRequest request)
    {

    if (!query.Expression.ToString().Contains("OrderBy"))
            throw new InvalidOperationException("Apply an OrderBy before paging to ensure stable results.");

        int skip = (request.Page - 1) * request.Size;
        return query.Skip(skip).Take(request.Size);
    }
}