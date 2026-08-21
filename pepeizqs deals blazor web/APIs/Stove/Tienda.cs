#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.Stove
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "stove",
				Nombre = "Stove",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/stove_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/stove_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/stove_icono.ico",
				Color = "#fc4420",
				AdminUso = true,
				UsuarioUso = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa }
			};

			return tienda;
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);
			int juegos2 = 0;

			JsonSerializerOptions opciones = new JsonSerializerOptions
			{
				UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
			};

			var servicio = new ServiceCollection().AddHttpClient().BuildServiceProvider();
			var factoria = servicio.GetRequiredService<IHttpClientFactory>();
			HttpClient cliente = factoria.CreateClient();

			cliente.DefaultRequestHeaders.UserAgent.ParseAdd(
				"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/114.0"
			);
			cliente.DefaultRequestHeaders.Add("X-LANG", "es");
			cliente.DefaultRequestHeaders.Add("X-NATION", "ES");

			int i = 1;
			int totalPaginas = 10;
			while (i <= totalPaginas)
			{
				var respuesta = await cliente.GetAsync("https://api.onstove.com/store/v1.0/products/search?q=&currency_code=USD&page=" + i.ToString() + "&size=36&direction=SCORE&rating.board=GRAC&price.min=0&price.max=99&tag_aggregation_size=6&genre_aggregation_size=6&on_discount=true&types=GAME&types=DLC&types=COLLECTION");
				string html = await respuesta.Content.ReadAsStringAsync();

				if (string.IsNullOrEmpty(html) == false)
				{
					if (html.Contains("{\"code\":97999,\"message\":\"INTERNAL SERVER ERROR\"}") == false)
					{
						StoveRespuesta respuesta2 = JsonSerializer.Deserialize<StoveRespuesta>(html, opciones);

						if (respuesta2 != null)
						{
							totalPaginas = respuesta2.Datos?.Paginas ?? 10;

							if (respuesta2.Datos?.Juegos?.Count > 0)
							{
								List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

								foreach (var juego in respuesta2.Datos.Juegos)
								{
									int descuento = 0;

									if (juego.Precio.Descuento.HasValue == true)
									{
										descuento = juego.Precio.Descuento.Value;
									}

									if (descuento > 0 && descuento < 100)
									{
										string nombre = WebUtility.HtmlDecode(juego.Nombre);

										string enlace = "https://store.onstove.com/games/" + juego.Id.ToString();

										string imagen = juego.Imagen;

										JuegoPrecio oferta = new JuegoPrecio
										{
											Nombre = nombre,
											Enlace = enlace,
											Imagen = imagen,
											Moneda = JuegoMoneda.Euro,
											Precio = juego.Precio.PrecioRebajado,
											Descuento = descuento,
											Tienda = Generar().Id,
											DRM = JuegoDRM.Stove,
											FechaDetectado = DateTime.Now,
											FechaActualizacion = DateTime.Now
										};

										ofertas.Add(oferta);
									}
								}

								if (ofertas?.Count > 0)
								{
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

				i += 1;
			}
		}
	}

	public class StoveRespuesta
	{
		[JsonPropertyName("value")]
		public StoveDatos Datos { get; set; }
	}

	public class StoveDatos
	{
		[JsonPropertyName("contents")]
		public List<StoveJuego> Juegos { get; set; }

		[JsonPropertyName("total_pages")]
		public int Paginas { get; set; }
	}

	public class StoveJuego
	{
		[JsonPropertyName("product_no")]
		public int Id { get; set; }

		[JsonPropertyName("product_name")]
		public string Nombre { get; set; }

		[JsonPropertyName("title_image_square")]
		public string Imagen { get; set; }

		[JsonPropertyName("amount")]
		public StovePrecio Precio { get; set; }
	}

	public class StovePrecio
	{
		[JsonPropertyName("discount_rate")]
		public int? Descuento { get; set; }

		[JsonPropertyName("original_price")]
		public decimal PrecioBase { get; set; }

		[JsonPropertyName("sales_price")]
		public decimal PrecioRebajado { get; set; }
	}
}
