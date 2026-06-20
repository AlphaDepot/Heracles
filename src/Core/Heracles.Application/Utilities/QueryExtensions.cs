using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Utilities;

public static class QueryExtensions
{
	public static Task<bool> ExistsAsync<T>(
		this IQueryable<T> query,
		Expression<Func<T, bool>> predicate,
		CancellationToken ct = default)
		where T : class
	{
		return query.AnyAsync(predicate, ct);
	}
}
