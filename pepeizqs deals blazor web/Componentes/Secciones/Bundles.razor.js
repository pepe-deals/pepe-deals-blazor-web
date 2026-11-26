export function cambiarEnlace(url) {
    history.pushState(null, '', url);
}

export function moverScroll(id) {
	const yOffset = -90;
	const element = document.getElementById(id);
	const y = element.getBoundingClientRect().top + window.pageYOffset + yOffset;

	window.scrollTo({ top: y, behavior: 'smooth' });
}

export function activarTooltip(triggerId, tooltipId) {
    const element = document.getElementById(triggerId);
    if (!element || element._tooltipHandler) return;

    const tooltipInicial = document.getElementById(tooltipId);
    if (tooltipInicial) tooltipInicial.style.visibility = 'hidden';

    element._tooltipHandler = (e) => {
        const tooltip = document.getElementById(tooltipId);
        if (!tooltip) return;

        if (window.innerWidth / 2 > e.clientX) {
            tooltip.style.top = (e.clientY + 10) + 'px';
            tooltip.style.left = (e.clientX + 20) + 'px';
        } else {
            tooltip.style.top = (e.clientY - 10) + 'px';
            tooltip.style.left = (e.clientX - 20 - tooltip.getBoundingClientRect().width) + 'px';
        }

        tooltip.style.visibility = 'visible';
    };

    element.addEventListener('mousemove', element._tooltipHandler);
}

export function desactivarTooltip(triggerId, tooltipId) {
    const element = document.getElementById(triggerId);
    if (!element?._tooltipHandler) return;

    element.removeEventListener('mousemove', element._tooltipHandler);
    delete element._tooltipHandler;

    const tooltip = document.getElementById(tooltipId);
    if (tooltip) tooltip.style.visibility = 'hidden';
}

let intervalosContador = new WeakMap();

export function iniciarContadores() {
    document.querySelectorAll('.contador-bundle[data-fin]').forEach(iniciarContador);
}

function iniciarContador(contador) {
    if (intervalosContador.has(contador)) return;

    const caja = contador.closest('.contador-bundle-caja');
    const fin = new Date(contador.dataset.fin).getTime();
    if (isNaN(fin)) return;

    const pad = n => String(n).padStart(2, "0");

    const pintar = () => {
        if (!contador.isConnected) {
            clearInterval(intervalosContador.get(contador));
            intervalosContador.delete(contador);
            return;
        }

        let resto = Math.floor((fin - Date.now()) / 1000);

        if (resto <= 0) {
            contador.textContent = "00:00:00";
            clearInterval(intervalosContador.get(contador));
            intervalosContador.delete(contador);
            return;
        }

        const d = Math.floor(resto / 86400); resto %= 86400;
        const h = Math.floor(resto / 3600); resto %= 3600;
        const m = Math.floor(resto / 60);
        const s = resto % 60;

        contador.textContent = (d > 0 ? d + "d " : "") + pad(h) + ":" + pad(m) + ":" + pad(s);
    };

    const intervalo = setInterval(pintar, 1000);
    intervalosContador.set(contador, intervalo);

    pintar();
    if (caja) caja.hidden = false;
}