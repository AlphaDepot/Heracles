export interface ParticleOptions {
	quantity: number;
	size: number;
	ease: number;
	staticity: number;
	vx: number;
	vy: number;
	rgb: string; // "255, 255, 255"
}

export interface ParticleInstance {
	dispose(): void;
}

interface Circle {
	x: number;
	y: number;
	size: number;
	alpha: number;
	targetAlpha: number;
	dx: number;
	dy: number;
	magnetism: number;
	translateX: number;
	translateY: number;
}

export function initParticles(
	canvas: HTMLCanvasElement,
	options: ParticleOptions
): ParticleInstance {

	const ctx = canvas.getContext("2d") as CanvasRenderingContext2D;
	const dpr = window.devicePixelRatio || 1;

	let circles: Circle[] = [];
	let mouse = {x: 0, y: 0};
	let rafId: number | null = null;
	let resizeObserver: ResizeObserver | null = null;

	function resize(): void {
		// Use offsetParent for absolute positioned canvas, fallback to parentElement
		const container = canvas.offsetParent || canvas.parentElement;
		if (!container) return;

		const rect = container.getBoundingClientRect();

		canvas.width = rect.width * dpr;
		canvas.height = rect.height * dpr;
		canvas.style.width = `${rect.width}px`;
		canvas.style.height = `${rect.height}px`;

		ctx.setTransform(dpr, 0, 0, dpr, 0, 0);

		circles = [];
		for (let i = 0; i < options.quantity; i++) {
			circles.push(createCircle(rect.width, rect.height));
		}
	}

	function createCircle(w: number, h: number): Circle {
		return {
			x: Math.random() * w,
			y: Math.random() * h,
			size: Math.random() * 2 + options.size,
			alpha: 0,
			targetAlpha: Math.random() * 0.6 + 0.1,
			dx: (Math.random() - 0.5) * 0.1,
			dy: (Math.random() - 0.5) * 0.1,
			magnetism: 0.1 + Math.random() * 4,
			translateX: 0,
			translateY: 0
		};
	}

	function drawCircle(c: Circle): void {
		ctx.beginPath();
		ctx.arc(c.x + c.translateX, c.y + c.translateY, c.size, 0, Math.PI * 2);
		ctx.fillStyle = `rgba(${options.rgb}, ${c.alpha})`;
		ctx.fill();
	}

	function animate(): void {
		const w = canvas.width / dpr;
		const h = canvas.height / dpr;

		ctx.clearRect(0, 0, w, h);

		circles.forEach((c, i) => {
			c.alpha += 0.02;
			if (c.alpha > c.targetAlpha) c.alpha = c.targetAlpha;

			c.x += c.dx + options.vx;
			c.y += c.dy + options.vy;

			c.translateX += (mouse.x / (options.staticity / c.magnetism) - c.translateX) / options.ease;
			c.translateY += (mouse.y / (options.staticity / c.magnetism) - c.translateY) / options.ease;

			drawCircle(c);

			if (
				c.x < -c.size ||
				c.x > w + c.size ||
				c.y < -c.size ||
				c.y > h + c.size
			) {
				circles[i] = createCircle(w, h);
			}
		});

		rafId = requestAnimationFrame(animate);
	}

	function onMouseMove(e: MouseEvent): void {
		const rect = canvas.getBoundingClientRect();
		mouse.x = e.clientX - rect.left - rect.width / 2;
		mouse.y = e.clientY - rect.top - rect.height / 2;
	}

	window.addEventListener("mousemove", onMouseMove);
	window.addEventListener("resize", resize);

	// Observe container size changes
	const container = canvas.offsetParent || canvas.parentElement;
	if (container) {
		resizeObserver = new ResizeObserver(() => resize());
		resizeObserver.observe(container);
	}

	resize();
	animate();

	return {
		dispose() {
			if (rafId !== null) cancelAnimationFrame(rafId);
			window.removeEventListener("mousemove", onMouseMove);
			window.removeEventListener("resize", resize);
			if (resizeObserver) resizeObserver.disconnect();
		}
	};
}