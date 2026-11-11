#nullable disable

using Dapper;

namespace BaseDatos.UsuariosActualizar
{
	public static class Limpiar
	{
		public static async Task Una(UsuarioActualizar usuario)
		{
			if (usuario == null)
			{
				return;
			}

			string eliminar = "DELETE FROM usuariosActualizar WHERE idUsuario=@idUsuario AND metodo=@metodo";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(eliminar, new
					{
						usuario.IdUsuario,
						usuario.Metodo
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuarios Actualizar Limpiar", ex);
			}
		}
	}
}