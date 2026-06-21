// js/resizing-panels.ts
var observers = {};

function getPanelSizes(root, first, second) {
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

function observeResize(id, dotNetRef, root, first, second) {
  if (!root || !first || !second) {
    console.warn("Missing panel references for resize observer");
    return;
  }
  if (observers[id]) {
    observers[id].disconnect();
    delete observers[id];
  }
  const observer = new ResizeObserver(() => {
    const rootRect = root.getBoundingClientRect();
    const firstRect = first.getBoundingClientRect();
    const secondRect = second.getBoundingClientRect();
    const payload = {
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

function disposeObserver(id) {
  if (observers[id]) {
    observers[id].disconnect();
    delete observers[id];
  }
}

export {
  disposeObserver,
  getPanelSizes,
  observeResize
};
