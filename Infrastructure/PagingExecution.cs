using HorsesForCourses.WebApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Infrastructure;

public static class PagingExecution
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PageRequest request,
        CancellationToken ct = default)
    {
        var total = await query.CountAsync(ct);
        var pageItems = await query
            .ApplyPaging(request)
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedResult<T>(pageItems, total, request.Page, request.Size);
    }
}