#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using Tiendas2;

namespace APIs.Gamesplanet
{
	public static class Tienda
	{
		public static Tiendas2.Tienda GenerarUk()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "gamesplanetuk",
				Nombre = "Gamesplanet (UK)",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/gamesplanet_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/gamesplanetuk_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/gamesplanet_icono.webp",
				Color = "#000",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static Tiendas2.Tienda GenerarFr()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "gamesplanetfr",
				Nombre = "Gamesplanet (FR)",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/gamesplanet_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/gamesplanetfr_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/gamesplanet_icono.webp",
				Color = "#000",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static Tiendas2.Tienda GenerarDe()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "gamesplanetde",
				Nombre = "Gamesplanet (DE)",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/gamesplanet_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/gamesplanetde_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/gamesplanet_icono.webp",
				Color = "#000",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static Tiendas2.Tienda GenerarUs()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "gamesplanetus",
				Nombre = "Gamesplanet (US)",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/gamesplanet_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/gamesplanetus_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/gamesplanet_icono.webp",
				Color = "#000",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			return enlace + "?ref=pepeizq";
		}

		private static GamesplanetJuego LeerJuegoXml(XmlReader reader)
		{
			GamesplanetJuego juego = new GamesplanetJuego();

			while (reader.Read() == true)
			{
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "product")
					break;

				if (reader.NodeType == XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "name":
							juego.Nombre = reader.ReadElementContentAsString();
							break;
						case "uid":
							juego.Id = reader.ReadElementContentAsString();
							break;
						case "link":
							juego.Enlace = reader.ReadElementContentAsString();
							break;
						case "price":
							juego.PrecioRebajado = reader.ReadElementContentAsString();
							break;
						case "price_base":
							juego.PrecioBase = reader.ReadElementContentAsString();
							break;
						case "teaser620":
							juego.Imagen = reader.ReadElementContentAsString();
							break;
						case "delivery_type":
							juego.DRM = reader.ReadElementContentAsString();
							break;
						case "steam_id":
							juego.SteamId = reader.ReadElementContentAsString();
							break;
						case "country_whitelist":
							juego.PaisesAprobados = reader.ReadElementContentAsString();
							break;
						case "country_blacklist":
							juego.PaisesRestringidos = reader.ReadElementContentAsString();
							break;
						default:
							reader.Skip();
							break;
					}
				}
			}

			return juego;
		}

		private static bool MirarRegiones(GamesplanetJuego juego, TiendaRegion region)
		{
			bool buscar = true;

			if (string.IsNullOrEmpty(juego.PaisesRestringidos) == false)
			{
				string[] paises = juego.PaisesRestringidos.Split(',');

				if (region == TiendaRegion.Europa && paises.Contains("es", StringComparer.OrdinalIgnoreCase))
				{
					buscar = false;
				}
				else if (region == TiendaRegion.EstadosUnidos && paises.Contains("us", StringComparer.OrdinalIgnoreCase))
				{
					buscar = false;
				}
			}

			if (buscar == true && string.IsNullOrEmpty(juego.PaisesAprobados) == false)
			{
				string[] paises = juego.PaisesAprobados.Split(',');
				bool encontrado = (region == TiendaRegion.Europa && paises.Contains("es", StringComparer.OrdinalIgnoreCase)) ||
								 (region == TiendaRegion.EstadosUnidos && paises.Contains("us", StringComparer.OrdinalIgnoreCase));

				if (encontrado == false)
				{
					buscar = false;
				}
			}

			return buscar;
		}

		private static JuegoPrecio ConvertirAOferta(GamesplanetJuego juego, TiendaRegion region, string tiendaId)
		{
			if (decimal.TryParse(juego.PrecioBase, out decimal precioBase) == false ||
				decimal.TryParse(juego.PrecioRebajado, out decimal precioRebajado) == false)
			{
				return null;
			}

			int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

			if (descuento <= 0)
			{
				return null;
			}

			string nombre = WebUtility.HtmlDecode(juego.Nombre);

			JuegoDRM drm = JuegoDRM2.Traducir(juego.DRM, GenerarUk().Id);

			JuegoMoneda monedaUsar = JuegoMoneda.Euro;

			if (tiendaId == GenerarUk().Id)
			{
				monedaUsar = JuegoMoneda.Libra;
			}
			else if (tiendaId == GenerarUs().Id)
			{
				monedaUsar = JuegoMoneda.Dolar;
			}

			return new JuegoPrecio
			{
				Nombre = nombre,
				Enlace = juego.Enlace,
				Imagen = juego.Imagen,
				Moneda = monedaUsar,
				Precio = precioRebajado,
				Descuento = descuento,
				Tienda = tiendaId,
				DRM = drm,
				FechaDetectado = DateTime.Now,
				FechaActualizacion = DateTime.Now
			};
		}

		public static async Task BuscarOfertasUk(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, GenerarUk().Id, DateTime.Now, 0);

            string htmluk = await Decompiladores.Estandar("https://uk.gamesplanet.com/api/v1/products/feed.xml");

			if (string.IsNullOrEmpty(htmluk) == false)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				using (var reader = XmlReader.Create(new StringReader(htmluk)))
				{
					reader.MoveToContent();
					while (reader.Read() == true)
					{
						if (reader.NodeType == XmlNodeType.Element && reader.Name == "product")
						{
							var juego = LeerJuegoXml(reader);

							if (juego != null && MirarRegiones(juego, region) == true)
							{
								var oferta = ConvertirAOferta(juego, region, GenerarUk().Id);

								if (oferta != null)
								{
									ofertas.Add(oferta);
								}
							}
						}
					}
				}

				if (ofertas?.Count > 0)
				{
					int juegos2 = 0;

					int tamaño = 500;
					var lotes = ofertas
						.Select((oferta, indice) => new { oferta, indice })
						.GroupBy(x => x.indice / tamaño)
						.Select(g => g.Select(x => x.oferta).ToList())
						.ToList();

					foreach (var lote in lotes)
					{
						try
						{
							await BaseDatos.Tiendas.Comprobar.Resto(region, lote);
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje(GenerarUk().Id, ex);
						}

						juegos2 += lote.Count;

						try
						{
							await BaseDatos.Admin.Actualizar.Tiendas(region, GenerarUk().Id, DateTime.Now, juegos2);
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje(GenerarUk().Id, ex);
						}
					}
				}
			}
		}

		public static async Task BuscarOfertasFr(TiendaRegion region)
        {
			await BaseDatos.Admin.Actualizar.Tiendas(region, GenerarFr().Id, DateTime.Now, 0);

			string htmlfr = await Decompiladores.Estandar("https://fr.gamesplanet.com/api/v1/products/feed.xml");

			if (string.IsNullOrEmpty(htmlfr) == false)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				using (var reader = XmlReader.Create(new StringReader(htmlfr)))
				{
					reader.MoveToContent();
					while (reader.Read() == true)
					{
						if (reader.NodeType == XmlNodeType.Element && reader.Name == "product")
						{
							var juego = LeerJuegoXml(reader);

							if (juego != null && MirarRegiones(juego, region) == true)
							{
								var oferta = ConvertirAOferta(juego, region, GenerarFr().Id);

								if (oferta != null)
								{
									ofertas.Add(oferta);
								}
							}
						}
					}
				}

				if (ofertas?.Count > 0)
				{
					int juegos2 = 0;

					int tamaño = 500;
					var lotes = ofertas
						.Select((oferta, indice) => new { oferta, indice })
						.GroupBy(x => x.indice / tamaño)
						.Select(g => g.Select(x => x.oferta).ToList())
						.ToList();

					foreach (var lote in lotes)
					{
						try
						{
							await BaseDatos.Tiendas.Comprobar.Resto(region, lote);
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje(GenerarFr().Id, ex);
						}

						juegos2 += lote.Count;

						try
						{
							await BaseDatos.Admin.Actualizar.Tiendas(region, GenerarFr().Id, DateTime.Now, juegos2);
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje(GenerarFr().Id, ex);
						}
					}
				}
			}
		}

		public static async Task BuscarOfertasDe(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, GenerarDe().Id, DateTime.Now, 0);

			string htmlde = await Decompiladores.Estandar("https://de.gamesplanet.com/api/v1/products/feed.xml");

			if (string.IsNullOrEmpty(htmlde) == false)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				using (var reader = XmlReader.Create(new StringReader(htmlde)))
				{
					reader.MoveToContent();
					while (reader.Read() == true)
					{
						if (reader.NodeType == XmlNodeType.Element && reader.Name == "product")
						{
							var juego = LeerJuegoXml(reader);

							if (juego != null && MirarRegiones(juego, region) == true)
							{
								var oferta = ConvertirAOferta(juego, region, GenerarDe().Id);

								if (oferta != null)
								{
									ofertas.Add(oferta);
								}
							}
						}
					}
				}

				if (ofertas?.Count > 0)
				{
					int juegos2 = 0;

					int tamaño = 500;
					var lotes = ofertas
						.Select((oferta, indice) => new { oferta, indice })
						.GroupBy(x => x.indice / tamaño)
						.Select(g => g.Select(x => x.oferta).ToList())
						.ToList();

					foreach (var lote in lotes)
					{
						try
						{
							await BaseDatos.Tiendas.Comprobar.Resto(region, lote);
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje(GenerarDe().Id, ex);
						}

						juegos2 += lote.Count;

						try
						{
							await BaseDatos.Admin.Actualizar.Tiendas(region, GenerarDe().Id, DateTime.Now, juegos2);
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje(GenerarDe().Id, ex);
						}
					}
				}
			}
		}

		public static async Task BuscarOfertasUs(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, GenerarUs().Id, DateTime.Now, 0);

			string htmlus = await Decompiladores.Estandar("https://us.gamesplanet.com/api/v1/products/feed.xml");

			if (string.IsNullOrEmpty(htmlus) == false)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				using (var reader = XmlReader.Create(new StringReader(htmlus)))
				{
					reader.MoveToContent();
					while (reader.Read() == true)
					{
						if (reader.NodeType == XmlNodeType.Element && reader.Name == "product")
						{
							var juego = LeerJuegoXml(reader);

							if (juego != null && MirarRegiones(juego, region) == true)
							{
								var oferta = ConvertirAOferta(juego, region, GenerarUs().Id);

								if (oferta != null)
								{
									ofertas.Add(oferta);
								}
							}
						}
					}
				}

				if (ofertas?.Count > 0)
				{
					int juegos2 = 0;

					int tamaño = 500;
					var lotes = ofertas
						.Select((oferta, indice) => new { oferta, indice })
						.GroupBy(x => x.indice / tamaño)
						.Select(g => g.Select(x => x.oferta).ToList())
						.ToList();

					foreach (var lote in lotes)
					{
						try
						{
							await BaseDatos.Tiendas.Comprobar.Resto(region, lote);
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje(GenerarUs().Id, ex);
						}

						juegos2 += lote.Count;

						try
						{
							await BaseDatos.Admin.Actualizar.Tiendas(region, GenerarUs().Id, DateTime.Now, juegos2);
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje(GenerarUs().Id, ex);
						}
					}
				}
			}
		}
    }

    [XmlRoot("products")]
	public class GamesplanetJuegos
	{
		[XmlElement("product")]
		public List<GamesplanetJuego> Juegos { get; set; }
	}

	public class GamesplanetJuego
	{
		[XmlElement("name")]
		public string Nombre { get; set; }

		[XmlElement("uid")]
		public string Id { get; set; }

		[XmlElement("link")]
		public string Enlace { get; set; }

		[XmlElement("price")]
		public string PrecioRebajado { get; set; }

		[XmlElement("price_base")]
		public string PrecioBase { get; set; }

		[XmlElement("teaser620")]
		public string Imagen { get; set; }

		[XmlElement("delivery_type")]
		public string DRM { get; set; }

		[XmlElement("steam_id")]
		public string SteamId { get; set; }

		[XmlElement("country_whitelist")]
		public string PaisesAprobados { get; set; }

		[XmlElement("country_blacklist")]
		public string PaisesRestringidos { get; set; }
	}
}
