#nullable disable

using Dapper;
using Juegos;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace BaseDatos.Extension
{
	public class Extension
	{
		public int Id { get; set; }
		public string Nombre { get; set; }
		public List<ExtensionPrecio> MinimosHistoricos { get; set; }
		public List<ExtensionPrecio> PreciosActuales { get; set; }
		public List<ExtensionBundle> Bundles { get; set; }
		public int BundlesPasados { get; set; }	
		public List<ExtensionGratis> Gratis { get; set; }
		public List<ExtensionSuscripcion> Suscripciones { get; set; }
		public int IdSteam { get; set; }
		public int IdGOG { get; set; }
		public string SlugGOG { get; set; }
		public string SlugEpic { get; set; }
	}

	public class ExtensionPrecio
	{
		public JuegoPrecio Datos { get; set; }
		public string Tienda { get; set; }
		public string TiendaIcono { get; set; }
	}

	public class ExtensionBundle
	{
		public string Nombre { get; set; }
		public string Tienda { get; set; }
		public string Enlace { get; set; }
	}

	public class ExtensionGratis
	{
		public JuegoGratisJson Datos { get; set; }
		public string NombreGratis { get; set; }
		public string IconoGratis { get; set; }
	}

	public class ExtensionSuscripcion
	{
		public JuegoSuscripcionJson Datos { get; set; }
		public string NombreSuscripcion { get; set; }
		public string IconoSuscripcion { get; set; }
	}


	public static class Buscar
	{
		public static async Task<Extension> Steam3(string region, string id)
		{
			string precioMinimosHistoricos = string.Empty;
			string precioActualesTiendas = string.Empty;

			if (region == "eu")
			{
				precioMinimosHistoricos = "precioMinimosHistoricos";
				precioActualesTiendas = "precioActualesTiendas";
			}
			else if (region == "us")
			{
				precioMinimosHistoricos = "precioMinimosHistoricosUS";
				precioActualesTiendas = "precioActualesTiendasUS";
			}

			string buscar = $@"SELECT j.id, j.nombre, j.{precioMinimosHistoricos}, j.{precioActualesTiendas}, 
(
	SELECT b.tienda, b.nombre, b.enlace
	FROM bundles b
	INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
	WHERE bj.juegoId = j.id
		AND b.fechaEmpieza <= GETDATE()
		AND b.fechaTermina >= GETDATE()
	FOR JSON PATH
) AS bundles2,
(
	SELECT COUNT(*)
	FROM bundles b
	INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
	WHERE bj.juegoId = j.id
		AND b.fechaTermina < GETDATE()
) AS bundlesPasados,
(
    SELECT g.*, g.gratis AS Tipo
    FROM gratis g
    WHERE g.juegoId = j.id
    FOR JSON PATH
) as gratis2, 
(
    SELECT s.*, s.suscripcion AS Tipo
    FROM suscripciones s
    WHERE s.juegoId = j.id
    FOR JSON PATH
) as suscripciones2, j.idSteam, j.idGOG, j.slugGOG, j.slugEpic FROM juegos j WHERE idSteam='" + id + "'";

			return await GenerarDatos(region, buscar, "Steam " + id);
		}

		public static async Task<Extension> Gog3(string region, string slug)
		{
			string precioMinimosHistoricos = string.Empty;
			string precioActualesTiendas = string.Empty;

			if (region == "eu")
			{
				precioMinimosHistoricos = "precioMinimosHistoricos";
				precioActualesTiendas = "precioActualesTiendas";
			}
			else if (region == "us")
			{
				precioMinimosHistoricos = "precioMinimosHistoricosUS";
				precioActualesTiendas = "precioActualesTiendasUS";
			}

			string buscar = $@"SELECT j.id, j.nombre, j.{precioMinimosHistoricos}, j.{precioActualesTiendas},
(
	SELECT b.tienda, b.nombre, b.enlace
	FROM bundles b
	INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
	WHERE bj.juegoId = j.id
		AND b.fechaEmpieza <= GETDATE()
		AND b.fechaTermina >= GETDATE()
	FOR JSON PATH
) AS bundles2,
(
    SELECT g.*, g.gratis AS Tipo
    FROM gratis g
    WHERE g.juegoId = j.id
    FOR JSON PATH
) as gratis2, 
(
    SELECT s.*, s.suscripcion AS Tipo
    FROM suscripciones s
    WHERE s.juegoId = j.id
    FOR JSON PATH
) as suscripciones2, j.idSteam, j.idGOG, j.slugGOG, j.slugEpic FROM juegos j WHERE slugGOG='" + slug + "'";

			return await GenerarDatos(region, buscar, "GOG " + slug);
		}

		public static async Task<Extension> EpicGames3(string region, string slug)
		{
			string precioMinimosHistoricos = string.Empty;
			string precioActualesTiendas = string.Empty;

			if (region == "eu")
			{
				precioMinimosHistoricos = "precioMinimosHistoricos";
				precioActualesTiendas = "precioActualesTiendas";
			}
			else if (region == "us")
			{
				precioMinimosHistoricos = "precioMinimosHistoricosUS";
				precioActualesTiendas = "precioActualesTiendasUS";
			}

			string buscar = $@"SELECT j.id, j.nombre, j.{precioMinimosHistoricos}, j.{precioActualesTiendas},
(
	SELECT b.tienda, b.nombre, b.enlace
	FROM bundles b
	INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
	WHERE bj.juegoId = j.id
		AND b.fechaEmpieza <= GETDATE()
		AND b.fechaTermina >= GETDATE()
	FOR JSON PATH
) AS bundles2,
(
    SELECT g.*, g.gratis AS Tipo
    FROM gratis g
    WHERE g.juegoId = j.id
    FOR JSON PATH
) as gratis2, 
(
    SELECT s.*, s.suscripcion AS Tipo
    FROM suscripciones s
    WHERE s.juegoId = j.id
    FOR JSON PATH
) as suscripciones2, j.idSteam, j.idGOG, j.slugGOG, j.slugEpic FROM juegos j WHERE slugEpic='" + slug + "'";

			return await GenerarDatos(region, buscar, "Epic " + slug);
		}

		private static async Task<Extension> GenerarDatos(string region, string buscar, string id)
		{
			if (buscar == null)
			{
				return null;
			}

			try
			{
				var fila2 = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<dynamic>(buscar);
				});

				IDictionary<string, object> fila = (IDictionary<string, object>)fila2;

				Extension extension = new Extension
				{
					MinimosHistoricos = new List<ExtensionPrecio>(),
					PreciosActuales = new List<ExtensionPrecio>(),
					Bundles = new List<ExtensionBundle>(),
					Gratis = new List<ExtensionGratis>(),
					Suscripciones = new List<ExtensionSuscripcion>()
				};

				if (fila == null)
				{
					return null;
				}

				string CogerString(string columna)
				{
					return fila.TryGetValue(columna, out var v) && v != null ? v.ToString() : null;
				}

				int CogerInt(string columna)
				{
					return fila.TryGetValue(columna, out var v) && v != null ? Convert.ToInt32(v) : 0;
				}

				extension.Id = CogerInt("id");
				extension.Nombre = CogerString("nombre");
				extension.IdSteam = CogerInt("idSteam");
				extension.IdGOG = CogerInt("idGOG");
				extension.SlugGOG = CogerString("slugGOG");
				extension.SlugEpic = CogerString("slugEpic");


				if (region == "eu")
				{
					CargarPrecios(
						CogerString("precioMinimosHistoricos"), extension.MinimosHistoricos
					);

					CargarPrecios(
						CogerString("precioActualesTiendas"), extension.PreciosActuales
					);
				}
				else if (region == "us")
				{
					CargarPrecios(
						CogerString("precioMinimosHistoricosUS"), extension.MinimosHistoricos
					);

					CargarPrecios(
						CogerString("precioActualesTiendasUS"), extension.PreciosActuales
					);
				}
				
				string jsonBundles = CogerString("bundles2");
				if (string.IsNullOrEmpty(jsonBundles) == false)
				{
					JsonSerializerOptions opciones = new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true,
						UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
					};

					extension.Bundles = JsonSerializer.Deserialize<List<ExtensionBundle>>(jsonBundles, opciones);
				}

				extension.BundlesPasados = CogerInt("bundlesPasados");

				string jsonGratis = CogerString("gratis2");
				if (string.IsNullOrEmpty(jsonGratis) == false)
				{
					List<JuegoGratisJson> lista = JsonSerializer.Deserialize<List<JuegoGratisJson>>(jsonGratis);

					if (lista?.Count > 0)
					{
						extension.Gratis = new List<ExtensionGratis>();

						foreach (var gratis in lista)
						{
							gratis.Enlace = Herramientas.EnlaceAcortador.Generar(gratis.Enlace, gratis.Tipo, false, false);

							extension.Gratis.Add(new ExtensionGratis
							{
								Datos = gratis,
								NombreGratis = Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).Nombre,
								IconoGratis = Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).ImagenIcono
							});
						}
					}
				}

				var jsonSuscripciones = CogerString("suscripciones2");
				if (string.IsNullOrEmpty(jsonSuscripciones) == false)
				{
					var lista = JsonSerializer.Deserialize<List<JuegoSuscripcionJson>>(jsonSuscripciones);

					if (lista?.Count > 0)
					{
						extension.Suscripciones = new List<ExtensionSuscripcion>();

						foreach (var suscripcion in lista)
						{
							suscripcion.Enlace = Herramientas.EnlaceAcortador.Generar(suscripcion.Enlace, suscripcion.Tipo, false, false);

							extension.Suscripciones.Add(new ExtensionSuscripcion
							{
								Datos = suscripcion,
								NombreSuscripcion = Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre,
								IconoSuscripcion = Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).ImagenIcono
							});
						}
					}
				}

				return extension;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Extension Generar " + id, ex, false);
			}

			return null;
		}

		private static void CargarPrecios(string json, List<ExtensionPrecio> destino)
		{
			if (string.IsNullOrEmpty(json) == true)
			{
				return;
			}

			List<JuegoPrecio> lista = JsonSerializer.Deserialize<List<JuegoPrecio>>(json);
			if (lista == null)
			{ 
				return;
			}

			foreach (JuegoPrecio precio in lista)
			{
				if (precio?.Tienda == null)
				{
					continue;
				}
				else if (precio.Tienda == APIs.Steam.Tienda.GenerarBundles().Id)
				{
					continue;
				}

				if (precio.Precio == 0)
				{
					continue;
				}

				Tiendas2.Tienda tienda = Tiendas2.TiendasCargar.DevolverTienda(precio.Tienda);
				if (tienda == null)
				{
					continue;
				}

				precio.Enlace = Herramientas.EnlaceAcortador.Generar(TiendaRegion.Europa, precio.Enlace, precio.Tienda, false, false);

				destino.Add(new ExtensionPrecio
				{
					Datos = precio,
					Tienda = tienda.Nombre,
					TiendaIcono = tienda.ImagenIcono
				});
			}
		}
	}
}
