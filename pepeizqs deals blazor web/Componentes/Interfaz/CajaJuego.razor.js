export function copiarTexto(text) {
    if (navigator.clipboard) {
        navigator.clipboard.writeText(text);
    }
}