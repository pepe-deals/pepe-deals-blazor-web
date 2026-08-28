#nullable disable

using Herramientas;
using Tiendas2;

namespace Tareas
{
    public class RedesSociales : BackgroundService
    {
        private readonly ILogger<RedesSociales> _logger;
        private readonly IServiceScopeFactory _factoria;
        private readonly IDecompiladores _decompilador;
		private readonly IConfiguration _configuracion;

		public RedesSociales(ILogger<RedesSociales> logger, IServiceScopeFactory factory, IDecompiladores decompilador, IConfiguration configuracion)
        {
            _logger = logger;
            _factoria = factory;
            _decompilador = decompilador;
            _configuracion = configuracion;
        }

        protected override async Task ExecuteAsync(CancellationToken tokenParar)
        {
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

            while (await timer.WaitForNextTickAsync(tokenParar))
            {
                string piscinaApp = _configuracion.GetValue<string>("PoolTiendas:Contenido");
                string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

                if (piscinaApp == piscinaUsada)
                {
					TimeSpan tiempoSiguiente = TimeSpan.FromHours(48);

					if (DateTime.Now.Hour == 21)
					{
						tiempoSiguiente = TimeSpan.FromSeconds(60);
					}

					if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("redditEU", tiempoSiguiente) == true)
					{
						try
						{
							List<Juegos.Juego> juegosDiaSteam = await BaseDatos.RedesSociales.Buscar.OfertasDelDia(TiendaRegion.Europa, (int)Juegos.JuegoDRM.Steam);

							if (juegosDiaSteam?.Count > 3)
							{
								bool posteado = await Herramientas.RedesSociales.Reddit.PostearOfertasDia(_configuracion, TiendaRegion.Europa, juegosDiaSteam, Juegos.JuegoDRM.Steam);

								if (posteado == false)
								{
									BaseDatos.Errores.Insertar.Mensaje("Tarea - Redes Sociales EU", "No se pudo postear las ofertas del día de Steam en Europa");
								}
								else
								{
									await BaseDatos.Admin.Actualizar.TareaUso("redditEU", DateTime.Now.AddMinutes(70));
								}
							}

							List<Juegos.Juego> juegosDiaGog = await BaseDatos.RedesSociales.Buscar.OfertasDelDia(TiendaRegion.Europa, (int)Juegos.JuegoDRM.GOG);

							if (juegosDiaGog?.Count > 3)
							{
								bool posteado = await Herramientas.RedesSociales.Reddit.PostearOfertasDia(_configuracion, TiendaRegion.Europa, juegosDiaGog, Juegos.JuegoDRM.GOG);

								if (posteado == false)
								{
									BaseDatos.Errores.Insertar.Mensaje("Tarea - Redes Sociales EU", "No se pudo postear las ofertas del día de GOG en Europa");
								}
							}
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Tarea - Redes Sociales EU", ex);
						}
					}

					await Task.Delay(10000);

					if (await BaseDatos.Admin.Buscar.TareaPosibleUsar("redditUS", tiempoSiguiente) == true)
					{
						try
						{
							List<Juegos.Juego> juegosDiaSteamUS = await BaseDatos.RedesSociales.Buscar.OfertasDelDia(TiendaRegion.EstadosUnidos, (int)Juegos.JuegoDRM.Steam);

							if (juegosDiaSteamUS?.Count > 3)
							{
								bool posteado = await Herramientas.RedesSociales.Reddit.PostearOfertasDia(_configuracion, TiendaRegion.EstadosUnidos, juegosDiaSteamUS, Juegos.JuegoDRM.Steam);

								if (posteado == false)
								{
									BaseDatos.Errores.Insertar.Mensaje("Tarea - Redes Sociales US", "No se pudo postear las ofertas del día de Steam en Estados Unidos");
								}
								else
								{
									await BaseDatos.Admin.Actualizar.TareaUso("redditUS", DateTime.Now.AddMinutes(70));
								}
							}

							List<Juegos.Juego> juegosDiaGogUS = await BaseDatos.RedesSociales.Buscar.OfertasDelDia(TiendaRegion.EstadosUnidos, (int)Juegos.JuegoDRM.GOG);

							if (juegosDiaGogUS?.Count > 3)
							{
								bool posteado = await Herramientas.RedesSociales.Reddit.PostearOfertasDia(_configuracion, TiendaRegion.EstadosUnidos, juegosDiaGogUS, Juegos.JuegoDRM.GOG);

								if (posteado == false)
								{
									BaseDatos.Errores.Insertar.Mensaje("Tarea - Redes Sociales US", "No se pudo postear las ofertas del día de GOG en Estados Unidos");
								}
							}
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Tarea - Redes Sociales US", ex);
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