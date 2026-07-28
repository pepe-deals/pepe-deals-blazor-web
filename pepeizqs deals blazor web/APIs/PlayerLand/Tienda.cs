#nullable disable

using Herramientas;
using Juegos;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.PlayerLand
{

	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "playerland",
				Nombre = "Playerland",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/playerland_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/playerland_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/playerland_icono.webp",
				Color = "#beb2f1",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
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

			int paginas = 10;

			List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

			int i = 1;
			while (i <= paginas)
			{
				string html = await Decompiladores.Estandar("https://player.land/api/listing/products?system=pc&platform=Steam&limit=96&page=" + i.ToString() + "&sort=first_release_date:desc");

				if (string.IsNullOrEmpty(html) == false)
				{
					PlayerLandRespuesta respuesta = JsonSerializer.Deserialize<PlayerLandRespuesta>(html, opciones);

					if (respuesta?.Items != null)
					{
						paginas = respuesta.TotalPages;

						foreach (var juego in respuesta.Items)
						{
							foreach (var variante in juego.Variants)
							{
								if (region == TiendaRegion.Europa && variante.PriceEu > 0 && variante.PriceRecommendedEu > 0 && juego.UnsellableEu == false)
								{
									decimal precioRebajado = variante.PriceEu / 100m;
									decimal precioBase = variante.PriceRecommendedEu / 100m;

									int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

									if (descuento > 0)
									{
										string imagen = juego.ImageVertical.HasValue ? "https://cdn.player.land/" + juego.Id.ToString() + "/image/cover_vertical/" + juego.Slug + ".webp?v=" + juego.ImageVertical : null;
										string enlace = "https://player.land/en/p-" + juego.Id + "/";

										JuegoPrecio oferta = new JuegoPrecio
										{
											Nombre = juego.Name,
											Enlace = enlace,
											Imagen = imagen,
											Moneda = JuegoMoneda.Euro,
											Precio = precioRebajado,
											Descuento = descuento,
											Tienda = Generar().Id,
											DRM = JuegoDRM.Steam,
											FechaDetectado = DateTime.Now,
											FechaActualizacion = DateTime.Now
										};

										ofertas.Add(oferta);
									}
								}
								else if (region == TiendaRegion.EstadosUnidos && variante.Price > 0 && variante.PriceRecommended > 0)
								{
									decimal precioRebajado = variante.Price / 100m;
									decimal precioBase = variante.PriceRecommended / 100m;

									int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

									if (descuento > 0)
									{
										string imagen = juego.ImageVertical.HasValue ? "https://cdn.player.land/" + juego.Id.ToString() + "/image/cover_vertical/" + juego.Slug + ".webp?v=" + juego.ImageVertical : null;
										string enlace = "https://player.land/en/p-" + juego.Id + "/";

										JuegoPrecio oferta = new JuegoPrecio
										{
											Nombre = juego.Name,
											Enlace = enlace,
											Imagen = imagen,
											Moneda = JuegoMoneda.Dolar,
											Precio = precioRebajado,
											Descuento = descuento,
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

				if (ofertas.Count > 0)
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

	public class PlayerLandRespuesta
	{
		[JsonPropertyName("items")]
		public List<PlayerLandJuego> Items { get; set; }

		[JsonPropertyName("total")]
		public int Total { get; set; }

		[JsonPropertyName("totalPages")]
		public int TotalPages { get; set; }

		[JsonPropertyName("limit")]
		public int Limit { get; set; }

		[JsonPropertyName("page")]
		public int Page { get; set; }

		[JsonPropertyName("region")]
		public string Region { get; set; }
	}

	public class PlayerLandJuego
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("image")]
		public long? Image { get; set; }

		[JsonPropertyName("image_vertical")]
		public long? ImageVertical { get; set; }

		[JsonPropertyName("slug")]
		public string Slug { get; set; }

		[JsonPropertyName("url_slug")]
		public string UrlSlug { get; set; }

		[JsonPropertyName("updated_at")]
		public long UpdatedAt { get; set; }

		[JsonPropertyName("genres")]
		public List<string> Genres { get; set; }

		[JsonPropertyName("variants")]
		public List<PlayerLandVariante> Variants { get; set; }

		[JsonPropertyName("first_release_date")]
		public long? FirstReleaseDate { get; set; }

		[JsonPropertyName("unsellable")]
		public bool Unsellable { get; set; }

		[JsonPropertyName("unsellable_eu")]
		public bool UnsellableEu { get; set; }

		[JsonPropertyName("released_at")]
		public long? ReleasedAt { get; set; }

		[JsonPropertyName("preorder")]
		public string Preorder { get; set; }
	}

	public class PlayerLandVariante
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("variant_id")]
		public int VariantId { get; set; }

		[JsonPropertyName("qty")]
		public int Qty { get; set; }

		[JsonPropertyName("qty_eu")]
		public int QtyEu { get; set; }

		[JsonPropertyName("price")]
		public int Price { get; set; }

		[JsonPropertyName("price_eu")]
		public int PriceEu { get; set; }

		[JsonPropertyName("price_recommended")]
		public int PriceRecommended { get; set; }

		[JsonPropertyName("price_recommended_eu")]
		public int PriceRecommendedEu { get; set; }

		[JsonPropertyName("unsellable")]
		public bool Unsellable { get; set; }

		[JsonPropertyName("unsellable_eu")]
		public bool UnsellableEu { get; set; }

		[JsonPropertyName("system")]
		public List<string> System { get; set; }

		[JsonPropertyName("platforms")]
		public List<string> Platforms { get; set; }

		[JsonPropertyName("manual")]
		public int Manual { get; set; }

		[JsonPropertyName("countries")]
		public List<string> Countries { get; set; }

		[JsonPropertyName("region")]
		public string Region { get; set; }

		[JsonPropertyName("is_best_seller")]
		public bool IsBestSeller { get; set; }

		[JsonPropertyName("is_trading")]
		public bool IsTrading { get; set; }

		[JsonPropertyName("preorder")]
		public string Preorder { get; set; }

		[JsonPropertyName("released_at")]
		public long? ReleasedAt { get; set; }

		[JsonPropertyName("key_dispatch_available_at")]
		public string KeyDispatchAvailableAt { get; set; }
	}
}
