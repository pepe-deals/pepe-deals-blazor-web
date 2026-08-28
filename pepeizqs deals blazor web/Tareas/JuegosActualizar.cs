#nullable disable

using APIs.Steam;
using Herramientas;

namespace Tareas
{
	public class JuegosActualizar : BackgroundService
	{
		private readonly ILogger<JuegosActualizar> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public JuegosActualizar(ILogger<JuegosActualizar> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
			_configuracion = configuracion;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(20));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				string piscinaTiendas = _configuracion.GetValue<string>("PoolTiendas:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaTiendas == piscinaUsada)
				{
					try
					{
						TimeSpan siguienteComprobacion = TimeSpan.FromMinutes(1);

						if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("fichasActualizar", siguienteComprobacion) == true && await BaseDatos.Admin.Buscar.TiendasLibre() == true)
						{
							await BaseDatos.Admin.Actualizar.TareaUso("fichasActualizar", DateTime.Now);

							#region Buscar 

							List<Juegos.Juego> juegosAleatorios = await BaseDatos.Juegos.Buscar.Aleatorios(true);

							if (juegosAleatorios?.Count > 0)
							{
								foreach (var juego in juegosAleatorios)
								{
									bool actualizarDatosAPI = false;
									TimeSpan tiempoComprobar = TimeSpan.FromDays(60);

									if (juego.Caracteristicas?.FechaLanzamientoSteam != null)
									{
										if (DateTime.Now.Subtract(juego.Caracteristicas.FechaLanzamientoSteam).TotalDays < 1)
										{
											tiempoComprobar = TimeSpan.FromHours(6);
										}
										else if (DateTime.Now.Subtract(juego.Caracteristicas.FechaLanzamientoSteam).TotalDays < 3)
										{
											tiempoComprobar = TimeSpan.FromHours(12);
										}
										else if (DateTime.Now.Subtract(juego.Caracteristicas.FechaLanzamientoSteam).TotalDays < 7)
										{
											tiempoComprobar = TimeSpan.FromDays(1);
										}
										else if (DateTime.Now.Subtract(juego.Caracteristicas.FechaLanzamientoSteam).TotalDays < 14)
										{
											tiempoComprobar = TimeSpan.FromDays(3);
										}
										else if (DateTime.Now.Subtract(juego.Caracteristicas.FechaLanzamientoSteam).TotalDays < 30)
										{
											tiempoComprobar = TimeSpan.FromDays(5);
										}
										else if (DateTime.Now.Subtract(juego.Caracteristicas.FechaLanzamientoSteam).TotalDays < 120)
										{
											tiempoComprobar = TimeSpan.FromDays(30);
										}
									}

									if (juego.FechaSteamAPIComprobacion.Add(tiempoComprobar) < DateTime.Now)
									{
										actualizarDatosAPI = true;
									}

									if (actualizarDatosAPI == true)
									{
										await BaseDatos.JuegosActualizar.Insertar.Ejecutar(juego.Id, juego.IdSteam, "SteamAPI");
									}
								}
							}

							#endregion

							#region Actualizar

							List<BaseDatos.JuegosActualizar.JuegoActualizar> fichas = await BaseDatos.JuegosActualizar.Buscar.Todos();

							if (fichas?.Count > 0)
							{
								foreach (var ficha in fichas)
								{
									if (ficha.Metodo == "SteamAPI")
									{
										Juegos.Juego nuevoJuego = await APIs.Steam.Juego.CargarDatosJuego(ficha.IdPlataforma.ToString());
										Juegos.Juego juego = await BaseDatos.Juegos.Buscar.UnJuego(ficha.IdJuego);

										if (nuevoJuego != null && juego != null)
										{
											await BaseDatos.Juegos.Actualizar.Media(nuevoJuego, juego);

											await BaseDatos.JuegosActualizar.Limpiar.Una(ficha);
										}
									}

									if (ficha.Metodo == "GOGAPI")
									{
										Juegos.Juego juego = await BaseDatos.Juegos.Buscar.UnJuego(ficha.IdJuego);

										if (juego != null)
										{
											Juegos.JuegoGalaxyGOG datosGOG = await APIs.GOG.Juego.GalaxyDatos(ficha.IdPlataforma.ToString());

											if (datosGOG != null)
											{
												juego.GalaxyGOG = datosGOG;
												juego.Idiomas = await APIs.GOG.Juego.GalaxyIdiomas(ficha.IdPlataforma.ToString(), juego.Idiomas);

												await BaseDatos.Juegos.Actualizar.GalaxyGOG(juego);
											}

											await BaseDatos.JuegosActualizar.Limpiar.Una(ficha);
										}
									}

									if (ficha.Metodo == "EpicAPI")
									{
										Juegos.Juego juego = await BaseDatos.Juegos.Buscar.UnJuego(ficha.IdJuego);

										if (juego != null)
										{
											if (string.IsNullOrEmpty(juego.SlugEpic) == false)
											{
												juego.EpicGames = await APIs.EpicGames.Juego.EpicGamesDatos(juego.SlugEpic);
												juego.Idiomas = await APIs.EpicGames.Juego.EpicGamesIdiomas(juego.SlugEpic, juego.Idiomas);

												await BaseDatos.Juegos.Actualizar.EpicGames(juego);
											}

											await BaseDatos.JuegosActualizar.Limpiar.Una(ficha);
										}
									}

									if (ficha.Metodo == "XboxAPI")
									{
										Juegos.Juego juego = await BaseDatos.Juegos.Buscar.UnJuego(ficha.IdJuego);

										if (juego != null)
										{
											if (string.IsNullOrEmpty(juego.IdXbox) == false)
											{
												juego.Xbox = await APIs.Xbox.Juego.XboxDatos(juego.IdXbox);
												juego.Idiomas = await APIs.Xbox.Juego.XboxIdiomas(juego.IdXbox, juego.Idiomas);

												await BaseDatos.Juegos.Actualizar.Xbox(juego);
											}

											await BaseDatos.JuegosActualizar.Limpiar.Una(ficha);
										}
									}
								}
							}

							#endregion
						}
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Tarea - Fichas Actualizar", ex);
					}
				}
			}
		}

		public override async Task StopAsync(CancellationToken stoppingToken)
		{
			await base.StopAsync(stoppingToken);
		}
	}
}