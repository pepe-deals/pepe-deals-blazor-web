//https://service-aggregation-layer.juno.ea.com/graphql?operationName=GameOffers&variables=%7B%22locale%22%3A%22es%22%2C%22subscriptionLevel%22%3A%22NON_SUBSCRIBER%22%2C%22gameId%22%3A%22ea-sports-fc-25%22%2C%22overrideCountryCode%22%3A%22ES%22%7D&extensions=%7B%22persistedQuery%22%3A%7B%22version%22%3A1%2C%22sha256Hash%22%3A%221b08dff7328b969bfefc4ee05b3eeeb6980552ede8b857b0c46c471edd12d14b%22%7D%7D
//https://www.ea.com/_next/data/IR-HBt2bCqAmiC2sPeash/es/games/ea-sports-fc/fc-25.json
//https://www.ea.com/_next/data/Pw6aX5Zr4vZj-XwNHrm9N/es/games/f1/f1-25/buy.json
//https://drop-api.ea.com/checkout/checkout-url/Origin.OFR.50.0004360?locale=en
//https://drop-api.ea.com/game/the-sims-3?locale=en&subscription-level=NON_SUBSCRIBER
//https://drop-api.ea.com/addon/the-sims-3-university-life?locale=en&subscription-level=NON_SUBSCRIBER
//https://drop-api.ea.com/game/mass-effect-legendary-edition?locale=es&subscription-level=NON_SUBSCRIBER
//https://drop-api.ea.com/news-articles?locale=es&limit=500&typeSlug=game-updates
//https://drop-api.ea.com/campaign/list?locale=es
//https://drop-api.ea.com/usp/split-fiction
//https://service-aggregation-layer.juno.ea.com/graphql?operationName=PlanSelection&variables=%7B%22overrideCountryCode%22%3A%22ES%22%2C%22locale%22%3A%22es%22%7D&extensions=%7B%22persistedQuery%22%3A%7B%22version%22%3A1%2C%22sha256Hash%22%3A%22a60817e7ed053ce4467a20930d6a445a5e3e14533ab9316e60662db48a25f131%22%7D%7D

#nullable disable

using Dapper;
using Herramientas;
using Juegos;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.EA
{
    public class Tienda
    {
        public static Tiendas2.Tienda Generar()
        {
            Tiendas2.Tienda tienda = new Tiendas2.Tienda
            {
                Id = "ea",
                Nombre = "EA",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/ea_logo.webp",
                Imagen300x80 = "/imagenes/tiendas/ea_300x80.webp",
                ImagenIcono = "/imagenes/tiendas/ea_icono.webp",
                Color = "#ff4747",
                AdminUso = true,
				UsuarioUso = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa }
			};

            return tienda;
        }

		public static List<string> juegos = new List<string>() {
			"alice-madness-returns",
			"amazing-adventures-riddle-of-the-two-knights",
			"amazing-adventures-the-caribbean-secret",
			"amazing-adventures-the-forgotten-dynasty",
			"amazing-adventures-the-lost-tomb",
			"a-way-out",
			"battlefield-1",
			"battlefield-3",
			"battlefield-4",
			"battlefield-5",
			"battlefield-6",
			"battlefield-2042",
			"battlefield-hardline",
			"bejeweled-3",
			"bejeweled-twist",
			"burnout-paradise-remastered",
			"college-football-27",
			"command-and-conquer-4-tiberian-twilight",
			"command-and-conquer-red-alert-3",
			"command-and-conquer-remastered",
			"crusader-no-regret",
			"crusader-no-remorse",
			"crysis",
			"crysis-2",
			"crysis-warhead",
			"crysis-remastered",
			"crysis-2-remastered",
			"crysis-3-remastered",
			"dead-space",
			"dead-space-classic",
			"dead-space-2",
			"dead-space-3",
			"dragon-age-2",
			"dragon-age-inquisition",
			"dragon-age-origins",
			"dragon-age-the-veilguard",
			"dungeon-keeper",
			"dungeon-keeper-2",
			"f1-23",
			"f1-24",
			"f1-25",
			"fe",
			"fc-24",
			"fc-25",
			"fc-26",
			"fe",
			"grid-legends",
			"immortals-of-aveum",
			"insaniquarium",
			"it-takes-two",
			"jade-empire",
			"jedi-fallen-order",
			"jedi-survivor",
			"lands-of-lore-3",
			"lands-of-lore-guardians-of-destiny",
			"lands-of-lore-the-throne-of-chaos",
			"lost-in-random",
			"madden-nfl-24",
			"madden-nfl-25",
			"madden-nfl-26",
			"madden-nfl-27",
			"magic-carpet",
			"mass-effect",
			"mass-effect-2",
			"mass-effect-3",
			"mass-effect-andromeda",
			"mass-effect-legendary-edition",
			"medal-of-honor",
			"medal-of-honor-airborne",
			"medal-of-honor-pacific-assault",
			"medal-of-honor-warfighter",
			"mirrors-edge",
			"mirrors-edge-catalyst",
			"mysims-cozy-bundle",
			"mystery-p-i-lost-in-los-angeles",
			"mystery-p-i-stolen-in-san-francisco",
			"mystery-p-i-the-curious-case-of-counterfeit-cove",
			"mystery-p-i-the-lottery-ticket",
			"mystery-p-i-the-new-york-fortune",
			"mystery-p-i-the-vegas-heist",
			"need-for-speed",
			"need-for-speed-heat",
			"need-for-speed-hot-pursuit-remastered",
			"need-for-speed-most-wanted",
			"need-for-speed-payback",
			"need-for-speed-rivals",
			"need-for-speed-unbound",
			"peggle-nights",
			"pga-tour",
			"plants-vs-zombies",
			"plants-vs-zombies-garden-warfare",
			"plants-vs-zombies-garden-warfare-2",
			"plants-vs-zombies-replanted",
			"populous",
			"populous-2-trials-of-the-olympian-gods",
			"populous-the-beginning",
			"sea-of-solitude",
			"simcity",
			"simcity-2000",
			"simcity-4",
			"sid-meiers-alpha-centauri",
			"skate",
			"split-fiction",
			"spore",
			"squadrons",
			"starflight",
			"starflight-2",
			"star-wars-battlefront",
			"star-wars-battlefront-2",
			"super-mega-baseball-3",
			"super-mega-baseball-4",
			"super-mega-baseball-extra-innings",
			"syndicate-1993",
			"the-bards-tale-trilogy",
			"the-saboteur",
			"the-sims-25th-birthday-bundle",
			"the-sims-25th-anniv-edition",
			"the-sims-2-25th-anniv-edition",
			"the-sims-3",
			"the-sims-4",
            "the-sims-medieval",
            "theme-hospital",
			"titanfall-2",
			"ultima-1-the-first-age-of-darkness",
			"ultima-2-the-revenge-of-the-enchantress",
			"ultima-3-exodus",
			"ultima-4-quest-of-the-avatar",
			"ultima-5-warriors-of-destiny",
			"ultima-6-the-false-prophet",
			"ultima-7",
			"ultima-8-pagan",
			"ultima-9-ascension",
			"unravel",
			"unravel-two",
			"warp",
			"wild-hearts",
			"wing-commander",
			"wing-commander-2-vengeance-of-the-kilrathi",
			"wing-commander-3-heart-of-the-tiger",
			"wrc-24",
			"zau",
			"zero-company",
			"zumas-revenge"
		};

		public static async Task BuscarOfertas(TiendaRegion region)
        {
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			int juegos2 = 0;

			foreach (var juego in juegos)
			{
				string html = await Decompiladores.GZipFormato3("https://drop-api.ea.com/game/" + juego);

				if (string.IsNullOrEmpty(html) == false)
				{
					EAJuego juegoEA = JsonSerializer.Deserialize<EAJuego>(html);

					if (juegoEA != null)
					{
						string nombre = WebUtility.HtmlDecode(juegoEA.Nombre);
						string slug = juegoEA.Slug;

						if (juegoEA.Franquicia != null && !string.IsNullOrEmpty(juegoEA.Franquicia.Slug))
						{
							slug = juegoEA.Franquicia.Slug + "/" + slug;
						}

						string enlace = "https://www.ea.com/games/" + slug + "/buy";

						List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

						if (juegoEA.Ediciones?.Count > 0)
						{
							foreach (var edicion in juegoEA.Ediciones)
							{
								if (edicion.Precio != null && !string.IsNullOrEmpty(edicion.Precio.PrecioRebajado))
								{
									string textoPrecioRebajado = edicion.Precio.PrecioRebajado;
									textoPrecioRebajado = textoPrecioRebajado.Replace("€", null);
									textoPrecioRebajado = textoPrecioRebajado.Replace(",", ".");
									textoPrecioRebajado = textoPrecioRebajado.Trim();

									string textoPrecioBase = edicion.Precio.PrecioBase;
									textoPrecioBase = textoPrecioBase.Replace("€", null);
									textoPrecioBase = textoPrecioBase.Replace(",", ".");
									textoPrecioBase = textoPrecioBase.Trim();

									decimal precioRebajado = decimal.Parse(textoPrecioRebajado);
									decimal precioBase = decimal.Parse(textoPrecioBase);

									int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

									if (descuento > 0)
									{
										string imagen = juegoEA.Imagenes.Ar1X1;

										JuegoPrecio oferta = new JuegoPrecio
										{
											Nombre = edicion.Nombre,
											Enlace = enlace + "#" + edicion.Slug,
											Imagen = imagen,
											Moneda = JuegoMoneda.Euro,
											Precio = precioRebajado,
											Descuento = descuento,
											Tienda = Generar().Id,
											DRM = JuegoDRM.EA,
											FechaDetectado = DateTime.Now,
											FechaActualizacion = DateTime.Now
										};

										ofertas.Add(oferta);
									}
								}
							}
						}

						if (juegoEA?.DLCs?.DLCs?.Count > 0)
						{
							foreach (var dlc in juegoEA.DLCs.DLCs)
							{
								if (dlc.Precio != null && !string.IsNullOrEmpty(dlc.Precio.PrecioRebajado))
								{
									string textoPrecioRebajado = dlc.Precio.PrecioRebajado;
									textoPrecioRebajado = textoPrecioRebajado.Replace("€", null);
									textoPrecioRebajado = textoPrecioRebajado.Replace(",", ".");
									textoPrecioRebajado = textoPrecioRebajado.Trim();

									string textoPrecioBase = dlc.Precio.PrecioBase;
									textoPrecioBase = textoPrecioBase.Replace("€", null);
									textoPrecioBase = textoPrecioBase.Replace(",", ".");
									textoPrecioBase = textoPrecioBase.Trim();

									decimal precioRebajado = decimal.Parse(textoPrecioRebajado);
									decimal precioBase = decimal.Parse(textoPrecioBase);

									int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

									if (descuento > 0)
									{
										string nombreDLC = WebUtility.HtmlDecode(dlc.Nombre);
										string slugDLC = dlc.Slug;
										string imagenDLC = dlc.Imagenes.Ar1X1;
										string enlaceDLC = enlace + "/addon/" + slugDLC;

										JuegoPrecio ofertaDLC = new JuegoPrecio
										{
											Nombre = nombreDLC,
											Enlace = enlaceDLC,
											Imagen = imagenDLC,
											Moneda = JuegoMoneda.Euro,
											Precio = precioRebajado,
											Descuento = descuento,
											Tienda = Generar().Id,
											DRM = JuegoDRM.EA,
											FechaDetectado = DateTime.Now,
											FechaActualizacion = DateTime.Now
										};

										ofertas.Add(ofertaDLC);
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

						if (juegoEA.Suscripciones != null)
						{
							string enlaceSuscripcion = enlace.Replace("/buy", null);

							var tipos = new[] { Suscripciones2.SuscripcionTipo.EAPlay, Suscripciones2.SuscripcionTipo.EAPlayPro };

							foreach (var tipo in tipos)
							{
								EASuscripcionFila fila = await Herramientas.BaseDatos.Select(async conexion =>
								{
									return await conexion.QueryFirstOrDefaultAsync<EASuscripcionFila>("SELECT idJuegos, descartado FROM tiendaea WHERE enlace=@enlace", new { enlace = enlaceSuscripcion });
								});

								if (fila != null)
								{
									string idJuegosTexto = fila.IdJuegos;
									string descartado = fila.Descartado;
									bool encontrado = false;

									if (string.IsNullOrEmpty(idJuegosTexto) == false && idJuegosTexto != "0" && descartado == "no")
									{
										encontrado = true;

										List<string> idJuegos = Herramientas.Listados.Generar(idJuegosTexto);

										foreach (var id in idJuegos)
										{
											bool insertar = true;
											var suscripciones = await BaseDatos.Suscripciones.Buscar.JuegoId(int.Parse(id));

											if (suscripciones?.Count > 0)
											{
												foreach (var suscripcion in suscripciones)
												{
													if (suscripcion.Tipo == tipo)
													{
														insertar = false;

														suscripcion.FechaTermina = DateTime.Now + TimeSpan.FromDays(1);

														await BaseDatos.Suscripciones.Actualizar.FechaTermina(suscripcion);
													}
												}
											}

											if (insertar == true)
											{
												DateTime nuevaFecha = DateTime.Now;
												nuevaFecha = nuevaFecha + TimeSpan.FromDays(1);

												JuegoSuscripcion nuevaSuscripcion = new JuegoSuscripcion
												{
													DRM = JuegoDRM.EA,
													Nombre = juegoEA.Nombre,
													FechaEmpieza = DateTime.Now,
													FechaTermina = nuevaFecha,
													JuegoId = int.Parse(id),
													Imagen = juegoEA.Imagenes.Ar1X1,
													ImagenNoticia = juegoEA.Imagenes.Ar1X1,
													Enlace = enlaceSuscripcion,
													Tipo = tipo
												};

												await BaseDatos.Suscripciones.Insertar.Ejecutar(int.Parse(id), nuevaSuscripcion);
											}
										}
									}

									if (descartado == "si")
									{
										encontrado = true;
									}

									if (encontrado == false)
									{
										var generar = tipo == Suscripciones2.SuscripcionTipo.EAPlay ? Suscripcion.Generar() : Suscripcion.GenerarPro();

										await BaseDatos.Suscripciones.Insertar.Temporal(generar.Id.ToString().ToLower(), enlaceSuscripcion, nombre);
									}
								}
							}
                        }
					}
				}
			}
		}
    }

	public class EASuscripcionFila
	{
		public string IdJuegos { get; set; }
		public string Descartado { get; set; }
	}

    #region Clases

    public class EAJuego
    {
		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("slug")]
        public string Slug { get; set; }

		[JsonPropertyName("packArt")]
		public EAJuegoImagenes Imagenes { get; set; }

		[JsonPropertyName("franchise")]
		public EAJuegoFranquicia Franquicia { get; set; }

		[JsonPropertyName("editions")]
		public List<EAJuegoEdicion> Ediciones { get; set; }

		[JsonPropertyName("subscriptionInfo")]
		public EAJuegoSuscripciones Suscripciones { get; set; }

		[JsonPropertyName("addonsInfo")]
		public EAJuegoDLCs DLCs { get; set; }
	}

	public class EAJuegoImagenes
	{
		[JsonPropertyName("ar1X1")]
		public string Ar1X1 { get; set; }
	}

	public class EAJuegoFranquicia
	{
		[JsonPropertyName("slug")]
		public string Slug { get; set; }
	}

	public class EAJuegoEdicion
	{
		[JsonPropertyName("slug")]
		public string Slug { get; set; }

		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("packArt")]
		public EAJuegoImagenes Imagenes { get; set; }

		[JsonPropertyName("price")]
		public EAEdicionPrecio Precio { get; set; }
	}

	public class EAEdicionPrecio
	{
		[JsonPropertyName("displayTotal")]
		public string PrecioBase { get; set; }

		[JsonPropertyName("displayTotalWithDiscount")]
		public string PrecioRebajado { get; set; }
	}

	public class EAJuegoSuscripciones
	{
		[JsonPropertyName("play_pro")]
		public EAJuegoSuscripcion EaPlayPro { get; set; }

		[JsonPropertyName("play")]
		public EAJuegoSuscripcion EaPlay { get; set; }
	}

	public class EAJuegoSuscripcion
	{
		[JsonPropertyName("acquisitionStartDate")]
		public string FechaEmpieza { get; set; }

		[JsonPropertyName("name")]
		public string Nombre { get; set; }
	}

	public class EAJuegoDLCs
	{
		[JsonPropertyName("items")]
		public List<EAJuegoDLC> DLCs { get; set; }
	}

	public class EAJuegoDLC
	{
		[JsonPropertyName("slug")]
		public string Slug { get; set; }

		[JsonPropertyName("title")]
		public string Nombre { get; set; }

		[JsonPropertyName("packArt")]
		public EAJuegoImagenes Imagenes { get; set; }

		[JsonPropertyName("price")]
		public EAEdicionPrecio Precio { get; set; }
	}

	#endregion
}
