#nullable disable

using Dapper;
using Herramientas;
using Juegos;
using System.Data;
using System.Text.Json;
using Tiendas2;

namespace Tareas.Minimos
{
	public class MarketplacesEuropa : BackgroundService
	{
		private readonly ILogger<MarketplacesEuropa> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;
		private readonly SemaphoreSlim _semaforo = new SemaphoreSlim(1, 1);

		public MarketplacesEuropa(ILogger<MarketplacesEuropa> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
			_configuracion = configuracion;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				if (await _semaforo.WaitAsync(0) == false)
				{
					continue;
				}

				try
				{
					string piscinaMinimos = _configuracion.GetValue<string>("PoolMinimos:Contenido");
					string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

					if (piscinaMinimos == piscinaUsada && await BaseDatos.Admin.Buscar.TareaPosibleUsar("mantenimiento", TimeSpan.FromMinutes(30)) == true)
					{
						await BaseDatos.Portada.Limpiar.Total(TiendaTipo.Marketplace, TiendaRegion.Europa);

						foreach (var tienda in TiendasCargar.GenerarListado())
						{
							List<Juego> juegosParaInsertar = new List<Juego>();

							List<JuegoMinimoTarea> juegos = await BaseDatos.Portada.Buscar.BuscarMinimos(TiendaTipo.Marketplace, TiendaRegion.Europa, tienda.Id);

							if (juegos == null)
							{
								continue;
							}

							foreach (var juego in juegos)
							{
								juego.IdMaestra = juego.Id;
								juego.PreciosHistoricosMarketplacesEU = juego.PreciosHistoricosMarketplacesEU.Where(x => x.DRM == juego.DRMElegido).ToList();
								juego.PrecioMinimosHistoricos = juego.PrecioMinimosHistoricos.Where(x => x.DRM == juego.DRMElegido).ToList();

								if ((juego.PreciosHistoricosMarketplacesEU?.Count > 0 && (juego.PreciosHistoricosMarketplacesEU[0].Precio > 0 || juego.PreciosHistoricosMarketplacesEU[0].PrecioCambiado > 0)) &&
									(juego.PrecioMinimosHistoricos?.Count > 0 && (juego.PrecioMinimosHistoricos[0].Precio > 0 || juego.PrecioMinimosHistoricos[0].PrecioCambiado > 0)))
								{
									decimal precioOficial = 0;
									decimal precioNoOficial = 0;

									if (juego.PrecioMinimosHistoricos[0].PrecioCambiado > 0)
									{
										precioOficial = juego.PrecioMinimosHistoricos[0].PrecioCambiado;
									}
									else if (juego.PrecioMinimosHistoricos[0].Precio > 0)
									{
										precioOficial = juego.PrecioMinimosHistoricos[0].Precio;
									}

									if (juego.PreciosHistoricosMarketplacesEU[0].PrecioCambiado > 0)
									{
										precioNoOficial = juego.PreciosHistoricosMarketplacesEU[0].PrecioCambiado;
									}
									else if (juego.PreciosHistoricosMarketplacesEU[0].Precio > 0)
									{
										precioNoOficial = juego.PreciosHistoricosMarketplacesEU[0].Precio;
									}

									if (precioNoOficial < precioOficial)
									{
										juegosParaInsertar.Add(juego);
									}
								}
							}

							int remesaTamaño = 500;
							for (int i = 0; i < juegosParaInsertar.Count; i += remesaTamaño)
							{
								var chunk = juegosParaInsertar.Skip(i).Take(remesaTamaño).ToList();
								DataTable tabla = new DataTable();
								tabla.Columns.Add("preciosHistoricosMarketplacesEU", typeof(string));
								tabla.Columns.Add("idMaestra", typeof(long));

								foreach (var juego in chunk)
								{
									tabla.Rows.Add(
										JsonSerializer.Serialize(juego.PreciosHistoricosMarketplacesEU),
										juego.IdMaestra
									);
								}

								DynamicParameters parametros = new DynamicParameters();
								parametros.Add("@Datos", tabla.AsTableValuedParameter("dbo.SeccionMinimosMarketplacesEUType"));

								await Herramientas.BaseDatos.Select(async conexion =>
								{
									return await conexion.ExecuteAsync(
										"dbo.UpsertSeccionMinimosMarketplacesEUBatch",
										parametros,
										commandType: CommandType.StoredProcedure,
										commandTimeout: 600
									);
								});
							}
						}
					}
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Tarea - Minimos Marketplaces Europa", ex, false);
				}
				finally
				{
					_semaforo.Release();
				}
			}
		}

		public override async Task StopAsync(CancellationToken stoppingToken)
		{
			await base.StopAsync(stoppingToken);
		}
	}
}
