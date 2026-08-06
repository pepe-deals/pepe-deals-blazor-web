//https://api.steampowered.com/IStoreBrowseService/GetItems/v1?input_json={"ids":[{"appid":1091500}],"context":{"language":"english","country_code":"ES","steam_realm":1},"data_request":{"include_reviews":true,"include_basic_info":true, "include_assets": true, "include_links": true, "include_tag_count": 20, "include_release": true, "include_platforms": true, "include_screenshots": true, "include_trailers": true, "include_supported_languages": true}}
//https://store.steampowered.com/api/appdetails/?appids=1091500&l=english

#nullable disable

using Herramientas;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace APIs.Steam
{
	public static class Juego
	{
		public static string dominioImagenes = "https://cdn.cloudflare.steamstatic.com";
		public static string dominioImagenes2 = "https://shared.cloudflare.steamstatic.com";
		public static string dominioImagenes3 = "https://shared.fastly.steamstatic.com";
		public static string dominioVideos = "https://video.cloudflare.steamstatic.com/store_trailers/";
		public static string dominioVideos2 = "https://video.akamai.steamstatic.com/store_trailers/";

		public static async Task<Juegos.Juego> CargarDatosJuego(string enlace)
		{
			JsonSerializerOptions opciones = new JsonSerializerOptions
			{
				UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
			};

			string id = LimpiarID(enlace);

			if (string.IsNullOrEmpty(id) == true)
			{
				return null;
			}

			string html2 = await Decompiladores.Estandar(@"https://api.steampowered.com/IStoreBrowseService/GetItems/v1?input_json={""ids"":[{""appid"":" + id + @"}],""context"":{""language"":""english"",""country_code"":""ES"",""steam_realm"":1},""data_request"":{""include_reviews"":true,""include_basic_info"":true, ""include_assets"": true, ""include_links"": true, ""include_tag_count"": 20, ""include_release"": true, ""include_platforms"": true, ""include_screenshots"": true, ""include_trailers"": true, ""include_supported_languages"": true, ""include_all_purchase_options"": true, ""include_extra_details"": true}}");

			if (string.IsNullOrEmpty(html2) == false)
			{
				SteamJuegoAPI2 datos2 = null;

				try
				{
					datos2 = JsonSerializer.Deserialize<SteamJuegoAPI2>(html2, opciones);
				}
				catch { }

				if (datos2?.Respuesta.Juegos.Count == 1)
				{
					if (datos2.Respuesta.Juegos[0] != null)
					{
						var juegoApi = datos2.Respuesta.Juegos[0];

						if (juegoApi.Visible == true)
						{
							string nombre = string.Empty;
							Juegos.JuegoTipo tipo = Juegos.JuegoTipo.Game;
							Juegos.JuegoCaracteristicas caracteristicas = new Juegos.JuegoCaracteristicas();
							Juegos.JuegoImagenes imagenes = new Juegos.JuegoImagenes();
							Juegos.JuegoMedia media = new Juegos.JuegoMedia();
							Juegos.JuegoAnalisis reseñas = new Juegos.JuegoAnalisis();
							List<string> categorias = new List<string>();
							List<string> etiquetas = new List<string>();
							Juegos.JuegoDeck deck = Juegos.JuegoDeck.Desconocido;
							Juegos.JuegoSteamOS steamOS = Juegos.JuegoSteamOS.Desconocido;
							Juegos.JuegoSteamFrame steamFrame = Juegos.JuegoSteamFrame.Desconocido;
							Juegos.JuegoSteamMachine steamMachine = Juegos.JuegoSteamMachine.Desconocido;
							bool freeToPlay = false;
							List<Juegos.JuegoIdioma> idiomas = new List<Juegos.JuegoIdioma>();
							Juegos.JuegoPrecio precio = null;
							int idMaestro = 0;
							bool delistado = false;

							#region Nombre

							if (string.IsNullOrEmpty(juegoApi.Nombre) == false)
							{
								Encoding Utf8 = Encoding.UTF8;
								byte[] utf8Bytes = Utf8.GetBytes(juegoApi.Nombre);
								nombre = Utf8.GetString(utf8Bytes);
							}

							#endregion

							#region Tipo

							if (juegoApi.Tipo == 0)
							{
								tipo = Juegos.JuegoTipo.Game;
							}
							else if (juegoApi.Tipo == 4)
							{
								tipo = Juegos.JuegoTipo.DLC;
							}
							else if (juegoApi.Tipo == 11)
							{
								tipo = Juegos.JuegoTipo.Music;
							}
							else if (juegoApi.Tipo == 6)
							{
								tipo = Juegos.JuegoTipo.Software;
							}

							#endregion

							#region Caracteristicas

							if (juegoApi?.Info?.Desarrolladores?.Count > 0)
							{
								List<Juegos.JuegoCaracteristicasCurator> desarrolladores = new List<Juegos.JuegoCaracteristicasCurator>();

								foreach (var desarrollador in juegoApi.Info.Desarrolladores)
								{
									Juegos.JuegoCaracteristicasCurator desarrollador2 = new Juegos.JuegoCaracteristicasCurator
									{
										Id = desarrollador.Id,
										Nombre = desarrollador.Nombre
									};

									desarrolladores.Add(desarrollador2);
								}

								caracteristicas.Desarrolladores2 = desarrolladores;
							}

							if (juegoApi?.Info?.Editores?.Count > 0)
							{
								List<Juegos.JuegoCaracteristicasCurator> editores = new List<Juegos.JuegoCaracteristicasCurator>();

								foreach (var editor in juegoApi.Info.Editores)
								{
									Juegos.JuegoCaracteristicasCurator editor2 = new Juegos.JuegoCaracteristicasCurator
									{
										Id = editor.Id,
										Nombre = editor.Nombre
									};

									editores.Add(editor2);
								}

								caracteristicas.Editores2 = editores;
							}

							if (juegoApi?.Info?.Franquicias?.Count > 0)
							{
								List<Juegos.JuegoCaracteristicasCurator> franquicias = new List<Juegos.JuegoCaracteristicasCurator>();

								foreach (var franquicia in juegoApi.Info.Franquicias)
								{
									Juegos.JuegoCaracteristicasCurator franquicia2 = new Juegos.JuegoCaracteristicasCurator
									{
										Id = franquicia.Id,
										Nombre = franquicia.Nombre
									};

									franquicias.Add(franquicia2);
								}

								caracteristicas.Franquicias = franquicias;
							}

							if (string.IsNullOrEmpty(juegoApi?.Info?.DescripcionCorta) == false)
							{
								caracteristicas.Descripcion = juegoApi.Info.DescripcionCorta;
							}

							if (juegoApi?.Lanzamiento?.Steam > 0)
							{
								caracteristicas.FechaLanzamientoSteam = DateTime.UnixEpoch.AddSeconds(juegoApi.Lanzamiento.Steam);
							}

							if (juegoApi?.Lanzamiento?.Original > 0)
							{
								caracteristicas.FechaLanzamientoOriginal = DateTime.UnixEpoch.AddSeconds(juegoApi.Lanzamiento.Original);
							}

							if (juegoApi?.AccesoAnticipado == true)
							{
								caracteristicas.AccesoAnticipado = true;
							}
							else
							{
								caracteristicas.AccesoAnticipado = false;
							}

							if (juegoApi?.Cosas?.DemoId?.Count > 0)
							{
								caracteristicas.DemoIdSteam = juegoApi.Cosas.DemoId[0];
							}

							if (juegoApi?.Cosas?.DemoId2?.Count > 0)
							{
								caracteristicas.DemoIdSteam = juegoApi.Cosas.DemoId2[0];
							}

							if (string.IsNullOrEmpty(juegoApi?.Extra?.PideCuenta) == false)
							{
								caracteristicas.CuentaNecesaria = juegoApi.Extra.PideCuenta;
							}

							if (string.IsNullOrEmpty(juegoApi?.Extra?.DRMExterno) == false)
							{
								caracteristicas.DRMExterno = juegoApi.Extra.DRMExterno;
							}

							#endregion

							#region Imagenes

							if (juegoApi.Imagenes != null)
							{
								if (string.IsNullOrEmpty(juegoApi.Imagenes?.Header_460x215) == false)
								{
									imagenes.Header_460x215 = dominioImagenes3 + "/store_item_assets/steam/apps/" + id + "/" + juegoApi.Imagenes.Header_460x215;
								}
								else
								{
									if (string.IsNullOrEmpty(juegoApi.Imagenes?.Capsule_231x87) == false)
									{
										imagenes.Header_460x215 = dominioImagenes3 + "/store_item_assets/steam/apps/" + id + "/" + juegoApi.Imagenes.Capsule_231x87;
									}
								}

								if (string.IsNullOrEmpty(juegoApi.Imagenes?.Capsule_231x87) == false)
								{
									imagenes.Capsule_231x87 = dominioImagenes3 + "/store_item_assets/steam/apps/" + id + "/" + juegoApi.Imagenes.Capsule_231x87;
								}

								if (string.IsNullOrEmpty(juegoApi.Imagenes?.Library_600x900) == false)
								{
									imagenes.Library_600x900 = dominioImagenes3 + "/store_item_assets/steam/apps/" + id + "/" + juegoApi.Imagenes.Library_600x900;
								}

								if (string.IsNullOrEmpty(juegoApi.Imagenes?.Library_600x900) == false)
								{
									imagenes.Library_1920x620 = dominioImagenes3 + "/store_item_assets/steam/apps/" + id + "/" + juegoApi.Imagenes.Library_1920x620;
								}

								imagenes.Logo = dominioImagenes3 + "/store_item_assets/steam/apps/" + id + "/logo.png";
							}

							#endregion

							#region Reseñas

							if (juegoApi?.Reseñas?.Filtrado != null)
							{
								reseñas.Porcentaje = juegoApi.Reseñas.Filtrado.Porcentaje.ToString();
								reseñas.Cantidad = juegoApi.Reseñas.Filtrado.Cantidad.ToString();
							}

							#endregion

							#region Categorias

							if (juegoApi?.Categorias != null)
							{
								if (juegoApi?.Categorias?.Tipo1?.Count > 0)
								{
									foreach (var nuevaCategoria in juegoApi.Categorias.Tipo1)
									{
										if (nuevaCategoria > 0)
										{
											categorias.Add(nuevaCategoria.ToString());
										}
									}
								}

								if (juegoApi?.Categorias?.Tipo2?.Count > 0)
								{
									foreach (var nuevaCategoria in juegoApi.Categorias.Tipo2)
									{
										if (nuevaCategoria > 0)
										{
											categorias.Add(nuevaCategoria.ToString());
										}
									}
								}

								if (juegoApi?.Categorias?.Tipo3?.Count > 0)
								{
									foreach (var nuevaCategoria in juegoApi.Categorias.Tipo3)
									{
										if (nuevaCategoria > 0)
										{
											categorias.Add(nuevaCategoria.ToString());
										}
									}
								}
							}

							#endregion

							#region Etiquetas

							if (juegoApi?.Etiquetas?.Count > 0)
							{
								foreach (int etiqueta in juegoApi.Etiquetas)
								{
									if (etiqueta > 0)
									{
										etiquetas.Add(etiqueta.ToString());
									}
								}
							}

							#endregion

							#region Plataformas

							if (juegoApi.Plataformas != null)
							{
								caracteristicas.Windows = false;
								caracteristicas.Mac = false;
								caracteristicas.Linux = false;

								if (juegoApi.Plataformas.Windows == true)
								{
									caracteristicas.Windows = true;
								}

								if (juegoApi.Plataformas.Mac == true)
								{
									caracteristicas.Mac = true;
								}

								if (juegoApi.Plataformas.Linux == true)
								{
									caracteristicas.Linux = true;
								}

								if (juegoApi.Plataformas.Deck > 0)
								{
									deck = (Juegos.JuegoDeck)juegoApi.Plataformas.Deck;
								}

								if (juegoApi.Plataformas.SteamOS > 0)
								{
									steamOS = (Juegos.JuegoSteamOS)juegoApi.Plataformas.SteamOS;
								}

								if (juegoApi.Plataformas.SteamFrame > 0)
								{
									steamFrame = (Juegos.JuegoSteamFrame)juegoApi.Plataformas.SteamFrame;
								}

								if (juegoApi.Plataformas.SteamMachine > 0)
								{
									steamMachine = (Juegos.JuegoSteamMachine)juegoApi.Plataformas.SteamMachine;
								}

								if (juegoApi.Plataformas.RV != null)
								{
									if (juegoApi.Plataformas.RV.Vrhmd == true)
									{
										if (caracteristicas.RealidadVirtual == null)
										{
											caracteristicas.RealidadVirtual = new Juegos.JuegoCaracteristicasRealidadVirtual();
										}

										caracteristicas.RealidadVirtual.Vrhmd = true;
									}

									if (juegoApi.Plataformas.RV.VrhmdOnly == true)
									{
										if (caracteristicas.RealidadVirtual == null)
										{
											caracteristicas.RealidadVirtual = new Juegos.JuegoCaracteristicasRealidadVirtual();
										}

										caracteristicas.RealidadVirtual.VrhmdOnly = true;
									}

									if (juegoApi.Plataformas.RV.HtcVive == true)
									{
										if (caracteristicas.RealidadVirtual == null)
										{
											caracteristicas.RealidadVirtual = new Juegos.JuegoCaracteristicasRealidadVirtual();
										}

										caracteristicas.RealidadVirtual.HtcVive = true;
									}

									if (juegoApi.Plataformas.RV.OculusRift == true)
									{
										if (caracteristicas.RealidadVirtual == null)
										{
											caracteristicas.RealidadVirtual = new Juegos.JuegoCaracteristicasRealidadVirtual();
										}

										caracteristicas.RealidadVirtual.OculusRift = true;
									}

									if (juegoApi.Plataformas.RV.WindowsMr == true)
									{
										if (caracteristicas.RealidadVirtual == null)
										{
											caracteristicas.RealidadVirtual = new Juegos.JuegoCaracteristicasRealidadVirtual();
										}

										caracteristicas.RealidadVirtual.WindowsMr = true;
									}

									if (juegoApi.Plataformas.RV.ValveIndex == true)
									{
										if (caracteristicas.RealidadVirtual == null)
										{
											caracteristicas.RealidadVirtual = new Juegos.JuegoCaracteristicasRealidadVirtual();
										}

										caracteristicas.RealidadVirtual.ValveIndex = true;
									}
								}
							}

							#endregion

							#region Capturas

							if (juegoApi.Capturas != null)
							{
								media.Capturas2 = new List<Juegos.JuegoMediaCaptura>();

								if (juegoApi?.Capturas?.TodasEdades?.Count > 0)
								{
									foreach (var captura in juegoApi.Capturas.TodasEdades)
									{
										Juegos.JuegoMediaCaptura captura2 = new Juegos.JuegoMediaCaptura
										{
											Posicion = captura.Posicion,
											Imagen = dominioImagenes2 + "/store_item_assets/" + captura.Nombre,
											MayorEdad = false
										};

										media.Capturas2.Add(captura2);
									}
								}

								if (juegoApi?.Capturas?.MayorEdad?.Count > 0)
								{
									foreach (var captura in juegoApi.Capturas.MayorEdad)
									{
										Juegos.JuegoMediaCaptura captura2 = new Juegos.JuegoMediaCaptura
										{
											Posicion = captura.Posicion,
											Imagen = dominioImagenes2 + "/store_item_assets/" + captura.Nombre,
											MayorEdad = true
										};

										media.Capturas2.Add(captura2);
									}
								}

								if (media.Capturas2.Count > 0)
								{
									media.Capturas2 = media.Capturas2.OrderBy(x => x.Posicion).ToList();
								}
							}

							#endregion

							#region Videos

							if (juegoApi.Trailers?.Videos?.Count > 0)
							{
								List<Juegos.JuegoMediaVideo> videos = new List<Juegos.JuegoMediaVideo>();

								foreach (var video in juegoApi.Trailers.Videos)
								{
									if (videos.Count == 4)
									{
										break;
									}

									string videoEnlace = string.Empty;

									if (video.DatosAdaptivo != null)
									{
										foreach (var videoDatos in video.DatosAdaptivo)
										{
											if (videoDatos.Encodeo.Contains("dash") == true || videoDatos.Encodeo.Contains("h264") == true)
											{
												videoEnlace = videoDatos.Enlace;
												videoEnlace = dominioVideos2 + videoEnlace;
											}
										}
									}
									else
									{
										if (video.DatosMax != null)
										{
											foreach (var videoDatos in video.DatosMax)
											{
												if (videoDatos.Tipo.Contains("mp4") == true)
												{
													videoEnlace = videoDatos.Fichero;
													videoEnlace = dominioVideos + video.EnlaceFormato.Replace("steam/apps/", null).Replace("${FILENAME}", videoEnlace);
												}
											}
										}
									}

									if (string.IsNullOrEmpty(videoEnlace) == false)
									{
										string microEnlace = string.Empty;

										foreach (var microDatos in video.DatosMicro)
										{
											if (microDatos.Tipo.Contains("mp4") == true)
											{
												microEnlace = microDatos.Fichero;
											}
										}

										Juegos.JuegoMediaVideo nuevoVideo = new Juegos.JuegoMediaVideo
										{
											Nombre = video.Nombre,
											Enlace = videoEnlace,
											MayorEdad = !video.MayorEdad,
											Captura = dominioImagenes2 + "/store_item_assets/" + video.EnlaceFormato.Replace("${FILENAME}", video.Captura),
											CapturaPequeña = dominioImagenes2 + "/store_item_assets/" + video.EnlaceFormato.Replace("${FILENAME}", video.CapturaPequeña),
											Micro = dominioVideos + video.EnlaceFormato.Replace("steam/apps/", null).Replace("${FILENAME}", microEnlace)
										};

										videos.Add(nuevoVideo);
									}
								}

								if (videos.Count > 0)
								{
									media.Videos = videos;
								}
							}

							#endregion

							#region FreeToPlay

							if (juegoApi.FreeToPlay == true)
							{
								freeToPlay = true;
							}

							#endregion

							#region Idiomas

							if (juegoApi.Idiomas?.Count > 0)
							{
								var idiomasSteam = Idiomas.ListadoIdiomasGenerar().ToDictionary(i => i.SteamID);

								foreach (var idioma in juegoApi.Idiomas)
								{
									if (idiomasSteam.TryGetValue(idioma.Id, out var idiomaBase))
									{
										idiomas.Add(new Juegos.JuegoIdioma
										{
											Idioma = idiomaBase.Id,
											Audio = idioma.Audio,
											Texto = idioma.Soportado || idioma.Texto,
											DRM = Juegos.JuegoDRM.Steam
										});
									}
								}
							}

							#endregion

							#region Enlaces

							if (juegoApi?.Enlaces?.Count > 0)
							{
								List<string> nuevosEnlaces = new List<string>();

								foreach (var enlace2 in juegoApi.Enlaces)
								{
									if (string.IsNullOrEmpty(enlace2.Enlace) == false)
									{
										nuevosEnlaces.Add(enlace2.Enlace);
									}
								}

								if (nuevosEnlaces.Count > 0)
								{
									caracteristicas.Enlaces = nuevosEnlaces;
								}
							}

							#endregion

							#region Precio 

							if (juegoApi.Precio != null)
							{
								string enlacePrecio = "https://store.steampowered.com/app/" + id;

								int descuento = 0;

								if (juegoApi.Precio.Descuento > 0)
								{
									descuento = juegoApi.Precio.Descuento;
								}

								string precioFormateado = juegoApi.Precio.PrecioRebajado;
								precioFormateado = precioFormateado.Replace("€", null);
								precioFormateado = precioFormateado.Replace(",", ".");
								precioFormateado = precioFormateado.Replace(".--", ".00");
								precioFormateado = precioFormateado.Trim();

								precio = new Juegos.JuegoPrecio
								{
									Descuento = descuento,
									DRM = Juegos.JuegoDRM.Steam,
									Precio = decimal.Parse(precioFormateado),
									Moneda = JuegoMoneda.Euro,
									FechaDetectado = DateTime.Now,
									FechaActualizacion = DateTime.Now,
									Enlace = enlacePrecio,
									Tienda = "steam"
								};
							}

							#endregion

							#region DLC

							if (juegoApi.Cosas != null)
							{
								idMaestro = juegoApi.Cosas.MaestroId;
							}

							#endregion

							#region Suscripciones

							if (juegoApi.Suscripciones?.Count > 0)
							{
								foreach (var suscripcion in juegoApi.Suscripciones)
								{
									if (suscripcion.PaqueteId > 0)
									{
										await BaseDatos.Suscripciones.Insertar.Steam(suscripcion.PaqueteId, int.Parse(id));
									}
								}
							}

							#endregion

							#region Delistado

							if (juegoApi.Delistado == true)
							{
								delistado = true;
							}

							#endregion

							Juegos.Juego juego = new Juegos.Juego
							{
								IdSteam = int.Parse(id),
								Nombre = nombre,
								Imagenes = imagenes,
								Caracteristicas = caracteristicas,
								Media = media,
								FechaSteamAPIComprobacion = DateTime.Now,
								FreeToPlay = freeToPlay.ToString(),
								Tipo = tipo,
								MayorEdad = "false",
								Analisis = reseñas,
								Delistado = delistado
							};

							if (tipo == Juegos.JuegoTipo.DLC || tipo == Juegos.JuegoTipo.Music)
							{
								juego.Imagenes.Logo = null;
								juego.Imagenes.Library_600x900 = null;
								juego.Imagenes.Library_1920x620 = null;

								if (idMaestro > 0)
								{
									Juegos.Juego maestro = await BaseDatos.Juegos.Buscar.UnJuego(null, idMaestro.ToString());

									if (maestro?.IdSteam > 0)
									{
										juego.Maestro = maestro.Id.ToString();
									}
								}
							}
							else if (tipo == Juegos.JuegoTipo.Software)
							{
								juego.Imagenes.Logo = null;
								juego.Imagenes.Library_600x900 = null;
								juego.Imagenes.Library_1920x620 = null;
							}
							else
							{
								juego.Deck = deck;
								juego.SteamOS = steamOS;
								juego.SteamFrame = steamFrame;
								juego.SteamMachine = steamMachine;
							}

							if (categorias.Count > 0)
							{
								juego.Categorias = categorias;
							}

							if (idiomas.Count > 0)
							{
								juego.Idiomas = idiomas;
							}

							if (etiquetas.Count > 0)
							{
								juego.Etiquetas = etiquetas;
							}

							if (precio != null)
							{
								juego.PrecioActualesTiendas = new List<Juegos.JuegoPrecio> { precio };
								juego.PrecioMinimosHistoricos = new List<Juegos.JuegoPrecio> { precio };
							}

							return juego;
						}
					}
				}
			}

			return null;
		}

        public static async Task<SteamDeckAPI> CargarDatosDeck(int juegoId, int steamId)
		{
			bool sePuedeBuscar = true;

			DateTime? fecha = await BaseDatos.Admin.Buscar.Fecha("steamapi");

			if (fecha != null)
			{
				if (fecha.Value.AddDays(3) > DateTime.Now)
				{
					sePuedeBuscar = false;
				}
			}

			if (sePuedeBuscar == false)
			{
				return null;
			}

			string id = steamId.ToString();

			if (string.IsNullOrEmpty(id) == false)
			{
				string html = await Decompiladores.Estandar("https://store.steampowered.com/saleaction/ajaxgetdeckappcompatibilityreport?nAppID=" + id + "&l=english&cc=en");
			
				if (string.IsNullOrEmpty(html) == false)
				{
					SteamDeckAPI api = null;
						
					try
					{
						api = JsonSerializer.Deserialize<SteamDeckAPI>(html);
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Deck API " + juegoId.ToString(), ex);
					}
					
					if (api != null)
					{
						return api;
					}
				}
			}	
				
			return null;
		}

		public static async Task<int> CargarDLCMaestro(string enlace)
		{
			string id = LimpiarID(enlace);

			if (string.IsNullOrEmpty(id) == true)
			{
				return 0;
			}

			string html2 = await Decompiladores.Estandar(@"https://api.steampowered.com/IStoreBrowseService/GetItems/v1?input_json={""ids"":[{""appid"":" + id + @"}],""context"":{""language"":""english"",""country_code"":""ES"",""steam_realm"":1},""data_request"":{""include_reviews"":true,""include_basic_info"":true, ""include_assets"": true, ""include_links"": true, ""include_tag_count"": 20, ""include_release"": true, ""include_platforms"": true, ""include_screenshots"": true, ""include_trailers"": true, ""include_supported_languages"": true}}");

			if (string.IsNullOrEmpty(html2) == false)
			{
				SteamJuegoAPI2 datos2 = null;

				try
				{
					datos2 = JsonSerializer.Deserialize<SteamJuegoAPI2>(html2);
				}
				catch { }

				if (datos2 != null)
				{
					return datos2?.Respuesta?.Juegos[0]?.Cosas?.MaestroId ?? 0;
				}
			}

			return 0;
		}

		public static async Task<int> CargarCantidadJugadores(string id)
		{
			string html = await Decompiladores.Estandar("https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid=" + id);
		
			if (string.IsNullOrEmpty(html) == false)
			{
				SteamCantidadJugadoresAPI api = null;

				try
				{
					api = JsonSerializer.Deserialize<SteamCantidadJugadoresAPI>(html);
				}
				catch { }

				if (api != null)
				{
					if (api.Datos != null)
					{
						return api.Datos.Cantidad;
					}
				}
			}

			return 0;
		}

		public static async Task<SteamReseñasApi> CargarDatosReseñas(int id2, string idioma)
		{
			string id = id2.ToString();

			if (string.IsNullOrEmpty(id) == false)
			{
				bool sePuedeBuscar = true;

				DateTime? fecha = await BaseDatos.Admin.Buscar.Fecha("steamapi");

				if (fecha != null)
				{
					if (fecha.Value.AddDays(3) > DateTime.Now)
					{
						sePuedeBuscar = false;
					}
				}

				if (sePuedeBuscar == false)
				{
					return null;
				}

				string html = await Decompiladores.Estandar("https://store.steampowered.com/ajaxappreviews/" + id + "?date_range_type=all&day_range=120&start_date=-1&end_date=-1&cursor=*&filter_offtopic_activity=true&playtime_filter_max=0&playtime_filter_min=0&playtime_type=all&purchase_type=all&review_type=all&use_review_quality=true&language=" + idioma + "&hardware_os=all&hardware_cpu=all&hardware_gpu=all&hardware_device_type=all");

				if (string.IsNullOrEmpty(html) == false)
				{
					JsonSerializerOptions opciones = new JsonSerializerOptions
					{
						UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
					};

					SteamReseñasApi api = null;
					
					try
					{
						api = JsonSerializer.Deserialize<SteamReseñasApi>(html, opciones);
					}
					catch (Exception ex)
					{ 
						BaseDatos.Errores.Insertar.Mensaje("Parseo SteamReseñasApi " + id, ex);
					}

					if (api != null)
					{
						return api;
					}
				}
			}

			return null;
		}

		public static async Task<SteamCMDDatos> UltimaActualizacioneInteligenciaArtificial(int id)
		{
			if (id > 0)
			{
				string html = await Decompiladores.Estandar("https://api.steamcmd.net/v1/info/" + id.ToString());

				if (string.IsNullOrEmpty(html) == false)
				{
					SteamCMDAPI api = JsonSerializer.Deserialize<SteamCMDAPI>(html);

					if (api?.Datos?.Count > 0)
					{
						foreach (var dato in api.Datos)
						{
							SteamCMDDatos datos = new SteamCMDDatos();

							if (string.IsNullOrEmpty(dato.Value.Common?.InteligenciaArtificial) == false)
							{
								if (dato.Value.Common?.InteligenciaArtificial == "1")
								{
									datos.InteligenciaArtificial = true;
								}
								else
								{
									datos.InteligenciaArtificial = false;
								}
							}

							if (dato.Value.Depots?.Branches?.Publico?.Ticks != null)
							{
								DateTimeOffset fecha = DateTimeOffset.FromUnixTimeSeconds(long.Parse(dato.Value.Depots?.Branches?.Publico?.Ticks));
								DateTime fechaCreado = fecha.UtcDateTime;

								datos.UltimaActualizacion = fechaCreado;
							}

							return datos;
						}
					}
				}
			}

			return null;
		}

		public static bool Detectar(string enlace)
		{
			bool resultado = false;

            if (enlace.Contains("https://store.steampowered.com/app/") == true)
            {
				resultado = true;
            }
			else if (enlace.Contains("https://store.steampowered.com/dlc/") == true)
			{
				resultado = true;
			}
			else if (enlace.Contains("https://steamdb.info/app/") == true)
			{
				resultado = true;
			}

			return resultado;
		}

		public static string LimpiarID(string enlace)
		{
			enlace = enlace.Replace("http://store.steampowered.com/app/", null);
			enlace = enlace.Replace("http://store.steampowered.com/dlc/", null);
			enlace = enlace.Replace("http://store.steampowered.com/bundle/", null);
			enlace = enlace.Replace("https://store.steampowered.com/app/", null);
			enlace = enlace.Replace("https://store.steampowered.com/dlc/", null);
			enlace = enlace.Replace("https://store.steampowered.com/bundle/", null);
			enlace = enlace.Replace("https://steamdb.info/app/", null);

			if (enlace.Contains("/") == true)
			{
				int int1 = enlace.IndexOf("/");
				enlace = enlace.Remove(int1, enlace.Length - int1);
			}

			if (enlace.Contains("?") == true)
			{
				int int1 = enlace.IndexOf("?");
				enlace = enlace.Remove(int1, enlace.Length - int1);
			}

			string id = enlace.Trim();

			return id;
		}

		public static string EnlaceAPI(string id)
		{
			string enlace = "https://api.steampowered.com/IStoreBrowseService/GetItems/v1?input_json={\"ids\":[{\"appid\":" + id + "}],\"context\":{\"language\":\"english\",\"country_code\":\"ES\",\"steam_realm\":1},\"data_request\":{\"include_reviews\":true,\"include_basic_info\":true, \"include_assets\": true, \"include_links\": true, \"include_tag_count\": 20, \"include_release\": true, \"include_platforms\": true, \"include_screenshots\": true, \"include_trailers\": true, \"include_supported_languages\": true, \"include_all_purchase_options\": true, \"include_extra_details\": true}}";

			return enlace;
		}
	}

	#region Clases Juego

	public class SteamJuegoAPI
	{
		[JsonPropertyName("data")]
		public SteamJuegoAPIDatos Datos { get; set; }
	}

	public class SteamJuegoAPIDatos
	{
		[JsonPropertyName("type")]
		public string Tipo { get; set; }

		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("steam_appid")]
		public object Id { get; set; }

		[JsonPropertyName("is_free")]
		public object Free2Play { get; set; }

		[JsonPropertyName("required_age")]
		public object MayorEdad { get; set; }

		[JsonPropertyName("short_description")]
		public string DescripcionCorta { get; set; }

		[JsonPropertyName("supported_languages")]
		public string Idiomas { get; set; }

		[JsonPropertyName("header_image")]
		public string ImagenHeader_460x215 { get; set; }

		[JsonPropertyName("capsule_image")]
		public string ImagenCapsule_231x87 { get; set; }

		[JsonPropertyName("publishers")]
		public List<string> Publishers { get; set; }

		[JsonPropertyName("developers")]
		public List<string> Desarrolladores { get; set; }

		[JsonPropertyName("dlc")]
		public List<object> DLCs { get; set; }

		[JsonPropertyName("price_overview")]
		public SteamJuegoAPIPrecio Precio { get; set; }

		[JsonPropertyName("platforms")]
		public SteamJuegoAPISistemas Sistemas { get; set; }

		[JsonPropertyName("screenshots")]
		public List<SteamJuegoAPICaptura> Capturas { get; set; }

		[JsonPropertyName("movies")]
		public List<SteamJuegoAPIVideo> Videos { get; set; }

		[JsonPropertyName("fullgame")]
		public SteamJuegoAPIMaestro Maestro { get; set; }

		[JsonPropertyName("categories")]
		public List<SteamJuegoAPICategoria> Categorias { get; set; }

		[JsonPropertyName("genres")]
		public List<SteamJuegoAPIGenero> Generos { get; set; }
	}

	public class SteamJuegoAPIPrecio
	{
		[JsonPropertyName("final_formatted")]
		public string Formateado { get; set; }

		[JsonPropertyName("discount_percent")]
		public object Descuento { get; set; }
	}

	public class SteamJuegoAPISistemas
	{
		[JsonPropertyName("windows")]
		public bool Windows { get; set; }

		[JsonPropertyName("mac")]
		public bool Mac { get; set; }

		[JsonPropertyName("linux")]
		public bool Linux { get; set; }
	}

	public class SteamJuegoAPICaptura
	{

		[JsonPropertyName("path_thumbnail")]
		public string Miniatura { get; set; }

		[JsonPropertyName("path_full")]
		public string Enlace { get; set; }
	}

	public class SteamJuegoAPIVideo
	{

		[JsonPropertyName("id")]
		public object Id { get; set; }

		[JsonPropertyName("mp4")]
		public SteamJuegoAPIVideoDatos Mp4 { get; set; }

		[JsonPropertyName("webm")]
		public SteamJuegoAPIVideoDatos Webm { get; set; }
	}

	public class SteamJuegoAPIVideoDatos
	{
		[JsonPropertyName("max")]
		public string Enlace { get; set; }
	}

	public class SteamJuegoAPIMaestro
	{
		[JsonPropertyName("appid")]
		public object Id { get; set; }

		[JsonPropertyName("name")]
		public string Nombre { get; set; }
	}

	public class SteamJuegoAPICategoria
	{
		[JsonPropertyName("id")]
		public object Id { get; set; }

		[JsonPropertyName("description")]
		public string Descripcion { get; set; }
	}

	public class SteamJuegoAPIGenero
	{
		[JsonPropertyName("id")]
		public object Id { get; set; }

		[JsonPropertyName("description")]
		public string Descripcion { get; set; }
	}

	#endregion

	#region Clases Juego2

	public class SteamJuegoAPI2
	{
		[JsonPropertyName("response")]
		public SteamJuegoAPI2Resultados Respuesta { get; set; }
	}

	public class SteamJuegoAPI2Resultados
	{
		[JsonPropertyName("store_items")]
		public List<SteamJuegoAPI2Juego> Juegos { get; set; }
	}

	public class SteamJuegoAPI2Juego
	{
		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("type")]
		public int Tipo { get; set; }

		[JsonPropertyName("visible")]
		public bool Visible { get; set; }

		[JsonPropertyName("is_free")]
		public bool FreeToPlay { get; set; }

		[JsonPropertyName("is_early_access")]
		public bool AccesoAnticipado { get; set; }

		[JsonPropertyName("unlisted")]
		public bool Delistado { get; set; }

		[JsonPropertyName("assets")]
		public SteamJuegoAPI2JuegoImagenes Imagenes { get; set; }

		[JsonPropertyName("reviews")]
		public SteamJuegoAPI2JuegoReseñas Reseñas { get; set; }

		[JsonPropertyName("tagids")]
		public List<int> Etiquetas { get; set; }

		[JsonPropertyName("categories")]
		public SteamJuegoAPI2JuegoCategorias Categorias { get; set; }

		[JsonPropertyName("basic_info")]
		public SteamJuegoAPI2JuegoInfo Info { get; set; }

		[JsonPropertyName("release")]
		public SteamJuegoAPI2JuegoLanzamiento Lanzamiento { get; set; }

		[JsonPropertyName("platforms")]
		public SteamJuegoAPI2JuegoPlataformas Plataformas { get; set; }

		[JsonPropertyName("screenshots")]
		public SteamJuegoAPI2JuegoCapturas Capturas { get; set; }

		[JsonPropertyName("trailers")]
		public SteamJuegoAPI2JuegoVideos Trailers { get; set; }

		[JsonPropertyName("supported_languages")]
		public List<SteamJuegoAPI2JuegoIdioma> Idiomas { get; set; }

		[JsonPropertyName("links")]
		public List<SteamJuegoAPI2JuegoEnlace> Enlaces { get; set; }

		[JsonPropertyName("best_purchase_option")]
		public SteamJuegoAPI2JuegoPrecio Precio { get; set; }

		[JsonPropertyName("related_items")]
		public SteamJuegoAPI2JuegoCosas Cosas { get; set; }

		[JsonPropertyName("accessories")]
		public List<SteamJuegoAPI2JuegoPrecio> Suscripciones { get; set; }

		[JsonPropertyName("extra_details")]
		public SteamJuegoAPI2JuegoExtra Extra { get; set; }
	}

	public class SteamJuegoAPI2JuegoImagenes
	{
		[JsonPropertyName("small_capsule")]
		public string Capsule_231x87 { get; set; }

		[JsonPropertyName("header")]
		public string Header_460x215 { get; set; }

		[JsonPropertyName("library_capsule")]
		public string Library_600x900 { get; set; }

		[JsonPropertyName("library_hero")]
		public string Library_1920x620 { get; set; }
	}

	public class SteamJuegoAPI2JuegoReseñas
	{
		[JsonPropertyName("summary_filtered")]
		public SteamJuegoAPI2JuegoReseñasFiltrado Filtrado { get; set; }
	}

	public class SteamJuegoAPI2JuegoReseñasFiltrado
	{
		[JsonPropertyName("review_count")]
		public int Cantidad { get; set; }

		[JsonPropertyName("percent_positive")]
		public int Porcentaje { get; set; }
	}

	public class SteamJuegoAPI2JuegoInfo
	{
		[JsonPropertyName("short_description")]
		public string DescripcionCorta { get; set; }

		[JsonPropertyName("publishers")]
		public List<SteamJuegoAPI2JuegoInfoDesarrollador> Editores { get; set; }

		[JsonPropertyName("developers")]
		public List<SteamJuegoAPI2JuegoInfoDesarrollador> Desarrolladores { get; set; }

		[JsonPropertyName("franchises")]
		public List<SteamJuegoAPI2JuegoInfoDesarrollador> Franquicias { get; set; }
	}

	public class SteamJuegoAPI2JuegoInfoDesarrollador
	{
		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("creator_clan_account_id")]
		public int Id { get; set; }
	}

	public class SteamJuegoAPI2JuegoLanzamiento
	{
		[JsonPropertyName("steam_release_date")]
		public int Steam { get; set; }

		[JsonPropertyName("original_release_date")]
		public int Original { get; set; }

		[JsonPropertyName("release_from_early_access_date")]
		public int SteamDejaAccesoAnticipado { get; set; }
	}

	public class SteamJuegoAPI2JuegoPlataformas
	{
		[JsonPropertyName("windows")]
		public bool Windows { get; set; }

		[JsonPropertyName("mac")]
		public bool Mac { get; set; }

		[JsonPropertyName("steamos_linux")]
		public bool Linux { get; set; }

		[JsonPropertyName("steam_deck_compat_category")]
		public int Deck { get; set; }

		[JsonPropertyName("steam_os_compat_category")]
		public int SteamOS { get; set; }

		[JsonPropertyName("steam_frame_compat_category")]
		public int SteamFrame { get; set; }

		[JsonPropertyName("steam_machine_compat_category")]
		public int SteamMachine { get; set; }

		[JsonPropertyName("vr_support")]
		public SteamJuegoAPI2JuegoPlataformasRV RV { get; set; }
	}

	public class SteamJuegoAPI2JuegoPlataformasRV
	{
		[JsonPropertyName("vrhmd")]
		public bool Vrhmd { get; set; }

		[JsonPropertyName("vrhmd_only")]
		public bool VrhmdOnly { get; set; }

		[JsonPropertyName("htc_vive")]
		public bool HtcVive { get; set; }

		[JsonPropertyName("oculus_rift")]
		public bool OculusRift { get; set; }

		[JsonPropertyName("windows_mr")]
		public bool WindowsMr { get; set; }

		[JsonPropertyName("valve_index")]
		public bool ValveIndex { get; set; }
	}

	public class SteamJuegoAPI2JuegoCapturas
	{
		[JsonPropertyName("all_ages_screenshots")]
		public List<SteamJuegoAPI2JuegoCaptura> TodasEdades { get; set; }

		[JsonPropertyName("mature_content_screenshots")]
		public List<SteamJuegoAPI2JuegoCaptura> MayorEdad { get; set; }
	}

	public class SteamJuegoAPI2JuegoCaptura
	{
		[JsonPropertyName("filename")]
		public string Nombre { get; set; }

		[JsonPropertyName("ordinal")]
		public int Posicion { get; set; }
	}

	public class SteamJuegoAPI2JuegoVideos
	{
		[JsonPropertyName("highlights")]
		public List<SteamJuegoAPI2JuegoVideo> Videos { get; set; }
	}

	public class SteamJuegoAPI2JuegoVideo
	{
		[JsonPropertyName("trailer_name")]
		public string Nombre { get; set; }

		[JsonPropertyName("trailer_url_format")]
		public string EnlaceFormato { get; set; }

        [JsonPropertyName("adaptive_trailers")]
        public List<SteamJuegoAPI2JuegoVideoDatos2> DatosAdaptivo { get; set; }

        [JsonPropertyName("trailer_max")]
		public List<SteamJuegoAPI2JuegoVideoDatos> DatosMax { get; set; }

		[JsonPropertyName("microtrailer")]
		public List<SteamJuegoAPI2JuegoVideoDatos> DatosMicro { get; set; }

		[JsonPropertyName("screenshot_full")]
		public string Captura { get; set; }

		[JsonPropertyName("screenshot_medium")]
		public string CapturaPequeña { get; set; }

		[JsonPropertyName("all_ages")]
		public bool MayorEdad { get; set; }
	}

	public class SteamJuegoAPI2JuegoVideoDatos
	{
		[JsonPropertyName("filename")]
		public string Fichero { get; set; }

		[JsonPropertyName("type")]
		public string Tipo { get; set; }
	}

    public class SteamJuegoAPI2JuegoVideoDatos2
    {
        [JsonPropertyName("cdn_path")]
        public string Enlace { get; set; }

        [JsonPropertyName("encoding")]
        public string Encodeo { get; set; }
    }

    public class SteamJuegoAPI2JuegoIdioma
	{
		[JsonPropertyName("elanguage")]
		public int Id { get; set; }

		[JsonPropertyName("supported")]
		public bool Soportado { get; set; }

		[JsonPropertyName("full_audio")]
		public bool Audio { get; set; }

		[JsonPropertyName("subtitles")]
		public bool Texto { get; set; }
	}

	public class SteamJuegoAPI2JuegoEnlace
	{
		[JsonPropertyName("url")]
		public string Enlace { get; set; }
	}

	public class SteamJuegoAPI2JuegoCategorias
	{
		[JsonPropertyName("supported_player_categoryids")]
		public List<int> Tipo1 { get; set; }

		[JsonPropertyName("feature_categoryids")]
		public List<int> Tipo2 { get; set; }

		[JsonPropertyName("controller_categoryids")]
		public List<int> Tipo3 { get; set; }
	}

	public class SteamJuegoAPI2JuegoPrecio
	{
		[JsonPropertyName("formatted_final_price")]
		public string PrecioRebajado { get; set; }

		[JsonPropertyName("formatted_original_price")]
		public string PrecioBase { get; set; }

		[JsonPropertyName("discount_pct")]
		public int Descuento { get; set; } = 0;

		[JsonPropertyName("packageid")]
		public int PaqueteId { get; set; }
	}

	public class SteamJuegoAPI2JuegoCosas
	{
		[JsonPropertyName("parent_appid")]
		public int MaestroId { get; set; }

		[JsonPropertyName("demo_appid")]
		public List<int> DemoId { get; set; }

		[JsonPropertyName("standalone_demo_appid")]
		public List<int> DemoId2 { get; set; }
	}

	public class SteamJuegoAPI2JuegoExtra
	{
		[JsonPropertyName("drm_third_party_type")]
		public string DRMExterno { get; set; }

		[JsonPropertyName("user_account_third_party")]
		public string PideCuenta { get; set; }

		[JsonPropertyName("anticheat")]
		public SteamJuegoAPI2JuegoExtraAntichetos Antichetos { get; set; }
	}

	public class SteamJuegoAPI2JuegoExtraAntichetos
	{
		[JsonPropertyName("kernel_mode")]
		public bool Kernel { get; set; }
	}

	#endregion

	#region Clases Deck

	public class SteamDeckAPI
	{
		[JsonPropertyName("results")]
		public SteamDeckAPIResultado Datos { get; set; }
	}

	public class SteamDeckAPIResultado
	{
		[JsonPropertyName("appid")]
		public object Id { get; set; }

		[JsonPropertyName("resolved_category")]
		public object Resultado { get; set; }

		[JsonPropertyName("resolved_items")]
		public List<SteamDeckAPIToken> Tokens { get; set; }

		[JsonPropertyName("steamos_resolved_category")]
		public object SteamOSResultado { get; set; }

		[JsonPropertyName("steamos_resolved_items")]
		public List<SteamDeckAPIToken> SteamOSTokens { get; set; }

		[JsonPropertyName("machine_resolved_category")]
		public object SteamMachineResultado { get; set; }

		[JsonPropertyName("machine_resolved_items")]
		public List<SteamDeckAPIToken> SteamMachineTokens { get; set; }

		[JsonPropertyName("frame_resolved_category")]
		public object SteamFrameResultado { get; set; }

		[JsonPropertyName("frame_resolved_items")]
		public List<SteamDeckAPIToken> SteamFrameTokens { get; set; }
	}

	public class SteamDeckAPIToken
	{
		[JsonPropertyName("display_type")]
		public object Tipo { get; set; }

		[JsonPropertyName("loc_token")]
		public string Token { get; set; }
	}

	#endregion

	#region Clases Cantidad Jugadores

	public class SteamCantidadJugadoresAPI
	{
		[JsonPropertyName("response")]
		public SteamCantidadJugadoresAPIRespuesta Datos { get; set; }
	}

	public class SteamCantidadJugadoresAPIRespuesta
	{
		[JsonPropertyName("player_count")]
		public int Cantidad { get; set; }
	}

	#endregion

	#region Clases Reseñas

	public class SteamReseñasApi
	{
		[JsonPropertyName("success")]
		public int Success { get; set; }

		[JsonPropertyName("bHasAppSpecificCommunityHub")]
		public bool BHasAppSpecificCommunityHub { get; set; }

		[JsonPropertyName("numReviewsPerPage")]
		public int NumReviewsPerPage { get; set; }

		[JsonPropertyName("reviewFilter")]
		public string ReviewFilter { get; set; }

		[JsonPropertyName("reviewStartDate")]
		public int ReviewStartDate { get; set; }

		[JsonPropertyName("reviewEndDate")]
		public int ReviewEndDate { get; set; }

		[JsonPropertyName("cAppRecommendationCount")]
		public int CAppRecommendationCount { get; set; }

		[JsonPropertyName("rgReviewTags")]
		public List<object> RgReviewTags { get; set; }

		[JsonPropertyName("query_summary")]
		public SteamReseñasApiSumario Sumario { get; set; }

		[JsonPropertyName("reviews")]
		public List<SteamReseñasApiReseña> Reseñas { get; set; }
	}

	public class SteamReseñasApiSumario
	{
		[JsonPropertyName("num_reviews")]
		public int NumReviews { get; set; }

		[JsonPropertyName("review_score")]
		public int ReviewScore { get; set; }

		[JsonPropertyName("review_score_desc")]
		public string ReviewScoreDesc { get; set; }

		[JsonPropertyName("total_positive")]
		public int TotalPositivos { get; set; }

		[JsonPropertyName("total_negative")]
		public int TotalNegativos { get; set; }

		[JsonPropertyName("total_reviews")]
		public int TotalReviews { get; set; }

		[JsonPropertyName("review_score_tooltip")]
		public string ReviewScoreTooltip { get; set; }
	}

	public class SteamReseñasApiReseña
	{
		[JsonPropertyName("recommendationid")]
		public string RecommendationId { get; set; }

		[JsonPropertyName("author")]
		public SteamReseñasApiAutor Author { get; set; }

		[JsonPropertyName("language")]
		public string Language { get; set; }

		[JsonPropertyName("appid")]
		public int AppId { get; set; }

		[JsonPropertyName("review")]
		public string ReviewText { get; set; }

		[JsonPropertyName("timestamp_created")]
		public long TimestampCreated { get; set; }

		[JsonPropertyName("timestamp_updated")]
		public long TimestampUpdated { get; set; }

		[JsonPropertyName("voted_up")]
		public bool VotedUp { get; set; }

		[JsonPropertyName("votes_up")]
		public int VotesUp { get; set; }

		[JsonPropertyName("votes_funny")]
		public int VotesFunny { get; set; }

		[JsonPropertyName("comment_count")]
		public int CommentCount { get; set; }

		[JsonPropertyName("steam_purchase")]
		public bool SteamPurchase { get; set; }

		[JsonPropertyName("received_for_free")]
		public bool ReceivedForFree { get; set; }

		[JsonPropertyName("refunded")]
		public bool Refunded { get; set; }

		[JsonPropertyName("written_during_early_access")]
		public bool WrittenDuringEarlyAccess { get; set; }

		[JsonPropertyName("primarily_steam_deck")]
		public bool PrimarilySteamDeck { get; set; }

		[JsonPropertyName("app_release_date")]
		public string AppReleaseDate { get; set; }

		[JsonPropertyName("reactions")]
		public List<SteamReseñasApiReacion> Reactions { get; set; }
	}

	public class SteamReseñasApiAutor
	{
		[JsonPropertyName("steamid")]
		public string SteamId { get; set; }

		[JsonPropertyName("personaname")]
		public string PersonaName { get; set; }

		[JsonPropertyName("persona_status")]
		public string PersonaStatus { get; set; }

		[JsonPropertyName("profile_url")]
		public string ProfileUrl { get; set; }

		[JsonPropertyName("num_games_owned")]
		public int NumGamesOwned { get; set; }

		[JsonPropertyName("num_reviews")]
		public int NumReviews { get; set; }

		[JsonPropertyName("playtime_forever")]
		public int TiempoJugadoTotal { get; set; }

		[JsonPropertyName("playtime_last_two_weeks")]
		public int PlaytimeLastTwoWeeks { get; set; }

		[JsonPropertyName("playtime_at_review")]
		public int PlaytimeAtReview { get; set; }

		[JsonPropertyName("deck_playtime_at_review")]
		public int? DeckPlaytimeAtReview { get; set; }

		[JsonPropertyName("last_played")]
		public long LastPlayed { get; set; }

		[JsonPropertyName("avatar")]
		public string Avatar { get; set; }
	}

	public class SteamReseñasApiReacion
	{
		[JsonPropertyName("reaction_type")]
		public int ReactionType { get; set; }

		[JsonPropertyName("count")]
		public int Count { get; set; }
	}

	#endregion

	#region Clases CMD Ultima Actualizacion e IA

	public class SteamCMDDatos
	{
		public bool InteligenciaArtificial { get; set; }
		public DateTime? UltimaActualizacion { get; set; }
	}

	public class SteamCMDAPI
	{
		[JsonPropertyName("data")]
		public Dictionary<string, SteamCMDAPIDatos> Datos { get; set; }
	}

	public class SteamCMDAPIDatos
	{
		[JsonPropertyName("common")]
		public SteamCMDAPICommon Common { get; set; }

		[JsonPropertyName("depots")]
		public SteamCMDAPIDepots Depots { get; set; }
	}

	public class SteamCMDAPICommon
	{
		[JsonPropertyName("aicontenttype")]
		public string InteligenciaArtificial { get; set; }
	}

	public class SteamCMDAPIDepots
	{
		[JsonPropertyName("branches")]
		public SteamCMDAPIBranches Branches { get; set; }
	}

	public class SteamCMDAPIBranches
	{
		[JsonPropertyName("public")]
		public SteamCMDAPIBranchesPublic Publico { get; set; }
	}

	public class SteamCMDAPIBranchesPublic
	{
		[JsonPropertyName("timeupdated")]
		public string Ticks { get; set; }
	}

	#endregion
}
