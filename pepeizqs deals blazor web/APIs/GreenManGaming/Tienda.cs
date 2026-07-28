#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Tiendas2;

namespace APIs.GreenManGaming
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "greenmangaming",
				Nombre = "Green Man Gaming",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/greenmangaming_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/greenmangaming_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/greenmangaming_icono.webp",
				Color = "#97ff9a",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			return "https://greenmangaming.sjv.io/c/1382810/1219987/15105?u=" + Uri.EscapeDataString(enlace);
		}

		public static Tiendas2.Tienda GenerarGold()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "greenmangaminggold",
				Nombre = "Green Man Gaming Gold",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/greenmangaminggold_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/greenmangaminggold_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/greenmangaming_icono.webp",
				Color = "#97ff9a",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			string enlace = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				enlace = "https://api.greenmangaming.com/api/productfeed/prices/current?cc=es&cur=eur&lang=en";
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				enlace = "https://api.greenmangaming.com/api/productfeed/prices/current?cc=us&cur=usd&lang=en";
			}

			if (string.IsNullOrEmpty(enlace) == false)
			{
				string html = await Decompiladores.Estandar(enlace);

				if (string.IsNullOrEmpty(html) == false)
				{
					GreenManGamingJuegos listaJuegos = new GreenManGamingJuegos();

					XmlSerializer xml = new XmlSerializer(typeof(GreenManGamingJuegos));
					listaJuegos = (GreenManGamingJuegos)xml.Deserialize(new StringReader(html));

					if (listaJuegos?.Juegos?.Count > 0)
					{
						List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

						foreach (GreenManGamingJuego juego in listaJuegos.Juegos)
						{
							if (int.Parse(juego.Stock) > 0)
							{
								decimal precioBase = decimal.Parse(juego.PrecioBase);
								decimal precioRebajado = decimal.Parse(juego.PrecioRebajado);

								int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

								if (descuento > 0)
								{
									string nombre = WebUtility.HtmlDecode(juego.Nombre);

									string enlaceJuego = juego.Enlace;

									string imagen = juego.Imagen;

									JuegoDRM drm = JuegoDRM2.Traducir(juego.DRM, Generar().Id);

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

		public static async Task BuscarOfertasGold(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, GenerarGold().Id, DateTime.Now, 0);

			int juegos2 = 0;
	
			for (decimal precioMin = 0; precioMin < 80; precioMin += 1)
			{
				HttpClient cliente = new HttpClient();
				cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				cliente.DefaultRequestHeaders.Add("Referer", "https://www.greenmangaming.com/");
				cliente.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
				cliente.DefaultRequestHeaders.Add("Origin", "https://www.greenmangaming.com");

				decimal precioMax = precioMin + 1;

				string peticionEnBruto = string.Empty;

				if (region == TiendaRegion.Europa)
				{
					string region2 = "ES";  
					string moneda2 = "EUR";

					peticionEnBruto = $@"{{
						""requests"":[{{
							""indexName"":""prod_ProductSearch_{region2}_Mrp_ASC"",
							""params"":""query=xp-view-all&ruleContexts=%5B%22{moneda2}%22%2C%22{moneda2}_{region2}%22%2C%22{region2}%22%5D&filters=IsSellable%3Atrue%20AND%20AvailableRegions%3A{region2}%20AND%20NOT%20ExcludeCountryCodes%3A{region}&hitsPerPage=1000&distinct=true&analytics=false&clickAnalytics=true&maxValuesPerFacet=10&facets=%5B%22Franchise%22%2C%22IsEarlyAccess%22%2C%22Genre%22%2C%22PlatformName%22%2C%22PublisherName%22%2C%22SupportedVrs%22%2C%22Regions.{region2}.ReleaseDateStatus%22%2C%22Regions.{region2}.Mrp%22%2C%22Regions.{region2}.IsOnSaleVip%22%2C%22Regions.{region}.IsXpOffer%22%2C%22DrmName%22%5D&tagFilters=&facetFilters=%5B%5B%22DrmName%3ASteam%22%5D%5D&numericFilters=%5B%22Regions.{region2}.Mrp%3E%3D{precioMin}%22,%22Regions.{region2}.Mrp%3C{precioMax}%22%5D""
						}}]
					}}";
				}
				else if (region == TiendaRegion.EstadosUnidos)
				{
					string region2 = "US";
					string moneda2 = "USD";

					peticionEnBruto = $@"{{
						""requests"":[{{
							""indexName"":""prod_ProductSearch_{region2}_Mrp_ASC"",
							""params"":""query=xp-view-all&ruleContexts=%5B%22{moneda2}%22%2C%22{moneda2}_{region2}%22%2C%22{region2}%22%5D&filters=IsSellable%3Atrue%20AND%20AvailableRegions%3A{region2}%20AND%20NOT%20ExcludeCountryCodes%3A{region}&hitsPerPage=1000&distinct=true&analytics=false&clickAnalytics=true&maxValuesPerFacet=10&facets=%5B%22Franchise%22%2C%22IsEarlyAccess%22%2C%22Genre%22%2C%22PlatformName%22%2C%22PublisherName%22%2C%22SupportedVrs%22%2C%22Regions.{region2}.ReleaseDateStatus%22%2C%22Regions.{region2}.Mrp%22%2C%22Regions.{region2}.IsOnSaleVip%22%2C%22Regions.{region}.IsXpOffer%22%2C%22DrmName%22%5D&tagFilters=&facetFilters=%5B%5B%22DrmName%3ASteam%22%5D%5D&numericFilters=%5B%22Regions.{region2}.Mrp%3E%3D{precioMin}%22,%22Regions.{region2}.Mrp%3C{precioMax}%22%5D""
						}}]
					}}";
				}

				if (string.IsNullOrEmpty(peticionEnBruto) == false)
				{
					HttpRequestMessage peticion = new HttpRequestMessage(HttpMethod.Post, "https://sczizsp09z-dsn.algolia.net/1/indexes/*/queries?x-algolia-agent=Algolia for JavaScript (4.5.1); Browser (lite); instantsearch.js (4.8.3); JS Helper (3.2.2)&x-algolia-api-key=7f2aba1ccb27473dcf2cace71bf20e85&x-algolia-application-id=SCZIZSP09Z");
					peticion.Content = new StringContent(peticionEnBruto, Encoding.UTF8, "application/json");

					HttpResponseMessage respuesta = await cliente.SendAsync(peticion);

					string html = string.Empty;

					try
					{
						html = await respuesta.Content.ReadAsStringAsync();
					}
					catch { }

					if (string.IsNullOrEmpty(html) == false)
					{
						GreenManGamingGold datos = JsonSerializer.Deserialize<GreenManGamingGold>(html);

						if (datos?.Resultados != null && datos.Resultados.Count > 0 && datos.Resultados[0]?.Juegos?.Count > 0)
						{
							List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

							foreach (var juego in datos.Resultados[0]?.Juegos)
							{
								foreach (var regionJuego in juego.Regiones)
								{
									if (regionJuego.Key == "ES" && region == TiendaRegion.Europa && juego.RegionesNoStock.TryGetValue("ES", out bool noStock) && noStock == false)
									{
										decimal precioBase = juego.Regiones["ES"].Rrp;
										decimal precioRebajado = juego.Regiones["ES"].Mrp;

										int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

										if (descuento > 0)
										{
											JuegoDRM drm = JuegoDRM2.Traducir(juego.DRM, Generar().Id);

											JuegoPrecio oferta = new JuegoPrecio
											{
												Nombre = WebUtility.HtmlDecode(juego.Nombre),
												Enlace = "https://www.greenmangaming.com" + juego.Enlace,
												Imagen = "https://images.greenmangaming.com" + juego.Imagen,
												Moneda = JuegoMoneda.Euro,
												Precio = precioRebajado,
												Descuento = descuento,
												Tienda = GenerarGold().Id,
												DRM = drm,
												FechaDetectado = DateTime.Now,
												FechaActualizacion = DateTime.Now
											};

											ofertas.Add(oferta);
										}
									}
									else if (regionJuego.Key == "US" && region == TiendaRegion.EstadosUnidos && juego.RegionesNoStock.TryGetValue("US", out bool noStockUS) && noStockUS == false)
									{
										if (juego.Regiones["US"].CurrencyCode == "USD" && juego.Regiones["US"].CountryCode == "US")
										{
											decimal precioBase = juego.Regiones["US"].Rrp;
											decimal precioRebajado = juego.Regiones["US"].Mrp;

											int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

											if (descuento > 0)
											{
												JuegoDRM drm = JuegoDRM2.Traducir(juego.DRM, Generar().Id);

												JuegoPrecio oferta = new JuegoPrecio
												{
													Nombre = WebUtility.HtmlDecode(juego.Nombre),
													Enlace = "https://www.greenmangaming.com" + juego.Enlace,
													Imagen = "https://images.greenmangaming.com" + juego.Imagen,
													Moneda = JuegoMoneda.Dolar,
													Precio = precioRebajado,
													Descuento = descuento,
													Tienda = GenerarGold().Id,
													DRM = drm,
													FechaDetectado = DateTime.Now,
													FechaActualizacion = DateTime.Now
												};

												ofertas.Add(oferta);
											}
										}
									}
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
										BaseDatos.Errores.Insertar.Mensaje(GenerarGold().Id, ex);
									}

									juegos2 += lote.Count;

									try
									{
										await BaseDatos.Admin.Actualizar.Tiendas(region, GenerarGold().Id, DateTime.Now, juegos2);
									}
									catch (Exception ex)
									{
										BaseDatos.Errores.Insertar.Mensaje(GenerarGold().Id, ex);
									}
								}
							}
						}
						else
						{
							break;
						}
					}
					else
					{
						break;
					}
				}
			}
		}
	}

	#region Normal

	[XmlRoot("products")]
	public class GreenManGamingJuegos
	{
		[XmlElement("product")]
		public List<GreenManGamingJuego> Juegos { get; set; }
	}

	public class GreenManGamingJuego
	{
		[XmlElement("product_name")]
		public string Nombre { get; set; }

		[XmlElement("deep_link")]
		public string Enlace { get; set; }

		[XmlElement("image_url")]
		public string Imagen { get; set; }

		[XmlElement("price")]
		public string PrecioRebajado { get; set; }

		[XmlElement("rrp_price")]
		public string PrecioBase { get; set; }

		[XmlElement("drm")]
		public string DRM { get; set; }

		[XmlElement("stock_level")]
		public string Stock { get; set; }
	}

	#endregion

	#region Gold

	public class GreenManGamingGold
	{
		[JsonPropertyName("results")]
		public List<GreenManGamingGoldResultado> Resultados { get; set; }
	}

	public class GreenManGamingGoldResultado
	{
		[JsonPropertyName("hits")]
		public List<GreenManGamingGoldJuego> Juegos { get; set; }
	}

	[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
	public class GreenManGamingGoldJuego
	{
		[JsonPropertyName("DisplayName")]
		public string Nombre { get; set; }

		[JsonPropertyName("Url")]
		public string Enlace { get; set; }

		[JsonPropertyName("ImageUrl")]
		public string Imagen { get; set; }

		[JsonPropertyName("DrmName")]
		public string DRM { get; set; }

		[JsonPropertyName("Regions")]
		public Dictionary<string, GreenManGamingGoldJuegoRegion> Regiones { get; set; }

		[JsonPropertyName("OutOfStockRegions")]
		public Dictionary<string, bool> RegionesNoStock { get; set; }
	}

	[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
	public class GreenManGamingGoldJuegoRegion
	{
		[JsonPropertyName("CountryCode")]
		public string CountryCode { get; set; }

		[JsonPropertyName("CurrencyCode")]
		public string CurrencyCode { get; set; }

		[JsonPropertyName("Mrp")]
		public decimal Mrp { get; set; }

		[JsonPropertyName("Rrp")]
		public decimal Rrp { get; set; }
	}

	#endregion
}
