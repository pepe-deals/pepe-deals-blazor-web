#nullable disable

using Dapper;
using Herramientas;

namespace Tareas
{
	public class LimpiezaLog : BackgroundService
	{
		private readonly ILogger<LimpiezaLog> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public LimpiezaLog(ILogger<LimpiezaLog> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
		{
			_logger = logger;
			_factoria = factory;
			_decompilador = decompilador;
			_configuracion = configuracion;
		}

		protected override async Task ExecuteAsync(CancellationToken tokenParar)
		{
			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				string piscinaApp = _configuracion.GetValue<string>("PoolWeb:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaApp == piscinaUsada && await BaseDatos.Admin.Buscar.TareaPosibleUsar("mantenimiento", TimeSpan.FromMinutes(30)) == true)
				{
					try
					{
						await Herramientas.BaseDatos.Select(async (conexion) =>
						{
							return await conexion.ExecuteAsync(@"DECLARE @tamañoMB DECIMAL(10,2), @usadoMB DECIMAL(10,2), @libreMB DECIMAL(10,2);

SELECT 
    @tamañoMB = CAST(size * 8.0 / 1024 AS DECIMAL(10,2)),
    @usadoMB = CAST(FILEPROPERTY(name, 'SpaceUsed') * 8.0 / 1024 AS DECIMAL(10,2))
FROM sys.database_files
WHERE type_desc = 'LOG';

SET @libreMB = @tamañoMB - @usadoMB;

IF @libreMB > 300 AND @tamañoMB > 400
BEGIN
    DBCC SHRINKFILE (pepeizq2_simply__winspace_es_1_log, 100);
END;");
						});
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Limpieza Log", ex, false);
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
