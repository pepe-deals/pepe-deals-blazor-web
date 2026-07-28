#nullable disable

using Herramientas;
using Juegos;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
				ImagenIcono = "/imagenes/tiendas/allyouplay_icono.webp",
				Color = "#ff4081",
				AdminEnseñar = true,
				AdminInteractuar = true
			};

			return tienda;
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			int juegos2 = 0;

			int i = 1;
			int total = 10;
			while (i < total)
			{
				HttpClient cliente = new HttpClient();
				cliente.BaseAddress = new Uri("https://www.allyouplay.com");
				cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				string peticionEnBruto = "{\"page_number\":" + i.ToString() + ",\"page_size\":24,\"specification_option_ids\":[4]}";

				HttpRequestMessage peticion = new HttpRequestMessage(HttpMethod.Post, "https://sefim.allyouplay.com/api-frontend/Catalog/GetCategory/69")
				{
					Content = new StringContent(peticionEnBruto, Encoding.UTF8, "application/json"),
					Headers = { { "POST", "/api-frontend/Catalog/GetCategory/69" },
								{ "Host", "sefim.allyouplay.com" },
								{ "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:133.0) Gecko/20100101 Firefox/133.0" },
								{ "Accept", "application/json, text/plain, */*" },
								{ "Accept-Language", "es-ES,es;q=0.8,en-US;q=0.5,en;q=0.3"},
								{ "Origin", "https://allyouplay.com" },
								{ "DNT", "1" },
								{ "Sec-GPC", "1" },
								{ "Connection", "keep-alive" },
								{ "Referer", "https://allyouplay.com/" },
								{ "Sec-Fetch-Dest", "empty" },
								{ "Sec-Fetch-Mode", "cors" },
								{ "Pragma", "no-cache" },
								{ "Cache-Control", "no-cache" }
					}
				}; 

				HttpResponseMessage respuesta = await cliente.SendAsync(peticion);

				string html = string.Empty;

				try
				{
					html = await respuesta.Content.ReadAsStringAsync();
				}
				catch { }

				if (string.IsNullOrEmpty(html) == false)
				{
					AllyouplayAPI datos = JsonSerializer.Deserialize<AllyouplayAPI>(html);

					if (datos != null)
					{
						if (datos.Datos != null)
						{
							if (datos.Datos.Catalogo != null)
							{
								total = datos.Datos.Catalogo.Paginas;

								foreach (var juego in datos.Datos.Catalogo.Juegos)
								{
									if (juego.Precios != null)
									{
										if (juego.Precios.PrecioRebajado != null && juego.Precios.PrecioBase != null)
										{
											decimal precioBase = juego.Precios.PrecioBase.Value;
											decimal precioRebajado = juego.Precios.PrecioRebajado.Value;

											int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

											if (descuento > 0)
											{
												JuegoPrecio oferta = new JuegoPrecio
												{
													Nombre = juego.Nombre,
													Enlace = "https://allyouplay.com/" + juego.Slug,
													Imagen = juego.Imagen,
													Moneda = JuegoMoneda.Euro,
													Precio = precioRebajado,
													Descuento = descuento,
													Tienda = Generar().Id,
													DRM = JuegoDRM.Steam,
													FechaDetectado = DateTime.Now,
													FechaActualizacion = DateTime.Now
												};

												try
												{
													await BaseDatos.Tiendas.Comprobar.Resto(region, oferta);
												}
												catch (Exception ex)
												{
													BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
												}

												juegos2 += 1;

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
				}

				i += 1;
			}
		}
	}

	public class AllyouplayAPI
	{
		[JsonPropertyName("category_model_dto")]
		public AllyouplayAPIDatos Datos { get; set; }
	}

	public class AllyouplayAPIDatos
	{
		[JsonPropertyName("catalog_products_model")]
		public AllyouplayAPICatalogo Catalogo { get; set; }
	}

	public class AllyouplayAPICatalogo
	{
		[JsonPropertyName("products")]
		public List<AllyouplayAPIJuego> Juegos { get; set; }

		[JsonPropertyName("total_pages")]
		public int Paginas { get; set; }
	}

	public class AllyouplayAPIJuego
	{
		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("se_name")]
		public string Slug { get; set; }

		[JsonPropertyName("image_url")]
		public string Imagen { get; set; }

		#nullable enable
		[JsonPropertyName("product_price")]
		public AllyouplayAPIJuegoPrecios? Precios { get; set; }
		#nullable disable
	}

	public class AllyouplayAPIJuegoPrecios
	{
		[JsonPropertyName("old_price_value")]
		public decimal? PrecioBase { get; set; }

		[JsonPropertyName("price_value")]
		public decimal? PrecioRebajado { get; set; }
	}
}
