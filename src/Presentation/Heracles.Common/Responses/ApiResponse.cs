namespace Heracles.Common.Responses;

public record ApiResponse<T>(string Version, T Data)
{
	public static ApiResponse<T> Ok(T data, string version = "v1")
	{
		return new ApiResponse<T>(version, data);
	}
}
