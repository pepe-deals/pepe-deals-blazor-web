using Microsoft.JSInterop;

namespace Servicios
{

	public class Marketplace
	{
		private readonly IJSRuntime _js;
		public bool valor { get; private set; } = false;
		public event Action? OnChange;

		public Marketplace(IJSRuntime js) => _js = js;

		public async Task AsignarValor(bool nuevoValor)
		{
			valor = nuevoValor;
			OnChange?.Invoke();
			await _js.InvokeVoidAsync("setCookie", "marketplace", valor, 365);
		}
	}
}
