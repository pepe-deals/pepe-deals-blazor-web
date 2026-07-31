#nullable disable

using Microsoft.Data.SqlClient;
using System.Data;

namespace Herramientas
{
	public static class BaseDatos
	{
		public static string cadenaConexion;

		public static SqlConnection Conectar(bool usarEstado = true)
		{
            SqlConnection conexion = new SqlConnection(cadenaConexion);
			
            ConnectionState estado = conexion.State;
            
			if (usarEstado == true && estado != ConnectionState.Open)
			{
				try
				{
					conexion.Open();
				}
				catch (SqlException ex) when (EsErrorDeTransporte(ex))
				{
					SqlConnection.ClearPool(conexion);
					conexion.Open();
				}
			}

			return conexion;	
        }

		private static bool EsErrorDeTransporte(SqlException ex)
		{
			int[] errores =
			{
				-1,      // General network error
				18,      // Connection has been closed by peer
				19,      // Physical connection is not usable
				1205,	 // Deadlock
				10053,   // Connection aborted
				10054,   // Connection reset by peer
				10060    // Timeout
			};

			return ex.Errors.Cast<SqlError>().Any(e => errores.Contains(e.Number));
		}

		public static async Task<T> RestoOperaciones<T>(Func<SqlConnection, SqlTransaction, Task<T>> accion, SqlConnection conexion = null)
		{
			bool cerrar = conexion == null;

			if (conexion == null)
			{
				conexion = Conectar();
			}

			if (conexion.State != ConnectionState.Open)
			{
				await conexion.OpenAsync();
			}

			await using var transaccion = conexion.BeginTransaction();

			try
			{
				var resultado = await accion(conexion, transaccion);
				transaccion.Commit();
				return resultado;
			}
			catch
			{
				try { transaccion.Rollback(); } catch { }
				throw;
			}
			finally
			{
				transaccion?.Dispose();

				if (cerrar == true)
				{
					await conexion.CloseAsync();
				}	
			}
		}

		public static async Task<T> Select<T>(Func<SqlConnection, Task<T>> accion, SqlConnection conexion = null)
		{
			bool cerrar = conexion == null;

			if (conexion == null)
			{
				conexion = Conectar();
			}

			if (conexion.State != ConnectionState.Open)
			{
				await conexion.OpenAsync();
			}

			try
			{
				return await accion(conexion);
			}
			finally
			{
				if (cerrar == true)
				{
					await conexion.CloseAsync();
				}
			}
		}

		public static T Select<T>(Func<SqlConnection, T> accion, SqlConnection conexion = null)
		{
			bool cerrar = conexion == null;

			if (conexion == null)
			{
				conexion = Conectar();
			}

			try
			{
				return accion(conexion);
			}
			finally
			{
				if (cerrar == true)
				{
					conexion.Close();
				}
			}
		}
	}
}
