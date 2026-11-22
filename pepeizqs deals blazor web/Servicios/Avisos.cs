using Microsoft.JSInterop;
using System.Globalization;

namespace Servicios
{
	public class Avisos
	{
		private readonly IJSRuntime _js;
		public DateTime? fecha { get; private set; }
		public event Action? OnChange;

		public Avisos(IJSRuntime js) => _js = js;

		public async Task AsignarFecha(DateTime nuevaFecha)
		{
			fecha = nuevaFecha.ToUniversalTime();
			OnChange?.Invoke();

			try
			{
				await _js.InvokeVoidAsync("setCookie", "avisos", fecha.Value.ToString("O"), 365);
			}
			catch (Exception)
			{
			}
		}

		public void CargarDesdeCookie(string? valorCookie)
		{
			if (DateTime.TryParse(valorCookie, CultureInfo.InvariantCulture,
				DateTimeStyles.RoundtripKind, out var resultado))
			{
				fecha = resultado;
			}
		}
	}
}