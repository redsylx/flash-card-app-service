using System;

namespace Main.Utils;

public class PaginationRequest {
    public bool IsPaged { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortField { get; set; } = "CreatedTime";
    public string SortOrder { get; set; } = "asc";
    public string? SearchField { get; set; }
    public string? SearchKeyword { get; set; }
    public int GetSkipNumber () => Math.Max(0, PageSize * (PageNumber - 1));
}