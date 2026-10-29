#nullable disable

using Bundles2;
using Microsoft.JSInterop;

namespace Servicios
{

	public class DiseñoBundle
	{
		private readonly IJSRuntime _js;
		public BundleDiseño diseño { get; private set; } = BundleDiseño.Lista;
		public event Action OnChange;

		public DiseñoBundle(IJSRuntime js) => _js = js;

		public void EstablecerDesdeServidor(string cookieValor)
		{
			if (int.TryParse(cookieValor, out int valor) && Enum.IsDefined(typeof(BundleDiseño), valor))
			{
				diseño = (BundleDiseño)valor;
			}
		}

		public async Task AsignarDiseño(BundleDiseño nuevoDiseño)
		{
			diseño = nuevoDiseño;
			OnChange?.Invoke();

			try
			{
				await _js.InvokeVoidAsync("setCookie", "user_bundledesign", diseño, 365);
			}
			catch (Exception)
			{
			}
		}
	}
}