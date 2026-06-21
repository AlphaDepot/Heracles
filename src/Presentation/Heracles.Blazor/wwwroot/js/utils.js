// js/utils.ts
function cleanHtml(id) {
  const el = document.getElementById(id);
  if (!el) return "";
  const clone = el.cloneNode(true);
  const walker = document.createTreeWalker(
    clone,
    NodeFilter.SHOW_COMMENT
  );
  const toRemove = [];
  while (walker.nextNode()) {
    const comment = walker.currentNode;
    toRemove.push(comment);
  }
  toRemove.forEach((c) => c.remove());
  return clone.innerHTML;
}
function getRect(el) {
  if (!el) return [0, 0];
  const r = el.getBoundingClientRect();
  const x = Number.isFinite(r.x) ? r.x : 0;
  const width = Number.isFinite(r.width) ? r.width : 0;
  return [x, width];
}
var threshold = 25;
var listeners = /* @__PURE__ */ new Map();
function createHandler(id) {
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
var handlers = /* @__PURE__ */ new Map();
function registerScrollDirection(id, dotNetObj) {
  if (listeners.has(id)) return;
  const listener = {
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
function unregisterScrollDirection(id) {
  const handler = handlers.get(id);
  if (handler) {
    window.removeEventListener("scroll", handler);
    handlers.delete(id);
  }
  listeners.delete(id);
}
function openPopup(url, width = 450, height = 450, windowName = "Popup") {
  const w = width;
  const h = height;
  const screenX = typeof window.screenX !== "undefined" ? window.screenX : window.screenLeft;
  const screenY = typeof window.screenY !== "undefined" ? window.screenY : window.screenTop;
  const outerW = window.outerWidth || document.documentElement.clientWidth || screen.width;
  const outerH = window.outerHeight || document.documentElement.clientHeight || screen.height;
  const left = Math.round(screenX + (outerW - w) / 2);
  const top = Math.round(screenY + (outerH - h) / 2);
  const features = `width=${w},height=${h},left=${left},top=${top},menubar=no,toolbar=no,location=no,status=no,scrollbars=no,resizable=no`;
  window.open(url, windowName, features);
}
function getElementContent(element) {
  if (!element) return "";
  const html = element.innerHTML.trim();
  const text = element.innerText.trim();
  const containsTags = /<\/?[a-z][\s\S]*>/i.test(html);
  return containsTags ? html : text;
}
async function copyToClipboard(text) {
  await navigator.clipboard.writeText(text);
}
var __keep = true;
export {
  __keep,
  cleanHtml,
  copyToClipboard,
  getElementContent,
  getRect,
  openPopup,
  registerScrollDirection,
  unregisterScrollDirection
};
