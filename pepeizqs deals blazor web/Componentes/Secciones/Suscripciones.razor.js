export function moverScroll(id) {
    const yOffset = -90;
    const element = document.getElementById(id);
    const y = element.getBoundingClientRect().top + window.pageYOffset + yOffset;

    window.scrollTo({ top: y, behavior: 'smooth' });
}

export function cambiarEnlace(url) {
    history.pushState(null, '', url);
}