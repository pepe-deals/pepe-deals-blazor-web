#nullable disable

using Dapper;
using Herramientas;
using Juegos;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.PlayStation
{

	public static class Suscripcion
	{
		public static Suscripciones2.Suscripcion Generar()
		{
			Suscripciones2.Suscripcion ps = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.PlayStationPlus,
				Nombre = "PlayStation Plus Premium",
				ImagenLogo = "/imagenes/suscripciones/psplus_logo.webp",
				ImagenIcono = "/imagenes/suscripciones/psplus_icono.webp",
				Enlace = "https://www.playstation.com/ps-plus/#subscriptions",
				DRMDefecto = JuegoDRM.PlayStation,
				UsuarioEnlacesEspecificos = true,
				ParaSiempre = false,
				Precio = 18.99,
				AdminPendientes = true,
				TablaPendientes = "suscripcionplaystationplus",
				SoloStreaming = true,
				AdminAñadir = false,
				UsuarioPuedeAbrir = false
			};

			return ps;
		}
	}

	public class PsPlusLetra
	{
		[JsonPropertyName("count")]
		public int Cantidad { get; set; }

		[JsonPropertyName("games")]
		public List<PsPlusJuego> Juegos { get; set; }
	}

	public class PsPlusJuego
	{
		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("imageUrl")]
		public string Imagen { get; set; }

		[JsonPropertyName("conceptUrl")]
		public string Enlace { get; set; }

		[JsonPropertyName("streamingSupported")]
		public bool AceptaStreaming { get; set; }
	}
}
