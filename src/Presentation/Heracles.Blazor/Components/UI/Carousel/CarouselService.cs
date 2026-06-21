using System.Collections.Concurrent;
using Heracles.Blazor.Components.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heracles.Blazor.Components.UI.Carousel;

public class CarouselService
{
	public static CarouselService? Instance;

	private readonly ConcurrentDictionary<string, CarouselState> _carousels = new();
	private readonly IJSRuntime _js;
	private IJSObjectReference? _module;

	public CarouselService(IJSRuntime js)
	{
		_js = js;
		Instance = this;
	}

	public event Action<string>? StateChanged;

	private async Task<IJSObjectReference> Module()
	{
		if (_module is null)
		{
			_module = await _js.InvokeAsync<IJSObjectReference>(
				"import",
				"/_content/Heracles.Blazor/js/carousel.js"
			);
		}

		return _module;
	}

	public void Register(string id, Orientation orientation, object options, object[] plugins)
	{
		_carousels[id] = new CarouselState
		{
			Orientation = orientation,
			Options = options,
			Plugins = plugins
		};
	}

	public async Task InitializeAsync(string id, ElementReference viewport)
	{
		if (!_carousels.TryGetValue(id, out var state))
		{
			return;
		}

		state.Viewport = viewport;

		var m = await Module();
		await m.InvokeVoidAsync("initCarousel", id, viewport, state.Options, state.Plugins);

		await RefreshState(id);
	}

	public async Task RefreshState(string id)
	{
		if (!_carousels.TryGetValue(id, out var state))
		{
			return;
		}

		var m = await Module();

		state.CanGoPrev = await m.InvokeAsync<bool>("canGoPrev", id);
		state.CanGoNext = await m.InvokeAsync<bool>("canGoNext", id);

		// FIX: Embla returns float → convert safely
		var selected = await m.InvokeAsync<double>("getSelectedSnap", id);
		state.SelectedIndex = (int)Math.Round(selected);

		var snaps = await m.InvokeAsync<double[]>("getSnapList", id);
		state.SnapList = snaps.Select(x => (int)Math.Round(x)).ToArray();

		StateChanged?.Invoke(id);
	}

	public bool CanGoPrev(string id)
	{
		return _carousels.TryGetValue(id, out var s) && s.CanGoPrev;
	}

	public bool CanGoNext(string id)
	{
		return _carousels.TryGetValue(id, out var s) && s.CanGoNext;
	}

	public int GetSelectedIndex(string id)
	{
		return _carousels.TryGetValue(id, out var s) ? s.SelectedIndex : 0;
	}

	public int GetCount(string id)
	{
		return _carousels.TryGetValue(id, out var s) ? s.SnapList.Length : 0;
	}

	public async Task GoPrevAsync(string id)
	{
		var m = await Module();
		await m.InvokeVoidAsync("goPrev", id);
		await RefreshState(id);
	}

	public async Task GoNextAsync(string id)
	{
		var m = await Module();
		await m.InvokeVoidAsync("goNext", id);
		await RefreshState(id);
	}

	public async Task ScrollToAsync(string id, int index)
	{
		var m = await Module();
		await m.InvokeVoidAsync("scrollTo", id, index);
		await RefreshState(id);
	}

	public async Task<int[]> GetSnapListAsync(string id)
	{
		var m = await Module();
		var snaps = await m.InvokeAsync<double[]>("getSnapList", id);
		return snaps.Select(x => (int)Math.Round(x)).ToArray();
	}

	[JSInvokable]
	public static async Task EmblaStateChanged(string id)
	{
		if (Instance is not null)
		{
			await Instance.RefreshState(id);
		}
	}

	private class CarouselState
	{
		public bool CanGoNext;
		public bool CanGoPrev;
		public object Options = null!;
		public Orientation Orientation = Orientation.Horizontal;
		public object[] Plugins = [];
		public int SelectedIndex;
		public int[] SnapList = [];
		public ElementReference Viewport;
	}
}
