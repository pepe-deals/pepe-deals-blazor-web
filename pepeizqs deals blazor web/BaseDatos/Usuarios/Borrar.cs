#nullable disable

using Dapper;

namespace BaseDatos.Usuarios
{
	public static class Borrar
	{
		public static async Task NotificacionesPush(string usuarioId)
		{
			try
			{
				string borrar = "DELETE FROM usuariosNotificaciones WHERE usuarioId=@usuarioId";

				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(borrar, new { usuarioId }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Borrar Notificaciones Push", ex);
			}
		}
	}
}
