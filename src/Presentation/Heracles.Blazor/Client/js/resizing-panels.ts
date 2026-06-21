// Global registry of observers by ID
const observers: Record<string, ResizeObserver> = {};

export interface ResizeCallback {
	rootWidth: number;
	rootHeight: number;
	firstPanelWidth: number;
	firstPanelHeight: number;
	secondPanelWidth: number;
	secondPanelHeight: number;
}

export function getPanelSizes(
	root: HTMLElement | null,
	first: HTMLElement | null,
	second: HTMLElement | null
): ResizeCallback {
	if (!root || !first || !second) {
		return {
			rootWidth: 0,
			rootHeight: 0,
			firstPanelWidth: 0,
			firstPanelHeight: 0,
			secondPanelWidth: 0,
			secondPanelHeight: 0
		};
	}

	const rootRect = root.getBoundingClientRect();
	const firstRect = first.getBoundingClientRect();
	const secondRect = second.getBoundingClientRect();

	return {
		rootWidth: rootRect.width,
		rootHeight: rootRect.height,
		firstPanelWidth: firstRect.width,
		firstPanelHeight: firstRect.height,
		secondPanelWidth: secondRect.width,
		secondPanelHeight: secondRect.height
	};
}

export function observeResize(
	id: string,
	dotNetRef: any,
	root: HTMLElement | null,
	first: HTMLElement | null,
	second: HTMLElement | null
): void {
	if (!root || !first || !second) {
		console.warn("Missing panel references for resize observer");
		return;
	}

	// Clean up any existing observer for this ID
	if (observers[id]) {
		observers[id].disconnect();
		delete observers[id];
	}

	const observer = new ResizeObserver(() => {
		const rootRect = root.getBoundingClientRect();
		const firstRect = first.getBoundingClientRect();
		const secondRect = second.getBoundingClientRect();

		const payload: ResizeCallback = {
			rootWidth: rootRect.width,
			rootHeight: rootRect.height,
			firstPanelWidth: firstRect.width,
			firstPanelHeight: firstRect.height,
			secondPanelWidth: secondRect.width,
			secondPanelHeight: secondRect.height
		};

		dotNetRef.invokeMethodAsync("OnResizeCallback", payload);
	});

	observer.observe(root);
	observer.observe(first);
	observer.observe(second);

	observers[id] = observer;
}

export function disposeObserver(id: string): void {
	if (observers[id]) {
		observers[id].disconnect();
		delete observers[id];
	}
}
