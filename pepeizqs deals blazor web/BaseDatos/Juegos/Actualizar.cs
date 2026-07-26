#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BaseDatos.Juegos
{
	public static class Actualizar
	{
		public static async Task Imagenes(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET imagenes=@imagenes WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						imagenes = JsonSerializer.Serialize(juego.Imagenes)
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Imagenes Actualizar " + juego.Id.ToString(), ex);
			}
		}

		public static async Task PreciosActuales(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET precioActualesTiendas=@precioActualesTiendas WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						precioActualesTiendas = JsonSerializer.Serialize(juego.PrecioActualesTiendas)
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Precios Actuales " + juego.Id.ToString(), ex);
			}
		}

		public static async Task PreciosHistoricos(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET precioMinimosHistoricos=@precioMinimosHistoricos WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						precioMinimosHistoricos = JsonSerializer.Serialize(juego.PrecioMinimosHistoricos)
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Precios Historicos " + juego.Id.ToString(), ex);
			}
		}

        public static async Task Bundles(Juego juego)
		{
			var valorBundles = juego.Bundles != null ? JsonSerializer.Serialize(juego.Bundles) : "null";

			string sqlActualizar = "UPDATE juegos " +
					"SET bundles=@bundles WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						bundles = valorBundles
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles " + juego.Id.ToString(), ex);
			}
		}

		public static async Task Gratis(Juego juego)
		{
			var valorGratis = juego.Gratis != null ? JsonSerializer.Serialize(juego.Gratis) : "null";

			string sqlActualizar = "UPDATE juegos " +
					"SET gratis=@gratis WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						gratis = valorGratis
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis 1 " + juego.Id.ToString(), ex);
			}

			string sqlActualizar2 = "UPDATE seccionMinimos " +
					"SET gratis=@gratis WHERE idMaestra=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar2, new
					{
						id = juego.Id,
						gratis = valorGratis
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis 2 " + juego.Id.ToString(), ex);
			}
		}

		public static async Task Suscripciones(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET suscripciones=@suscripciones WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						suscripciones = JsonSerializer.Serialize(juego.Suscripciones)
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones 1 " + juego.Id.ToString(), ex);
			}

			string sqlActualizar2 = "UPDATE seccionMinimos " +
					"SET suscripciones=@suscripciones WHERE idMaestra=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar2, new
					{
						id = juego.Id,
						suscripciones = JsonSerializer.Serialize(juego.Suscripciones)
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones 2 " + juego.Id.ToString(), ex);
			}
		}

		public static async Task DlcMaestro(Juego juego)
		{
            string sqlActualizar = "UPDATE juegos " +
					"SET maestro=@maestro WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						maestro = juego.Maestro
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Maestro " + juego.Id.ToString(), ex);
			}
		}

		public static async Task FreeToPlay(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET freeToPlay=@freeToPlay WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						freeToPlay = juego.FreeToPlay
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("F2P " + juego.Id.ToString(), ex);
			}
		}

		public static async Task MayorEdad(Juego juego)
		{
			string sqlActualizar = @"
				UPDATE juegos
				SET mayorEdad=@mayorEdad
				WHERE id = @Id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						mayorEdad = juego.MayorEdad
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Mayor Edad 1 " + juego.Id.ToString(), ex);
			}
		}

		public static async Task OcultarPortada(Juego juego)
		{
			string sqlActualizar = @"
				UPDATE juegos
				SET ocultarPortada=@ocultarPortada
				WHERE id = @Id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						ocultarPortada = juego.OcultarPortada
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Ocultar Portada 1 " + juego.Id.ToString(), ex);
			}
		}

		public static async Task Media(Juego nuevoJuego, Juego juego)
		{
			if (nuevoJuego == null || juego == null)
			{
				return;
			}

			if (juego.Imagenes == null)
			{ 
				juego.Imagenes = new JuegoImagenes();
			}

			juego.Imagenes.Library_600x900 = ProcesarImagen(juego.Imagenes.Library_600x900,
						   nuevoJuego.Imagenes.Library_600x900,
						   juego.IdSteam,
						   "library_capsule.");

			juego.Imagenes.Library_1920x620 = ProcesarImagen(juego.Imagenes.Library_1920x620,
						   nuevoJuego.Imagenes.Library_1920x620,
						   juego.IdSteam,
						   "library_hero.");

			juego.Imagenes.Logo = ProcesarImagen(juego.Imagenes.Logo,
						   nuevoJuego.Imagenes.Logo,
						   juego.IdSteam,
						   "logo.");

			juego.Imagenes.Capsule_231x87 = nuevoJuego.Imagenes.Capsule_231x87;
			juego.Imagenes.Header_460x215 = nuevoJuego.Imagenes.Header_460x215;

			juego.Nombre = nuevoJuego.Nombre;
			juego.Caracteristicas = nuevoJuego.Caracteristicas ?? new JuegoCaracteristicas();
			juego.Deck = nuevoJuego.Deck;
			juego.SteamOS = nuevoJuego.SteamOS;
			juego.SteamMachine = nuevoJuego.SteamMachine;
			juego.SteamFrame = nuevoJuego.SteamFrame;
			juego.Media = nuevoJuego.Media;
			juego.Categorias = nuevoJuego.Categorias;
			juego.Etiquetas = nuevoJuego.Etiquetas;
			juego.Delistado = nuevoJuego.Delistado;

			if (juego.Idiomas == null)
			{
				juego.Idiomas = new List<JuegoIdioma>();
			}

			if (nuevoJuego.Idiomas != null)
			{
				foreach (var nuevo in nuevoJuego.Idiomas)
				{
					var existente = juego.Idiomas.FirstOrDefault(x => x.DRM == nuevo.DRM && x.Idioma == nuevo.Idioma);

					if (existente != null)
					{
						existente.Audio = nuevo.Audio;
						existente.Texto = nuevo.Texto;
					}
					else
					{
						juego.Idiomas.Add(nuevo);
					}
				}
			}

			string columnasAnalisis = "";
			object paramReseñas = null;

			if (string.IsNullOrEmpty(nuevoJuego.Analisis?.Porcentaje) == false &&
				string.IsNullOrEmpty(nuevoJuego.Analisis?.Cantidad) == false)
			{
				columnasAnalisis = ", analisis=@analisis";
				paramReseñas = JsonSerializer.Serialize(nuevoJuego.Analisis);
			}

			string sql = $@"
				UPDATE juegos SET
					nombre=@nombre,
					imagenes=@imagenes,
					caracteristicas=@caracteristicas,
					media=@media,
					nombreCodigo=@nombreCodigo,
					categorias=@categorias,
					etiquetas=@etiquetas,
					fechaSteamAPIComprobacion=@fechaSteamAPIComprobacion,
					idiomas=@idiomas,
					deck=@deck,
					steamOS=@steamOS,
					steamMachine=@steamMachine,
					steamFrame=@steamFrame,
					delistado=@delistado
					{columnasAnalisis}
				WHERE id=@id;";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sql, new
					{
						id = juego.Id,
						nombre = juego.Nombre,
						imagenes = JsonSerializer.Serialize(juego.Imagenes),
						caracteristicas = JsonSerializer.Serialize(juego.Caracteristicas),
						media = JsonSerializer.Serialize(juego.Media),
						nombreCodigo = Herramientas.Buscador.LimpiarNombre(juego.Nombre),
						categorias = JsonSerializer.Serialize(juego.Categorias),
						etiquetas = JsonSerializer.Serialize(juego.Etiquetas),
						fechaSteamAPIComprobacion = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"),
						idiomas = JsonSerializer.Serialize(juego.Idiomas),
						deck = juego.Deck,
						steamOS = juego.SteamOS,
						steamMachine = juego.SteamMachine,
						steamFrame = juego.SteamFrame,
						delistado = juego.Delistado,
						analisis = paramReseñas
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Steam API " + juego.Id.ToString(), ex);
			}
		}

		private static string ProcesarImagen(string vieja, string nueva, long idSteam, string patron)
		{
			if (string.IsNullOrEmpty(vieja) == false)
			{
				bool cambiar = true;

				if (vieja.Contains("i.imgur.com") == true)
				{
					cambiar = false;
				}
				else if (!Regex.IsMatch(vieja, $"{Regex.Escape(patron)}.*?/{idSteam}/"))
				{
					cambiar = false;
				}

				if (cambiar == true)
				{ 
					vieja = nueva; 
				}
			}
			else
			{
				vieja = nueva;
			}

			return vieja;
		}

		public static async Task Tipo(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET tipo=@tipo WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						tipo = juego.Tipo
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Tipo " + juego.Id.ToString(), ex);
			}
		}

		public static async Task IdSteam(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET idSteam=@idSteam WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						idSteam = juego.IdSteam
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Id Steam " + juego.Id.ToString(), ex);
			}
		}

		public static async Task IdGOG(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET idGOG=@idGOG WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						idGOG = juego.IdGog
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Id GOG " + juego.Id.ToString(), ex);
			}
		}

		public static async Task SlugGOG(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET slugGOG=@slugGOG WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						slugGOG = juego.SlugGOG
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Slug GOG " + juego.Id.ToString(), ex);
			}
		}

		public static async Task SlugEpic(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET slugEpic=@slugEpic WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						slugEpic = juego.SlugEpic
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Slug Epic " + juego.Id.ToString(), ex);
			}
		}

		public static async Task ExeEpic(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET exeEpic=@exeEpic WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						exeEpic = juego.ExeEpic
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Exe Epic " + juego.Id.ToString(), ex);
			}
		}

		public static async Task IdXbox(int idJuego, string idXbox)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET idXbox=@idXbox WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = idJuego,
						exeUbisoft = idXbox
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Id Xbox " + idJuego.ToString(), ex);
			}
		}

		public static async Task ExeUbisoft(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET exeUbisoft=@exeUbisoft WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						exeUbisoft = juego.ExeUbisoft
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Exe Ubisoft " + juego.Id.ToString(), ex);
			}
		}

		public static async Task Deck(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET deck=@deck, deckTokens=@deckTokens, deckComprobacion=@deckComprobacion, steamOS=@steamOS, steamOSTokens=@steamOSTokens, steamMachine=@steamMachine, steamMachineTokens=@steamMachineTokens, steamFrame=@steamFrame, steamFrameTokens=@steamFrameTokens WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = juego.Id,
						deck = juego.Deck,
						deckTokens = JsonSerializer.Serialize(juego.DeckTokens),
						deckComprobacion = juego.DeckComprobacion,
						steamOS = juego.SteamOS,
						steamOSTokens = JsonSerializer.Serialize(juego.SteamOSTokens),
						steamMachine = juego.SteamMachine,
						steamMachineTokens = JsonSerializer.Serialize(juego.SteamMachineTokens),
						steamFrame = juego.SteamFrame,
						steamFrameTokens = JsonSerializer.Serialize(juego.SteamFrameTokens)
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Deck " + juego.Id.ToString(), ex);
			}
		}

		public static async Task DeckVacio(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET deckComprobacion=@deckComprobacion WHERE id=@id";

			var parametros = new
			{
				id = juego.Id,
				deckComprobacion = juego.DeckComprobacion
			};

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Deck Comprobacion " + juego.Id.ToString(), ex);
			}
		}

		public static async Task HistoricosEU(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET historicos=@historicos WHERE id=@id";

			var parametros = new
			{
				id = juego.Id,
				historicos = JsonSerializer.Serialize(juego.Historicos)
			};

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Historicos EU " + juego.Id.ToString(), ex);
			}
		}

		public static async Task HistoricosUS(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET historicosUS=@historicosUS WHERE id=@id";

			var parametros = new
			{
				id = juego.Id,
				historicosUS = JsonSerializer.Serialize(juego.HistoricosUS)
			};

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Historicos US " + juego.Id.ToString(), ex);
			}
		}

		public static async Task GalaxyGOG(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET galaxyGOG=@galaxyGOG, idiomas=@idiomas WHERE id=@id";

			var parametros = new
			{
				id = juego.Id,
				galaxyGOG = JsonSerializer.Serialize(juego.GalaxyGOG),
				idiomas = JsonSerializer.Serialize(juego.Idiomas)
			};

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar GOG Galaxy " + juego.Id.ToString(), ex);
			}
		}

		public static async Task EpicGames(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET epicGames=@epicGames, idiomas=@idiomas WHERE id=@id";

			var parametros = new
			{
				id = juego.Id,
				epicGames = JsonSerializer.Serialize(juego.EpicGames),
				idiomas = JsonSerializer.Serialize(juego.Idiomas)
			};

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Epic Games " + juego.Id.ToString(), ex);
			}
		}

		public static async Task Xbox(Juego juego)
		{
			string sqlActualizar = "UPDATE juegos " +
					"SET xbox=@xbox, idiomas=@idiomas WHERE id=@id";

			var parametros = new
			{
				id = juego.Id,
				xbox = JsonSerializer.Serialize(juego.Xbox),
				idiomas = JsonSerializer.Serialize(juego.Idiomas)
			};

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Xbox " + juego.Id.ToString(), ex);
			}
		}

		public static async Task CantidadJugadoresSteam(Juego juego)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("UPDATE juegos SET cantidadJugadoresSteam=@cantidadJugadoresSteam WHERE id=@id", new
					{
						id = juego.Id,
						cantidadJugadoresSteam = JsonSerializer.Serialize(juego.CantidadJugadores)
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Cantidad Jugadores " + juego.Id.ToString(), ex);
			}
		}

		public static async void UltimasActualizacioneseInteligenciaArticial(int idJuego, DateTime? fechaSteam, DateTime? fechaGOG, bool inteligenciaArtificial = false, SqlConnection conexion = null)
		{
			var sql = new StringBuilder();
			sql.Append("UPDATE juegos SET ");
			sql.Append("inteligenciaArtificial = @inteligenciaArtificial, ");
			sql.Append("ultimaActualizacion = @ultimaActualizacion ");

			if (fechaSteam != null)
			{
				sql.Append(", ultimaActualizacionSteam = @ultimaActualizacionSteam ");
			}

			if (fechaGOG != null)
			{
				sql.Append(", ultimaActualizacionGOG = @ultimaActualizacionGOG ");
			}

			sql.Append("WHERE id = @id;");

			var parametros = new
			{
				id = idJuego,
				inteligenciaArtificial,
				ultimaActualizacion = DateTime.Now,
				ultimaActualizacionSteam = fechaSteam,
				ultimaActualizacionGOG = fechaGOG
			};

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sql.ToString(), parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar SteamCMD " + idJuego.ToString(), ex);
			}
		}
	}
}
