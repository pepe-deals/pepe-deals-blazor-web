#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.Yuplay
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "yuplay",
				Nombre = "Yuplay",
				Tipo = TiendaTipo.NoOficial,
				ImagenLogo = "/imagenes/tiendas/yuplay_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/yuplay_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/yuplay_icono.webp",
				Color = "#558205",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa }
			};

			return tienda;
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			JsonSerializerOptions opciones = new JsonSerializerOptions
			{
				UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
			};

			int i = 1;
			int paginas = 2;

			List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

			while (i < paginas)
			{
				string html = await Decompiladores.Estandar("https://www.yuplay.com/api/products/?page=" + i.ToString() + "&recommendation=instock&platforms=steam&s=discount-desc");

				if (string.IsNullOrEmpty(html) == false)
				{
					YuplayRespuesta respuesta = JsonSerializer.Deserialize<YuplayRespuesta>(html, opciones);

					if (respuesta != null)
					{
						paginas = respuesta.Extensions?.Pagination?.NumPages ?? 2;

						if (respuesta?.Productos?.Count > 0)
						{
							foreach (var producto in respuesta.Productos)
							{
								if (producto.Descuento != null && string.IsNullOrEmpty(producto.Descuento.Valor) == false && int.Parse(producto.Descuento.Valor) > 0)
								{
									string nombre = WebUtility.HtmlDecode(producto.Nombre);
									string enlaceJuego = "https://www.yuplay.com" + producto.Enlace + "?partner=19b1d908fe49e597";
									string imagen = producto.Imagenes?.Width616 ?? producto.Imagenes?.Width267 ?? "";

									JuegoPrecio oferta = new JuegoPrecio
									{
										Nombre = nombre,
										Enlace = enlaceJuego,
										Imagen = imagen,
										Moneda = JuegoMoneda.Euro,
										Precio = decimal.Parse(producto.Precio ?? "0"),
										Descuento = int.Parse(producto.Descuento.Valor),
										Tienda = Generar().Id,
										DRM = JuegoDRM.Steam,
										FechaDetectado = DateTime.Now,
										FechaActualizacion = DateTime.Now
									};

									ofertas.Add(oferta);
								}
							}
						}
					}
				}

				i += 1;
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

	public class YuplayRespuesta
	{
		[JsonPropertyName("show_headline")]
		public bool ShowHeadline { get; set; }

		[JsonPropertyName("show_filters")]
		public bool ShowFilters { get; set; }

		[JsonPropertyName("show_sorting")]
		public bool ShowSorting { get; set; }

		[JsonPropertyName("products")]
		public List<YuplayProducto> Productos { get; set; } = new();

		[JsonPropertyName("ts")]
		public long Ts { get; set; }

		[JsonPropertyName("extensions")]
		public YuplayExtensiones Extensions { get; set; }
	}

	public class YuplayExtensiones
	{
		[JsonPropertyName("count")]
		public int Count { get; set; }

		[JsonPropertyName("pagination")]
		public Pagination Pagination { get; set; }
	}

	public class Pagination
	{
		[JsonPropertyName("current_page")]
		public int CurrentPage { get; set; }

		[JsonPropertyName("num_pages")]
		public int NumPages { get; set; }

		[JsonPropertyName("previous_page")]
		public int? PreviousPage { get; set; }

		[JsonPropertyName("next_page")]
		public int? NextPage { get; set; }
	}

	public class YuplayProducto
	{
		[JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("unique_product_hash")]
		public string UniqueProductHash { get; set; }

		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("images")]
		public YuplayImagenes Imagenes { get; set; }

		[JsonPropertyName("link")]
		public string Enlace { get; set; }

		[JsonPropertyName("discount")]
		public YuplayDescuento Descuento { get; set; }

		[JsonPropertyName("price")]
		public string Precio { get; set; }

		[JsonPropertyName("base_price")]
		public string BasePrice { get; set; }

		[JsonPropertyName("tmp_price")]
		public string TmpPrice { get; set; }

		[JsonPropertyName("currency")]
		public string Currency { get; set; }

		[JsonPropertyName("currency_code")]
		public string CurrencyCode { get; set; }
	}

	public class YuplayImagenes
	{
		[JsonPropertyName("width_267")]
		public string Width267 { get; set; }

		[JsonPropertyName("width_616")]
		public string Width616 { get; set; }
	}

	public class YuplayDescuento
	{
		[JsonPropertyName("value")]
		public string Valor { get; set; }
	}

}
