namespace Application.Common.Responses;

public class PagedResponse<T>
{
	public required IEnumerable<T> Data { get; set; }
	public int PageNumber { get; set; }
	public int PageSize { get; set; }

	public int TotalPages { get; set; }
	public int TotalItems { get; set; }
}
