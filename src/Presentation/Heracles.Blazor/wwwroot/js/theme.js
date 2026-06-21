// js/theme.ts
function getSystemPreference() {
  if (window.matchMedia) {
    return window.matchMedia("(prefers-color-scheme: dark)").matches;
  }
  return false;
}
function setItem(key, value) {
  localStorage.setItem(key, value);
}
function getItem(key) {
  return localStorage.getItem(key);
}
function removeItem(key) {
  localStorage.removeItem(key);
}
export {
  getItem,
  getSystemPreference,
  removeItem,
  setItem
};
