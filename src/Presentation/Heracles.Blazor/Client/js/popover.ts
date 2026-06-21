import {
	arrow,
	autoUpdate,
	computePosition,
	type ComputePositionReturn,
	flip,
	type Middleware,
	offset,
	type Placement,
	shift,
	size
} from '@floating-ui/dom';

let cleanupAutoUpdate: (() => void) | null = null;

export interface PopoverOptions {
	offset?: number;
	placement?: Placement;
	showArrow?: boolean;
	matchRefWidth?: boolean;
}

async function waitForElement(selector: string): Promise<Element> {
	return new Promise((resolve, reject) => {
		const el = document.querySelector(selector);
		if (el) {
			resolve(el);
			return;
		}

		const observer = new MutationObserver(() => {
			const el2 = document.querySelector(selector);
			if (el2) {
				observer.disconnect();
				resolve(el2);
			}
		});

		observer.observe(document.documentElement, {
			childList: true,
			subtree: true
		});

		setTimeout(() => {
			observer.disconnect();
			reject(new Error(`Element not found: ${selector}`));
		}, 5000);
	});
}

function portal(element: Element | null): void {
	if (!element) return;
	if (!document.body.contains(element)) {
		document.body.appendChild(element);
	}
}

async function initialize(id: string, options?: PopoverOptions): Promise<void> {
	try {
		const popover = await waitForElement(`[data-popover="${id}"]`) as HTMLElement;
		const target = document.querySelector(`[data-popovertarget="${id}"]`) as HTMLElement | null;
		const overlay = document.querySelector(`[data-popover-overlay="${id}"]`) as HTMLElement | null;
		const arrowElement = popover.querySelector('[data-slot="arrow"]') as HTMLElement | null;

		const ref: Element | null =
			target && target.children.length === 1
				? (target.firstElementChild as Element)
				: target;

		if (!ref) return;

		portal(popover);
		if (overlay) portal(overlay);

		const {
			offset: offsetVal = 4,
			placement = 'bottom',
			showArrow = false,
			matchRefWidth = false
		} = options || {};

		const middlewares: Middleware[] = [
			offset(offsetVal),
			flip(),
			shift()
		];

		if (showArrow && arrowElement) {
			middlewares.push(arrow({element: arrowElement}));
		}

		if (matchRefWidth) {
			middlewares.push(
				size({
					apply({rects, elements}) {
						Object.assign((elements.floating as HTMLElement).style, {
							width: `${rects.reference.width}px`
						});
					}
				})
			);
		}

		const update = async (): Promise<void> => {
			const data: ComputePositionReturn = await computePosition(
				ref as any,
				popover,
				{
					placement,
					middleware: middlewares
				}
			);

			Object.assign(popover.style, {
				left: `${data.x}px`,
				top: `${data.y}px`
			});

			if (showArrow && arrowElement && data.middlewareData.arrow) {
				const {x: arrowX, y: arrowY} = data.middlewareData.arrow as {
					x?: number | null;
					y?: number | null;
				};

				const staticSide = {
					top: 'bottom',
					right: 'left',
					bottom: 'top',
					left: 'right'
				}[data.placement.split('-')[0] as 'top' | 'right' | 'bottom' | 'left'];

				Object.assign(arrowElement.style, {
					left: arrowX != null ? `${arrowX}px` : '',
					top: arrowY != null ? `${arrowY}px` : '',
					right: '',
					bottom: '',
					[staticSide]: '-4px'
				});
			}
		};

		await update();
		cleanupAutoUpdate = autoUpdate(ref as any, popover, update);
	} catch (err) {
		console.error('popover.initialize error', err);
	}
}

function destroy(): void {
	if (cleanupAutoUpdate) {
		cleanupAutoUpdate();
		cleanupAutoUpdate = null;
	}
}

export const popover = {
	initialize,
	destroy
};