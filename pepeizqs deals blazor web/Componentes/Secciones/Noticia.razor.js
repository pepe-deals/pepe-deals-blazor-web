export function cambiarEnlace(url) {
    history.pushState(null, '', url);
}

export function copiarTexto(text) {
    if (navigator.clipboard) {
        navigator.clipboard.writeText(text);
    }
}