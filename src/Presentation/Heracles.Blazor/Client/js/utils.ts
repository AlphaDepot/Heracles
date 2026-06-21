/**
 * Cleans the HTML content of an element by removing all HTML comments.
 * @param id
 */
export function
cleanHtml(id: string): string {
	const el = document.getElementById(id);
	if (!el) return "";

	const clone = el.cloneNode(true) as HTMLElement;

	const walker = document.createTreeWalker(
		clone,
		NodeFilter.SHOW_COMMENT
	);

	const toRemove: Comment[] = [];

	while (walker.nextNode()) {
		const comment = walker.currentNode as Comment;
		toRemove.push(comment);
	}

	toRemove.forEach(c => c.remove());

	return clone.innerHTML;
}


/**
 * Returns the bounding rect (x + width) of an element.
 * @param el - The element reference from Blazor
 */
export function getRect(el: HTMLElement | null): [number, number] {
	if (!el) return [0, 0];

	const r = el.getBoundingClientRect();

	const x = Number.isFinite(r.x) ? r.x : 0;
	const width = Number.isFinite(r.width) ? r.width : 0;
	return [x, width];
}


// Scroll Direction Detection
const threshold = 25;

// Minimum scroll delta to consider
interface ScrollListener {
	lastScrollY: number;
	callback: (direction: "up" | "down") => void;
}

// Map of listeners
const listeners = new Map<string, ScrollListener>();

/**
 * Creates a scroll handler for the given id.
 * @param id
 */
function createHandler(id: string) {
	return function handleScroll() {
		const listener = listeners.get(id);
		if (!listener) return;

		const currentY = window.scrollY;
		const diff = currentY - listener.lastScrollY;

		if (Math.abs(diff) < threshold) return;

		if (diff > 0) listener.callback("down");
		else listener.callback("up");

		listener.lastScrollY = currentY;
	};
}

// Map of handlers
const handlers = new Map<string, (this: Window, ev: Event) => any>();

/**
 * Registers a scroll direction listener.
 * @param id
 * @param dotNetObj
 */
export function registerScrollDirection(id: string, dotNetObj: any) {
	if (listeners.has(id)) return;

	const listener: ScrollListener = {
		lastScrollY: window.scrollY,
		callback: (direction) => {
			dotNetObj.invokeMethodAsync("OnScrollDirection", direction);
		}
	};

	listeners.set(id, listener);

	const handler = createHandler(id);
	handlers.set(id, handler);

	window.addEventListener("scroll", handler);
}

/**
 * Unregisters a scroll direction listener.
 * @param id
 */
export function unregisterScrollDirection(id: string) {
	const handler = handlers.get(id);
	if (handler) {
		window.removeEventListener("scroll", handler);
		handlers.delete(id);
	}

	listeners.delete(id);
}

/**
 * Opens a centered popup window.
 * @param url
 * @param width
 * @param height
 * @param windowName
 */
export function openPopup(
	url: string,
	width: number = 450,
	height: number = 450,
	windowName: string = "Popup"
): void {

	const w = width;
	const h = height;

	const screenX = typeof window.screenX !== "undefined" ? window.screenX : window.screenLeft;
	const screenY = typeof window.screenY !== "undefined" ? window.screenY : window.screenTop;

	const outerW = window.outerWidth || document.documentElement.clientWidth || screen.width;
	const outerH = window.outerHeight || document.documentElement.clientHeight || screen.height;

	const left = Math.round(screenX + (outerW - w) / 2);
	const top = Math.round(screenY + (outerH - h) / 2);

	const features =
		`width=${w},height=${h},left=${left},top=${top},` +
		`menubar=no,toolbar=no,location=no,status=no,scrollbars=no,resizable=no`;

	window.open(url, windowName, features);
}

/**
 * Gets the content of an element, returning HTML if it contains tags, otherwise plain text.
 * @param element
 */
export function getElementContent(element: HTMLElement): string {
	if (!element) return "";

	const html = element.innerHTML.trim();
	const text = element.innerText.trim();

	// If HTML contains tags → return HTML
	const containsTags = /<\/?[a-z][\s\S]*>/i.test(html);

	return containsTags ? html : text;
}

/**
 * Copies the given text to the clipboard.
 * @param text
 */
export async function copyToClipboard(text: string): Promise<void> {
	await navigator.clipboard.writeText(text);
}


// This is here to prevent tree shaking of this file
export const __keep = true;
