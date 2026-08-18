#nullable disable

using Herramientas;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.Fanatical
{
	public static class Bundle
	{
		public static Bundles2.Bundle Generar()
		{
			Bundles2.Bundle bundle = new Bundles2.Bundle()
			{
				BundleTipo = Bundles2.BundleTipo.Fanatical,
				Tienda = "Fanatical",
				ImagenTienda = "/imagenes/bundles/fanatical_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/fanatical_icono.webp",
				EnlaceBase = "fanatical.com",
				Pick = false,
				Twitter = "fanatical"
            };

            DateTime fechaEmpieza = DateTime.Now;
            fechaEmpieza = new DateTime(fechaEmpieza.Year, fechaEmpieza.Month, fechaEmpieza.Day, fechaEmpieza.Hour, 0, 0);

            bundle.FechaEmpieza = fechaEmpieza;

            DateTime fechaTermina = DateTime.Now;
			fechaTermina = fechaTermina.AddDays(35);
			fechaTermina = new DateTime(fechaTermina.Year, fechaTermina.Month, fechaTermina.Day, fechaTermina.Hour, 0, 0);

			bundle.FechaTermina = fechaTermina;

			return bundle;
		}

		public static string Referido(string enlace)
		{
			string enlaceEncoded = Uri.EscapeDataString(enlace);
			return "https://www.awin1.com/cread.php?awinmid=118821&awinaffid=2693824&ued=" + enlaceEncoded;
		}

		public static async Task<Bundles2.Bundle> ExtraerDatos(Bundles2.Bundle bundle)
		{
			string html = await Decompiladores.Estandar("https://www.fanatical.com/feeds/minimal-feed.jsonl?apikey=" + Tienda.ApiKey);

			if (string.IsNullOrEmpty(html) == false)
			{
				string[] lineas = html.Split('\n', StringSplitOptions.RemoveEmptyEntries);
				JsonSerializerOptions opciones = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
				};

				string enlaceBuscar = bundle.Enlace;

				if (enlaceBuscar.Contains("https://www.fanatical.com/en/") == false)
				{
					enlaceBuscar = enlaceBuscar.Replace("https://www.fanatical.com/", "https://www.fanatical.com/en/");
				}

				for (int i = 0; i < lineas.Length; i += 50)
				{
					var tanda = lineas.Skip(i).Take(50);
					string tandaJson = "[" + string.Join(",", tanda.Where(l => !string.IsNullOrWhiteSpace(l))) + "]";

					List<FanaticalJuego> juegos = null;

					try
					{
						juegos = JsonSerializer.Deserialize<List<FanaticalJuego>>(tandaJson, opciones);
					}
					catch (JsonException ex)
					{
						BaseDatos.Errores.Insertar.Mensaje(Tienda.Generar().Id, ex, false);
					}

					if (juegos?.Count > 0)
					{
						foreach (var juego in juegos)
						{
							if (juego.Enlace.Contains(enlaceBuscar) == true)
							{
								bundle.Nombre = WebUtility.HtmlDecode(juego.Nombre);

								string imagen = juego.Imagen;
								bundle.Imagen = imagen;
								bundle.ImagenNoticia = imagen;

								DateTime fechaTermina = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
								fechaTermina = fechaTermina.AddSeconds(Convert.ToDouble(juego.FechaTermina));
								fechaTermina = fechaTermina.ToLocalTime();
								bundle.FechaTermina = Convert.ToDateTime(fechaTermina);

								if (bundle.Enlace.Contains("/pick-and-mix/") == true)
								{
									bundle.Pick = true;
								}

								int j = 0;
								foreach (var tier in juego.BundleTiers)
								{
									j += 1;

									if (bundle.Tiers == null)
									{
										bundle.Tiers = new List<Bundles2.BundleTier>();
									}

									Bundles2.BundleTier tierNuevo = new Bundles2.BundleTier
									{
										Posicion = j,
										Precio = tier.Precio?.GetValueOrDefault("EUR").ToString() ?? "0"
									};

									if (tierNuevo.Precio.Contains(".") == false)
									{
										tierNuevo.Precio = tierNuevo.Precio + ".00";
									}

									if (bundle.Pick == true)
									{
										tierNuevo.CantidadJuegos = tier.ProductosPorTier;
									}

									bundle.Tiers.Add(tierNuevo);

									var juegosBundle = tier.Productos.Where(p => p.SteamId != null && p.SteamId > 0).ToList();
									List<int> juegosBundleSteam = juegosBundle.Select(j => j.SteamId ?? 0).ToList();
									List<Juegos.Juego> juegosBundleWeb = await BaseDatos.Juegos.Buscar.MultiplesJuegosSteam2(false, TiendaRegion.Europa, juegosBundleSteam);

									if (juegosBundleWeb?.Count > 0)
									{
										foreach (var juegob in juegosBundleWeb)
										{
											if (bundle.Juegos == null)
											{
												bundle.Juegos = new List<Bundles2.BundleJuego>();
											}

											Bundles2.BundleJuego juegod = new Bundles2.BundleJuego
											{
												JuegoId = juegob.Id.ToString(),
												Nombre = juegob.Nombre,
												Imagen = juegob.Imagenes.Capsule_231x87,
												DRM = Juegos.JuegoDRM.Steam,
												Tier = tierNuevo
											};

											bool añadir = true;

											if (bundle.Juegos.Count > 0)
											{
												foreach (var juegoe in bundle.Juegos)
												{
													if (juegoe.JuegoId == juegod.JuegoId)
													{
														añadir = false;
														break;
													}
												}
											}

											if (añadir == true)
											{
												bundle.Juegos.Add(juegod);
											}
										}
									}
								}

								return bundle;
							}
						}
					}

					juegos.Clear();
					juegos = null;
				}
			}

			if (bundle.Enlace.Contains("/pick-and-mix/") == true)
			{
				bundle.Pick = true;

				string html2 = await Decompiladores.Estandar(bundle.Enlace);

				if (string.IsNullOrEmpty(html2) == false)
				{
					if (html2.Contains("<title>") == true)
					{
						int int1 = html2.IndexOf("<title>");
						string temp1 = html2.Remove(0, int1 + 7);

						int int2 = temp1.IndexOf("</title>");
						string temp2 = temp1.Remove(int2, temp1.Length - int2);

						if (temp2.Contains("|") == true)
						{
							int int3 = temp2.IndexOf("|");
							temp2 = temp2.Remove(int3, temp2.Length - int3);
						}

						bundle.Nombre = temp2.Trim();
					}

					if (html2.Contains("<img srcset=") == true)
					{
						int int1 = html2.LastIndexOf("<img srcset=");
						string temp1 = html.Remove(0, int1 + 7);

						int int2 = temp1.IndexOf("https://fanatical");
						string temp2 = temp1.Remove(0, int2);

						int int3 = temp2.IndexOf("?");
						string temp3 = temp2.Remove(int3, temp2.Length - int3);

						bundle.Imagen = temp3.Trim();
						bundle.ImagenNoticia = temp3.Trim();
					}
				}
			}

			return bundle;
		}
	}
}
