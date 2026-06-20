using Heracles.Shared.Responses;

namespace Heracles.Application.Utilities;

public static class PagedResponseFactory
{
	public static PagedResponse<T> Create<T>(
		IEnumerable<T> data,
		int totalItems,
		int? pageNumber,
		int? pageSize)
	{
		var size = pageSize ?? 10;
		var number = pageNumber ?? 1;

		return new PagedResponse<T>
		{
			Data = data,
			PageNumber = number,
			PageSize = size,
			TotalItems = totalItems,
			TotalPages = (int)Math.Ceiling(totalItems / (double)size)
		};
	}
}
