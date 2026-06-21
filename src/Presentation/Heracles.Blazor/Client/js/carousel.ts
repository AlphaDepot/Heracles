import EmblaCarousel, {EmblaCarouselType, EmblaOptionsType, EmblaPluginType,} from "embla-carousel";
import Autoplay from "embla-carousel-autoplay";

const instances: Record<string, EmblaCarouselType> = {};

declare const DotNet: {
	invokeMethodAsync<T = any>(
		assemblyName: string,
		methodIdentifier: string,
		...args: any[]
	): Promise<T>;
};

function notifyDotNet(id: string) {
	// Notify .NET when Embla changes slide (drag, click, snap).
	// Note: TypeScript marks this signature as deprecated due to a .d.ts issue,
	// but this is still the official and correct Blazor API per Microsoft docs:
	// https://learn.microsoft.com/aspnet/core/blazor/javascript-interoperability/call-dotnet-from-javascript?view=aspnetcore-10.0
	void DotNet.invokeMethodAsync<void>("Heracles.Blazor", "EmblaStateChanged", id);
}

// Resolve plugins
const pluginRegistry: Record<string, (opts: any) => any> = {
	autoplay: (opts) => Autoplay(opts),
	// wheelGestures: (opts) => WheelGesturesPlugin(opts),
	// add more here later
};

function resolvePlugin(p: any) {

	if (!p || typeof p !== "object") return null;
	const factory = pluginRegistry[p.name];
	if (!factory) {
		return null;
	}
	return factory(p.options || {});
}


export function initCarousel(
	id: string,
	viewport: HTMLElement,
	options: EmblaOptionsType,
	plugins: EmblaPluginType[]
) {
	if (!viewport) return;


	const resolvedPlugins = (plugins || [])
		.map(resolvePlugin)
		.filter(Boolean);

	const api = EmblaCarousel(viewport, options, resolvedPlugins);
	instances[id] = api;


	// Start autoplay if present
	const autoplay = resolvedPlugins.find(
		(p: any) => p && typeof p.play === "function"
	) as any;

	if (autoplay) {
		autoplay.play();
	}


	// Initial sync
	notifyDotNet(id);

	// Sync on slide change
	api.on("select", () => notifyDotNet(id));
}

export function destroyCarousel(id: string) {
	const api = instances[id];
	if (api) {
		api.rootNode().remove();
		delete instances[id];
	}
}

export function goPrev(id: string) {
	const api = instances[id];
	api?.goToPrev();
}

export function goNext(id: string) {
	const api = instances[id];
	api?.goToNext();
}

export function canGoPrev(id: string): boolean {
	const api = instances[id];
	return api ? api.canGoToPrev() : false;
}

export function canGoNext(id: string): boolean {
	const api = instances[id];
	return api ? api.canGoToNext() : false;
}


export function getSelectedSnap(id: string): number {
	const api = instances[id];
	return api ? api.selectedSnap() : 0;
}


export function scrollTo(id: string, index: number) {
	const api = instances[id];
	api?.goTo(index);
}

export function getSnapList(id: string): number[] {
	const api = instances[id];
	return api ? api.snapList() : [];
}
