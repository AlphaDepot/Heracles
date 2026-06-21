// js/text-input.ts
var instances = /* @__PURE__ */ new Map();
function initialize(element, dotNetRef, instanceId, config) {
  if (!element || !dotNetRef) return;
  const input = element;
  const state = {
    element: input,
    dotNetRef,
    config,
    debounceTimer: null,
    rafId: null,
    pendingValue: null
  };
  const callOnInput = (value) => {
    dotNetRef.invokeMethodAsync("JsOnInput", value).catch(() => {
    });
  };
  const callOnChange = (value) => {
    dotNetRef.invokeMethodAsync("JsOnChange", value).catch(() => {
    });
  };
  const updateCharacterCount = () => {
    if (!config.hasCharacterCount || !config.characterCountSelector) return;
    const wrapper = input.closest("[data-textarea-wrapper]");
    if (!wrapper) return;
    const counter = wrapper.querySelector(config.characterCountSelector);
    if (!counter) return;
    const len = input.value.length;
    counter.textContent = config.maxLength ? `${len}/${config.maxLength}` : `${len}`;
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
        callOnInput(state.pendingValue);
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
  const stored = {
    state,
    handleInput,
    handleChange,
    element
  };
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
function updateConfig(instanceId, config) {
  const stored = instances.get(instanceId);
  if (stored) {
    stored.state.config = config;
  }
}
function dispose(instanceId) {
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
export {
  dispose,
  initialize,
  updateConfig
};
