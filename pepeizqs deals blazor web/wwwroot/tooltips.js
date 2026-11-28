(() => {
    let tooltip;
    let directo;

    function ocultar() {
        if (tooltip) tooltip.style.visibility = 'hidden';
        if (directo) directo.style.visibility = 'hidden';
        directo = null;
    }

    function posicionar(el, e) {
        const izquierda = e.clientX < window.innerWidth / 2;
        el.style.top = (e.clientY + (izquierda ? 10 : -10)) + 'px';
        el.style.left = (izquierda ? e.clientX + 20 : e.clientX - 20 - el.offsetWidth) + 'px';
        el.style.visibility = 'visible';
    }

    function crear() {
        tooltip = document.createElement('div');
        tooltip.className = 'tooltip-relleno';
        tooltip.style.position = 'fixed';
        tooltip.style.pointerEvents = 'none';

        const texto = document.createElement('div');
        texto.style.cssText = 'margin: 8px; max-width: 300px; font-size: 14px;';
        tooltip.appendChild(texto);

        document.body.appendChild(tooltip);
    }

    document.addEventListener('mousemove', (e) => {
        const trigger = e.target.closest?.('[data-tooltip]');
        if (!trigger) return ocultar();

        const propio = trigger.querySelector(':scope > [data-tooltip-directo]');

        if (propio !== directo) ocultar();

        if (propio) {
            if (tooltip) tooltip.style.visibility = 'hidden';
            if (propio.childElementCount === 0) return; 

            directo = propio;
            propio.style.position = 'fixed';
            propio.style.pointerEvents = 'none';
            posicionar(propio, e);
            return;
        }

        if (!tooltip?.isConnected) crear();

        const contenido = trigger.querySelector('[data-tooltip-contenido]');

        if (contenido) {
            if (tooltip.firstChild.innerHTML !== contenido.innerHTML) {
                tooltip.firstChild.innerHTML = contenido.innerHTML;
            }
        } else {
            tooltip.firstChild.textContent = trigger.dataset.tooltip;
        }

        const izquierda = e.clientX < window.innerWidth / 2;
        tooltip.style.top = (e.clientY + (izquierda ? 10 : -10)) + 'px';
        tooltip.style.left = (izquierda ? e.clientX + 20 : e.clientX - 20 - tooltip.offsetWidth) + 'px';
        tooltip.style.visibility = 'visible';
    });

    document.addEventListener('scroll', ocultar, true);
    document.documentElement.addEventListener('mouseleave', ocultar);
})();