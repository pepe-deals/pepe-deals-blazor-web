using Microsoft.JSInterop;
using Tiendas2;

namespace Servicios
{
	public class Moneda
	{
		private readonly IJSRuntime _js;
		public TiendaRegion region { get; private set; } = TiendaRegion.Europa;
		public event Action? OnChange;

		public Moneda(IJSRuntime js) => _js = js;

		public void EstablecerDesdeServidor(string cookieValor)
		{
			if (int.TryParse(cookieValor, out int valor) && Enum.IsDefined(typeof(TiendaRegion), valor))
			{
				region = (TiendaRegion)valor;
			}
		}

		public async Task AsignarRegion(TiendaRegion nuevaRegion)
		{
			region = nuevaRegion;
			OnChange?.Invoke();

			try
			{
				await _js.InvokeVoidAsync("setCookie", "user_currency", region, 365);
			}
			catch (Exception)
			{
			}
		}
	}
}
