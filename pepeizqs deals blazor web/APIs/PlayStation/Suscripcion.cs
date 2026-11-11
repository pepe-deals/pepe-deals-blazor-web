#nullable disable

using Dapper;
using Herramientas;
using Juegos;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.PlayStation
{

	public static class Suscripcion
	{
		public static Suscripciones2.Suscripcion Generar()
		{
			Suscripciones2.Suscripcion ps = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.PlayStationPlus,
				Nombre = "PlayStation Plus Premium",
				ImagenLogo = "/imagenes/suscripciones/psplus_logo.webp",
				ImagenIcono = "/imagenes/suscripciones/psplus_icono.webp",
				Enlace = "https://www.playstation.com/ps-plus/#subscriptions",
				DRMDefecto = JuegoDRM.PlayStation,
				UsuarioEnlacesEspecificos = true,
				ParaSiempre = false,
				Precio = 18.99,
				AdminPendientes = true,
				TablaPendientes = "suscripcionplaystationplus",
				SoloStreaming = true,
				AdminAñadir = false,
				UsuarioPuedeAbrir = true
			};

			return ps;
		}

		public static async Task Buscar()
		{
			await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, Generar().Id.ToString().ToLower(), DateTime.Now, 0);

			int cantidad = 0;

			List<string> enlaces = [
				"https://www.playstation.com/bin/imagic/gameslist?locale=es-es&categoryList=plus-monthly-games-list",
				"https://www.playstation.com/bin/imagic/gameslist?locale=es-es&categoryList=plus-classics-list"];

			foreach (var enlace2 in enlaces)
			{
				string html = await Decompiladores.Estandar(enlace2);

				if (string.IsNullOrEmpty(html) == false)
				{
					List<PsPlusLetra> datos = JsonSerializer.Deserialize<List<PsPlusLetra>>(html);

					if (datos?.Count > 0)
					{
						foreach (var letra in datos)
						{
							foreach (var juego in letra.Juegos)
							{
								if (juego.AceptaStreaming == true)
								{
									bool encontrado = false;

									string sqlBuscar = $"SELECT idJuegos FROM {Generar().TablaPendientes} WHERE enlace=@enlace";

									try
									{
										string idJuegosTexto = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
										{
											return await conexion.QueryFirstOrDefaultAsync<string>(sqlBuscar, new { juego.Enlace });
										});

										if (string.IsNullOrEmpty(idJuegosTexto) == false)
										{
											encontrado = true;

											if (idJuegosTexto != "0")
											{
												cantidad += 1;
												await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, Generar().Id.ToString().ToLower(), DateTime.Now, cantidad);

												List<string> idJuegos = Herramientas.Listados.Generar(idJuegosTexto);

												if (idJuegos.Count > 0)
												{
													foreach (var id in idJuegos)
													{
														bool insertar = true;
														var suscripciones = await BaseDatos.Suscripciones.Buscar.JuegoId(int.Parse(id));

														if (suscripciones?.Count > 0)
														{
															foreach (var suscripcion in suscripciones)
															{
																if (suscripcion.Tipo == Suscripciones2.SuscripcionTipo.PlayStationPlus)
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
																DRM = JuegoDRM.PlayStation,
																Nombre = id,
																FechaEmpieza = DateTime.Now,
																FechaTermina = nuevaFecha,
																JuegoId = int.Parse(id),
																Enlace = juego.Enlace,
																Tipo = Suscripciones2.SuscripcionTipo.PlayStationPlus
															};

															await BaseDatos.Suscripciones.Insertar.Ejecutar(int.Parse(id), nuevaSuscripcion);
														}
													}
												}
											}
										}
									}
									catch (Exception ex)
									{
										BaseDatos.Errores.Insertar.Mensaje("Psplus", ex);
									}

									if (encontrado == false)
									{
										await BaseDatos.Suscripciones.Insertar.Temporal(Generar().Id.ToString().ToLower(), juego.Enlace, juego.Nombre, juego.Imagen);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public class PsPlusLetra
	{
		[JsonPropertyName("count")]
		public int Cantidad { get; set; }

		[JsonPropertyName("games")]
		public List<PsPlusJuego> Juegos { get; set; }
	}

	public class PsPlusJuego
	{
		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("imageUrl")]
		public string Imagen { get; set; }

		[JsonPropertyName("conceptUrl")]
		public string Enlace { get; set; }

		[JsonPropertyName("streamingSupported")]
		public bool AceptaStreaming { get; set; }
	}
}
