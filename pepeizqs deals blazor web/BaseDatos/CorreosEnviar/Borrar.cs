#nullable disable

using Dapper;

namespace BaseDatos.CorreosEnviar
{
	public static class Borrar
	{
		public static async Task Ejecutar(int id)
		{
			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("DELETE FROM correosEnviar WHERE id=@id", new { id }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Correos Enviar Borrar", ex);
			}
		}
	}
}
