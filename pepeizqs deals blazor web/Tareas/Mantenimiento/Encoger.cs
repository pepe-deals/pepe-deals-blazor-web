#nullable disable

using Dapper;
using Herramientas;

namespace Tareas.Mantenimiento
{
	public class Encoger : BackgroundService
	{
		private readonly ILogger<Encoger> _logger;
		private readonly IServiceScopeFactory _factoria;
		private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public Encoger(ILogger<Encoger> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
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

			using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(20));

			while (await timer.WaitForNextTickAsync(tokenParar))
			{
				if (piscinaWeb == piscinaUsada)
				{
					TimeSpan tiempoSiguiente = TimeSpan.FromMinutes(5);

					if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("encoger", tiempoSiguiente) == true)
					{
						await BaseDatos.Admin.Actualizar.TareaUso("encoger", DateTime.Now);

						await Herramientas.Conexiones.Pepedeals.Select(async (conexion) =>
						{
							return await conexion.ExecuteAsync("EXEC dbo.EmergenciaShrinkYRebuild");
						});
					}
				}
			}
		}
	}
}
