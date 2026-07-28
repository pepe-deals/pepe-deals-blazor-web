//https://www.fanatical.com/api/products-group/killer-bundle/en
//https://www.fanatical.com/api/all/en
//https://www.fanatical.com/api/algolia/bundles?altRank=false
//https://www.fanatical.com/api/algolia/onsaleresults

#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.Fanatical
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "fanatical",
				Nombre = "Fanatical",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/fanatical_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/fanatical_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/fanatical_icono.webp",
				Color = "#ffcf89",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			string enlaceEncoded = Uri.EscapeDataString(enlace);
			return "https://www.awin1.com/cread.php?awinmid=118821&awinaffid=2693824&ued=" + enlaceEncoded;
		}

		public static string ApiKey { get; set; }

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			int juegos2 = 0;

			string html = await Decompiladores.Estandar("https://www.fanatical.com/feeds/minimal-feed.jsonl?apikey=" + ApiKey);

			if (string.IsNullOrEmpty(html) == false)
			{
				try
				{
					string[] lineas = html.Split('\n', StringSplitOptions.RemoveEmptyEntries);
					JsonSerializerOptions opciones = new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true,
						UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
					};

					int tamañoTanda = 500;
					int totalLineas = lineas.Length;
					int totalProcesado = 0;

					int arranque = await BaseDatos.Admin.Buscar.TiendasValorAdicional(region, Generar().Id, "valorAdicional");

					if (arranque < 0)
					{
						arranque = 0;
					}

					for (int i = arranque; i < lineas.Length; i += tamañoTanda)
					{
						var tanda = lineas.Skip(i).Take(tamañoTanda);
						string tandaJson = "[" + string.Join(",", tanda.Where(l => !string.IsNullOrWhiteSpace(l))) + "]";

						List<FanaticalJuego> juegos = null;

						try
						{
							juegos = JsonSerializer.Deserialize<List<FanaticalJuego>>(tandaJson, opciones);
						}
						catch (JsonException ex)
						{
							BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex, false);
						}

						if (juegos?.Count > 0)
						{
							int ofertasEnEstaTanda = await ProcesarTanda(juegos, region);
							juegos2 += ofertasEnEstaTanda;

							await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, juegos2);
						}

						totalProcesado += tamañoTanda;

						int proximaTanda = i + tamañoTanda;

						if (proximaTanda >= totalLineas)
						{
							await BaseDatos.Admin.Actualizar.TiendasValorAdicional(region, Generar().Id, "valorAdicional", 0);
						}
						else
						{
							await BaseDatos.Admin.Actualizar.TiendasValorAdicional(region, Generar().Id, "valorAdicional", proximaTanda);
						}

						juegos.Clear();
						juegos = null;
						GC.Collect();
					}
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex, false);
				}
			}
		}

		private static async Task<int> ProcesarTanda(List<FanaticalJuego> juegos, TiendaRegion region)
		{
			List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

			foreach (FanaticalJuego juego in juegos)
			{
				bool autorizar = true;

				if (juego.Regiones?.Count > 0)
				{
					autorizar = false;
					foreach (string region2 in juego.Regiones)
					{
						if (region == TiendaRegion.Europa && region2 == "ES")
						{
							autorizar = true;
						}
						else if (region == TiendaRegion.EstadosUnidos && region2 == "US")
						{
							autorizar = true;
						}
					}
				}

				if (autorizar == true)
				{
					string descuentoTexto = juego.Descuento?.ToString();

					if (descuentoTexto != null)
					{
						if (descuentoTexto.Contains("."))
						{
							int int1 = descuentoTexto.IndexOf(".");
							descuentoTexto = descuentoTexto.Remove(int1, descuentoTexto.Length - int1);
						}

						int descuento = 0;

						try
						{
							descuento = int.Parse(descuentoTexto);
						}
						catch { }

						if (descuento > 0 && juego.Disponible == true)
						{
							string nombre = WebUtility.HtmlDecode(juego.Nombre);
							string imagen = juego.Imagen;
							string enlace = juego.Enlace;
							enlace = enlace.Replace("/en/", "/");

							decimal precioRebajado = 0;

							if (region == TiendaRegion.Europa)
							{
								precioRebajado = juego.PrecioRebajado?.GetValueOrDefault("EUR") ?? 0;
							}
							else if (region == TiendaRegion.EstadosUnidos)
							{
								precioRebajado = juego.PrecioRebajado?.GetValueOrDefault("USD") ?? 0;
							}

							if (string.IsNullOrEmpty(juego.DRM) == false && precioRebajado > 0)
							{
								string drmTexto = juego.DRM;
								JuegoDRM drm = JuegoDRM2.Traducir(drmTexto, Generar().Id);

								JuegoPrecio oferta = new JuegoPrecio
								{
									Nombre = nombre,
									Enlace = enlace,
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

								if (juego.FechaTermina != null)
								{
									if (Convert.ToDouble(juego.FechaTermina) > 0)
									{
										DateTime fechaTermina = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
										fechaTermina = fechaTermina.AddSeconds(Convert.ToDouble(juego.FechaTermina));
										fechaTermina = fechaTermina.ToLocalTime();

										oferta.FechaTermina = fechaTermina;
									}
								}

								ofertas.Add(oferta);
							}
						}
					}
				}
			}

			if (ofertas?.Count > 0)
			{
				try
				{
					await BaseDatos.Tiendas.Comprobar.Resto(region, ofertas);
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
				}
			}

			int ofertasProcessadas = ofertas.Count;

			ofertas.Clear();
			ofertas = null;

			return ofertasProcessadas;
		}
	}

	#region Clases

	public class FanaticalJuego
	{
		[JsonPropertyName("product_id")]
		public string ProductId { get; set; }

		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("sku")]
		public string SKU { get; set; }

		[JsonPropertyName("drm")]
		public string DRM { get; set; }  

		[JsonPropertyName("image")]
		public string Imagen { get; set; }

		[JsonPropertyName("url")]
		public string Enlace { get; set; }

		[JsonPropertyName("discount_percent")]
		public decimal? Descuento { get; set; }

		[JsonPropertyName("valid_until")]
		public long? FechaTermina { get; set; }

		[JsonPropertyName("price")]
		public Dictionary<string, decimal?> PrecioRebajado { get; set; }

		[JsonPropertyName("full_price")]
		public Dictionary<string, decimal?> PrecioBase { get; set; }

		[JsonPropertyName("included_regions")]
		public List<string> Regiones { get; set; }

		[JsonPropertyName("in_stock")]
		public bool Disponible { get; set; }

		[JsonPropertyName("type")]
		public string Tipo { get; set; }

		[JsonPropertyName("bundle_tiers")]
		public List<FanaticalBundleTier> BundleTiers { get; set; }
	}

	public class FanaticalBundleTier
	{
		[JsonPropertyName("tier_product_count")]
		public int ProductosPorTier { get; set; }

		[JsonPropertyName("products")]
		public List<FanaticalBundleProducto> Productos { get; set; } = new List<FanaticalBundleProducto>();

		[JsonPropertyName("price")]
		public Dictionary<string, decimal> Precio { get; set; }
	}

	public class FanaticalBundleProducto
	{
		[JsonPropertyName("product_id")]
		public string ProductId { get; set; }

		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("slug")]
		public string Slug { get; set; }

		[JsonPropertyName("type")]
		public string Tipo { get; set; }

		[JsonPropertyName("drm")]
		public string DRM { get; set; }

		[JsonPropertyName("steam_id")]
		public int? SteamId { get; set; }

		[JsonPropertyName("image")]
		public string Imagen { get; set; }
	}

    #endregion
}
