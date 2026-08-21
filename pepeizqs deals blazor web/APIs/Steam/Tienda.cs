//https://store.steampowered.com/search/results/?query&start=350&count=50&dynamic_data=&sort_by=Price_ASC&force_infinite=1&supportedlang=english&specials=1&hidef2p=1&ndl=1&infinite=1&ignore_preferences=1
//https://api.steampowered.com/IStoreQueryService/Query/v1/?input_json={%22query%22:{%22filters%22:{%22tagids_must_match%22:[{%22tagids%22:[%229%22]}]}},%22context%22:{%22language%22:%22english%22,%22country_code%22:%22US%22,%22steam_realm%22:%221%22},%22data_request%22:{%22include_basic_info%22:true}}
//https://store.steampowered.com/saleaction/ajaxgetdeckappcompatibilityreport?nAppID=1868140&l=spanish&cc=ES
//https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid=730
//https://api.steampowered.com/IStoreService/GetAppList/v1/?key=[devkey]&max_results=50000
//https://store.steampowered.com/appreviews/730?json=1
//https://store.steampowered.com/curator/185907/ajaxgetcreatorhomeinfo?get_appids=true
//https://store.steampowered.com/actions/ajaxresolvebundles?bundleids=45867&cc=ES&l=english

#nullable disable

using Herramientas;
using Juegos;
using Microsoft.VisualBasic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.Steam
{
	public static class Tienda
	{
        public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "steam",
				Nombre = "Steam",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/steam_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/steam_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/steam_icono.webp",
				Color = "#2e4460",
                AdminUso = true,
				UsuarioUso = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static Tiendas2.Tienda GenerarBundles()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "steambundles",
				Nombre = "Steam (Bundles)",
				ImagenLogo = "/imagenes/tiendas/steam_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/steam_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/steam_icono.webp",
				Color = "#2e4460",
				AdminUso = false,
				UsuarioUso = false
			};

			return tienda;
		}

		public static string Referido(string enlace)
        {
            return enlace + "?curator_clanid=33500256";
        }

		public static async Task BuscarOfertas(bool mirarOfertas)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, Generar().Id, DateTime.Now, 0);

			JsonSerializerOptions opciones = new JsonSerializerOptions
			{
				UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
			};

			int juegos = 0;

			bool rapido = false;
			int arranque = await BaseDatos.Admin.Buscar.TiendasValorAdicional(TiendaRegion.Europa, Generar().Id, "valorAdicional");
			int tope = await BaseDatos.Admin.Buscar.TiendasValorAdicional(TiendaRegion.Europa, Generar().Id, "valorAdicional2");

			if (DateTime.Now.Hour == 19 && DateTime.Now.Minute >= 0 && DateTime.Now.Minute < 10)
			{
				arranque = 0;
				rapido = true;
			}
			else if (DateTime.Now.Hour == 20)
			{
				rapido = true;
            }

			if (arranque >= tope - 50)
			{
				arranque = 0;
			}

			int i = arranque;
            while (i < tope)
            {
				string html = null;

				if (mirarOfertas == true)
				{
					string html2 = await Decompiladores.GZipFormato3("https://store.steampowered.com/search/results/?query&start=" + i.ToString() + "&count=50&dynamic_data=&force_infinite=1&supportedlang=english&specials=1&ndl=1&infinite=1&ignore_preferences=1&l=english&category1=998%2C21%2C990%2C994");

					try
					{
						SteamQueryAPI datos = JsonSerializer.Deserialize<SteamQueryAPI>(html2, opciones);

						if (datos != null)
						{
							html = datos.Html;
						}

						tope = datos.Total;
					}
					catch { }

					if (html == null)
					{
						html = html2;
					}

					if (html2.Contains("total_count") == true)
					{
						int int1 = html2.IndexOf("total_count");

						if (int1 != -1)
						{
							string temp1 = html2.Remove(0, int1);

							int int2 = temp1.IndexOf(":");
							string temp2 = temp1.Remove(0, int2 + 1);

							int int3 = temp2.IndexOf(",");
							string temp3 = temp2.Remove(int3, temp2.Length - int3);

							tope = int.Parse(temp3.Trim());

							if (arranque > tope)
							{
								i = 0;
							}
						}
					}

					if (tope < 1000)
					{
						tope = 10000;
					}
				}
				else
				{
					string html2 = await Decompiladores.GZipFormato3("https://store.steampowered.com/search/results/?query&start=" + i.ToString() + "&count=50&dynamic_data=&force_infinite=1&supportedlang=english&ndl=1&infinite=1&l=english");

					try
					{
						SteamQueryAPI datos = JsonSerializer.Deserialize<SteamQueryAPI>(html2, opciones);

						if (datos != null)
						{
							html = datos.Html;
						}
					}
					catch { }

					if (html == null)
					{
						html = html2;
					}
				}

				await BaseDatos.Admin.Actualizar.TiendasValorAdicional(TiendaRegion.Europa, Generar().Id, "valorAdicional2", tope);

				if (string.IsNullOrEmpty(html) == false)
				{
					if (html.Contains("<!-- List Items -->") == true)
					{
						int int1 = html.IndexOf("<!-- List Items -->");
						html = html.Remove(0, int1);

						int int2 = html.IndexOf("<!-- End List Items -->");
						html = html.Remove(int2, html.Length - int2);

						List<JuegoPrecio> ofertas = new List<JuegoPrecio>();
						List<JuegoReseñas> reseñas = new List<JuegoReseñas>();

						int j = 0;
						while (j < 50)
						{
							if (html.Contains("<a href=" + Strings.ChrW(34) + "https://store.steampowered.com/") == true)
							{
								int int3 = html.IndexOf("<a href=" + Strings.ChrW(34) + "https://store.steampowered.com/");
								string temp3 = html.Remove(0, int3 + 5);

								html = temp3;

								int int4 = temp3.IndexOf("</a>");
								string temp4 = temp3.Remove(int4, temp3.Length - int4);

								int int5 = temp4.IndexOf("<span class=" + Strings.ChrW(34) + "title" + Strings.ChrW(34) + ">");
								string temp5 = temp4.Remove(0, int5);

								int int6 = temp5.IndexOf("</span>");
								string temp6 = temp5.Remove(int6, temp5.Length - int6);

								int5 = temp6.IndexOf(">");
								temp6 = temp6.Remove(0, int5 + 1);

								string titulo = temp6.Trim();
								titulo = WebUtility.HtmlDecode(titulo);

								int int7 = temp4.IndexOf("https://");
								string temp7 = temp4.Remove(0, int7);

								int int8 = temp7.IndexOf("?");
								string temp8 = temp7.Remove(int8, temp7.Length - int8);

								string enlace = temp8.Trim();

								if (enlace.Contains("https://store.steampowered.com/app/") == true)
								{
									enlace = "https://store.steampowered.com/app/" + Juego.LimpiarID(enlace);
								}

								int int9 = temp4.IndexOf("<img src=");
								string temp9 = temp4.Remove(0, int9 + 10);

								int int10 = temp9.IndexOf(Strings.ChrW(34));
								string temp10 = temp9.Remove(int10, temp9.Length - int10);

								if (string.IsNullOrEmpty(temp10) == false)
								{
									int10 = temp10.IndexOf("?");

									if (int10 > -1)
									{
										temp10 = temp10.Remove(int10, temp10.Length - int10);
									}
								}

								string imagen = temp10?.Trim();

								JuegoAnalisis reseña = new JuegoAnalisis
								{
									Cantidad = "0",
									Porcentaje = "0"
								};

								if (enlace.Contains("https://store.steampowered.com/app/") == true || enlace.Contains("https://store.steampowered.com/bundle/") == true)
								{
									int int11 = temp4.IndexOf("data-tooltip-html=");

									if (int11 != -1)
									{
										string temp11 = temp4.Remove(0, int11);

										int int12 = temp11.IndexOf("%");
										string temp12 = temp11.Remove(int12, temp11.Length - int12);

										temp12 = temp12.Remove(0, temp12.Length - 2);
										temp12 = temp12.Trim();

										if (temp12.Contains(";") == true)
										{
											temp12 = temp12.Replace(";", "0");
										}

										if (temp12 == "00")
										{
											temp12 = "100";
										}

										string porcentaje = temp12;

										int int13 = temp4.IndexOf("data-tooltip-html=");
										string temp13 = temp4.Remove(0, int13);

										int int14 = temp13.IndexOf("user reviews");
										string temp14 = temp13.Remove(int14, temp13.Length - int14);

										int14 = temp14.IndexOf("of the");
										temp14 = temp14.Remove(0, int14 + 6);

										string cantidad = temp14.Trim();

										if (cantidad.Length > 1)
										{
											reseña.Cantidad = cantidad;
											reseña.Porcentaje = porcentaje;
										}
									}
								}

								bool suficientesReseñas = false;

								if (reseña.Cantidad.Length > 1)
								{
									suficientesReseñas = true;
								}

								if (suficientesReseñas == true)
								{
									int int11 = temp4.IndexOf("data-discount=" + Strings.ChrW(34));

									if (int11 != -1)
									{
										string temp11 = temp4.Remove(0, int11);

										int11 = temp11.IndexOf(Strings.ChrW(34));
										temp11 = temp11.Remove(0, int11 + 1);

										int int12 = temp11.IndexOf(Strings.ChrW(34));
										string temp12 = temp11.Remove(int12, temp11.Length - int12);

										int descuento = 0;

										if (int12 != -1)
										{
											temp12 = temp12.Replace("-", null);
											temp12 = temp12.Replace("%", null);

											descuento = int.Parse(temp12.Trim());
										}

										if (descuento > 0)
										{
											int int13 = temp4.IndexOf(Strings.ChrW(34) + "discount_final_price" + Strings.ChrW(34));
											string temp13 = temp4.Remove(0, int13);

											int13 = temp13.IndexOf(Strings.ChrW(34) + ">");
											temp13 = temp13.Remove(0, int13 + 2);

											int int14 = temp13.IndexOf("</div>");
											string temp14 = temp13.Remove(int14, temp13.Length - int14);

											if (temp14 != null)
											{
												temp14 = temp14.Replace("--", "00");
												temp14 = temp14.Replace(",", ".");
												temp14 = temp14.Replace("€", null);
												temp14 = temp14.Replace(" ", null);
											}

											bool precioFormato = true;

											if (temp14.Contains("Free") == true)
											{
												precioFormato = false;
											}
											else if (temp14.Length == 0)
											{
												precioFormato = false;
											}

											if (precioFormato == true)
											{
												decimal precio = decimal.Parse(temp14.Trim());

												reseñas.Add(new JuegoReseñas
												{
													IdSteam = int.Parse(Juego.LimpiarID(enlace)),
													Cantidad = reseña.Cantidad,
													Porcentaje = reseña.Porcentaje
												});

												JuegoPrecio oferta = new JuegoPrecio
												{
													Nombre = titulo,
													Imagen = imagen,
													Tienda = Generar().Id,
													DRM = JuegoDRM.Steam,
													Descuento = descuento,
													Precio = precio,
													Moneda = JuegoMoneda.Euro,
													Enlace = enlace,
													FechaDetectado = DateTime.Now,
													FechaActualizacion = DateTime.Now
												};

												if (enlace.Contains("https://store.steampowered.com/app/") == true)
												{
													ofertas.Add(oferta);
												}

												juegos += 1;
											}
										}
									}
								}
							}
							else
							{
								break;
							}

							j += 1;
						}

						if (ofertas?.Count > 0)
						{
							try
							{
								await BaseDatos.Tiendas.Comprobar.Steam(TiendaRegion.Europa, ofertas, reseñas, rapido);
							}
							catch (Exception ex)
							{
								BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
							}
						}

						try
						{
							await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, Generar().Id, DateTime.Now, juegos);
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
						}
					}
				}

				i += 50;
				await BaseDatos.Admin.Actualizar.TiendasValorAdicional(TiendaRegion.Europa, Generar().Id, "valorAdicional", i);
			}
		}

		public static async Task BuscarOfertas2(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			JsonSerializerOptions opciones = new JsonSerializerOptions
			{
				UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
			};

			int arranque = 0;
			int tope = 1000;

			arranque = await BaseDatos.Admin.Buscar.TiendasValorAdicional(region, Generar().Id, "valorAdicional");
			tope = await BaseDatos.Admin.Buscar.TiendasValorAdicional(region, Generar().Id, "valorAdicional2");

			int juegos2 = 0;
			bool rapido = false;

			if (DateTime.Now.Hour == 19 || DateTime.Now.Hour == 20)
			{
				rapido = true;
			}

			if (arranque >= tope - 50)
			{
				arranque = 0;
			}

			while (arranque < tope)
			{
				string rapidoTexto1 = null;
				string rapidoTexto2 = null;

				if (rapido == false)
				{
					rapidoTexto1 = "%22include_all_purchase_options%22:true,";
					rapidoTexto2 = ",%22include_apps%22:true,%22include_packages%22:true,%22include_bundles%22:true,%22include_dlc%22:true,%22include_software%22:true,%22include_music%22:true";
				}

				string regionTexto = "ES";

				if (region == TiendaRegion.EstadosUnidos)
				{
					regionTexto = "US";
				}

				string html = await Decompiladores.Estandar("https://api.steampowered.com/IStoreQueryService/Query/v1/?input_json={%22query%22:{%22start%22:" + arranque + ",%22count%22:1000,%22filters%22:{%22released_only%22:true,%22type_filters%22:{%22include_games%22:true" + rapidoTexto2 + "},%22price_filters%22:[{%22exclude_free_items%22:true,%22min_discount_percent%22:%221%22}]}},%22context%22:{%22language%22:%22english%22,%22country_code%22:%22" + regionTexto + "%22,%22steam_realm%22:%221%22},%22data_request%22:{" + rapidoTexto1 + "%22include_reviews%22:true}}");

				if (string.IsNullOrEmpty(html) == false)
				{
					SteamStoreQueryRoot resultado = JsonSerializer.Deserialize<SteamStoreQueryRoot>(html, opciones);
					List<SteamStoreItem> juegos = resultado?.Respuesta?.TiendaResultados;

					tope = (int)resultado?.Respuesta?.Metadata?.CantidadTotal;

					await BaseDatos.Admin.Actualizar.TiendasValorAdicional(region, Generar().Id, "valorAdicional2", tope);

					if (juegos?.Count > 0)
					{
						List<JuegoPrecio> ofertas = new List<JuegoPrecio>();
						List<JuegoPrecio> bundles = new List<JuegoPrecio>();
						List<JuegoReseñas> reseñas = new List<JuegoReseñas>();

						foreach (var juego in juegos)
						{
							if (juego.OpcionCompraMejor != null)
							{
								SteamPurchaseOption opcionCompra = juego.OpcionCompraMejor;

								if (opcionCompra.MustPurchaseAsSet == true)
								{
									continue;
								}

								if (opcionCompra.Descuento != null)
								{
									if (opcionCompra.Descuento > 0 && opcionCompra.PaqueteId > 0 && juego.Reseñas?.SumarioFiltrado?.ReseñasCantidad > 9)
									{
										string precioString = opcionCompra.FinalPriceInCents;
										decimal precio = decimal.Parse(precioString) / 100m;

										JuegoAnalisis reseña = new JuegoAnalisis
										{
											Cantidad = juego.Reseñas.SumarioFiltrado.ReseñasCantidad.ToString(),
											Porcentaje = juego.Reseñas.SumarioFiltrado.ReseñasPorcentaje.ToString()
										};

										reseñas.Add(new JuegoReseñas
										{
											IdSteam = (int)juego.Id,
											Cantidad = reseña.Cantidad,
											Porcentaje = reseña.Porcentaje
										});

										JuegoPrecio oferta = new JuegoPrecio
										{
											Nombre = juego.Nombre,
											Tienda = Generar().Id,
											DRM = JuegoDRM.Steam,
											Descuento = (int)opcionCompra.Descuento,
											Precio = precio,
											Moneda = JuegoMoneda.Euro,
											Enlace = "https://store.steampowered.com/app/" + juego.AppId.ToString(),
											FechaDetectado = DateTime.Now,
											FechaActualizacion = DateTime.Now
										};

										if (region == TiendaRegion.EstadosUnidos)
										{
											oferta.Moneda = JuegoMoneda.Dolar;
										}

										if (juego.OpcionCompraMejor?.ActiveDiscounts?.Count > 0)
										{
											oferta.FechaTermina = DateTime.UnixEpoch.AddSeconds(juego.OpcionCompraMejor?.ActiveDiscounts[0]?.DiscountEndDate ?? 0).ToLocalTime();
										}

										ofertas.Add(oferta);

										juegos2 += 1;
									}
								}
							}

							if (rapido == false)
							{
								if (juego.OpcionesCompra?.Count > 0)
								{
									foreach (var opcionCompra in juego.OpcionesCompra)
									{
										if (opcionCompra == null)
										{
											continue;
										}

										if (opcionCompra.BundleId.HasValue == false ||
											opcionCompra.BundleDescuento.HasValue == false ||
											opcionCompra.BundleId.Value <= 0 ||
											opcionCompra.BundleDescuento.Value <= 0 ||
											opcionCompra.MustPurchaseAsSet == true)
										{
											continue;
										}

										if (juego.Reseñas?.SumarioFiltrado?.ReseñasCantidad > 9)
										{
											JuegoSteamBundle bundle = new JuegoSteamBundle();
											bundle.Junto = false;
											bundle.Descuento = opcionCompra.BundleDescuento.GetValueOrDefault();

											string precioString = opcionCompra.FinalPriceInCents;
											decimal precio = decimal.Parse(precioString) / 100m;

											JuegoPrecio oferta = new JuegoPrecio
											{
												Nombre = opcionCompra.BundleNombre,
												Tienda = GenerarBundles().Id,
												DRM = JuegoDRM.Steam,
												Descuento = opcionCompra.Descuento.GetValueOrDefault(),
												Precio = precio,
												Moneda = JuegoMoneda.Euro,
												Enlace = "https://store.steampowered.com/bundle/" + opcionCompra.BundleId.ToString(),
												FechaDetectado = DateTime.Now,
												FechaActualizacion = DateTime.Now,
												BundleSteam = bundle
											};

											if (opcionCompra.ActiveDiscounts?.Count > 0)
											{
												oferta.FechaTermina = DateTime.UnixEpoch.AddSeconds(opcionCompra.ActiveDiscounts[0]?.DiscountEndDate ?? 0).ToLocalTime();
											}

											if (region == TiendaRegion.EstadosUnidos)
											{
												oferta.Moneda = JuegoMoneda.Dolar;
											}

											bundles.Add(oferta);

											juegos2 += 1;
										}
									}
								}
							}
						}

						if (ofertas?.Count > 0)
						{
							try
							{
								await BaseDatos.Tiendas.Comprobar.Steam(region, ofertas, reseñas, rapido);
							}
							catch (Exception ex)
							{
								BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
							}
						}

						if (bundles?.Count > 0)
						{
							try
							{
								await BaseDatos.Tiendas.Comprobar.Resto(region, bundles);
							}
							catch (Exception ex)
							{
								BaseDatos.Errores.Insertar.Mensaje(GenerarBundles().Id, ex);
							}
						}

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

				arranque += 1000;
				await BaseDatos.Admin.Actualizar.TiendasValorAdicional(region, Generar().Id, "valorAdicional", arranque);
			}
		}

		public static string IdsEspeciales(string id)
		{
			#region Total War: SHOGUN 2 DLC es juego

			if (id == "201270")
			{
				id = "34330";
			}

			#endregion

			#region Call of Duty®: Modern Warfare® 3(2011) Config es juego

			if (id == "115300")
			{
				id = "42680";
			}

			#endregion

			#region The Elder Scrolls IV: Oblivion® Game of the Year Edition Deluxe

			if (id == "900883")
			{
				id = "22330";
			}

			#endregion

			return id;
		}
	}

	public class SteamQueryAPI
	{
		[JsonPropertyName("results_html")]
		public string Html { get; set; }

		[JsonPropertyName("total_count")]
		public int Total { get; set; }
	}

	//----------------------------------------------------------------------

	public class SteamStoreQueryRoot
	{
		[JsonPropertyName("response")]
		public SteamStoreQueryResponse Respuesta { get; set; }
	}

	public class SteamStoreQueryResponse
	{
		[JsonPropertyName("metadata")]
		public SteamQueryMetadata Metadata { get; set; }

		[JsonPropertyName("store_items")]
		public List<SteamStoreItem> TiendaResultados { get; set; }
	}

	public class SteamQueryMetadata
	{
		[JsonPropertyName("total_matching_records")]
		public int CantidadTotal { get; set; }
	}

	public class SteamStoreItem
	{
		[JsonPropertyName("id")]
		public uint Id { get; set; }

		[JsonPropertyName("appid")]
		public uint? AppId { get; set; }

		[JsonPropertyName("success")]
		public int Success { get; set; }

		[JsonPropertyName("visible")]
		public bool Visible { get; set; }

		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("store_url_path")]
		public string StoreUrlPath { get; set; }

		[JsonPropertyName("type")]
		public int Tipo { get; set; }

		[JsonPropertyName("is_free")]
		public bool? IsFree { get; set; }

		[JsonPropertyName("reviews")]
		public SteamReviews Reseñas { get; set; }

		[JsonPropertyName("best_purchase_option")]
		public SteamPurchaseOption OpcionCompraMejor { get; set; }

		[JsonPropertyName("purchase_options")]
		public List<SteamPurchaseOption> OpcionesCompra { get; set; }

		[JsonPropertyName("accessories")]
		public List<SteamPurchaseOption> Suscripciones { get; set; }
	}

	public class SteamReviews
	{
		[JsonPropertyName("summary_filtered")]
		public SteamReviewSummary SumarioFiltrado { get; set; }

		[JsonPropertyName("summary_language_specific")]
		public SteamReviewSummary SummaryLanguageSpecific { get; set; }
	}

	public class SteamReviewSummary
	{
		[JsonPropertyName("review_count")]
		public int ReseñasCantidad { get; set; }

		[JsonPropertyName("percent_positive")]
		public int ReseñasPorcentaje { get; set; }

		[JsonPropertyName("review_score")]
		public int ReviewScore { get; set; }

		[JsonPropertyName("review_score_label")]
		public string ReviewScoreLabel { get; set; }
	}

	public class SteamPurchaseOption
	{
		[JsonPropertyName("packageid")]
		public uint? PaqueteId { get; set; }

		[JsonPropertyName("bundleid")]
		public uint? BundleId { get; set; }

		[JsonPropertyName("purchase_option_name")]
		public string BundleNombre { get; set; }

		[JsonPropertyName("final_price_in_cents")]
		public string FinalPriceInCents { get; set; }

		[JsonPropertyName("original_price_in_cents")]
		public string OriginalPriceInCents { get; set; }

		[JsonPropertyName("formatted_final_price")]
		public string PrecioFormateado { get; set; }

		[JsonPropertyName("formatted_original_price")]
		public string FormattedOriginalPrice { get; set; }

		[JsonPropertyName("discount_pct")]
		public int? Descuento { get; set; }

		[JsonPropertyName("bundle_discount_pct")]
		public int? BundleDescuento { get; set; }

		[JsonPropertyName("included_game_count")]
		public int IncludedGameCount { get; set; }

		[JsonPropertyName("must_purchase_as_set")]
		public bool MustPurchaseAsSet { get; set; }

		[JsonPropertyName("user_can_purchase_as_gift")]
		public bool UserCanPurchaseAsGift { get; set; }

		[JsonPropertyName("active_discounts")]
		public List<SteamDiscount> ActiveDiscounts { get; set; }
	}

	public class SteamDiscount
	{
		[JsonPropertyName("discount_amount")]
		public string DiscountAmount { get; set; }

		[JsonPropertyName("discount_description")]
		public string DiscountDescription { get; set; }

		[JsonPropertyName("discount_end_date")]
		public long DiscountEndDate { get; set; }
	}

	public class SteamRelatedItems
	{
		[JsonPropertyName("parent_appid")]
		public uint? ParentAppId { get; set; }

		[JsonPropertyName("demo_appid")]
		public List<uint> DemoAppId { get; set; }

		[JsonPropertyName("standalone_demo_appid")]
		public List<uint> StandaloneDemoAppId { get; set; }
	}
}
