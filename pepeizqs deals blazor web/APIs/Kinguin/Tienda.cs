#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using Tiendas2;

namespace APIs.Kinguin
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "kinguin",
				Nombre = "Kinguin",
				Tipo = TiendaTipo.Marketplace,
				ImagenLogo = "/imagenes/tiendas/kinguin_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/kinguin_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/kinguin_icono.ico",
				Color = "#558205",
				AdminUso = true,
				UsuarioUso = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa  }
			};

			return tienda;
		}

		public static string LimpiarEnlace(string enlace)
		{
			if (string.IsNullOrEmpty(enlace) == true)
			{
				return enlace;
			}

			int posicionU = enlace.IndexOf("&u=", StringComparison.OrdinalIgnoreCase);

			if (posicionU == -1)
			{
				return enlace;
			}

			string despuesDeU = enlace[(posicionU + 3)..];
			int posicionAmpersand = despuesDeU.IndexOf('&');
			string urlCodificada = posicionAmpersand >= 0 ? despuesDeU[..posicionAmpersand] : despuesDeU;
			string urlDecodificada = WebUtility.UrlDecode(urlCodificada);

			int posicionInterrogacion = urlDecodificada.IndexOf('?');

			if (posicionInterrogacion >= 0)
			{
				urlDecodificada = urlDecodificada[..posicionInterrogacion];
			}

			return urlDecodificada;
		}

		public static string Referido(TiendaRegion region, string enlace)
		{
			if (string.IsNullOrEmpty(enlace) == true)
			{
				return enlace;
			}

			string campaignId = string.Empty;
			string moneda = string.Empty;
			string intsrc = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				campaignId = "3885020";
				moneda = "eur";
				intsrc = "CATF_33299";
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				campaignId = "3834946";
				moneda = "usd";
				intsrc = "CATF_32047"; 
			}

			string destino = $"{enlace}?__currency={moneda}";
			string destinoCodificado = WebUtility.UrlEncode(destino);

			return $"https://kinguin.sjv.io/c/1382810/{campaignId}/50048?u={destinoCodificado}&intsrc={intsrc}";
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			List<ImpactCatalogItem> resultados = new List<ImpactCatalogItem>();

			if (region == TiendaRegion.Europa)
			{
				resultados = await Herramientas.Impact.ObtenerCatalogo("33299");
			}
			//else if (region == TiendaRegion.EstadosUnidos)
			//{
			//	resultados = await Herramientas.Impact.ObtenerCatalogo("32047");
			//}

			if (resultados?.Count > 0)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				foreach (var resultado in resultados)
				{
					if (string.IsNullOrEmpty(resultado.Nombre) == false)
					{
						JuegoDRM drmJuego = JuegoDRM.NoEspecificado;

						if (region == TiendaRegion.Europa && resultado.Nombre.Contains("EU Steam CD Key") == true)
						{
							drmJuego = JuegoDRM.Steam;
						}
						else if (region == TiendaRegion.EstadosUnidos && resultado.Nombre.Contains("US Steam CD Key") == true)
						{
							drmJuego = JuegoDRM.Steam;
						}

						if (drmJuego != JuegoDRM.NoEspecificado)
						{
							if (string.IsNullOrEmpty(resultado.Disponibilidad) == false && resultado.Disponibilidad == "InStock" && resultado.Nombre.Contains("EN Language Only") == false)
							{
								string nombre = WebUtility.HtmlDecode(resultado.Nombre);

								nombre = nombre.Replace("EU Steam CD Key", null);
								nombre = nombre.Replace("US Steam CD Key", null);
								nombre = nombre.Trim();

								string enlaceJuego = LimpiarEnlace(resultado.Url);
								enlaceJuego = enlaceJuego.Replace("https://www.kinguin.net/es/", "https://www.kinguin.net/");

								string imagen = resultado.ImagenUrl;

								JuegoDRM drm = JuegoDRM.Steam;

								JuegoPrecio oferta = new JuegoPrecio
								{
									Nombre = nombre,
									Enlace = enlaceJuego,
									Imagen = imagen,
									Moneda = JuegoMoneda.Euro,
									Precio = resultado.PrecioActual.Value,
									Descuento = 0,
									Tienda = Generar().Id,
									DRM = drm,
									FechaDetectado = DateTime.Now,
									FechaActualizacion = DateTime.Now
								};

								if (region == TiendaRegion.EstadosUnidos)
								{
									oferta.Moneda = JuegoMoneda.Dolar;
								}

								if (drm == JuegoDRM.Steam)
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
