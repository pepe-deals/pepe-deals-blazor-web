//https://partner.dlgamer.com/secure/affiliation

//https://static.dlgamer.com/feeds/general_feed_en.json
//https://static.dlgamer.com/feeds/general_feed_us.json
//https://static.dlgamer.com/feeds/general_feed_eu.json

#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.DLGamer
{
	public class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "dlgamer",
				Nombre = "DLGamer",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/dlgamer_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/dlgamer_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/dlgamer_icono.webp",
				Color = "#b9aa21",
				AdminUso = true,
				UsuarioUso = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			return enlace + "?affil=pepeizqdeals";
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			JsonSerializerOptions opciones = new JsonSerializerOptions
			{
				UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
			};

			string enlace = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				enlace = "https://static.dlgamer.com/feeds/general_feed_eu.json";
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				enlace = "https://static.dlgamer.com/feeds/general_feed_us.json";
			}

			if (string.IsNullOrEmpty(enlace) == false)
			{
				string html = await Decompiladores.NoSeguro(enlace);

				if (string.IsNullOrEmpty(html) == false)
				{
					BaseDatos.Errores.Insertar.Mensaje("Arreglar DLGamer", html);

					DLGamerJuegos basedatos = null;

					try
					{
						basedatos = JsonSerializer.Deserialize<DLGamerJuegos>(html, opciones);
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
					}

					if (basedatos != null)
					{
						List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

						foreach (var juegoDL in basedatos.Datos)
						{
							decimal precioRebajado = juegoDL.Value.PrecioRebajado;
							decimal precioBase = juegoDL.Value.PrecioBase;

							int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

							if (descuento > 0)
							{
								string nombre = WebUtility.HtmlDecode(juegoDL.Value.Nombre);

								string enlaceJuego = juegoDL.Value.Enlace;

								string imagen = "https://www.dlgamer.com" + juegoDL.Value.Imagen;

								JuegoDRM drm = JuegoDRM2.Traducir(juegoDL.Value.DRM, Generar().Id);

								JuegoPrecio oferta = new JuegoPrecio
								{
									Nombre = nombre,
									Enlace = enlaceJuego,
									Imagen = imagen,
									Moneda = JuegoMoneda.Euro,
									Precio = precioRebajado,
									Descuento = descuento,
									Tienda = Generar().Id,
									DRM = drm,
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
	}

	#region Clases

	public class DLGamerJuegos
	{
		[JsonPropertyName("products")]
		public Dictionary<string, DLGamerJuego> Datos { get; set; }
	}

	public class DLGamerJuego
	{
		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("price")]
		public decimal PrecioRebajado { get; set; }

		[JsonPropertyName("price_strike")]
		public decimal PrecioBase { get; set; }

		[JsonPropertyName("price_purcent")]
		public string Descuento { get; set; }

		[JsonPropertyName("link")]
		public string Enlace { get; set; }

		[JsonPropertyName("image_box")]
		public string Imagen { get; set; }

		[JsonPropertyName("drm")]
		public string DRM { get; set; }
	}

	#endregion
}
