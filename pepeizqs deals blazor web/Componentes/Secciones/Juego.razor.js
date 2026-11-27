export function cambiarEnlace(url) {
    history.pushState(null, '', url);
}

export function moverScroll(id) {
    const yOffset = -185;
    const element = document.getElementById(id);
    const y = element.getBoundingClientRect().top + window.pageYOffset + yOffset;

	window.scrollTo({ top: y, behavior: 'smooth' });
}