// js/theme.ts
var STORAGE_KEY = "heracles-theme";
function applyTheme(mode) {
  if (mode === "system") {
    applyDarkMode(getSystemPreference());
  } else {
    applyDarkMode(mode === "dark");
  }
}
function applyDarkMode(isDark) {
  const root = document.documentElement;
  root.classList.toggle("dark", isDark);
}
function getSystemPreference() {
  return window.matchMedia("(prefers-color-scheme: dark)").matches;
}
function loadTheme() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}
function saveTheme(mode) {
  try {
    const state = { mode };
    localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
  } catch {
  }
}
export {
  applyDarkMode,
  applyTheme,
  getSystemPreference,
  loadTheme,
  saveTheme
};
