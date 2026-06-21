namespace Heracles.Blazor.Components.UI.Dock;

public class DockService
{
	public event Func<double, Task>? MouseMoved;

	public async Task NotifyMouseMove(double x)
	{
		if (MouseMoved is null)
		{
			return;
		}

		var handlers = MouseMoved.GetInvocationList();
		foreach (var @delegate in handlers)
		{
			var handler = (Func<double, Task>)@delegate;
			try
			{
				await handler(x);
			}
			catch
			{
				// swallow exceptions from disposed components
			}
		}
	}
}
