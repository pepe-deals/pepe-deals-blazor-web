#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Tiendas2;

namespace APIs.GOG
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "gog",
				Nombre = "GOG",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/gog_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/gog_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/gog_icono.ico",
				Color = "#7f3694",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static async Task BuscarOfertasAntiguo(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			int juegos2 = 0;

			int i = 1;
			while (i < 300)
			{
				string html = await Decompiladores.Estandar("https://www.gog.com/games/feed?format=xml&country=ES&currency=EUR&page=" + i.ToString());

				if (string.IsNullOrEmpty(html) == false)
				{
					XmlSerializer xml = new XmlSerializer(typeof(GOGJuegos));
					GOGJuegos listaJuegos = null;

					try
					{
						using (TextReader lector = new StringReader(html))
						{
							listaJuegos = (GOGJuegos)xml.Deserialize(lector);
						}
					}
					catch { }

					if (listaJuegos != null)
					{
						if (listaJuegos.Catalogo != null)
						{
							if (listaJuegos.Catalogo.Juegos.Count > 0)
							{
								foreach (GOGJuego juego in listaJuegos.Catalogo.Juegos)
								{
									int descuento = int.Parse(juego.Descuento);

									if (descuento > 0)
									{
                                        string nombre = WebUtility.HtmlDecode(juego.Nombre);

                                        string enlace = juego.Enlace;

                                        string slug = enlace;
                                        slug = slug.Replace("https://www.gog.com/en/game/", null);

                                        string imagen = "https:" + juego.ImagenVertical;

                                        string tempPrecio = juego.Precio;
                                        tempPrecio = tempPrecio.Replace("€", null);

                                        decimal precioRebajado = decimal.Parse(tempPrecio);

                                        JuegoPrecio oferta = new JuegoPrecio
										{
											Nombre = nombre,
											Enlace = enlace,
											Imagen = imagen,
											Moneda = JuegoMoneda.Euro,
											Precio = precioRebajado,
											Descuento = descuento,
											Tienda = Generar().Id,
											DRM = JuegoDRM.GOG,
											FechaDetectado = DateTime.Now,
											FechaActualizacion = DateTime.Now
										};

										try
										{
											await BaseDatos.Tiendas.Comprobar.Resto(region, oferta, juego.Id, slug);
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
					else
					{
						break;
					}
				}

				i += 1;
			}
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			int juegos2 = 0;

			int i = 1;
			int limite = 100;
			while (i < limite + 1)
			{
				string enlace2 = string.Empty;

				if (region == TiendaRegion.Europa)
				{
					enlace2 = "https://catalog.gog.com/v1/catalog?limit=48&order=desc:trending&discounted=eq:true&productType=in:game,pack,dlc,extras&page=" + i.ToString() + "&countryCode=ES&locale=en-US&currencyCode=EUR";
				}
				else if (region == TiendaRegion.EstadosUnidos)
				{
					enlace2 = "https://catalog.gog.com/v1/catalog?limit=48&order=desc:trending&discounted=eq:true&productType=in:game,pack,dlc,extras&page=" + i.ToString() + "&countryCode=US&locale=en-US&currencyCode=USD";
				}

                string html = await Decompiladores.Estandar(enlace2);

				if (string.IsNullOrEmpty(html) == false)
				{
					GOGOfertas datos = null;
					
					try
					{
                        datos = JsonSerializer.Deserialize<GOGOfertas>(html);
                    }
					catch
					{
						if (html.Contains("overcapacity.jpg") == true)
						{
							await Task.Delay(60000);
							i -= 1;
						}
						else
						{
                            BaseDatos.Errores.Insertar.Mensaje("GOG API", html, enlace2);
                        }
                    }
                    
					if (datos != null)
					{
						limite = datos.Paginas;

						foreach (var juego in datos.Juegos)
						{
							string precioBaseTexto = juego.Precios.PrecioBase;
							precioBaseTexto = precioBaseTexto.Replace("€", null);
							precioBaseTexto = precioBaseTexto.Replace("$", null);
							precioBaseTexto = precioBaseTexto.Replace(",", ".");
							precioBaseTexto = precioBaseTexto.Trim();

							string precioRebajadoTexto = juego.Precios.PrecioRebajado;
							precioRebajadoTexto = precioRebajadoTexto.Replace("€", null);
							precioRebajadoTexto = precioRebajadoTexto.Replace("$", null);
							precioRebajadoTexto = precioRebajadoTexto.Replace(",", ".");
							precioRebajadoTexto = precioRebajadoTexto.Trim();

							decimal precioBase = decimal.Parse(precioBaseTexto);
							decimal precioRebajado = decimal.Parse(precioRebajadoTexto);

							int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

							if (descuento > 0)
							{
								string nombre = WebUtility.HtmlDecode(juego.Nombre);
								string enlace = "https://www.gog.com/en/game/" + juego.Slug;
								string imagen = juego.Imagen;

								JuegoPrecio oferta = new JuegoPrecio
								{
									Nombre = nombre,
									Enlace = enlace,
									Imagen = imagen,
									Moneda = JuegoMoneda.Euro,
									Precio = precioRebajado,
									Descuento = descuento,
									Tienda = Generar().Id,
									DRM = JuegoDRM.GOG,
									FechaDetectado = DateTime.Now,
									FechaActualizacion = DateTime.Now
								};

								if (region == TiendaRegion.EstadosUnidos)
								{
									oferta.Moneda = JuegoMoneda.Dolar;
								}

								try
								{
									if (juego.Tipo == "game" || juego.Tipo == "dlc")
									{
										await BaseDatos.Tiendas.Comprobar.Resto(region, oferta, juego.Id, juego.Slug);
									}
									else
									{
										await BaseDatos.Tiendas.Comprobar.Resto(region, oferta);
									}
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

				i += 1;
			}
		}
	}

	#region Ofertas (Antiguo)

	[XmlRoot("catalogue")]
	public class GOGJuegos
	{
		[XmlElement("products")]
		public GOGJuegosCatalogo Catalogo { get; set; }
	}

	public class GOGJuegosCatalogo
	{
		[XmlElement("product")]
		public List<GOGJuego> Juegos { get; set; }
	}

	public class GOGJuego
	{
		[XmlElement("id")]
		public string Id { get; set; }

		[XmlElement("title")]
		public string Nombre { get; set; }

		[XmlElement("price")]
		public string Precio { get; set; }

		[XmlElement("discount")]
		public string Descuento { get; set; }

		[XmlElement("img_icon")]
		public string ImagenHorizontal { get; set; }

		[XmlElement("img_cover")]
		public string ImagenVertical { get; set; }

		[XmlElement("link")]
		public string Enlace { get; set; }
    }

	#endregion

	#region Ofertas (Nuevo)

	public class GOGOfertas
	{
		[JsonPropertyName("pages")]
		public int Paginas { get; set; }

		[JsonPropertyName("products")]
		public List<GOGOfertasJuego> Juegos { get; set; }
	}

	public class GOGOfertasJuego
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("slug")]
		public string Slug { get; set; }

		[JsonPropertyName("title")]
		public string Nombre { get; set; }

		[JsonPropertyName("coverHorizontal")]
		public string Imagen { get; set; }

		[JsonPropertyName("productType")]
		public string Tipo { get; set; }

		[JsonPropertyName("price")]
		public GOGOfertasJuegoPrecio Precios { get; set; }
	}

	public class GOGOfertasJuegoPrecio
	{
		[JsonPropertyName("final")]
		public string PrecioRebajado { get; set; }

		[JsonPropertyName("base")]
		public string PrecioBase { get; set; }
	}

	#endregion

	#region Datos

	public class GOGGalaxy
	{
	    [JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("purchase_link")]
		public string PurchaseLink { get; set; }

		[JsonPropertyName("slug")]
		public string Slug { get; set; }

		[JsonPropertyName("content_system_compatibility")]
		public GogContentSystemCompatibility ContentSystemCompatibility { get; set; }

		[JsonPropertyName("languages")]
		public Dictionary<string, string> Languages { get; set; }

		[JsonPropertyName("links")]
		public GogLinks Links { get; set; }

		[JsonPropertyName("in_development")]
		public GogInDevelopment InDevelopment { get; set; }

		[JsonPropertyName("is_secret")]
		public bool IsSecret { get; set; }

		[JsonPropertyName("is_installable")]
		public bool IsInstallable { get; set; }

		[JsonPropertyName("game_type")]
		public string GameType { get; set; }

		[JsonPropertyName("is_pre_order")]
		public bool IsPreOrder { get; set; }

		[JsonPropertyName("images")]
		public GogImages Images { get; set; }

		[JsonPropertyName("dlcs")]
		public List<object> Dlcs { get; set; }

		[JsonPropertyName("description")]
		public GogDescription Description { get; set; }

		[JsonPropertyName("screenshots")]
		public List<GogScreenshot> Screenshots { get; set; }

		[JsonPropertyName("videos")]
		public List<GogVideo> Videos { get; set; }

		[JsonPropertyName("related_products")]
		public List<object> RelatedProducts { get; set; }

		[JsonPropertyName("changelog")]
		public string Changelog { get; set; }

		[JsonPropertyName("downloads")]
		public GogDownloads Downloads { get; set; }

		[JsonPropertyName("expanded_dlcs")]
		public List<object> ExpandedDlcs { get; set; }
	}


	public class GogContentSystemCompatibility
	{
		[JsonPropertyName("windows")]
		public bool Windows { get; set; }

		[JsonPropertyName("osx")]
		public bool Osx { get; set; }

		[JsonPropertyName("linux")]
		public bool Linux { get; set; }
	}

	public class GogLinks
	{
		[JsonPropertyName("purchase_link")]
		public string PurchaseLink { get; set; }

		[JsonPropertyName("product_card")]
		public string ProductCard { get; set; }

		[JsonPropertyName("support")]
		public string Support { get; set; }

		[JsonPropertyName("forum")]
		public string Forum { get; set; }
	}

	public class GogInDevelopment
	{
		[JsonPropertyName("active")]
		public bool Active { get; set; }

		[JsonPropertyName("until")]
		public DateTime? Until { get; set; }
	}

	public class GogImages
	{
		[JsonPropertyName("background")]
		public string Background { get; set; }

		[JsonPropertyName("logo")]
		public string Logo { get; set; }

		[JsonPropertyName("logo2x")]
		public string Logo2x { get; set; }

		[JsonPropertyName("sidebarIcon")]
		public string SidebarIcon { get; set; }

		[JsonPropertyName("sidebarIcon2x")]
		public string SidebarIcon2x { get; set; }

		[JsonPropertyName("menuNotificationAv")]
		public string MenuNotificationAv { get; set; }

		[JsonPropertyName("menuNotificationAv2")]
		public string MenuNotificationAv2 { get; set; }

		[JsonPropertyName("icon")]
		public string Icon { get; set; }
	}

	public class GogDownloads
	{
		[JsonPropertyName("installers")]
		public List<GogInstaller> Installers { get; set; }

		[JsonPropertyName("patches")]
		public List<object> Patches { get; set; }

		[JsonPropertyName("language_packs")]
		public List<object> LanguagePacks { get; set; }

		[JsonPropertyName("bonus_content")]
		public List<GogBonusContent> BonusContent { get; set; }
	}

	public class GogInstaller
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("os")]
		public string Os { get; set; }

		[JsonPropertyName("language")]
		public string Language { get; set; }

		[JsonPropertyName("language_full")]
		public string LanguageFull { get; set; }

		[JsonPropertyName("version")]
		public string Version { get; set; }

		[JsonPropertyName("total_size")]
		public long TotalSize { get; set; }

		[JsonPropertyName("files")]
		public List<GogFile> Files { get; set; }
	}

	public class GogFile
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("size")]
		public long Size { get; set; }

		[JsonPropertyName("downlink")]
		public string Downlink { get; set; }
	}

	public class GogBonusContent
	{
		[JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("type")]
		public string Type { get; set; }

		[JsonPropertyName("count")]
		public int Count { get; set; }

		[JsonPropertyName("total_size")]
		public long TotalSize { get; set; }

		[JsonPropertyName("files")]
		public List<GogFile> Files { get; set; }
	}

	public class GogDescription
	{
		[JsonPropertyName("lead")]
		public string Lead { get; set; }

		[JsonPropertyName("full")]
		public string Full { get; set; }

		[JsonPropertyName("whats_cool_about_it")]
		public string WhatsCoolAboutIt { get; set; }
	}

	public class GogScreenshot
	{
		[JsonPropertyName("image_id")]
		public string ImageId { get; set; }

		[JsonPropertyName("formatter_template_url")]
		public string FormatterTemplateUrl { get; set; }

		[JsonPropertyName("formatted_images")]
		public List<GogFormattedImage> FormattedImages { get; set; }
	}

	public class GogFormattedImage
	{
		[JsonPropertyName("formatter_name")]
		public string FormatterName { get; set; }

		[JsonPropertyName("image_url")]
		public string ImageUrl { get; set; }
	}

	public class GogVideo
	{
		[JsonPropertyName("video_url")]
		public string VideoUrl { get; set; }

		[JsonPropertyName("thumbnail_url")]
		public string ThumbnailUrl { get; set; }

		[JsonPropertyName("provider")]
		public string Provider { get; set; }
	}

	public class GOGGalaxy2
    {
        [JsonPropertyName("_embedded")]
        public GOGGalaxy2Caracteristicas Caracteristicas { get; set; }

		[JsonPropertyName("_links")]
		public GOGGalaxy2Enlaces Enlaces { get; set; }

		[JsonPropertyName("releaseStatus")]
		public string SeHaLanzado { get; set; }
	}

    public class GOGGalaxy2Caracteristicas
    {
		[JsonPropertyName("product")]
		public GOGGalaxy2Producto Producto { get; set; }

		[JsonPropertyName("localizations")]
		public List<GOGGalaxy2Idioma> Idiomas { get; set; }

		[JsonPropertyName("features")]
        public List<GOGGalaxy2Caracteristica> Datos { get; set; }

		[JsonPropertyName("properties")]
		public List<GOGGalaxy2Propiedad> Propiedades { get; set; }
	}

	public class GOGGalaxy2Producto
	{
		[JsonPropertyName("gogReleaseDate")]
		public string FechaLanzamiento { get; set; }
	}

	public class GOGGalaxy2Idioma
	{
		[JsonPropertyName("_embedded")]
		public GOGGalaxy2IdiomaDatos Datos { get; set; }
	}

	public class GOGGalaxy2IdiomaDatos
	{
		[JsonPropertyName("language")]
		public GOGGalaxy2IdiomaDatosIdioma Idioma { get; set; }

		[JsonPropertyName("localizationScope")]
		public GOGGalaxy2IdiomaDatosTipo Tipo { get; set; }
	}

	public class GOGGalaxy2IdiomaDatosIdioma
	{
		[JsonPropertyName("code")]
		public string Codigo { get; set; }

		[JsonPropertyName("name")]
		public string Nombre { get; set; }
	}

	public class GOGGalaxy2IdiomaDatosTipo
	{
		[JsonPropertyName("type")]
		public string Nombre { get; set; }
	}

	public class GOGGalaxy2Caracteristica
	{
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

	public class GOGGalaxy2Propiedad
	{
		[JsonPropertyName("slug")]
		public string Slug { get; set; }
	}
	public class GOGGalaxy2Enlaces
	{
		[JsonPropertyName("isIncludedInGames")]
		public List<GOGGalaxy2Enlace> Listado { get; set; }
	}

	public class GOGGalaxy2Enlace
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("isSecret")]
		public bool Secreto { get; set; }

		[JsonPropertyName("releaseStatus")]
		public string Estado { get; set; }
	}

	#endregion
}
