#nullable disable

using Dapper;

namespace BaseDatos.UsuariosActualizar
{
	public static class Insertar
	{
		public static async Task Ejecutar(string idUsuario, string metodo)
		{
			if (string.IsNullOrEmpty(idUsuario) == false && string.IsNullOrEmpty(metodo) == false)
			{
				string añadir = @"
					MERGE INTO usuariosActualizar AS target
					USING (SELECT @idUsuario AS idUsuario, @metodo AS metodo) AS source
					ON target.idUsuario = source.idUsuario
					WHEN MATCHED THEN
						UPDATE SET metodo = source.metodo
					WHEN NOT MATCHED THEN
						INSERT (idUsuario, metodo)
						VALUES (source.idUsuario, source.metodo);
						";

				try
				{
					await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(añadir, new
						{
							idUsuario,
							metodo
						}, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuarios Actualizar Insertar", ex);
				}
			}
		}
	}
}
