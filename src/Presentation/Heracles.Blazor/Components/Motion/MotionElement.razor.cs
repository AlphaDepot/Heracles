using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Heracles.Blazor.Components.Motion;

public class MotionElement : ComponentBase
{
	[Inject] public MotionService Motion { get; set; } = null!;

	[Parameter] public string Tag { get; set; } = "div";
	[Parameter] public string? Class { get; set; }
	[Parameter] public RenderFragment? ChildContent { get; set; }

	[Parameter] public object? Initial { get; set; }
	[Parameter] public object? Animate { get; set; }
	[Parameter] public object? Exit { get; set; }
	[Parameter] public object? Transition { get; set; }
	[Parameter] public object? WhileTap { get; set; }

	[Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
	[Parameter] public EventCallback<MouseEventArgs> OnMouseEnter { get; set; }
	[Parameter] public EventCallback<MouseEventArgs> OnMouseLeave { get; set; }
	[Parameter] public EventCallback<MouseEventArgs> OnMouseMove { get; set; }
	[Parameter] public EventCallback<MouseEventArgs> OnMouseDown { get; set; }
	[Parameter] public EventCallback<MouseEventArgs> OnMouseUp { get; set; }
	[Parameter] public EventCallback<MouseEventArgs> OnMouseOver { get; set; }
	[Parameter] public EventCallback<MouseEventArgs> OnMouseOut { get; set; }

	[Parameter] public EventCallback<PointerEventArgs> OnPointerEnter { get; set; }
	[Parameter] public EventCallback<PointerEventArgs> OnPointerLeave { get; set; }
	[Parameter] public EventCallback<PointerEventArgs> OnPointerMove { get; set; }
	[Parameter] public EventCallback<PointerEventArgs> OnPointerDown { get; set; }
	[Parameter] public EventCallback<PointerEventArgs> OnPointerUp { get; set; }

	public ElementReference ElementRef { get; private set; }

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await Motion.Register(ElementRef, new
			{
				initial = Initial,
				animate = Animate,
				exit = Exit,
				transition = Transition,
				whileTap = WhileTap
			});
		}
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, Tag);

		if (!string.IsNullOrWhiteSpace(Class))
		{
			builder.AddAttribute(1, "class", Class);
		}


		builder.AddAttribute(2, "onclick",
			EventCallback.Factory.Create<MouseEventArgs>(this, HandleClick));

		if (OnMouseEnter.HasDelegate)
		{
			builder.AddAttribute(3, "onmouseenter", OnMouseEnter);
		}

		if (OnMouseLeave.HasDelegate)
		{
			builder.AddAttribute(4, "onmouseleave", OnMouseLeave);
		}

		if (OnMouseMove.HasDelegate)
		{
			builder.AddAttribute(5, "onmousemove", OnMouseMove);
		}

		if (OnMouseDown.HasDelegate)
		{
			builder.AddAttribute(6, "onmousedown", OnMouseDown);
		}

		if (OnMouseUp.HasDelegate)
		{
			builder.AddAttribute(7, "onmouseup", OnMouseUp);
		}

		if (OnMouseOver.HasDelegate)
		{
			builder.AddAttribute(8, "onmouseover", OnMouseOver);
		}

		if (OnMouseOut.HasDelegate)
		{
			builder.AddAttribute(9, "onmouseout", OnMouseOut);
		}

		if (OnPointerEnter.HasDelegate)
		{
			builder.AddAttribute(10, "onpointerenter", OnPointerEnter);
		}

		if (OnPointerLeave.HasDelegate)
		{
			builder.AddAttribute(11, "onpointerleave", OnPointerLeave);
		}

		if (OnPointerMove.HasDelegate)
		{
			builder.AddAttribute(12, "onpointermove", OnPointerMove);
		}

		if (OnPointerDown.HasDelegate)
		{
			builder.AddAttribute(13, "onpointerdown", OnPointerDown);
		}

		if (OnPointerUp.HasDelegate)
		{
			builder.AddAttribute(14, "onpointerup", OnPointerUp);
		}

		builder.AddElementReferenceCapture(15, r => ElementRef = r);

		builder.AddContent(16, ChildContent);

		builder.CloseElement();
	}

	private async Task HandleClick(MouseEventArgs e)
	{
		if (OnClick.HasDelegate)
		{
			await OnClick.InvokeAsync(e);
		}

		if (WhileTap != null)
		{
			await Motion.Animate(ElementRef, WhileTap, new { duration = 0.15 });
		}
	}
}
