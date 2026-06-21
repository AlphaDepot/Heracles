using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heracles.Blazor.Services;

public class JavascriptUtils
{
	private readonly Lazy<Task<IJSObjectReference>> _module;

	public JavascriptUtils(IJSRuntime js)
	{
		_module = new Lazy<Task<IJSObjectReference>>(() =>
			js.InvokeAsync<IJSObjectReference>(
				"import", "/_content/Heracles.Blazor/js/utils.js"
			).AsTask()
		);
	}

	/// <summary>
	///     Cleans the inner HTML content of an element identified by its ID by removing
	///     unnecessary or unsafe elements.
	/// </summary>
	/// <param name="id">The ID of the HTML element to clean.</param>
	/// <returns>
	///     Returns a cleaned version of the HTML content of the specified element, with all HTML comments removed.
	/// </returns>
	public async Task<string> CleanHtml(string id)
	{
		var m = await _module.Value;
		return await m.InvokeAsync<string>("cleanHtml", id);
	}

	/// <summary>
	///     Copies the specified text to the clipboard asynchronously.
	/// </summary>
	/// <param name="text">The text to be copied to the clipboard.</param>
	/// <returns>
	///     Does not return any value.
	/// </returns>
	public async Task CopyToClipboard(string text)
	{
		var m = await _module.Value;
		await m.InvokeVoidAsync("copyToClipboard", text);
	}


	/// <summary>
	///     Retrieves the rectangle properties of a specified HTML element, including its X-coordinate
	///     and width within the rendered client area.
	/// </summary>
	/// <param name="el">The reference to the HTML element whose rectangle properties are to be retrieved.</param>
	/// <returns>
	///     A tuple containing the X-coordinate and the width of the specified element.
	///     If the element is not valid or its width is zero, the returned values will be (0, 0).
	/// </returns>
	public async Task<(double X, double Width)> GetRect(ElementReference el)
	{
		var m = await _module.Value;

		var raw = await m.InvokeAsync<object>("getRect", el);

		if (raw is not object[] { Length: 2 } arr)
		{
			return (0, 0);
		}

		var x = Convert.ToDouble(arr[0]);
		var width = Convert.ToDouble(arr[1]);

		return width == 0 ? (0, 0) : (x, width);
	}

	/// <summary>
	///     Registers a callback to monitor the scroll direction for a specified HTML element by its ID.
	/// </summary>
	/// <typeparam name="T">The type of the .NET object that will handle the callback.</typeparam>
	/// <param name="id">The ID of the HTML element to monitor for scroll direction changes.</param>
	/// <param name="dotNetObj">A reference to the .NET object that contains the callback implementation.</param>
	/// <returns>
	///     An asynchronous task that completes once the scroll direction listener has been successfully registered.
	/// </returns>
	public async Task RegisterScrollDirection<T>(string id, DotNetObjectReference<T> dotNetObj)
		where T : class
	{
		var m = await _module.Value;
		await m.InvokeVoidAsync("registerScrollDirection", id, dotNetObj);
	}

	/// <summary>
	///     Unregisters a previously registered scroll direction listener for a specified element
	///     based on its ID. This effectively stops monitoring scroll direction changes for the element.
	/// </summary>
	/// <param name="id">The ID of the HTML element whose scroll direction listener should be unregistered.</param>
	/// <returns>
	///     A task that represents the asynchronous operation. Does not return a value.
	/// </returns>
	public async Task UnregisterScrollDirection(string id)
	{
		var m = await _module.Value;
		await m.InvokeVoidAsync("unregisterScrollDirection", id);
	}

	/// <summary>
	///     Opens a new popup window with the specified URL and dimensions.
	/// </summary>
	/// <param name="url">The URL to be loaded in the popup window.</param>
	/// <param name="width">The width of the popup window, in pixels. Default value is 450.</param>
	/// <param name="height">The height of the popup window, in pixels. Default value is 450.</param>
	/// <param name="windowName">The name of the popup window. Default value is "Popup".</param>
	/// <returns>
	///     An asynchronous task that completes when the popup window is successfully opened.
	/// </returns>
	public async Task OpenPopup(string url, int width = 450, int height = 450, string windowName = "Popup")
	{
		var m = await _module.Value;
		await m.InvokeVoidAsync("openPopup", url, width, height, windowName);
	}

	/// <summary>
	///     Retrieves the inner content of a specified HTML element as a string.
	/// </summary>
	/// <param name="elementReference">The reference to the HTML element whose content is to be retrieved.</param>
	/// <returns>
	///     Returns the inner content of the specified HTML element as a string.
	/// </returns>
	public async Task<string> GetElementContent(ElementReference elementReference)
	{
		var m = await _module.Value;
		return await m.InvokeAsync<string>("getElementContent", elementReference);
	}
}
