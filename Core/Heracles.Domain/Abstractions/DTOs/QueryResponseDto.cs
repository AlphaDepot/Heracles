namespace Heracles.Domain.Abstractions.DTOs;

public sealed class QueryResponseDto<T> where T : class
{
    public required List<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
}