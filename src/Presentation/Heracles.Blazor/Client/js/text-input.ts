
/**
 * Text Input JavaScript interop module.
 * Handles input/change events in JS to minimize C# interop calls.
 *
 * Modes:
 *   - onchange:  JS only calls C# on the native change event (zero interop during typing).
 *                For inputs this fires on blur and Enter; for textareas it fires on blur only.
 *   - immediate: JS batches calls via requestAnimationFrame.
 *   - debounced: JS debounces calls via setTimeout.
 */

interface DotNetObject {
	invokeMethodAsync(method: string, ...args: any[]): Promise<any>;
}

type InputMode = "onchange" | "immediate" | "debounced";

export interface InputConfig {
	mode: InputMode;
	debounceMs: number;
	hasCharacterCount?: boolean;
	characterCountSelector?: string;
	maxLength?: number | null;
	notifyOnBlur?: boolean;
}

interface InputState {
	element: HTMLInputElement | HTMLTextAreaElement;
	dotNetRef: DotNetObject;
	config: InputConfig;
	debounceTimer: number | null;
	rafId: number | null;
	pendingValue: string | null;
}

interface StoredInstance {
	state: InputState;
	handleInput: () => void;
	handleChange: () => void;
	handleBlur?: () => void;
	element: HTMLElement;
}

const instances = new Map<string, StoredInstance>();

/**
 * Initializes JS event handling for a text input or textarea element.
 * @param {HTMLElement} element - The input or textarea element.
 * @param {DotNetObject} dotNetRef - Reference to the Blazor component.
 * @param {string} instanceId - Unique ID for this instance.
 * @param {object} config - Configuration object.
 * @param {string} config.mode - 'onchange' | 'immediate' | 'debounced'
 * @param {number} config.debounceMs - Debounce interval (debounced mode only).
 * @param {boolean} [config.hasCharacterCount] - Whether to update a character count element.
 * @param {string} [config.characterCountSelector] - CSS selector for the counter element.
 * @param {number|null} [config.maxLength] - Max length for character count display.
 * @param {boolean} [config.notifyOnBlur] - When true, also fires JsOnChange on blur even if value didn't change.
 */
export function initialize(
	element: HTMLElement,
	dotNetRef: DotNetObject,
	instanceId: string,
	config: InputConfig
): void {
	if (!element || !dotNetRef) return;

	const input = element as HTMLInputElement | HTMLTextAreaElement;

	const state: InputState = {
		element: input,
		dotNetRef,
		config,
		debounceTimer: null,
		rafId: null,
		pendingValue: null
	};

	const callOnInput = (value: string) => {
		dotNetRef.invokeMethodAsync("JsOnInput", value).catch(() => {});
	};

	const callOnChange = (value: string) => {
		dotNetRef.invokeMethodAsync("JsOnChange", value).catch(() => {});
	};

	const updateCharacterCount = () => {
		if (!config.hasCharacterCount || !config.characterCountSelector) return;

		const wrapper = input.closest("[data-textarea-wrapper]");
		if (!wrapper) return;

		const counter = wrapper.querySelector(config.characterCountSelector);
		if (!counter) return;

		const len = input.value.length;
		counter.textContent = config.maxLength
			? `${len}/${config.maxLength}`
			: `${len}`;
	};

	const cancelPending = () => {
		if (state.debounceTimer !== null) {
			clearTimeout(state.debounceTimer);
			state.debounceTimer = null;
		}
		if (state.rafId !== null) {
			cancelAnimationFrame(state.rafId);
			state.rafId = null;
		}
	};

	const handleInput = () => {
		const value = input.value;

		updateCharacterCount();

		if (config.mode === "onchange") return;

		if (config.mode === "immediate") {
			if (state.rafId !== null) cancelAnimationFrame(state.rafId);

			state.pendingValue = value;
			state.rafId = requestAnimationFrame(() => {
				state.rafId = null;
				callOnInput(state.pendingValue!);
			});
			return;
		}

		if (config.mode === "debounced") {
			if (state.debounceTimer !== null) {
				clearTimeout(state.debounceTimer);
			}
			state.debounceTimer = window.setTimeout(() => {
				state.debounceTimer = null;
				callOnInput(value);
			}, config.debounceMs);
		}
	};

	const handleChange = () => {
		cancelPending();
		callOnChange(input.value);
	};

	input.addEventListener("input", handleInput);
	input.addEventListener("change", handleChange);

	const stored: StoredInstance = {
		state,
		handleInput,
		handleChange,
		element
	};

	// notifyOnBlur support
	if (config.notifyOnBlur) {
		let changeHandledBlur = false;

		const originalHandleChange = handleChange;

		const handleChangeForBlur = () => {
			changeHandledBlur = true;
			originalHandleChange();
		};

		const handleBlur = () => {
			if (!changeHandledBlur) {
				cancelPending();
				callOnChange(input.value);
			}
			changeHandledBlur = false;
		};

		input.removeEventListener("change", handleChange);
		input.addEventListener("change", handleChangeForBlur);
		input.addEventListener("blur", handleBlur);

		stored.handleChange = handleChangeForBlur;
		stored.handleBlur = handleBlur;
	}

	instances.set(instanceId, stored);
}

/**
 * Updates the configuration for an existing instance.
 * @param {string} instanceId - The instance to update.
 * @param {object} config - New configuration object.
 */
export function updateConfig(instanceId: string, config: InputConfig): void {
	const stored = instances.get(instanceId);
	if (stored) {
		stored.state.config = config;
	}
}

/**
 * Removes event handlers and cleans up state.
 * @param {string} instanceId - The instance to dispose.
 */
export function dispose(instanceId: string): void {
	const stored = instances.get(instanceId);
	if (!stored) return;

	const { element, handleInput, handleChange, handleBlur, state } = stored;

	element.removeEventListener("input", handleInput);
	element.removeEventListener("change", handleChange);

	if (handleBlur) {
		element.removeEventListener("blur", handleBlur);
	}

	if (state.debounceTimer !== null) {
		clearTimeout(state.debounceTimer);
	}
	if (state.rafId !== null) {
		cancelAnimationFrame(state.rafId);
	}

	instances.delete(instanceId);
}
