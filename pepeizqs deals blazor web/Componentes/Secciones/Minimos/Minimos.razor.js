export function cambiarEnlace(url) {
    history.pushState(null, '', url);
}