using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heracles.Blazor.Components.Motion;

public class MotionService
{
	private readonly Lazy<Task<IJSObjectReference>> _module;

	public MotionService(IJSRuntime js)
	{
		_module = new Lazy<Task<IJSObjectReference>>(() =>
			js.InvokeAsync<IJSObjectReference>(
				"import", "/_content/Heracles.Blazor/js/motion.js"
			).AsTask()
		);
	}

	public async Task Register(ElementReference el, object options)
	{
		var m = await _module.Value;
		await m.InvokeVoidAsync("motionInterop.register", el, options);
	}

	public async Task Animate(ElementReference el, object keyframes, object? options = null)
	{
		var m = await _module.Value;
		await m.InvokeVoidAsync("motionInterop.animate", el, keyframes, options);
	}

	public async Task PlayExit(ElementReference el)
	{
		var m = await _module.Value;
		await m.InvokeVoidAsync("motionInterop.playExit", el);
	}

	public async Task Swap(ElementReference oldEl, ElementReference newEl)
	{
		var m = await _module.Value;
		await m.InvokeVoidAsync("motionInterop.swap", oldEl, newEl);
	}
}
