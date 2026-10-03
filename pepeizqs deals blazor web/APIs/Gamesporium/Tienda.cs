#nullable disable

using Herramientas;
using Herramientas.Afiliados;
using Juegos;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using Tiendas2;

namespace APIs.Gamesporium
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "gamesporium",
				Nombre = "Gamesporium",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/gamesporium_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/gamesporium_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/gamesporium_icono.webp",
				Color = "#141414",
				AdminUso = true,
				UsuarioUso = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa }
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			return Daisycon.CrearEnlaceAfiliado(enlace);
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			string enlace = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				enlace = Daisycon.GamesporiumEuropa;
			}
			//else if (region == TiendaRegion.EstadosUnidos)
			//{
			//	enlace = "https://feed.mulwi.com/f/b74893-2/general_us.xml";
			//}

			if (string.IsNullOrEmpty(enlace) == true)
			{
				return;
			}

			string html = await Decompiladores.Estandar(enlace);

			if (string.IsNullOrEmpty(html) == false)
			{
				var resultados = await Daisycon.ObtenerFeed(enlace);

				if (resultados?.Info?.TotalProductos > 0)
				{
					List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

					foreach (var programa in resultados.Programas)
					{
						foreach (var resultado in programa.Productos)
						{
							if (resultado.Info?.Descuento > 0 && resultado.Info?.EnStock == true && resultado.Info?.Moneda == "EUR" && resultado.Info?.PlataformaActivacion == "Steam" && resultado.Info?.Region == "ES")
							{
								string nombre = WebUtility.HtmlDecode(resultado.Info?.Nombre);

								string enlaceJuego = Daisycon.LimpiarEnlace(resultado.Info?.Url, "https://gamesporium.com");

								string imagen = resultado.Info?.ImagenUrl;

								JuegoPrecio oferta = new JuegoPrecio
								{
									Nombre = nombre,
									Enlace = enlaceJuego,
									Imagen = imagen,
									Precio = resultado.Info?.PrecioActual.Value ?? 0,
									Descuento = resultado.Info?.Descuento.Value ?? 0,
									Tienda = Generar().Id,
									DRM = JuegoDRM.Steam,
									FechaDetectado = DateTime.Now,
									FechaActualizacion = DateTime.Now,
									Moneda = JuegoMoneda.Euro
								};

								ofertas.Add(oferta);
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
								BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
							}

							juegos2 += lote.Count;

							try
							{
								await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, juegos2);
							}
							catch (Exception ex)
							{
								BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
							}
						}
					}
				}
			}
		}
	}

	[XmlRoot("rss")]
	public class GamesporiumJuegos
	{
		[XmlArray("products")]
		[XmlArrayItem("product")]
		public List<GamesporiumJuego> Juegos { get; set; }
	}

	public class GamesporiumJuego
	{
		[XmlElement("ProductName")]
		public string Nombre { get; set; }

		[XmlElement("ProductURL")]
		public string Enlace { get; set; }

		[XmlElement("ImageURL")]
		public string Imagen { get; set; }

		[XmlElement("Currency")]
		public string Moneda { get; set; }

		[XmlElement("CompareAtPrice")]
		public string PrecioRebajado { get; set; }

		[XmlElement("Price")]
		public string PrecioBase { get; set; }

		[XmlElement("DRM")]
		public string DRM { get; set; }

		[XmlElement("WhitelistCountries")]
		public string PaisesAprobados { get; set; }

		[XmlElement("StockStatus")]
		public string StockEstado { get; set; }
	}
}
