#nullable disable

using Herramientas;
using Tiendas2;

namespace Tareas
{
	public class Comprobador : BackgroundService
	{
		private readonly ILogger<Comprobador> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public Comprobador(ILogger<Comprobador> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
			_configuracion = configuracion;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				string piscinaTiendas = _configuracion.GetValue<string>("PoolTiendas:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaTiendas == piscinaUsada && await BaseDatos.Admin.Buscar.TareaPosibleUsar("mantenimiento", TimeSpan.FromMinutes(30)) == true)
				{
					bool seEjecutoAlgo = false;

					#region Tiendas Europa

					foreach (var tienda in Tiendas2.TiendasCargar.GenerarListado())
					{
						TimeSpan siguienteComprobacion = TimeSpan.Zero;

						if (tienda.Id == APIs.Battlenet.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(4);
							}

							if (DateTime.Now.Hour == 20)
							{
								siguienteComprobacion = TimeSpan.FromMinutes(30);
							}
						}
						//else if (tienda.Id == APIs.DLGamer.Tienda.Generar().Id)
						//{
						//	siguienteComprobacion = TimeSpan.FromHours(4);

						//	if (DateTime.Now.Hour == 19)
						//	{
						//		siguienteComprobacion = TimeSpan.FromHours(5);
						//	}
						//}
						else if (tienda.Id == APIs.EA.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(4);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(5);
							}
							if (DateTime.Now.Hour == 20)
							{
								siguienteComprobacion = TimeSpan.FromMinutes(30);
							}
						}
						else if (tienda.Id == APIs.EpicGames.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(10);
							}
						}
						else if (tienda.Id == APIs.Fanatical.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(3);
							}

							if (DateTime.Now.Hour == 17)
							{
								siguienteComprobacion = TimeSpan.FromMinutes(30);
							}
						}
						else if (tienda.Id == APIs.GameBillet.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(4);
							}
						}
						else if (tienda.Id == APIs.GamersGate.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(3);
							}
						}
						else if (tienda.Id == APIs.Gamesplanet.Tienda.GenerarDe().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);

							if (DateTime.Now.Hour == 10)
							{
								siguienteComprobacion = TimeSpan.FromMinutes(30);
							}

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(3);
							}
						}
						else if (tienda.Id == APIs.Gamesplanet.Tienda.GenerarFr().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);

							if (DateTime.Now.Hour == 10)
							{
								siguienteComprobacion = TimeSpan.FromMinutes(30);
							}

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(3);
							}
						}
						else if (tienda.Id == APIs.Gamesplanet.Tienda.GenerarUk().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);

							if (DateTime.Now.Hour == 10)
							{
								siguienteComprobacion = TimeSpan.FromMinutes(30);
							}

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(3);
							}
						}
						else if (tienda.Id == APIs.Gamesplanet.Tienda.GenerarUs().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);

							if (DateTime.Now.Hour == 10)
							{
								siguienteComprobacion = TimeSpan.FromMinutes(30);
							}

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(3);
							}
						}
						else if (tienda.Id == APIs.Gamesporium.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(4);
							}
						}
						else if (tienda.Id == APIs.GOG.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(4);
							}

							if (DateTime.Now.Hour == 15)
							{
								siguienteComprobacion = TimeSpan.FromMinutes(30);
							}
						}
						else if (tienda.Id == APIs.GreenManGaming.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(3);
							}

							if (DateTime.Now.Hour == 17)
							{
								siguienteComprobacion = TimeSpan.FromMinutes(30);
							}
						}
						else if (tienda.Id == APIs.GreenManGaming.Tienda.GenerarGold().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(3);
							}

							if (DateTime.Now.Hour == 17)
							{
								siguienteComprobacion = TimeSpan.FromMinutes(30);
							}
						}
						else if (tienda.Id == APIs.Humble.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);

							if (DateTime.Now.Hour == 19 || DateTime.Now.Hour == 20)
							{
								siguienteComprobacion = TimeSpan.FromMinutes(30);
							}
						}
						else if (tienda.Id == APIs.IndieGala.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(3);
							}
						}
						else if (tienda.Id == APIs.JoyBuggy.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(4);
							}
						}
						else if (tienda.Id == APIs.Muvegames.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(4);
							}
						}
						else if (tienda.Id == APIs.Nexus.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(4);
							}
						}
						else if (tienda.Id == APIs.PlanetPlay.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(4);
							}
						}
						else if (tienda.Id == APIs.Playsum.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(3);
							}
						}
						else if (tienda.Id == APIs.Steam.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);

							if (DateTime.Now.Hour == 17 || DateTime.Now.Hour == 18)
							{
								siguienteComprobacion = TimeSpan.FromHours(8);
							}

							if (DateTime.Now.Hour == 19)
							{
								if (DateTime.Now.Minute > 3 && DateTime.Now.Minute < 30)
								{
									siguienteComprobacion = TimeSpan.FromSeconds(10);
								}
								else
								{
									siguienteComprobacion = TimeSpan.FromMinutes(5);
								}
							}

							if (DateTime.Now.Hour == 20)
							{
								if (DateTime.Now.Minute < 30)
								{
									siguienteComprobacion = TimeSpan.FromMinutes(10);
								}
								else
								{
									siguienteComprobacion = TimeSpan.FromMinutes(20);
								}
							}
						}
						else if (tienda.Id == APIs.Stove.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(4);
							}
						}
						else if (tienda.Id == APIs.Ubisoft.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(4);
							}
						}
						else if (tienda.Id == APIs.Voidu.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(4);

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(5);
							}
						}
						else if (tienda.Id == APIs.WinGameStore.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);

							if (DateTime.Now.Hour == 18)
							{
								siguienteComprobacion = TimeSpan.FromMinutes(20);
							}

							if (DateTime.Now.Hour == 19)
							{
								siguienteComprobacion = TimeSpan.FromHours(3);
							}
						}
						else if (tienda.Id == APIs.Loaded.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);
						}
						else if (tienda.Id == APIs.Yuplay.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(2);
						}

						if (siguienteComprobacion > TimeSpan.Zero)
						{
							bool sePuedeUsar = await BaseDatos.Admin.Buscar.TiendasPosibleUsar(siguienteComprobacion, tienda.Id);

							if (sePuedeUsar == true)
							{
								try
								{
									await TiendasCargar.TareasGestionador(TiendaRegion.Europa, tienda.Id);

									seEjecutoAlgo = true;
								}
								catch (Exception ex)
								{
									BaseDatos.Errores.Insertar.Mensaje("Comprobador: " + tienda.Id, ex);
								}
							}
						}
					}

					#endregion

					#region Suscripciones

					foreach (var suscripcion in Suscripciones2.SuscripcionesCargar.GenerarListado())
					{
						TimeSpan siguienteComprobacion = TimeSpan.Zero;

						if (suscripcion.Id == APIs.AmazonLuna.Suscripcion.GenerarPremium().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(4);
						}
						else if (suscripcion.Id == APIs.GTAPlus.Suscripcion.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(4);
						}
						else if (suscripcion.Id == APIs.IndiePass.Suscripcion.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(4);
						}
						else if (suscripcion.Id == APIs.PlayStation.Suscripcion.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(4);
						}
						else if (suscripcion.Id == APIs.Ubisoft.Suscripcion.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(4);
						}
						else if (suscripcion.Id == APIs.Ubisoft.Suscripcion.GenerarPremium().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(4);
						}
						else if (suscripcion.Id == APIs.Xbox.Suscripcion.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(3);
						}

						if (siguienteComprobacion > TimeSpan.Zero)
						{
							bool sePuedeUsar = await BaseDatos.Admin.Buscar.TiendasPosibleUsar(siguienteComprobacion, suscripcion.Id.ToString());
							
							if (sePuedeUsar == true)
							{
								try
								{
									if (suscripcion.Id == APIs.AmazonLuna.Suscripcion.GenerarPremium().Id)
									{
										await APIs.AmazonLuna.Suscripcion.Buscar();
									}
									else if (suscripcion.Id == APIs.GTAPlus.Suscripcion.Generar().Id)
									{
										await APIs.GTAPlus.Suscripcion.Buscar();
									}
									else if (suscripcion.Id == APIs.IndiePass.Suscripcion.Generar().Id)
									{
										await APIs.IndiePass.Suscripcion.Buscar();
									}
									else if (suscripcion.Id == APIs.PlayStation.Suscripcion.Generar().Id)
									{
										await APIs.PlayStation.Suscripcion.Buscar();
									}
									else if (suscripcion.Id == APIs.Ubisoft.Suscripcion.Generar().Id)
									{
										await APIs.Ubisoft.Suscripcion.Buscar();
									}
									else if (suscripcion.Id == APIs.Ubisoft.Suscripcion.GenerarPremium().Id)
									{
										await APIs.Ubisoft.Suscripcion.BuscarPremium();
									}
									else if (suscripcion.Id == APIs.Xbox.Suscripcion.Generar().Id)
									{
										await APIs.Xbox.Suscripcion.Buscar();
									}

									seEjecutoAlgo = true;
								}
								catch (Exception ex)
								{
									BaseDatos.Errores.Insertar.Mensaje("Comprobador: " + suscripcion.Id.ToString(), ex);
								}
							}
						}
					}

					#endregion

					#region Streaming

					foreach (var streaming in Streaming2.StreamingCargar.GenerarListado())
					{
						TimeSpan siguienteComprobacion = TimeSpan.Zero;

						if (streaming.Id == APIs.GOG.Streaming.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(5);
						}
						else if (streaming.Id == APIs.Boosteroid.Streaming.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(4);
						}
						else if (streaming.Id == APIs.GeforceNOW.Streaming.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(5);
						}

						if (siguienteComprobacion > TimeSpan.Zero)
						{
							bool sePuedeUsar = await BaseDatos.Admin.Buscar.TiendasPosibleUsar(siguienteComprobacion, streaming.Id.ToString());

							if (sePuedeUsar == true)
							{
								try
								{
									if (streaming.Id == APIs.GOG.Streaming.Generar().Id)
									{
										await APIs.GOG.Streaming.Buscar();
									}
									else if (streaming.Id == APIs.Boosteroid.Streaming.Generar().Id)
									{
										await APIs.Boosteroid.Streaming.Buscar();
									}
									else if (streaming.Id == APIs.GeforceNOW.Streaming.Generar().Id)
									{
										await APIs.GeforceNOW.Streaming.Buscar();
									}

									seEjecutoAlgo = true;
								}
								catch (Exception ex)
								{
									BaseDatos.Errores.Insertar.Mensaje("Comprobador: " + streaming.Id.ToString(), ex);
								}
							}
						}
					}

					#endregion

					#region Tiendas Estados Unidos

					foreach (var tienda in Tiendas2.TiendasCargar.GenerarListado())
					{
						TimeSpan siguienteComprobacion = TimeSpan.Zero;

						if (tienda.Id == APIs.Steam.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(5);
						}
						else if (tienda.Id == APIs.Fanatical.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(5);
						}
						else if (tienda.Id == APIs.GamersGate.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(5);
						}
						else if (tienda.Id == APIs.Gamesplanet.Tienda.GenerarUk().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(5);
						}
						else if (tienda.Id == APIs.Gamesplanet.Tienda.GenerarFr().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(5);
						}
						else if (tienda.Id == APIs.Gamesplanet.Tienda.GenerarDe().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(5);
						}
						else if (tienda.Id == APIs.Gamesplanet.Tienda.GenerarUs().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(5);
						}
						else if (tienda.Id == APIs.GreenManGaming.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(5);
						}
						else if (tienda.Id == APIs.GreenManGaming.Tienda.GenerarGold().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.GOG.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.IndieGala.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.WinGameStore.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.GameBillet.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.Playsum.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.PlanetPlay.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						//else if (tienda.Id == APIs.DLGamer.Tienda.Generar().Id)
						//{
						//	siguienteComprobacion = TimeSpan.FromHours(6);
						//}
						else if (tienda.Id == APIs.JoyBuggy.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.Battlenet.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.Nexus.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}
						else if (tienda.Id == APIs.Loaded.Tienda.Generar().Id)
						{
							siguienteComprobacion = TimeSpan.FromHours(6);
						}

						if (DateTime.Now.Hour == 18 || DateTime.Now.Hour == 19)
						{
							siguienteComprobacion = TimeSpan.Zero;
						}

						if (siguienteComprobacion > TimeSpan.Zero)
						{
							bool sePuedeUsar = await BaseDatos.Admin.Buscar.TiendasPosibleUsarUS(siguienteComprobacion, tienda.Id);

							if (sePuedeUsar == true)
							{
								try
								{
									await Tiendas2.TiendasCargar.TareasGestionador(TiendaRegion.EstadosUnidos, tienda.Id);

									seEjecutoAlgo = true;
								}
								catch (Exception ex)
								{
									BaseDatos.Errores.Insertar.Mensaje("Comprobador: " + tienda.Id, ex);
								}
							}
						}
					}

					#endregion

					if (seEjecutoAlgo)
					{
						var tareasEnUso = await BaseDatos.Admin.Buscar.TiendasEnUso(TimeSpan.FromSeconds(60));

						if (tareasEnUso.Count == 0)
						{
							Environment.Exit(1);
						}
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