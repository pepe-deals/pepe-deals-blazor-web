#nullable disable

using Dapper;

namespace BaseDatos.Portapapeles
{
	public static class Borrar
	{
		public static async Task Ejecutar(string id)
		{
			string borrar = "DELETE FROM portapapeles WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(borrar, new
					{
						id
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Portapapeles Delete", ex);
			}
		}

		public static async Task Limpieza()
		{
			string limpiar = "TRUNCATE TABLE portapapeles";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.QueryAsync(limpiar, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Portapapeles Truncate", ex);
			}
		}
	}
}
