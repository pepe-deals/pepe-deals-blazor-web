#nullable disable

using Juegos;
using Tiendas2;

namespace Herramientas.RedesSociales
{
	public static class Reddit
	{
		public static async Task<bool> Postear(IConfiguration configuracion, Noticias.Noticia noticia, string dominio)
		{
			bool estado = false;

			string cuenta = configuracion.GetValue<string>("Reddit:Cuenta");
			string contraseña = configuracion.GetValue<string>("Reddit:Contraseña");
			string clienteId = configuracion.GetValue<string>("Reddit:ClientID");
			string clienteSecreto = configuracion.GetValue<string>("Reddit:ClientSecret");

			Reddit2.BotWebAgent webAgent = new Reddit2.BotWebAgent(cuenta, contraseña, clienteId, clienteSecreto, "https://" + dominio + "/");
			RedditSharp.Reddit reddit = new RedditSharp.Reddit(webAgent, false);

			string titulo = string.Empty;

			if (noticia.NoticiaTipo == Noticias.NoticiaTipo.Bundles)
			{
				titulo = "[Bundle] " + noticia.TituloEn;
			}

			if (noticia.NoticiaTipo == Noticias.NoticiaTipo.Gratis)
			{
				titulo = "[Free] " + noticia.TituloEn;
			}

			if (noticia.NoticiaTipo == Noticias.NoticiaTipo.Suscripciones)
			{
				titulo = "[Subscriptions] " + noticia.TituloEn;
			}

			string enlace = string.Empty;

			if (string.IsNullOrEmpty(noticia.Enlace) == true)
			{
				enlace = "https://" + dominio + "/news/" + noticia.Id.ToString();
			}
			else
			{
				enlace = noticia.Enlace;
			}

			if (string.IsNullOrEmpty(titulo) == false)
			{
				RedditSharp.Things.Subreddit subredditEU = await reddit.GetSubredditAsync("gamedealsue");

				if (subredditEU != null)
				{
					try
					{
						RedditSharp.Things.Post post = await subredditEU.SubmitPostAsync(titulo, enlace);

						if (post != null)
						{
							estado = true;
						}
					}
					catch (UriFormatException)
					{
						// Bug conocido de RedditSharp: el post se crea correctamente en Reddit,
						// pero falla al parsear la respuesta (campo url/permalink mal formado).
						estado = true;
					}
					catch (Exception ex)
					{
						global::BaseDatos.Errores.Insertar.Mensaje("Reddit Postear Noticia EU", ex);
					}
				}

				RedditSharp.Things.Subreddit subredditUS = await reddit.GetSubredditAsync("gamedealsUS");

				if (subredditUS != null)
				{
					try
					{
						RedditSharp.Things.Post post = await subredditUS.SubmitPostAsync(titulo, enlace);

						if (post != null)
						{
							estado = true;
						}
					}
					catch (UriFormatException)
					{
						// Bug conocido de RedditSharp: el post se crea correctamente en Reddit,
						// pero falla al parsear la respuesta (campo url/permalink mal formado).
						estado = true;
					}
					catch (Exception ex)
					{
						global::BaseDatos.Errores.Insertar.Mensaje("Reddit Postear Noticia US", ex);
					}
				}
			}
				
			return estado;
		}

		public static async Task<string> Bundle(Bundles2.Bundle bundle)
		{
			string texto = null;

			if (bundle != null)
			{
				texto = "[" + bundle.Enlace + "](" + bundle.Enlace + ")" + Environment.NewLine + Environment.NewLine;

				if (bundle.Tiers != null)
				{
					bundle.Tiers.Sort(delegate (Bundles2.BundleTier t1, Bundles2.BundleTier t2)
					{
						return t1.Posicion.CompareTo(t2.Posicion);
					});

					decimal totalMinimos = 0;

					foreach (var tier in bundle.Tiers.ToList())
					{
						List<Bundles2.BundleJuego> juegosTier = new List<Bundles2.BundleJuego>();

						if (bundle.Juegos?.Count > 0)
						{
							foreach (var juego in bundle.Juegos)
							{
								if (juego.Tier?.Posicion == tier.Posicion)
								{
									juegosTier.Add(juego);
								}
							}
						}

						if (juegosTier.Count > 0)
						{
							juegosTier = juegosTier.OrderBy(x => x.Nombre).ToList();
						}

						foreach (var juego in juegosTier)
						{
							if (juego.Juego?.Tipo == Juegos.JuegoTipo.DLC)
							{
								if (string.IsNullOrEmpty(juego.Juego?.Maestro) == false)
								{
									if (juego.Juego?.Maestro != "no")
									{
										foreach (var juego2 in juegosTier)
										{
											if (juego2.JuegoId == juego.Juego.Maestro)
											{
												if (juego2.DLCs == null)
												{
													juego2.DLCs = new List<string>();
												}

												bool añadir = true;

												if (juego2.DLCs.Count > 0)
												{
													foreach (var dlc in juego2.DLCs)
													{
														if (dlc == juego.JuegoId)
														{
															añadir = false;
															break;
														}
													}
												}

												if (añadir == true)
												{
													juego2.DLCs.Add(juego.JuegoId);
												}
											}
										}
									}
								}
							}
						}

						if (bundle.Pick == true)
						{
							if (tier.Posicion == 1)
							{
								foreach (var tier2 in bundle.Tiers)
								{
									if (tier2.CantidadJuegos == 1)
									{
										texto = texto + "**" + tier2.CantidadJuegos.ToString() + " " + Herramientas.Idiomas.BuscarTexto("en", "String21", "Bundle") + " • " + Herramientas.Precios.Euro(decimal.Parse(tier2.Precio)) + "**" + Environment.NewLine + Environment.NewLine;
									}
									else if (tier2.CantidadJuegos > 1)
									{
										texto = texto + "**" + tier2.CantidadJuegos.ToString() + " " + Herramientas.Idiomas.BuscarTexto("en", "String8", "Bundle") + " • " + Herramientas.Precios.Euro(decimal.Parse(tier2.Precio)) + "** / " + Herramientas.Precios.Euro(decimal.Parse(tier2.Precio) / tier2.CantidadJuegos) + " (" + Herramientas.Idiomas.BuscarTexto("en", "String20", "Bundle") + ")" + Environment.NewLine + Environment.NewLine;
									}
								}

								texto = texto + Environment.NewLine;
							}
						}
						else
						{
							texto = texto + "**Tier " + tier.Posicion.ToString() + ": " + Herramientas.Precios.Euro(decimal.Parse(tier.Precio)) + "**  " + Environment.NewLine;

							foreach (var juego in juegosTier)
							{
								if (juego.Juego == null)
								{
									juego.Juego = await global::BaseDatos.Juegos.Buscar.UnJuego(juego.JuegoId);
								}

								if (juego.Juego?.PrecioMinimosHistoricos != null)
								{
									foreach (var historico in juego.Juego.PrecioMinimosHistoricos)
									{
										if (historico.DRM == juego.DRM)
										{
											if (historico.PrecioCambiado > 0)
											{
												totalMinimos = totalMinimos + historico.PrecioCambiado;
											}
											else
											{
												totalMinimos = totalMinimos + historico.Precio;
											}

											break;
										}
									}
								}
							}

							texto = texto + Herramientas.Idiomas.BuscarTexto("en", "String14", "Bundle") + ": " + Herramientas.Precios.Euro(totalMinimos) + Environment.NewLine + Environment.NewLine;
						}

						if (juegosTier.Count > 0)
						{
							foreach (var juego in juegosTier)
							{
								if (juego.Juego == null)
								{
									juego.Juego = await global::BaseDatos.Juegos.Buscar.UnJuego(juego.JuegoId);
								}

								if (juego.Juego?.Tipo == Juegos.JuegoTipo.DLC)
								{
									if (string.IsNullOrEmpty(juego.Juego?.Maestro) == false)
									{
										if (juego.Juego?.Maestro != "no")
										{
											foreach (var juego2 in juegosTier)
											{
												if (juego2.JuegoId == juego.Juego?.Maestro)
												{
													if (juego2.DLCs == null)
													{
														juego2.DLCs = new List<string>();
													}

													bool añadir = true;

													if (juego2.DLCs.Count > 0)
													{
														foreach (var dlc in juego2.DLCs)
														{
															if (dlc == juego.JuegoId)
															{
																añadir = false;
																break;
															}
														}
													}

													if (añadir == true)
													{
														juego2.DLCs.Add(juego.JuegoId);
													}
												}
											}
										}
									}
								}
							}

							bool hayDLCsEnTier = juegosTier.Any(j => j.DLCs != null && j.DLCs.Count > 0);

							if (hayDLCsEnTier == true)
							{
								texto += "Game | DRM | Historical Price | Reviews | DLCs" + Environment.NewLine;
								texto += "---- | ---- | ------------- | ----- | -----" + Environment.NewLine;
							}
							else
							{
								texto += "Game | DRM | Historical Price | Reviews" + Environment.NewLine;
								texto += "---- | ---- | ------------- | -----" + Environment.NewLine;
							}

							foreach (var juego in juegosTier)
							{
								bool mostrar = true;

								if (juego.Juego?.Tipo == Juegos.JuegoTipo.DLC)
								{
									if (string.IsNullOrEmpty(juego.Juego.Maestro) == false)
									{
										if (juego.Juego.Maestro != "no")
										{
											foreach (var juego2 in juegosTier)
											{
												if (juego2.JuegoId == juego.Juego.Maestro)
												{
													mostrar = false;
													break;
												}
											}
										}
									}
								}

								if (mostrar == true)
								{
									string nombre = juego.Nombre;
									string precioMinimo = "--";
									string dlcs = "--";
									string reseñas = "--";

									if (juego.Juego != null)
									{
										if (juego.DRM == Juegos.JuegoDRM.Steam && juego.Juego.IdSteam > 0)
										{
											nombre = "[" + nombre + "](https://store.steampowered.com/app/" + juego.Juego.IdSteam + ")";
										}
										else if (juego.DRM == Juegos.JuegoDRM.GOG && string.IsNullOrEmpty(juego.Juego.SlugGOG) == false)
										{
											nombre = "[url=https://www.gog.com/game/" + juego.Juego.SlugGOG + "]" + nombre + "[/url]";
										}
										else if (juego.DRM == Juegos.JuegoDRM.Epic && string.IsNullOrEmpty(juego.Juego.SlugEpic) == false)
										{
											nombre = "[url=https://www.epicgames.com/store/p/" + juego.Juego.SlugEpic + "]" + nombre + "[/url]";
										}

										if (juego.Juego.PrecioMinimosHistoricos != null)
										{
											decimal precioMinimoDecimal = 0;
											decimal precioMinimoComparar = 10000;

											foreach (var historico in juego.Juego.PrecioMinimosHistoricos)
											{
												if (historico.DRM == juego.DRM)
												{
													if (historico.PrecioCambiado > 0 && historico.PrecioCambiado < precioMinimoComparar)
													{
														precioMinimoComparar = historico.PrecioCambiado;
													}
													else if (historico.Precio > 0 && historico.Precio < precioMinimoComparar)
													{
														precioMinimoComparar = historico.Precio;
													}
												}
											}

											if (precioMinimoComparar < 10000)
											{
												precioMinimoDecimal = precioMinimoComparar;
											}

											if (juego.DLCs?.Count > 0)
											{
												foreach (var dlc in juego.DLCs)
												{
													Juegos.Juego juegoDLC = await global::BaseDatos.Juegos.Buscar.UnJuego(dlc);

													if (juegoDLC?.PrecioMinimosHistoricos != null)
													{
														decimal precioMinimoDLCComparar = 10000;

														foreach (var historico in juegoDLC.PrecioMinimosHistoricos)
														{
															if (historico.DRM == juego.DRM)
															{
																if (historico.PrecioCambiado > 0 && historico.PrecioCambiado < precioMinimoDLCComparar)
																{
																	precioMinimoDLCComparar = historico.PrecioCambiado;
																}
																else if (historico.Precio > 0 && historico.Precio < precioMinimoDLCComparar)
																{
																	precioMinimoDLCComparar = historico.Precio;
																}
															}
														}

														if (precioMinimoDLCComparar < 10000)
														{
															precioMinimoDecimal = precioMinimoDecimal + precioMinimoDLCComparar;
														}
													}
												}
											}

											if (precioMinimoDecimal > 0)
											{
												precioMinimo = Herramientas.Precios.Euro(precioMinimoDecimal);
											}

											if (juego.DLCs?.Count > 0)
											{
												if (juego.DLCs.Count == 1)
												{
													dlcs = "1 DLC";
												}
												else if (juego.DLCs.Count > 1)
												{
													dlcs = juego.DLCs?.Count.ToString() + " DLCs";
												}
											}

											if (juego.Juego.Analisis != null)
											{
												reseñas = juego.Juego.Analisis.Porcentaje + "%";
											}
										}
									}

									if (hayDLCsEnTier == true)
									{
										texto += nombre + " | "
											+ Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + " | "
											+ precioMinimo + " | "
											+ reseñas + " | "
											+ dlcs + Environment.NewLine;
									}
									else
									{
										texto += nombre + " | "
											+ Juegos.JuegoDRM2.DevolverDRM(juego.DRM) + " | "
											+ precioMinimo + " | "
											+ reseñas + Environment.NewLine;
									}
								}
							}

							texto = texto + Environment.NewLine + Environment.NewLine;
						}
					}
				}
			}

			if (string.IsNullOrEmpty(texto) == false)
			{
				texto = texto.Trim();
			}

			return texto;
		}

		public static async Task<string> Gratis(List<JuegoGratis> gratis)
        {
            string texto = null;

            if (gratis?.Count > 0)
            {
                foreach (var gratis2 in gratis)
                {
                    texto = texto + "[" + gratis2.Enlace + "](" + gratis2.Enlace + ")" + Environment.NewLine + Environment.NewLine;

                    if (gratis2.FechaTermina >= DateTime.Now)
                    {
                        texto = texto + "You can claim it for free until " + gratis2.FechaTermina.ToString("m") + ", also other data:" + Environment.NewLine + Environment.NewLine;

                        Juego juego = await global::BaseDatos.Juegos.Buscar.UnJuego(gratis2.JuegoId);

                        if (juego != null)
                        {
                            if (juego.Analisis != null)
                            {
								string reseñasTemp = juego.Analisis?.Cantidad;
								reseñasTemp = reseñasTemp.Replace(",", null);
								int reseñasTemp2 = int.Parse(reseñasTemp);

								texto = texto + ">It has an " + juego.Analisis.Porcentaje + "% rating on Steam with " + reseñasTemp2.ToString("N0").Replace(",", ".") + " reviews." + Environment.NewLine;
							}

							List<int> bundlesActivos = new List<int>();
                            List<int> bundlesViejunos = new List<int>();

                            if (juego.Bundles?.Count > 0)
                            {
                                foreach (var bundle in juego.Bundles)
                                {
                                    if (bundle.FechaEmpieza <= DateTime.Now && bundle.FechaTermina >= DateTime.Now)
                                    {
                                        bundlesActivos.Add(bundle.BundleId);
                                    }
                                    else
                                    {
                                        bundlesViejunos.Add(bundle.BundleId);
                                    }
                                }
                            }

                            if (bundlesActivos.Count > 0)
                            {
                                foreach (var bundle in bundlesActivos)
                                {
                                    Bundles2.Bundle bundle2 = await global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
                                        texto = texto + ">It's in the bundle: [" + bundle2.Nombre + " • " + bundle2.Tienda + "](" + bundle2.Enlace + ")" + Environment.NewLine + Environment.NewLine;
                                    }
                                }
                            }

							List<string> bundlesViejunosTabla = new List<string>();

							if (bundlesViejunos.Count > 0)
                            {
                                foreach (var bundle in bundlesViejunos)
                                {
                                    Bundles2.Bundle bundle2 = await global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
										bundlesViejunosTabla.Add($"| {bundle2.Nombre} | {bundle2.Tienda} | {Calculadora.DiferenciaTiempo(bundle2.FechaEmpieza, "en")} |");
									}
                                }
                            }

                            if (bundlesViejunosTabla.Count > 0)
                            {
								texto += Environment.NewLine + "**Old Bundles:**" + Environment.NewLine + Environment.NewLine;
								texto += "Name|Store|Time" + Environment.NewLine + "---|---|---" + Environment.NewLine;
								texto += string.Join(Environment.NewLine, bundlesViejunosTabla) + Environment.NewLine;
							}
                        }

						if (juego.Gratis?.Count > 0)
						{
							List<string> gratisViejunosTabla = new List<string>();

							foreach (var gratis3 in juego.Gratis)
							{
								if (gratis3.FechaEmpieza <= DateTime.Now && gratis3.FechaTermina >= DateTime.Now)
								{
								}
								else
								{
									gratisViejunosTabla.Add($"{Gratis2.GratisCargar.DevolverGratis(gratis3.Tipo).Nombre}|{Calculadora.DiferenciaTiempo(gratis3.FechaEmpieza, "en")}");
								}
							}

							if (gratisViejunosTabla.Count > 0)
							{
								texto += Environment.NewLine + "**Old Free:**" + Environment.NewLine + Environment.NewLine;
								texto += "Name|Time" + Environment.NewLine + "---|---" + Environment.NewLine;
								texto += string.Join(Environment.NewLine, gratisViejunosTabla) + Environment.NewLine;
							}
						}

						if (juego.Suscripciones?.Count > 0)
                        {
							List<string> suscripcionesViejunasTabla = new List<string>();

							foreach (var suscripcion in juego.Suscripciones)
                            {
                                if (suscripcion.FechaEmpieza <= DateTime.Now && suscripcion.FechaTermina >= DateTime.Now)
                                {
                                    texto = texto + ">It's in the subscription: [" + Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre + "](" + suscripcion.Enlace + ")" + Environment.NewLine + Environment.NewLine;
                                }
                                else
                                {
									suscripcionesViejunasTabla.Add($"{Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre}|{Calculadora.DiferenciaTiempo(suscripcion.FechaEmpieza, "en")}");
								}
                            }

							if (suscripcionesViejunasTabla.Count > 0)
							{
								texto += Environment.NewLine + "**Old Subscriptions:**" + Environment.NewLine + Environment.NewLine;
								texto += "Name|Time" + Environment.NewLine + "---|---" + Environment.NewLine;
								texto += string.Join(Environment.NewLine, suscripcionesViejunasTabla) + Environment.NewLine;
							}
						}

						texto = texto + Environment.NewLine + Environment.NewLine;
					}
                }
            }

            return texto;
        }

        public static async Task<string> Suscripciones(List<JuegoSuscripcion> suscripciones)
        {
            string texto = null;

            if (suscripciones?.Count > 0)
            {
                foreach (var suscripcion2 in suscripciones)
                {
                    texto = texto + "[" + suscripcion2.Nombre + "](" + suscripcion2.Enlace + ") - " + JuegoDRM2.DevolverDRM(suscripcion2.DRM) + Environment.NewLine + Environment.NewLine;

                    if (suscripcion2.FechaTermina >= DateTime.Now)
                    {
                        Juego juego = await global::BaseDatos.Juegos.Buscar.UnJuego(suscripcion2.JuegoId);

                        if (juego != null)
                        {
                            if (juego.Analisis?.Porcentaje.Length > 1)
                            {
                                string reseñasTemp = juego.Analisis?.Cantidad;
                                reseñasTemp = reseñasTemp.Replace(",", null);
                                int reseñasTemp2 = int.Parse(reseñasTemp);

                                texto = texto + ">It has an " + juego.Analisis.Porcentaje + "% rating on Steam with " + reseñasTemp2.ToString("N0").Replace(",", ".") + " reviews." + Environment.NewLine;
                            }

                            List<int> bundlesActivos = new List<int>();
                            List<int> bundlesViejunos = new List<int>();

                            if (juego.Bundles?.Count > 0)
                            {
                                foreach (var bundle in juego.Bundles)
                                {
                                    if (bundle.FechaEmpieza <= DateTime.Now && bundle.FechaTermina >= DateTime.Now)
                                    {
                                        bundlesActivos.Add(bundle.BundleId);
                                    }
                                    else
                                    {
                                        bundlesViejunos.Add(bundle.BundleId);
                                    }
                                }
                            }

                            if (bundlesActivos.Count > 0)
                            {
                                foreach (var bundle in bundlesActivos)
                                {
                                    Bundles2.Bundle bundle2 = await global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
                                        texto = texto + ">It's in the bundle: [" + bundle2.Nombre + " • " + bundle2.Tienda + "](" + bundle2.Enlace + ")" + Environment.NewLine + Environment.NewLine;
                                    }
                                }
                            }

                            if (bundlesViejunos.Count > 0)
                            {
								List<string> bundlesViejunosTabla = new List<string>();

								foreach (var bundle in bundlesViejunos)
                                {
                                    Bundles2.Bundle bundle2 = await global::BaseDatos.Bundles.Buscar.UnBundle(bundle);

                                    if (bundle2 != null)
                                    {
										bundlesViejunosTabla.Add($"{bundle2.Nombre}|{bundle2.Tienda}|{Calculadora.DiferenciaTiempo(bundle2.FechaEmpieza, "en")}");
									}
                                }

								if (bundlesViejunosTabla.Count > 0)
								{
									texto += Environment.NewLine + "**Old Bundles:**" + Environment.NewLine + Environment.NewLine;
									texto += "Name|Store|Time" + Environment.NewLine + "---|---|---" + Environment.NewLine;
									texto += string.Join(Environment.NewLine, bundlesViejunosTabla) + Environment.NewLine;
								}
							}
                        }

                        if (juego.Gratis?.Count > 0)
                        {
							List<string> gratisViejunosTabla = new List<string>();

							foreach (var gratis in juego.Gratis)
                            {
                                if (gratis.FechaEmpieza <= DateTime.Now && gratis.FechaTermina >= DateTime.Now)
                                {
                                    texto = texto + ">It's free in: [" + Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).Nombre + "](" + gratis.Enlace + ")" + Environment.NewLine + Environment.NewLine;
                                }
                                else
                                {
									gratisViejunosTabla.Add($"{Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).Nombre}|{Calculadora.DiferenciaTiempo(gratis.FechaEmpieza, "en")}");
								}
                            }

							if (gratisViejunosTabla.Count > 0)
							{
								texto += Environment.NewLine + "**Old Free:**" + Environment.NewLine + Environment.NewLine;
								texto += "Name|Time" + Environment.NewLine + "---|---" + Environment.NewLine;
								texto += string.Join(Environment.NewLine, gratisViejunosTabla) + Environment.NewLine;
							}
						}

						if (juego.Suscripciones?.Count > 0)
						{
							List<string> suscripcionesViejunasTabla = new List<string>();

							foreach (var suscripcion in juego.Suscripciones)
							{
								if (suscripcion.FechaEmpieza <= DateTime.Now && suscripcion.FechaTermina >= DateTime.Now)
								{
									texto = texto + ">It's in the subscription: [" + Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre + "](" + suscripcion.Enlace + ")" + Environment.NewLine + Environment.NewLine;
								}
								else if (suscripcion.FechaEmpieza != DateTime.MinValue)
								{
									suscripcionesViejunasTabla.Add($"{Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre}|{Calculadora.DiferenciaTiempo(suscripcion.FechaEmpieza, "en")}");
								}
							}

							if (suscripcionesViejunasTabla.Count > 0)
							{
								texto += Environment.NewLine + "**Old Subscriptions:**" + Environment.NewLine + Environment.NewLine;
								texto += "Name|Time" + Environment.NewLine + "---|---" + Environment.NewLine;
								texto += string.Join(Environment.NewLine, suscripcionesViejunasTabla) + Environment.NewLine;
							}
						}
					}

                    texto = texto + Environment.NewLine + Environment.NewLine;
                }
            }

            return texto;
        }

        public static async Task<bool> PostearOfertasDia(IConfiguration configuracion, TiendaRegion region, List<Juego> juegos, JuegoDRM drm)
        {
            string cuenta = configuracion.GetValue<string>("Reddit:Cuenta");
            string contraseña = configuracion.GetValue<string>("Reddit:Contraseña");
            string clientId = configuracion.GetValue<string>("Reddit:ClientID");
            string clientSecret = configuracion.GetValue<string>("Reddit:ClientSecret");

            var webAgent = new Reddit2.BotWebAgent(cuenta, contraseña, clientId, clientSecret, "https://pepeizqdeals.com/");
            var reddit = new RedditSharp.Reddit(webAgent, false);

			string sub = "gamedealsue";

			if (region == TiendaRegion.EstadosUnidos)
			{
				sub = "gamedealsUS";
			}

			RedditSharp.Things.Subreddit subreddit = await reddit.GetSubredditAsync(sub);

            string titulo = "Today's Deals for " + JuegoDRM2.DevolverDRM(drm) + " [" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString() + "]";
            string texto = null;

            if (juegos?.Count > 0)
            {
				if (juegos.Count == 1)
				{
					titulo = titulo + " - " + juegos[0].Nombre;
				}

				if (juegos.Count == 2)
				{
					titulo = titulo + " - " + juegos[0].Nombre + " and " + juegos[1].Nombre;
				}

				if (juegos.Count > 2)
				{
					titulo = titulo + " - " + juegos[0].Nombre + ", " + juegos[1].Nombre + " and more";
				}
				
				Dictionary<string, List<Juego>> juegosPorTienda = new Dictionary<string, List<Juego>>();
				List<Tienda> tiendas = TiendasCargar.GenerarListado();

                foreach (var juego in juegos)
                {
					if (region == TiendaRegion.Europa)
					{
						Tienda tiendaJuego = tiendas.FirstOrDefault(t => t.Id == juego.PrecioMinimosHistoricos[0].Tienda);
						string nombreTienda = tiendaJuego.Nombre;

						if (juegosPorTienda.ContainsKey(nombreTienda) == false)
						{
							juegosPorTienda[nombreTienda] = new List<Juego>();
						}

						juegosPorTienda[nombreTienda].Add(juego);
					}
					else if (region == TiendaRegion.EstadosUnidos)
					{
						Tienda tiendaJuego = tiendas.FirstOrDefault(t => t.Id == juego.PrecioMinimosHistoricosUS[0].Tienda);
						string nombreTienda = tiendaJuego.Nombre;

						if (juegosPorTienda.ContainsKey(nombreTienda) == false)
						{
							juegosPorTienda[nombreTienda] = new List<Juego>();
						}

						juegosPorTienda[nombreTienda].Add(juego);
					}
				}

				foreach (var juegosTienda in juegosPorTienda)
                {
					string tienda = juegosTienda.Key;
					List<Juego> listaJuegos = juegosTienda.Value;

                    if (listaJuegos?.Count > 0)
                    {
                        listaJuegos = listaJuegos.OrderBy(t => t.Nombre).ToList();

                        string codigo = string.Empty;

						if (region == TiendaRegion.Europa)
						{
							if (string.IsNullOrEmpty(listaJuegos[0]?.PrecioMinimosHistoricos[0]?.CodigoTexto) == false)
							{
								codigo = Environment.NewLine + ">Code: **" + listaJuegos[0]?.PrecioMinimosHistoricos[0]?.CodigoTexto + "**";
							}

							texto = texto + "**" + tienda + "**" + codigo + Environment.NewLine;

							foreach (var juego in listaJuegos)
							{
								string precio = Precios.Euro(juego.PrecioMinimosHistoricos[0].Precio);

								if (juego.PrecioMinimosHistoricos[0].PrecioCambiado > 0)
								{
									precio = Precios.Euro(juego.PrecioMinimosHistoricos[0].PrecioCambiado);
								}

								texto = texto + "* [" + juego.Nombre + " • " + juego.PrecioMinimosHistoricos[0].Descuento.ToString() + "% • " + precio + "](" + juego.PrecioMinimosHistoricos[0].Enlace + ")" + Environment.NewLine;
							}
						}
						else if (region == TiendaRegion.EstadosUnidos)
						{
							if (string.IsNullOrEmpty(listaJuegos[0]?.PrecioMinimosHistoricosUS[0]?.CodigoTexto) == false)
							{
								codigo = Environment.NewLine + ">Code: **" + listaJuegos[0]?.PrecioMinimosHistoricosUS[0]?.CodigoTexto + "**";
							}

							texto = texto + "**" + tienda + "**" + codigo + Environment.NewLine;

							foreach (var juego in listaJuegos)
							{
								string precio = Precios.Dolar(juego.PrecioMinimosHistoricosUS[0].Precio);

								if (juego.PrecioMinimosHistoricosUS[0].PrecioCambiado > 0)
								{
									precio = Precios.Dolar(juego.PrecioMinimosHistoricosUS[0].PrecioCambiado);
								}

								texto = texto + "* [" + juego.Nombre + " • " + juego.PrecioMinimosHistoricosUS[0].Descuento.ToString() + "% • " + precio + "](" + juego.PrecioMinimosHistoricosUS[0].Enlace + ")" + Environment.NewLine;
							}
						}

						texto = texto + Environment.NewLine + "---" + Environment.NewLine + Environment.NewLine;
					}
				}
            }

			if (string.IsNullOrEmpty(texto) == false)
			{
				try
				{
					var post = await subreddit.SubmitTextPostAsync(titulo, texto);

					if (string.IsNullOrEmpty(post?.Id) == false)
					{
						return true;
					}
				}
				catch (Exception ex)
				{
					global::BaseDatos.Errores.Insertar.Mensaje("Reddit Ofertas Dia", ex);
				}
			}

			return false;
		}
    }
}