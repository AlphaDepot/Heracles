// js/navigation-menu.ts
function getFocusableElements(container) {
  if (!container) return [];
  const elements = Array.from(
    container.querySelectorAll('a[href], [role="menuitem"]')
  );
  return elements.filter((el) => {
    if (el.getAttribute("aria-disabled") === "true") return false;
    if (el.hidden) return false;
    const style = window.getComputedStyle(el);
    if (style.display === "none" || style.visibility === "hidden") return false;
    return true;
  });
}
function navigateNext(container) {
  const items = getFocusableElements(container);
  if (items.length === 0) return null;
  const currentIndex = items.findIndex((item) => item === document.activeElement);
  let nextIndex;
  if (currentIndex === -1) {
    nextIndex = 0;
  } else if (currentIndex === items.length - 1) {
    nextIndex = 0;
  } else {
    nextIndex = currentIndex + 1;
  }
  items[nextIndex]?.focus();
  return items[nextIndex] ?? null;
}
function navigatePrevious(container) {
  const items = getFocusableElements(container);
  if (items.length === 0) return null;
  const currentIndex = items.findIndex((item) => item === document.activeElement);
  let prevIndex;
  if (currentIndex === -1) {
    prevIndex = items.length - 1;
  } else if (currentIndex === 0) {
    prevIndex = items.length - 1;
  } else {
    prevIndex = currentIndex - 1;
  }
  items[prevIndex]?.focus();
  return items[prevIndex] ?? null;
}
function navigateFirst(container) {
  const items = getFocusableElements(container);
  if (items.length === 0) return null;
  items[0]?.focus();
  return items[0] ?? null;
}
function navigateLast(container) {
  const items = getFocusableElements(container);
  if (items.length === 0) return null;
  const lastIndex = items.length - 1;
  items[lastIndex]?.focus();
  return items[lastIndex] ?? null;
}
function focusElement(element) {
  if (element) {
    element.focus();
  }
}
function setupKeyboardNavigation(container, dotNetRef) {
  const handleKeyDown = (e) => {
    const items = getFocusableElements(container);
    const focusedItemIndex = items.indexOf(document.activeElement);
    const hasFocusedItem = focusedItemIndex !== -1;
    switch (e.key) {
      case "ArrowDown":
        e.preventDefault();
        if (!hasFocusedItem) navigateFirst(container);
        else navigateNext(container);
        break;
      case "ArrowUp":
        e.preventDefault();
        if (!hasFocusedItem) navigateLast(container);
        else navigatePrevious(container);
        break;
      case "Home":
        e.preventDefault();
        navigateFirst(container);
        break;
      case "End":
        e.preventDefault();
        navigateLast(container);
        break;
      case "Escape":
        e.preventDefault();
        dotNetRef?.invokeMethodAsync("HandleEscapeKey");
        break;
    }
  };
  container.addEventListener("keydown", handleKeyDown);
  return {
    dispose: () => {
      container.removeEventListener("keydown", handleKeyDown);
    }
  };
}
export {
  focusElement,
  navigateFirst,
  navigateLast,
  navigateNext,
  navigatePrevious,
  setupKeyboardNavigation
};
