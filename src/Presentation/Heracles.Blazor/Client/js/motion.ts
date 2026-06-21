import {animate} from "motion";

export const motionInterop = {
	register(element: HTMLElement, options: any) {
		if (!element) return;
		(element as any).__motion = options;

		if (options.initial) {
			animate(element, options.initial, {duration: 0});
		}

		if (options.animate) {
			animate(element, options.animate, options.transition || {});
		}
	},

	animate(element: HTMLElement, keyframes: any, options?: any) {
		animate(element, keyframes, options || {});
	},

	playExit(element: HTMLElement): Promise<void> {
		return new Promise(resolve => {
			const opts = (element as any).__motion;
			if (!opts?.exit) {
				resolve();
				return;
			}

			animate(element, opts.exit, opts.transition || {})
				.finished.then(() => resolve());
		});
	},

	/** Presence: animate out old element, animate in new one */
	async swap(oldEl: HTMLElement, newEl: HTMLElement) {
		const oldOpts = (oldEl as any).__motion;
		const newOpts = (newEl as any).__motion;

		// Exit old
		if (oldOpts?.exit) {
			await animate(oldEl, oldOpts.exit, oldOpts.transition || {}).finished;
		}

		// Remove old
		oldEl.remove();

		// Initial new
		if (newOpts?.initial) {
			animate(newEl, newOpts.initial, {duration: 0});
		}

		// Animate new
		if (newOpts?.animate) {
			animate(newEl, newOpts.animate, newOpts.transition || {});
		}
	}
};

