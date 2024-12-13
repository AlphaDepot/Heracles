namespace Application.Common.Requests;

/// <summary>
///     Represents a query request with search and sorting options.
/// </summary>
public class QueryRequest
{
	/// <summary>
	///     Gets or sets the search term.
	/// </summary>
	public string? SearchTerm { get; set; }

	/// <summary>
	///     Gets or sets the field to sort by.
	/// </summary>
	public string? SortBy { get; set; }

	/// <summary>
	///     Gets or sets the sort order (e.g., ascending or descending).
	/// </summary>
	public string? SortOrder { get; set; }

	/// <summary>
	///     Gets or sets the number of items per page. Default is 10.
	/// </summary>
	public int PageSize { get; set; } = 10;

	/// <summary>
	///     Gets or sets the page number. Default is 1.
	/// </summary>
	public int PageNumber { get; set; } = 1;
}
