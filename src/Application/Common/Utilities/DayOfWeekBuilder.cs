namespace Application.Common.Utilities;

/// <summary>
///     <see cref="DayOfWeek" /> Builder
/// </summary>
public static class DayOfWeekBuilder
{
	/// <summary>
	///     Returns a <see cref="DayOfWeek" /> enum value based on the given day name.
	/// </summary>
	/// <param name="dayName">The day name, such as "Monday" or "Tuesday".</param>
	/// <returns>
	///     The corresponding <see cref="DayOfWeek" /> enum value, or throws an <see cref="ArgumentException" /> if the
	///     day name is invalid.
	/// </returns>
	public static DayOfWeek? GetDayOfWeek(string dayName)
	{
		return dayName switch
		{
			"Sunday" => DayOfWeek.Sunday,
			"Monday" => DayOfWeek.Monday,
			"Tuesday" => DayOfWeek.Tuesday,
			"Wednesday" => DayOfWeek.Wednesday,
			"Thursday" => DayOfWeek.Thursday,
			"Friday" => DayOfWeek.Friday,
			"Saturday" => DayOfWeek.Saturday,
			_ => null
		};
	}
}
