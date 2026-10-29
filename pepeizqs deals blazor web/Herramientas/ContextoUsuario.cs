#nullable disable

using Bundles2;
using Microsoft.AspNetCore.Components;
using Tiendas2;

namespace Herramientas
{

	public static class ContextoUsuario
	{
		public static ContextoUsuarioDatos Leer(IHttpContextAccessor contexto)
		{
			ContextoUsuarioDatos datos = new ContextoUsuarioDatos
			{
				Idioma = contexto?.HttpContext?.Request?.Headers["Accept-Language"].ToString().Split(";")?.FirstOrDefault()?.Split(",").FirstOrDefault() ?? "en",
				UsuarioId = contexto?.HttpContext?.User?.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value,
				UserAgent = contexto?.HttpContext?.Request?.Headers?.UserAgent.ToString(),
				Dominio = contexto?.HttpContext?.Request?.Host.Value,
				Region = contexto?.HttpContext?.Request?.Cookies.TryGetValue("user_currency", out string valorRegion) == true && int.TryParse(valorRegion, out int valorRegionInt) && Enum.IsDefined(typeof(TiendaRegion), valorRegionInt) ? (TiendaRegion)valorRegionInt : TiendaRegion.Europa,
				NoOficial = contexto?.HttpContext?.Request?.Cookies.TryGetValue("noofficial", out string valorNoOficial) == true && bool.TryParse(valorNoOficial, out bool valorNoOficialBool) ? valorNoOficialBool : true,
				Marketplace = contexto?.HttpContext?.Request?.Cookies.TryGetValue("marketplace", out string valorMarketplace) == true && bool.TryParse(valorMarketplace, out bool valorMarketplaceBool) ? valorMarketplaceBool : true,
				DiseñoBundle = contexto?.HttpContext?.Request?.Cookies.TryGetValue("user_bundledesign", out string valorDiseño) == true && int.TryParse(valorDiseño, out int valorDiseñoInt) && Enum.IsDefined(typeof(BundleDiseño), valorDiseñoInt) ? (BundleDiseño)valorDiseñoInt : BundleDiseño.Cuadricula
			};

			return datos;
        }
	}

    public class ContextoUsuarioDatos
	{
		public string Idioma {  get; set; }
        public string UsuarioId { get; set; }
        public string UserAgent { get; set; }
		public string Dominio { get; set; }
		public TiendaRegion Region { get; set; } = TiendaRegion.Europa;
		public bool NoOficial { get; set; } = true;
		public bool Marketplace { get; set; } = true;
		public BundleDiseño DiseñoBundle { get; set; } = BundleDiseño.Cuadricula;
	}

	public class ContextoUsuarioServicio : IDisposable
	{
		private readonly PersistentComponentState _estado;
		private readonly PersistingComponentStateSubscription _suscripcion;

		public ContextoUsuarioDatos Datos { get; private set; }
		public event Action OnCambio;

		public ContextoUsuarioServicio(IHttpContextAccessor accessor, PersistentComponentState estado)
		{
			_estado = estado;

			if (_estado.TryTakeFromJson<ContextoUsuarioDatos>("contextoUsuario", out var guardado))
			{
				Datos = guardado;
			}
			else
			{
				Datos = ContextoUsuario.Leer(accessor);
				_suscripcion = _estado.RegisterOnPersisting(() =>
				{
					_estado.PersistAsJson("contextoUsuario", Datos);
					return Task.CompletedTask;
				});
			}
		}

		public void Actualizar(Action<ContextoUsuarioDatos> cambio)
		{
			cambio(Datos);
			OnCambio?.Invoke();
		}

		public void Dispose() => _suscripcion.Dispose();
	}
}
