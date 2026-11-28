export function cambiarEnlace(url) {
    history.pushState(null, '', url);
}

export function moverScroll(id) {
	const yOffset = -90;
	const element = document.getElementById(id);
	const y = element.getBoundingClientRect().top + window.pageYOffset + yOffset;

	window.scrollTo({ top: y, behavior: 'smooth' });
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