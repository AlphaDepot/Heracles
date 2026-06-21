/**
 * Minimal Tailwind-compatible theme module for dark/light/system mode.
 * Fully typed, CSP-safe, and designed to work with Blazor interop.
 *
 * This module:
 * - Applies dark/light mode by toggling the `dark` class on <html>
 * - Supports system preference detection
 * - Persists theme mode to localStorage
 * - Loads saved theme mode on startup
 */

const STORAGE_KEY = "heracles-theme";

/**
 * Represents the allowed theme modes.
 * - "light"  → force light mode
 * - "dark"   → force dark mode
 * - "system" → follow OS preference
 */
export type ThemeMode = "light" | "dark" | "system";

/**
 * Shape of the saved theme state in localStorage.
 */
export interface ThemeState {
	mode: ThemeMode;
}

/**
 * Applies the theme mode to the document.
 * If the mode is "system", the OS preference is used.
 *
 * @param mode - The theme mode to apply.
 */
export function applyTheme(mode: ThemeMode): void {
	if (mode === "system") {
		applyDarkMode(getSystemPreference());
	} else {
		applyDarkMode(mode === "dark");
	}
}

/**
 * Toggles the Tailwind `dark` class on the <html> element.
 *
 * @param isDark - Whether dark mode should be enabled.
 */
export function applyDarkMode(isDark: boolean): void {
	const root = document.documentElement;
	root.classList.toggle("dark", isDark);
}

/**
 * Returns true if the user's OS prefers dark mode.
 *
 * @returns boolean - True if `(prefers-color-scheme: dark)` matches.
 */
export function getSystemPreference(): boolean {
	return window.matchMedia("(prefers-color-scheme: dark)").matches;
}

/**
 * Loads the saved theme mode from localStorage.
 *
 * @returns ThemeState | null - The saved theme state, or null if none exists.
 */
export function loadTheme(): ThemeState | null {
	try {
		const raw = localStorage.getItem(STORAGE_KEY);
		return raw ? (JSON.parse(raw) as ThemeState) : null;
	} catch {
		// localStorage unavailable or corrupted
		return null;
	}
}

/**
 * Saves the theme mode to localStorage.
 *
 * @param mode - The theme mode to persist.
 */
export function saveTheme(mode: ThemeMode): void {
	try {
		const state: ThemeState = { mode };
		localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
	} catch {
		// localStorage unavailable — ignore silently
	}
}
