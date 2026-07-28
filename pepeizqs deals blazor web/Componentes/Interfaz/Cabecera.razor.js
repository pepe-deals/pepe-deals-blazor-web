export function copiarTexto(text) {
    if (navigator.clipboard) {
        navigator.clipboard.writeText(text);
    }
}

export function teclearBusqueda(inputEl, dotnetRef, delay) {
    let timer;
    inputEl.addEventListener('input', (e) => {
        clearTimeout(timer);

        mostrarSpinner(true);

        timer = setTimeout(async () => {
            await dotnetRef.invokeMethodAsync('EventoTecleoBuscador', e.target.value);
            mostrarSpinner(false); 
        }, delay);
    });
}

function mostrarSpinner(visible) {
    const spinner = document.getElementById('spinner-buscador');
    const icono = document.getElementById('icono-buscador');

    if (spinner) spinner.style.display = visible ? 'flex' : 'none';
    if (icono) icono.style.display = visible ? 'none' : 'block';
}

export function toggleBodyScroll(disable) {
    if (disable) {
        document.body.classList.add('no-scroll');
    } else {
        document.body.classList.remove('no-scroll');
    }
}

export function activarTooltip(triggerId, tooltipId) {
    const element = document.getElementById(triggerId);
    if (!element || element._tooltipHandler) return;

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
    };

    element.addEventListener('mousemove', element._tooltipHandler);
}

export function desactivarTooltip(triggerId) {
    const element = document.getElementById(triggerId);
    if (!element?._tooltipHandler) return;

    element.removeEventListener('mousemove', element._tooltipHandler);
    delete element._tooltipHandler;
}

export function mostrarTooltipBuscador(id) {
    document.getElementById(id).style.display = 'block';
}

export function ocultarTooltipBuscador(id) {
    document.getElementById(id).style.display = 'none';
}

export function actualizarPosicionTooltipBuscador(clientX, clientY, id) {
    var tooltip = document.getElementById(id);
    if (!tooltip) return;

    var tooltipRect = tooltip.getBoundingClientRect();
    var offsetX = 15;
    var offsetY = 15;

    var left, top;

    if (clientX > window.innerWidth / 2) {
        left = clientX - tooltipRect.width - offsetX;
    } else {
        left = clientX + offsetX;
    }

    top = clientY + offsetY;

    if (left < 0) left = offsetX;
    if (left + tooltipRect.width > window.innerWidth) {
        left = window.innerWidth - tooltipRect.width - offsetX;
    }
    if (top + tooltipRect.height > window.innerHeight) {
        top = clientY - tooltipRect.height - offsetY;
    }

    tooltip.style.left = left + 'px';
    tooltip.style.top = top + 'px';
}

let cabeceraSuperiorEl = null;
let ultimoScrollY = window.scrollY;
let tickingScroll = false;

function alActualizarScroll() {
    const scrollActual = window.scrollY;

    if (scrollActual <= 0) {
        cabeceraSuperiorEl.classList.remove("oculta");
    } else if (scrollActual > ultimoScrollY) {
        cabeceraSuperiorEl.classList.add("oculta");
    }

    ultimoScrollY = scrollActual;
    tickingScroll = false;
}

function alHacerScroll() {
    if (tickingScroll === false) {
        window.requestAnimationFrame(alActualizarScroll);
        tickingScroll = true;
    }
}

let resizeObserverCabecera = null;

export function iniciarScrollCabecera() {
    cabeceraSuperiorEl = document.querySelector(".cabecera-superior");

    if (cabeceraSuperiorEl != null) {
        window.addEventListener("scroll", alHacerScroll, { passive: true });
    }

    observarAlturaCabecera();
}

export function destruirScrollCabecera() {
    window.removeEventListener("scroll", alHacerScroll);

    if (resizeObserverCabecera != null) {
        resizeObserverCabecera.disconnect();
        resizeObserverCabecera = null;
    }
}

function observarAlturaCabecera() {
    const contenedor = document.querySelector('.contenedor-cabeceras');
    if (!contenedor) return;

    const actualizar = () => {
        const altura = contenedor.getBoundingClientRect().height;
        document.documentElement.style.setProperty('--alturaCabeceraReal', `${altura}px`);
    };

    resizeObserverCabecera = new ResizeObserver(actualizar);
    resizeObserverCabecera.observe(contenedor);

    actualizar();
}