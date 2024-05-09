namespace Heracles.Domain.Abstractions.Queries;

public sealed class QueryResponse<T> where T : class
{
    public required List<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public int TotalPages { get; set; }
}