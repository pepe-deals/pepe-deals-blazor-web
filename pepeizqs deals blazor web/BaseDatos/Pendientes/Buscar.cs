#nullable disable

using Dapper;

namespace BaseDatos.Pendientes
{
	public static class Buscar
	{
		public static async Task<string> IDs(string nombre)
		{
			try
			{
				nombre = (nombre.EndsWith("PC", StringComparison.OrdinalIgnoreCase) ? nombre[..^2] : nombre).TrimEnd();
				nombre = (nombre.EndsWith("(PC)", StringComparison.OrdinalIgnoreCase) ? nombre[..^2] : nombre).TrimEnd();

				nombre = System.Text.RegularExpressions.Regex.Replace(
					nombre, @"\s*PC\s*[-–—]\s*DLC$", "",
					System.Text.RegularExpressions.RegexOptions.IgnoreCase
				).TrimEnd();

				string busqueda1 = "SELECT id FROM juegos WHERE nombre=@nombre OR REPLACE(nombreCodigo, ' ', '')=@nombreLimpio";

				var id = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<int?>(busqueda1, new
					{
						nombre,
						nombreLimpio = Herramientas.Buscador.LimpiarNombre(nombre, true)
					});
				});

				if (id != null)
				{
					return id.Value.ToString();
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Buscar IDs 1", ex);
			}

			try
			{
				string busqueda2 = "SELECT ids FROM juegosIDs WHERE nombre=@nombre OR REPLACE(nombreCodigo, ' ', '')=@nombreLimpio";

				var ids = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(busqueda2, new 
					{ 
						nombre,
						nombreLimpio = Herramientas.Buscador.LimpiarNombre(nombre, true)
					});
				});

				if (ids != null)
				{
					return ids;
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Buscar IDs 2", ex);
			}

			return "0";
		}

		public static async Task<int> Cantidad()
		{
			var sentencias = new List<string>();

			// Tiendas
			foreach (var tienda in Tiendas2.TiendasCargar.GenerarListado())
			{
				if (tienda.Id != APIs.Steam.Tienda.Generar().Id &&
					tienda.Id != APIs.Steam.Tienda.GenerarBundles().Id)
				{
					sentencias.Add($"SELECT COUNT(*) FROM tienda{tienda.Id} WHERE (idJuegos = '0' AND descartado = 'no') OR (idJuegos IS NULL AND descartado IS NULL)");
				}
			}

			// Suscripciones
			foreach (var suscripcion in Suscripciones2.SuscripcionesCargar.GenerarListado())
			{
				if (suscripcion.AdminPendientes == true)
				{
					sentencias.Add($"SELECT COUNT(*) FROM temporal{suscripcion.Id}");
				}
			}

			// Streaming
			foreach (var streaming in Streaming2.StreamingCargar.GenerarListado())
			{
				sentencias.Add($"SELECT COUNT(*) FROM streaming{streaming.Id} WHERE (idJuego IS NULL OR idJuego = '0') AND (descartado IS NULL OR descartado = 0)");
			}

			// Plataformas
			foreach (var plataforma in Plataformas2.PlataformasCargar.GenerarListado())
			{
				sentencias.Add($"SELECT COUNT(*) FROM temporal{plataforma.Id}juegos");
			}

			if (sentencias.Count == 0)
			{
				return 0;
			}

			string sql = string.Join(Environment.NewLine + "UNION ALL" + Environment.NewLine, sentencias)
						 + " OPTION (MAXDOP 8);";

			try
			{
				var resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<int>(sql)).ToList();
				});

				return resultados.Sum();
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Total Cantidad", ex);
			}

			return 0;
		}

		public static async Task<List<Pendiente>> Tienda(string tiendaId)
        {
			string tabla = $"tienda{tiendaId}";
			string sql = $@"SELECT enlace, nombre, imagen FROM {tabla} WHERE (idJuegos = '0' AND descartado = 'no') OR (idJuegos IS NULL AND descartado IS NULL)";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Pendiente>(sql)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Tienda " + tiendaId, ex);
			}

			return new List<Pendiente>();
		}

        public static async Task<List<Pendiente>> Suscripcion(Suscripciones2.SuscripcionTipo id)
        {
			string tabla = $"temporal{id}";
			string sql = $@"SELECT enlace, nombre, imagen FROM {tabla}";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Pendiente>(sql)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Suscripcion " + id.ToString(), ex);
			}

			return new List<Pendiente>();
		}

        public static async Task<List<Pendiente>> Streaming(Streaming2.StreamingTipo id)
        {
			string tabla = $"streaming{id}";
			string sql = $@"SELECT nombreCodigo as Enlace, nombre FROM {tabla} WHERE (idJuego IS NULL OR idJuego = '0') AND (descartado IS NULL OR descartado = 0)";

			if (id == Streaming2.StreamingTipo.Boosteroid)
			{
				sql = $@"SELECT id as Enlace, nombre FROM {tabla} WHERE (idJuego IS NULL OR idJuego = '0') AND (descartado IS NULL OR descartado = 0)";
			}

			try
			{
				var filas = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Pendiente>(sql)).ToList();
				});

				List<Pendiente> lista = new List<Pendiente>();

				foreach (var fila in filas)
				{
					lista.Add(new Pendiente
					{
						Enlace = fila.Enlace?.ToString(),
						Nombre = fila.Nombre
					});
				}

				return lista;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Streaming " + id.ToString(), ex);
			}

			return new List<Pendiente>();			
		}

        public static async Task<List<Pendiente>> Plataforma(Plataformas2.PlataformaTipo id)
        {
			string tabla = $"temporal{id}juegos";
			string sql = $@"SELECT id as Enlace, Nombre FROM {tabla}";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Pendiente>(sql)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Plataforma " + id.ToString(), ex);
			}

			return new List<Pendiente>();
		}

        public static async Task<Pendiente> PrimerJuegoTienda(string tiendaId)
		{
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Pendiente>($"SELECT TOP 1 * FROM tienda{tiendaId} WHERE (idJuegos='0' AND descartado='no') OR (idJuegos IS NULL AND descartado IS NULL)");
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Tienda Primer Juego " + tiendaId, ex);
			}

			return null;
		}

        public static async Task<Pendiente> PrimerJuegoSuscripcion(string suscripcionId)
        {
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Pendiente>($"SELECT TOP 1 * FROM temporal{suscripcionId}");
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Suscripcion Primer Juego " + suscripcionId, ex);
			}

			return null;
		}

        public static async Task<Pendiente> PrimerJuegoStreaming(string streamingId)
        {
			string sql = $"SELECT TOP 1 nombreCodigo as Enlace, nombre FROM streaming{streamingId} WHERE idJuego IS NULL AND descartado IS NULL";

			if (streamingId == Streaming2.StreamingTipo.Boosteroid.ToString())
			{
				sql = $@"SELECT TOP 1 id as Enlace, nombre FROM streaming{streamingId} WHERE idJuego IS NULL AND descartado IS NULL";
			}

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Pendiente>(sql);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Streaming Primer Juego " + streamingId, ex);
			}

			return null;
		}

		public static async Task<Pendiente> PrimerJuegoPlataforma(string plataformaId)
		{
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Pendiente>($"SELECT TOP 1 * FROM temporal{plataformaId}juegos");
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Plataforma Primer Juego " + plataformaId, ex);
			}

			return null;
		}
	}

	public class Pendiente
	{
		public string Enlace { get; set; }
		public string Nombre { get; set; }
		public string Imagen { get; set; }
	}

	public class PendientesTienda
	{
		public List<Pendiente> Pendientes { get; set; }
		public Tiendas2.Tienda Tienda { get; set; }
	}

	public class PendientesSuscripcion
	{
		public List<Pendiente> Pendientes { get; set; }
		public Suscripciones2.Suscripcion Suscripcion { get; set; }
	}

	public class PendientesStreaming
    {
        public List<Pendiente> Pendientes { get; set; }
		public Streaming2.Streaming Streaming { get; set; }
	}

	public class PendientesPlataforma
	{
		public List<Pendiente> Pendientes { get; set; }
		public Plataformas2.Plataforma Plataforma { get; set; }
	}
}
