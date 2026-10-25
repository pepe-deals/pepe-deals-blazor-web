#nullable disable

using Dapper;
using Herramientas;

namespace Tareas.Mantenimiento
{
	public class Diario : BackgroundService
	{
		private readonly ILogger<Diario> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public Diario(ILogger<Diario> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
			_configuracion = configuracion;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			string piscinaWeb = _configuracion.GetValue<string>("PoolWeb:Contenido");
			string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				if (piscinaWeb == piscinaUsada)
				{
					try
					{
						TimeSpan tiempoSiguiente = TimeSpan.FromHours(48);

						if (DateTime.Now.Hour == 4)
						{
							tiempoSiguiente = TimeSpan.FromMinutes(40);
						}

						if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("mantenimiento", tiempoSiguiente) == true)
						{
							await BaseDatos.Admin.Actualizar.TareaUso("mantenimiento", DateTime.Now);

							#region Limpiar Imagenes

							string directorio = Path.Combine(AppContext.BaseDirectory, "imagenes", "noticias");

							if (Directory.Exists(directorio) == true)
							{
								var ficheros = Directory.GetFiles(directorio, "*.jpeg");
								DateTime limiteFecha = DateTime.Now.AddDays(-30);

								foreach (var fichero in ficheros)
								{
									var ficheroInfo = new FileInfo(fichero);

									if (ficheroInfo.LastWriteTime < limiteFecha)
									{
										try
										{
											File.Delete(fichero);
										}
										catch (Exception ex)
										{
											BaseDatos.Errores.Insertar.Mensaje($"Error al eliminar {ficheroInfo.Name}", ex);
										}
									}
								}
							}

							#endregion

							await Herramientas.BaseDatos.Select(async (conexion) =>
							{
								return await conexion.ExecuteAsync("TRUNCATE TABLE seccionMinimos");
							});

							await Herramientas.BaseDatos.Select(async (conexion) =>
							{
								return await conexion.ExecuteAsync("TRUNCATE TABLE seccionMinimosUS");
							});

							await Herramientas.BaseDatos.Select(async (conexion) =>
							{
								return await conexion.ExecuteAsync("TRUNCATE TABLE seccionMinimosNoOficialesEU");
							});

							await Herramientas.BaseDatos.Select(async (conexion) =>
							{
								return await conexion.ExecuteAsync("TRUNCATE TABLE seccionMinimosNoOficialesUS");
							});

							await Herramientas.BaseDatos.Select(async (conexion) =>
							{
								return await conexion.ExecuteAsync("TRUNCATE TABLE seccionMinimosMarketplacesEU");
							});

							await Herramientas.BaseDatos.Select(async (conexion) =>
							{
								return await conexion.ExecuteAsync("TRUNCATE TABLE seccionMinimosMarketplacesUS");
							});

							await BaseDatos.Reseñas.Limpiar.Ejecutar();
							await BaseDatos.Juegos.Limpiar.Minimos();
							await BaseDatos.Portapapeles.Borrar.Limpieza();
							await Divisas.ActualizarDatos2();

							await BaseDatos.Mantenimiento.Indices.Ejecutar(_configuracion);
						}
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Tarea - Mantenimiento", ex);
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