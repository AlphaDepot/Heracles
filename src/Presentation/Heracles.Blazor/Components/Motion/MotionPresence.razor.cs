using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Heracles.Blazor.Components.Motion;

public class MotionPresence : ComponentBase
{
	private RenderFragment? _previous;

	// Do not Initialize in constructor to avoid Blazor warnings
	private MotionElement? _previousElement;
	[Inject] public MotionService Motion { get; set; } = null!;

	[Parameter] public RenderFragment? ChildContent { get; set; }


	protected override async Task OnParametersSetAsync()
	{
		if (_previousElement != null)
		{
			await Motion.PlayExit(_previousElement.ElementRef);
		}

		_previous = ChildContent;
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.AddContent(0, ChildContent);
	}
}
