#nullable disable

using Dapper;
using Juegos;
using System.Text.Json;
using Tiendas2;

namespace APIs.AmazonLuna
{
	public static class Suscripcion
	{
		public static Suscripciones2.Suscripcion GenerarClaims()
		{
			Suscripciones2.Suscripcion amazon = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.LunaClaims,
				Nombre = "Luna",
				ImagenLogo = "/imagenes/suscripciones/lunaclaims_logo.webp",
				ImagenIcono = "/imagenes/streaming/amazonluna_icono.webp",
				ImagenFondo = "/imagenes/streaming/amazonluna_fondo.webp",
				ColorDestacado = "rgba(28, 23, 43, 0.3)",
				Enlace = "https://luna.amazon.es/claims/",
				DRMDefecto = JuegoDRM.Amazon,
				UsuarioEnlacesEspecificos = true,
				ParaSiempre = true,
				SuscripcionesRelacionadas = new List<Suscripciones2.SuscripcionTipo>() { Suscripciones2.SuscripcionTipo.LunaStandard, Suscripciones2.SuscripcionTipo.LunaPremium },
				Precio = 4.99,
				AdminAñadir = true,
				UsuarioPuedeAbrir = true
			};

			DateTime fechaPrime = DateTime.Now;
			fechaPrime = fechaPrime.AddMonths(1);
			fechaPrime = new DateTime(fechaPrime.Year, fechaPrime.Month, fechaPrime.Day, 19, 0, 0);

			amazon.FechaSugerencia = fechaPrime;

			return amazon;
		}

		public static Suscripciones2.Suscripcion GenerarStandard()
		{
			Suscripciones2.Suscripcion amazon = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.LunaStandard,
				Nombre = "Luna Standard",
				ImagenLogo = "/imagenes/suscripciones/lunastandard_logo.webp",
				ImagenIcono = "/imagenes/streaming/amazonluna_icono.webp",
				ImagenFondo = "/imagenes/streaming/amazonluna_fondo.webp",
				ColorDestacado = "rgba(28, 23, 43, 0.3)",
				Enlace = "https://luna.amazon.es/subscription/luna-standard",
				DRMDefecto = JuegoDRM.AmazonLuna,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = false,
				Precio = 4.99,
				AdminPendientes = true,
				TablaPendientes = "suscripcionlunapremium",
				SoloStreaming = true,
				AdminAñadir = false,
				UsuarioPuedeAbrir = true
			};

			return amazon;
		}

		public static Suscripciones2.Suscripcion GenerarPremium()
		{
			Suscripciones2.Suscripcion amazon = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.LunaPremium,
				Nombre = "Luna Premium",
				ImagenLogo = "/imagenes/suscripciones/lunapremium_logo.webp",
				ImagenIcono = "/imagenes/streaming/amazonluna_icono.webp",
				ImagenFondo = "/imagenes/streaming/amazonluna_fondo.webp",
				ColorDestacado = "rgba(28, 23, 43, 0.3)",
				Enlace = "https://luna.amazon.es/subscription/luna-premium/B085TRCCT6",
				DRMDefecto = JuegoDRM.AmazonLuna,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = false,
				Precio = 9.99,
				AdminPendientes = true,
				TablaPendientes = "suscripcionlunapremium",
				SoloStreaming = true,
				AdminAñadir = false,
				UsuarioPuedeAbrir = true
			};

			return amazon;
		}

		public static string Referido(string enlace)
		{
			if (enlace.Contains("?") == true)
			{
				enlace = enlace + "&tag=ofedeunpan-21";
			}
			else
			{
				enlace = enlace + "?tag=ofedeunpan-21";
			}

			return enlace;
		}

		public static async Task Buscar()
		{
			int cantidad = 0;

			#region Standard

			await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, GenerarStandard().Id.ToString().ToLower(), DateTime.Now, 0);

			List<AmazonLunaJuego> juegosStandard = null;

			try 
			{
				var filas = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<AmazonLunaFila>(
						"SELECT contenido FROM temporallunastandardjson"
					)).ToList();
				});

				foreach (var fila in filas)
				{
					juegosStandard = JsonSerializer.Deserialize<List<AmazonLunaJuego>>(fila.Contenido);
				}
			}
			catch (Exception ex) 
			{ 
				BaseDatos.Errores.Insertar.Mensaje("Luna Standard Suscripcion 1", ex); 
			}

			if (juegosStandard?.Count > 0)
			{
				foreach (var juego in juegosStandard)
				{
					if (string.IsNullOrEmpty(juego.Id) == false)
					{
						bool encontrado = false;

						string idJuegosTexto = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
						{
							return await conexion.QueryFirstOrDefaultAsync<string>($"SELECT idJuegos FROM {GenerarPremium().TablaPendientes} WHERE enlace=@enlace", new { enlace = juego.Id });
						});

						if (string.IsNullOrEmpty(idJuegosTexto) == false)
						{
							encontrado = true;

							if (idJuegosTexto != "0")
							{
								List<string> idJuegos = Herramientas.Listados.Generar(idJuegosTexto);

								if (idJuegos.Count > 0)
								{
									foreach (var id in idJuegos)
									{
										cantidad += 1;
										await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, GenerarStandard().Id.ToString().ToLower(), DateTime.Now, cantidad);

										bool insertar = true;
										var suscripciones = await BaseDatos.Suscripciones.Buscar.JuegoId(int.Parse(id));

										if (suscripciones?.Count > 0)
										{
											foreach (var suscripcion in suscripciones)
											{
												if (suscripcion.Tipo == Suscripciones2.SuscripcionTipo.LunaStandard)
												{
													insertar = false;

													suscripcion.FechaTermina = DateTime.Now + TimeSpan.FromDays(2);

													await BaseDatos.Suscripciones.Actualizar.FechaTermina(suscripcion);
												}
											}
										}

										if (insertar == true)
										{
											DateTime nuevaFecha = DateTime.Now;
											nuevaFecha = nuevaFecha + TimeSpan.FromDays(2);

											JuegoSuscripcion nuevaSuscripcion = new JuegoSuscripcion
											{
												DRM = JuegoDRM.AmazonLuna,
												Nombre = juego.Nombre,
												FechaEmpieza = DateTime.Now,
												FechaTermina = nuevaFecha,
												JuegoId = int.Parse(id),
												Enlace = juego.Id,
												Tipo = Suscripciones2.SuscripcionTipo.LunaStandard
											};

											await BaseDatos.Suscripciones.Insertar.Ejecutar(int.Parse(id), nuevaSuscripcion);
										}
									}
								}
							}
						}

						if (encontrado == false)
						{
							await BaseDatos.Suscripciones.Insertar.Temporal(GenerarStandard().Id.ToString().ToLower(), juego.Id, juego.Nombre);
						}
					}
				}

				try 
				{
					await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync("DELETE FROM temporallunastandardjson WHERE enlace='1'", transaction: sentencia);
					});
				} 
				catch (Exception ex) 
				{ 
					BaseDatos.Errores.Insertar.Mensaje(GenerarStandard().Id.ToString().ToLower(), ex); 
				}
			}

			#endregion

			#region Premium

			await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, GenerarPremium().Id.ToString().ToLower(), DateTime.Now, 0);

			cantidad = 0;

			List<AmazonLunaJuego> juegosPremium = null;

			try
			{
				var filas = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<AmazonLunaFila>("SELECT contenido FROM temporallunapremiumjson")).ToList();
				});

				foreach (var fila in filas)
				{
					juegosPremium = JsonSerializer.Deserialize<List<AmazonLunaJuego>>(fila.Contenido);
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Luna Premium Suscripcion 1", ex);
			}

			if (juegosPremium?.Count > 0)
			{
				foreach (var juego in juegosPremium)
				{
					if (string.IsNullOrEmpty(juego.Id) == false)
					{
						bool encontrado = false;

						string idJuegosTexto = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
						{
							return await conexion.QueryFirstOrDefaultAsync<string>($"SELECT idJuegos FROM {GenerarPremium().TablaPendientes} WHERE enlace=@enlace", new { enlace = juego.Id });
						});

						if (string.IsNullOrEmpty(idJuegosTexto) == false)
						{
							encontrado = true;

							if (idJuegosTexto != "0")
							{
								List<string> idJuegos = Herramientas.Listados.Generar(idJuegosTexto);

								if (idJuegos.Count > 0)
								{
									foreach (var id in idJuegos)
									{
										cantidad += 1;
										await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, GenerarPremium().Id.ToString().ToLower(), DateTime.Now, cantidad);

										bool insertar = true;
										var suscripciones = await BaseDatos.Suscripciones.Buscar.JuegoId(int.Parse(id));

										if (suscripciones?.Count > 0)
										{
											foreach (var suscripcion in suscripciones)
											{
												if (suscripcion.Tipo == Suscripciones2.SuscripcionTipo.LunaPremium)
												{
													insertar = false;

													suscripcion.FechaTermina = DateTime.Now + TimeSpan.FromDays(2);

													await BaseDatos.Suscripciones.Actualizar.FechaTermina(suscripcion);
												}
											}
										}

										if (insertar == true)
										{
											DateTime nuevaFecha = DateTime.Now;
											nuevaFecha = nuevaFecha + TimeSpan.FromDays(2);

											JuegoSuscripcion nuevaSuscripcion = new JuegoSuscripcion
											{
												DRM = JuegoDRM.AmazonLuna,
												Nombre = juego.Nombre,
												FechaEmpieza = DateTime.Now,
												FechaTermina = nuevaFecha,
												JuegoId = int.Parse(id),
												Enlace = juego.Id,
												Tipo = Suscripciones2.SuscripcionTipo.LunaPremium
											};

											await BaseDatos.Suscripciones.Insertar.Ejecutar(int.Parse(id), nuevaSuscripcion);
										}
									}
								}
							}
						}

						if (encontrado == false)
						{
							await BaseDatos.Suscripciones.Insertar.Temporal(GenerarPremium().Id.ToString().ToLower(), juego.Id, juego.Nombre);
						}
					}
				}

				try
				{
					await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync("DELETE FROM temporallunapremiumjson WHERE enlace='1'", transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje(GenerarPremium().Id.ToString().ToLower(), ex);
				}
			}

			#endregion
		}
	}

	public class AmazonLunaFila
	{
		public string Contenido { get; set; }
	}

	public class AmazonLunaJuego
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
	}
}
