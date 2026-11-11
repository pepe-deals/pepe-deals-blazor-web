#nullable disable

using Dapper;
using Juegos;
using Microsoft.Data.SqlClient;
using pepeizqs_deals_web.Data;
using System.Drawing;

namespace BaseDatos.Recompensas
{
	public class RecompensaJuego
	{
		public int Id;
		public int JuegoId;
		public string Clave;
		public int Coins;
		public DateTime FechaEmpieza;
		public string UsuarioId;
		public JuegoDRM DRM;
		public string JuegoNombre;
		public DateTime? FechaCaduca;
	}

	public static class Juegos
	{
		public static async Task Insertar(RecompensaJuego recompensa)
		{
			string insertar = @"
				INSERT INTO recompensasJuegos 
				(juegoId, clave, coins, fecha, juegoNombre, drm" + (recompensa.FechaCaduca != null ? ", fechaCaduca" : "") + @")
				VALUES 
				(@juegoId, @clave, @coins, @fecha, @juegoNombre, @drm" + (recompensa.FechaCaduca != null ? ", @fechaCaduca" : "") + @")";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(insertar, new
					{
						juegoId = recompensa.JuegoId.ToString(),
						clave = recompensa.Clave,
						coins = recompensa.Coins,
						fecha = recompensa.FechaEmpieza,
						juegoNombre = recompensa.JuegoNombre,
						drm = recompensa.DRM,
						fechaCaduca = recompensa.FechaCaduca
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Recompensa Insertar", ex);
			}
		}

		public static async Task<List<RecompensaJuego>> Disponibles()
		{
			try
			{
				string busqueda = "SELECT * FROM recompensasJuegos WHERE usuarioId IS NULL AND (fechaCaduca IS NULL OR fechaCaduca > GETDATE()) ORDER BY juegoNombre";

				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<RecompensaJuego>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Recompensas Disponibles", ex);
			}

			return null;
		}

		public static async Task<List<RecompensaJuego>> Aleatorios()
		{
			try
			{
				string busqueda = @"SELECT TOP 5 *
FROM recompensasJuegos
WHERE usuarioId IS NULL
  AND (fechaCaduca IS NULL OR fechaCaduca > GETDATE())
ORDER BY NEWID();";

				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<RecompensaJuego>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Recompensas Disponibles", ex);
			}

			return null;
		}

		public static async Task Actualizar(int id, string usuarioId)
		{
			try
			{
				string actualizar = "UPDATE recompensasJuegos " +
					"SET usuarioId=@usuarioId WHERE id=@id";

				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new { id, usuarioId }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Recompensas Actualizar", ex);
			}			
		}

        public static async Task<List<RecompensaJuego>> LeerJuegosUsuario(string usuarioId)
		{
			try
			{
				string busqueda = "SELECT * FROM recompensasJuegos WHERE usuarioId=@usuarioId ORDER BY juegoNombre";

				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<RecompensaJuego>(busqueda, new { usuarioId })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Recompensas Leer Juegos Usuario", ex);
			}

			return null;
        }
    }
}
