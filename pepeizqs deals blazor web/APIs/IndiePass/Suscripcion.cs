#nullable disable

using Dapper;
using Juegos;
using System.Text.Json;
using Tiendas2;

namespace APIs.IndiePass
{
	public static class Suscripcion
	{
		public static Suscripciones2.Suscripcion Generar()
		{
			Suscripciones2.Suscripcion indiepass = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.IndiePass,
				Nombre = "Indie Pass",
				ImagenLogo = "/imagenes/suscripciones/indiepass_logo.webp",
				ImagenIcono = "/imagenes/suscripciones/indiepass_icono.webp",
				ImagenFondo = "/imagenes/suscripciones/indiepass_fondo.webp",
				ColorDestacado = "#48255c",
				Enlace = "https://www.indiepass.com/",
				DRMDefecto = JuegoDRM.IndiePass,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = false,
				Precio = 5.99,
				AdminPendientes = true,
				TablaPendientes = "suscripcionindiepass",
				AdminAñadir = false,
				UsuarioPuedeAbrir = true
			};

			return indiepass;
		}

		public static async Task Buscar()
		{
			await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, Generar().Id.ToString().ToLower(), DateTime.Now, 0);

			int cantidad = 0;

			List<IndiePassJuego> juegos = null;

			try
			{
				var filas = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<IndiePassFila>(
						"SELECT contenido FROM temporalindiepassjson"
					)).ToList();
				});

				foreach (var fila in filas)
				{
					juegos = JsonSerializer.Deserialize<List<IndiePassJuego>>(fila.Contenido);
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Indie Pass Suscripcion 1", ex);
			}

			if (juegos?.Count > 0)
			{
				foreach (var juego in juegos)
				{
					if (string.IsNullOrEmpty(juego.id.ToString()) == false)
					{
						bool encontrado = false;

						string idJuegosTexto = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
						{
							return await conexion.QueryFirstOrDefaultAsync<string>($"SELECT idJuegos FROM {Generar().TablaPendientes} WHERE enlace=@enlace", new { enlace = juego.id });
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
										await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, Generar().Id.ToString().ToLower(), DateTime.Now, cantidad);

										bool insertar = true;
										var suscripciones = await BaseDatos.Suscripciones.Buscar.JuegoId(int.Parse(id));

										if (suscripciones?.Count > 0)
										{
											foreach (var suscripcion in suscripciones)
											{
												if (suscripcion.Tipo == Suscripciones2.SuscripcionTipo.IndiePass)
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
												DRM = JuegoDRM.IndiePass,
												Nombre = juego.title,
												FechaEmpieza = DateTime.Now,
												FechaTermina = nuevaFecha,
												JuegoId = int.Parse(id),
												Enlace = juego.id.ToString(),
												Tipo = Suscripciones2.SuscripcionTipo.IndiePass
											};

											await BaseDatos.Suscripciones.Insertar.Ejecutar(int.Parse(id), nuevaSuscripcion);
										}
									}
								}
							}
						}

						if (encontrado == false)
						{
							await BaseDatos.Suscripciones.Insertar.Temporal(Generar().Id.ToString().ToLower(), juego.id.ToString(), juego.title);
						}
					}
				}

				try
				{
					await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync("DELETE FROM temporalindiepassjson WHERE enlace='1'", transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje(Generar().Id.ToString().ToLower(), ex);
				}
			}
		}
	}

	public class IndiePassFila
	{
		public string Contenido { get; set; }
	}

	public class IndiePassJuego
	{
		public int id { get; set; }
		public string title { get; set; }
	}
}
