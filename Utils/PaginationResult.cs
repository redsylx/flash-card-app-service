using System.Collections.Generic;

namespace Main.Utils;

public class PaginationResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int MaxPageNumber { get; set; }
    public int PageSize { get; set; }

    public PaginationResult(List<T> items, int totalCount, int pageNumber, int maxPageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        MaxPageNumber = maxPageNumber;
        PageSize = pageSize;
    }
}