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