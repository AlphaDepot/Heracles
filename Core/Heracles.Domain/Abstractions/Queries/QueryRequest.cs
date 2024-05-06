namespace Heracles.Domain.Abstractions.Queries;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class QueryRequest
{
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}