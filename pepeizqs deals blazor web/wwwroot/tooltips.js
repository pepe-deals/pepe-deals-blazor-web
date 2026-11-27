(() => {
    let tooltip;

    function ocultar() {
        if (tooltip) tooltip.style.visibility = 'hidden';
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

        if (!tooltip?.isConnected) crear();

        tooltip.firstChild.textContent = trigger.dataset.tooltip;

        const izquierda = e.clientX < window.innerWidth / 2;
        tooltip.style.top = (e.clientY + (izquierda ? 10 : -10)) + 'px';
        tooltip.style.left = (izquierda ? e.clientX + 20 : e.clientX - 20 - tooltip.offsetWidth) + 'px';
        tooltip.style.visibility = 'visible';
    });

    document.addEventListener('scroll', ocultar, true);
    document.documentElement.addEventListener('mouseleave', ocultar);
})();