#nullable disable

using Dapper;

namespace BaseDatos.Juegos
{
	public static class Limpiar
	{
		public static async Task Minimos()
		{
			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("TRUNCATE TABLE seccionMinimos", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				Errores.Insertar.Mensaje("Limpiar Minimos", ex);
			}
		}
	}
}
