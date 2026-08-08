#nullable disable

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
				NoOficial = contexto?.HttpContext?.Request?.Cookies.TryGetValue("noofficial", out string valorNoOficial) == true && bool.TryParse(valorNoOficial, out bool valorNoOficialBool) ? valorNoOficialBool : false,
				Marketplace = contexto?.HttpContext?.Request?.Cookies.TryGetValue("marketplace", out string valorMarketplace) == true && bool.TryParse(valorMarketplace, out bool valorMarketplaceBool) ? valorMarketplaceBool : false
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
		public bool NoOficial { get; set; } = false;
		public bool Marketplace { get; set; } = false;
	}
}
