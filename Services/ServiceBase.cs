using System.Linq;
using Main.Utils;
using System;
using System.Linq.Dynamic.Core;

namespace Main.Services;

public class ServiceBase {
    protected readonly Context _context;
    public ServiceBase(Context context)
    {
        _context = context;
    }

    protected PaginationResult<T> GetPaginationResult<T>(IQueryable<T> query, PaginationRequest req)
    {
        if (!string.IsNullOrEmpty(req.SearchField) && !string.IsNullOrEmpty(req.SearchKeyword))
            query = query.Where($"{req.SearchField}.Contains(@0)", req.SearchKeyword);

        if (!string.IsNullOrEmpty(req.SortField))
            query = query.OrderBy($"{req.SortField} {req.SortOrder}");

        var totalCount = query.Count();

        var items = query
            .Skip(req.GetSkipNumber())
            .Take(req.PageSize)
            .ToList();

        var maxPageNumber = (int)Math.Ceiling((double)totalCount / Math.Max(1, req.PageSize));

        return new PaginationResult<T>(items, totalCount, req.PageNumber, maxPageNumber, req.PageSize);
    }
}