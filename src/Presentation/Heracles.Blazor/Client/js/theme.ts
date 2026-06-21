// Returns true if the OS is currently in dark mode
export function getSystemPreference(): boolean {
	if (window.matchMedia) {
		return window.matchMedia("(prefers-color-scheme: dark)").matches;
	}
	return false; // fallback to light mode
}

// Saves a value to localStorage
export function setItem(key: string, value: string): void {
	localStorage.setItem(key, value);
}

// Retrieves a value from localStorage
export function getItem(key: string): string | null {
	return localStorage.getItem(key);
}

// Removes a value from localStorage
export function removeItem(key: string): void {
	localStorage.removeItem(key);
}

