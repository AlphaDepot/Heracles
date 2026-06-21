import Sortable, {Options as SortableOptions, SortableEvent} from "sortablejs";

export interface DotNetCallback {
	invokeMethodAsync(method: string, args: unknown): void;
}

export interface BlazorSortableOptions {
	sortableOptions?: SortableOptions;
	onEnd?: DotNetCallback;
	onUpdate?: DotNetCallback;
}

export const sortableInterop = {
	create(element: HTMLElement, options: BlazorSortableOptions) {
		console.log("Creating sortable instance for element:", element);

		if (!element) return;

		const mappedOptions: SortableOptions = {
			...(options.sortableOptions ?? {}),
			handle: ".sortable-item" // instead of .drag-handle
		};

		if (options.onEnd) {
			mappedOptions.onEnd = (evt: SortableEvent) => {
				options.onEnd!.invokeMethodAsync("Invoke", {
					oldIndex: evt.oldIndex,
					newIndex: evt.newIndex
				});
			};
		}

		if (options.onUpdate) {
			mappedOptions.onUpdate = (evt: SortableEvent) => {
				options.onUpdate!.invokeMethodAsync("Invoke", {
					oldIndex: evt.oldIndex,
					newIndex: evt.newIndex
				});
			};
		}

		(element as any).__sortable = Sortable.create(element, mappedOptions);
	},

	destroy(element: HTMLElement) {
		console.log("Destroying sortable instance for element:", element);
		const sortable = (element as any).__sortable as Sortable | undefined;
		if (sortable) {
			sortable.destroy();
			delete (element as any).__sortable;
		}
	},

	option(element: HTMLElement, name: string, value: unknown) {
		console.log("Setting option for sortable instance:", name, value);
		const sortable = (element as any).__sortable as Sortable | undefined;
		if (sortable) {
			sortable.option(
				name as keyof SortableOptions,
				value as SortableOptions[keyof SortableOptions]
			);
		}
	}
};

export default sortableInterop;
