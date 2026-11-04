#nullable disable

using Herramientas;
using Herramientas.Afiliados;
using Juegos;
using System.Net;
using Tiendas2;

namespace APIs.Allyouplay
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "allyouplay",
				Nombre = "Allyouplay",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/allyouplay_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/allyouplay_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/allyouplay_icono.ico",
				Color = "#ff4081",
				AdminUso = true,
				UsuarioUso = true
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
				campaignId = "2866230";
				moneda = "eur";
				intsrc = "CATF_22827";
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				campaignId = "3433719";
				moneda = "usd";
				intsrc = "CATF_29429";
			}

			string destino = $"{enlace}?__currency={moneda}";
			string destinoCodificado = WebUtility.UrlEncode(destino);

			return $"https://anandadigitalbv.sjv.io/c/1382810/{campaignId}/30655?u={destinoCodificado}&intsrc={intsrc}";
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			List<ImpactCatalogItem> resultados = new List<ImpactCatalogItem>();

			if (region == TiendaRegion.Europa)
			{
				resultados = await Impact.ObtenerCatalogo("22827");
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				resultados = await Impact.ObtenerCatalogo("29429");
			}

			if (resultados?.Count > 0)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				foreach (var resultado in resultados)
				{
					if (string.IsNullOrEmpty(resultado.Nombre) == false)
					{
						string enlaceJuego = LimpiarEnlace(resultado.Url);

						JuegoDRM drmJuego = AsignarDRM(resultado.Nombre, enlaceJuego);

						if (drmJuego != JuegoDRM.NoEspecificado && string.IsNullOrEmpty(resultado.Disponibilidad) == false && resultado.Disponibilidad == "InStock")
						{
							if (string.IsNullOrEmpty(resultado.PrecioActualTexto) == false && string.IsNullOrEmpty(resultado.PrecioOriginalTexto) == false)
							{
								int descuento = Herramientas.Calculadora.SacarDescuento(decimal.Parse(resultado.PrecioOriginalTexto), decimal.Parse(resultado.PrecioActualTexto));

								if (descuento > 0)
								{
									string nombre = WebUtility.HtmlDecode(resultado.Nombre);
									nombre = nombre.Replace(" - Early Access", null);
									nombre = nombre.Replace(" - Pre-Order", null);
									nombre = nombre.Replace("(Steam)", null);
									nombre = nombre.Replace("(US)", null);
									nombre = nombre.Replace("(US/CA)", null);
									nombre = nombre.Trim();

									JuegoPrecio oferta = new JuegoPrecio
									{
										Nombre = nombre,
										Enlace = enlaceJuego,
										Imagen = resultado.ImagenUrl,
										Moneda = JuegoMoneda.Euro,
										Precio = resultado.PrecioActual.Value,
										Descuento = descuento,
										Tienda = Generar().Id,
										DRM = drmJuego,
										FechaDetectado = DateTime.Now,
										FechaActualizacion = DateTime.Now
									};

									if (region == TiendaRegion.EstadosUnidos)
									{
										oferta.Moneda = JuegoMoneda.Dolar;
									}

									ofertas.Add(oferta);
								}
							}
						}
					}
				}

				if (ofertas?.Count > 0)
				{
					ofertas = ofertas.GroupBy(o => o.Enlace, StringComparer.OrdinalIgnoreCase).Select(g => g.First()).ToList();

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

		private static JuegoDRM AsignarDRM(string nombre, string enlace)
		{
			nombre = nombre.ToLower();

			JuegoDRM drmJuego = JuegoDRM.Steam;

			if (nombre.Contains("win10") == true || nombre.Contains("xbox") == true)
			{
				drmJuego = JuegoDRM.NoEspecificado;
			}

			if (enlace.Contains("win10") == true || enlace.Contains("xbox") == true || enlace.Contains("age-of-empires-2-definitive-edition") == true)
			{
				drmJuego = JuegoDRM.NoEspecificado;
			}

			if (nombre.Contains("(epic)") == true || nombre.Contains("[epic]") == true)
			{
				drmJuego = JuegoDRM.Epic;
			}

			if (enlace.Contains("tchia") == true || enlace.Contains("mythforce") == true)
			{
				drmJuego = JuegoDRM.Epic;
			}

			if (enlace.Contains("max-payne-3") == true || enlace.Contains("la-noire") == true || enlace.Contains("red-dead-redemption-2") == true ||
				enlace.Contains("bully-scholarship-edition") == true || enlace.Contains("grand-theft-auto-v") == true)
			{
				drmJuego = JuegoDRM.Rockstar;
			}

			return drmJuego;
		}
	}
}
