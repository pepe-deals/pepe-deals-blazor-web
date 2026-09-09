#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using Tiendas2;

namespace APIs.Loaded
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "loaded",
				Nombre = "Loaded",
				Tipo = TiendaTipo.NoOficial,
				ImagenLogo = "/imagenes/tiendas/loaded_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/loaded_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/loaded_icono.webp",
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

			string campaignId = region == TiendaRegion.EstadosUnidos ? "1625317" : "1625375";
			string moneda = region == TiendaRegion.EstadosUnidos ? "usd" : "eur";
			string intsrc = region == TiendaRegion.EstadosUnidos ? "APIG_12134" : "APIG_12138";

			string destino = $"{enlace}?__currency={moneda}";
			string destinoCodificado = WebUtility.UrlEncode(destino);

			return $"https://go.loaded.com/c/1382810/{campaignId}/18216?u={destinoCodificado}&intsrc={intsrc}";
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			List<ImpactCatalogItem> resultados = new List<ImpactCatalogItem>(); 
				
			if (region == TiendaRegion.Europa)
			{
				resultados = await Herramientas.Impact.ObtenerCatalogo("12138");
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				resultados = await Herramientas.Impact.ObtenerCatalogo("12134");
			}

			if (resultados?.Count > 0)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				foreach (var resultado in resultados)
				{
					if (resultado.Disponibilidad == "InStock")
					{	
						if (resultado.Descuento.HasValue == true && resultado.Descuento.Value > 0)
						{
							string nombre = WebUtility.HtmlDecode(resultado.Nombre);

							string[] sufijosAEliminar =
							{
								"PC - DLC STEAM (EU)",
								"PC - DLC (North America)",
								"PC - DLC (Europe & UK)",
								"PC - DLC (EU)",
								"PC - DLC",
								"PC DLC (Steam)",
								"DLC (Global)",
								"PC (Steam) (EU & UK)",
								"PC (North America)",
								"PC (Europe & UK)",
								"PC (EU & UK) (Steam)",
								"PC (EU & UK)",
								"PC (EU)",
								"PC (EN)",
								"PC (WW)",
								"PC (EMEA)",
								"(PC)",
								"PC",
								"(EN)"
							};

							bool cambiado;
							do
							{
								cambiado = false;
								foreach (var sufijo in sufijosAEliminar)
								{
									if (nombre.EndsWith(sufijo, StringComparison.OrdinalIgnoreCase))
									{
										nombre = nombre[..^sufijo.Length].TrimEnd();
										cambiado = true;
										break;
									}
								}
							} while (cambiado);

							string enlaceJuego = resultado.Url;

							string imagen = resultado.ImagenUrl;

							JuegoDRM drm = JuegoDRM2.Traducir(resultado.Plataforma, Generar().Id);

							JuegoPrecio oferta = new JuegoPrecio
							{
								Nombre = nombre,
								Enlace = LimpiarEnlace(enlaceJuego),
								Imagen = imagen,
								Moneda = JuegoMoneda.Euro,
								Precio = resultado.PrecioActual.Value,
								Descuento = resultado.Descuento.Value,
								Tienda = Generar().Id,
								DRM = drm,
								FechaDetectado = DateTime.Now,
								FechaActualizacion = DateTime.Now
							};

							if (region == TiendaRegion.EstadosUnidos)
							{
								oferta.Moneda = JuegoMoneda.Dolar;
							}

							if (drm == JuegoDRM.Steam || drm == JuegoDRM.Epic || drm == JuegoDRM.GOG)
							{
								ofertas.Add(oferta);
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
