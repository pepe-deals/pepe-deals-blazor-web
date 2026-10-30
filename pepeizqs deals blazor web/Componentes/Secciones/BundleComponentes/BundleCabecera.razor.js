export function iniciarContador(idContador, idTexto, idCaja) {
    const contador = document.getElementById(idContador);
    const texto = document.getElementById(idTexto);
    const caja = document.getElementById(idCaja);
    if (!contador) return;

    const fin = new Date(contador.dataset.fin).getTime();
    if (texto) texto.hidden = true;
    if (caja) caja.hidden = false;

    const pad = n => String(n).padStart(2, "0");

    const pintar = () => {
        if (!document.getElementById(idContador)) { clearInterval(intervalo); return; }

        let resto = Math.floor((fin - Date.now()) / 1000);
        if (resto <= 0) {
            contador.textContent = "00:00:00";
            clearInterval(intervalo);
            return;
        }

        const d = Math.floor(resto / 86400); resto %= 86400;
        const h = Math.floor(resto / 3600); resto %= 3600;
        const m = Math.floor(resto / 60);
        const s = resto % 60;

        contador.textContent = (d > 0 ? d + "d " : "") + pad(h) + ":" + pad(m) + ":" + pad(s);
    };

    const intervalo = setInterval(pintar, 1000);
    pintar();
}