import mediumZoom, {Zoom} from "medium-zoom";

type ElementInput = string | HTMLElement | HTMLElement[] | NodeListOf<HTMLElement>;

/**
 * Creates a zoom instance for a single <img> element.
 * This uses the PURE build of medium-zoom, which is required
 * for frameworks like Blazor that re-render DOM nodes.
 */
export function createZoom(element: ElementInput, options?: object) {
	// Ensure the element is a real <img>
	if (!(element instanceof HTMLImageElement)) {
		console.error("medium-zoom: element is not an <img>", element);
		return null;
	}

	// Create the zoom instance
	const zoom = mediumZoom(element, options);

	return zoom;
}

/**
 * Opens the zoom programmatically.
 */
export function open(zoom: Zoom, options: object) {
	return zoom.open(options);
}

/**
 * Closes the zoom programmatically.
 */
export function close(zoom: Zoom) {
	return zoom.close();
}

/**
 * Toggles the zoom programmatically.
 */
export function toggle(zoom: Zoom, options: object) {
	return zoom.toggle(options);
}

/**
 * Updates zoom options.
 */
export function update(zoom: Zoom, options: object) {
	return zoom.update(options);
}

/**
 * Detaches an element from the zoom instance.
 */
export function detach(zoom: Zoom, element: ElementInput) {
	return zoom.detach(element);
}

/**
 * Attaches an element to the zoom instance.
 */
export function attach(zoom: Zoom, element: ElementInput) {
	return zoom.attach(element);
}

/**
 * Returns the current zoom options.
 */
export function getOptions(zoom: Zoom) {
	return zoom.getOptions();
}

/**
 * Returns all images attached to the zoom instance.
 */
export function getImages(zoom: Zoom) {
	return zoom.getImages();
}

/**
 * Returns the currently zoomed image (if any).
 */
export function getZoomedImage(zoom: Zoom) {
	return zoom.getZoomedImage();
}