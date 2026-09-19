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
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
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
			else if (region == TiendaRegion.EstadosUnidos)
			{
				resultados = await Herramientas.Impact.ObtenerCatalogo("32047");
			}

			if (resultados?.Count > 0)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				foreach (var resultado in resultados)
				{
					if (string.IsNullOrEmpty(resultado.Nombre) == false)
					{
						JuegoDRM drmJuego = JuegoDRM.NoEspecificado;

						if (region == TiendaRegion.Europa && (
							resultado.Nombre.ToLower().Contains("eu steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("eu pc steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("eu/mea/au/nz steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("eu/na steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("eu/na pc steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("eu (without ru/cis) steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("eu (without de) steam cd key") == true))
						{
							drmJuego = JuegoDRM.Steam;
						}
						else if (region == TiendaRegion.EstadosUnidos && (
							resultado.Nombre.ToLower().Contains("us steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("us pc steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("na steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("na pc steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("na/latam steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("na/latam pc steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("eu/na steam cd key") == true ||
							resultado.Nombre.ToLower().Contains("eu/na pc steam cd key") == true))
						{
							drmJuego = JuegoDRM.Steam;
						}
						else
						{
							if (resultado.Nombre.ToLower().Contains("steam cd key") == true)
							{
								drmJuego = JuegoDRM.Steam;
							}

							if (region == TiendaRegion.EstadosUnidos && resultado.Nombre.Contains("EMEA Steam CD Key") == true)
							{
								drmJuego = JuegoDRM.NoEspecificado;
							}
						}

						string[] textosDrmNoEspecificado = new[]
						{
							"languages only",
							"language only",
							"ru vpn required steam pc cd key",
							"ru vpn required steam cd key",
							"ru vpn required pc steam cd key",
							"ru vpn activated steam pc cd key",
							"ru vpn activated steam cd key",
							"ru vpn activated pc steam cd key",
							"ru/cis pc steam cd key",
							"ru/cis steam cd key",
							"ru pc steam cd key",
							"ru steam cd key",
							"ru/cis/tr pc steam cd key",
							"ru/cis/tr steam cd key",
							"cn vpn activated steam pc cd key",
							"cn vpn activated steam cd key",
							"asia pc steam cd key",
							"asia steam cd key",
							"latam pc steam cd key",
							"latam steam cd key",
							"latin america pc steam cd key",
							"latin america steam cd key",
							"anzac pc steam cd key",
							"anzac steam cd key",
							"mea pc steam cd key", 
							"mea steam cd key",
							"tr pc steam cd key",
							"tr steam cd key",
							"jp pc steam cd key",
							"jp steam cd key",
							"pl pc steam cd key",
							"pl steam cd key",
							"sea/oceania pc steam cd key",
							"sea/oceania steam cd key",
							"asia/oceania pc steam cd key",
							"asia/oceania steam cd key",
							"asia/south america pc steam cd key",
							"asia/south america steam cd key",
							"asia/pacific pc steam cd key",
							"asia/pacific steam cd key",
							"asia/africa pc steam cd key",
							"asia/africa steam cd key",
							"asia + africa pc steam cd key",
							"asia + africa steam cd key",
							"asia pc steam cd key",
							"asia dlc steam cd key",
							"asia steam cd key",
							"south america pc steam cd key",
							"south america steam cd key",
							"mena/af pc steam cd key",
							"mena/af steam cd key",
							"mena pc steam cd key",
							"mena steam cd key",
							"cn pc steam cd key",
							"cn steam cd key",
							"sea pc steam cd key",
							"sea steam cd key",
							"de pc steam cd key",
							"de steam cd key",
							"in pc steam cd key",
							"in steam cd key",
							"pl pc steam cd key",
							"pl steam cd key",
							"au pc steam cd key",
							"au steam cd key",
							"cz pc steam cd key",
							"cz steam cd key",
							"cis pc steam cd key",
							"cis steam cd key",
							"br pc steam cd key",
							"br steam cd key",
							"au pc steam cd key",
							"au steam cd key",
							"middle east pc steam cd key",
							"middle east steam cd key",
							"(pcr) pc steam cd key",
							"(pcr) steam cd key",
							"outside europe pc steam cd key",
							"outside europe steam cd key",
							"ar/by/ba/br/in/me/ru/rs/tr/ua pc steam cd key",
							"ar/by/ba/br/in/me/ru/rs/tr/ua steam cd key",
							"za/kw/qa/sa/tr/ae pc steam cd key",
							"za/kw/qa/sa/tr/ae steam cd key",
							"fr pc steam cd key",
							"fr steam cd key",
							"meza pc steam cd key",
							"meza steam cd key",
							"ar pc steam cd key",
							"ar steam cd key",
							"tr/africa pc steam cd key",
							"tr/africa steam cd key",
							"cis pc steam cd key",
							"cis steam cd key",
							"brasil pc steam cd key",
							"brasil steam cd key",
							"china pc steam cd key",
							"china steam cd key"
						};

						string nombreMinusculas = resultado.Nombre.ToLower();

						if (textosDrmNoEspecificado.Any(frase => nombreMinusculas.Contains(frase)))
						{
							drmJuego = JuegoDRM.NoEspecificado;
						}

						string[] textos2DrmNoEspecificado = new[]
						{
							"-steam-gift-pc-bundle",
							"-steam-gift"
						};

						string enlaceMinusculas = resultado.Url.ToLower();

						if (textos2DrmNoEspecificado.Any(frase => enlaceMinusculas.Contains(frase)))
						{
							drmJuego = JuegoDRM.NoEspecificado;
						}

						if (drmJuego != JuegoDRM.NoEspecificado)
						{
							if (string.IsNullOrEmpty(resultado.Disponibilidad) == false && resultado.Disponibilidad == "InStock")
							{
								string nombre = WebUtility.HtmlDecode(resultado.Nombre);

								nombre = nombre.Replace("EU/NA PC Steam CD Key", null);
								nombre = nombre.Replace("EU/NA Steam CD Key", null);
								nombre = nombre.Replace("EU PC Steam CD Key", null);
								nombre = nombre.Replace("EU Steam CD Key", null);
								nombre = nombre.Replace("EU PC Steam CD Key", null);
								nombre = nombre.Replace("EU (without RU/CIS) Steam CD Key", null);
								nombre = nombre.Replace("EU (without DE) Steam CD Key", null);
								nombre = nombre.Replace("US Steam CD Key", null);
								nombre = nombre.Replace("US PC Steam CD Key", null);
								nombre = nombre.Replace("NA Steam CD Key", null);
								nombre = nombre.Replace("NA PC Steam CD Key", null);
								nombre = nombre.Replace("RoW Steam CD Key", null);
								nombre = nombre.Replace("RoW PC Steam CD Key", null);
								nombre = nombre.Replace("PC Steam CD Key", null);
								nombre = nombre.Replace("Steam CD Key", null);
								nombre = nombre.Replace("Steam CD key", null);
								nombre = nombre.Replace("DLC", null);
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
