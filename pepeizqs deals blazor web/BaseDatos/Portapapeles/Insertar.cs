#nullable disable

using Dapper;

namespace BaseDatos.Portapapeles
{
	public static class Insertar
	{
		public static async Task Ejecutar(string id, string contenido)
		{
			if (await Buscar.YaExiste(id) == false)
			{
				string insertar = "INSERT INTO portapapeles (id, contenido) VALUES (@id, @contenido)";

				try
				{
					await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(insertar, new { id, contenido }, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Portapapeles Insertar", ex);
				}
			}
			else
			{
				string actualizar = "UPDATE portapapeles SET contenido=@contenido WHERE id=@id";

				try
				{
					await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(actualizar, new { id, contenido }, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Portapapeles Actualizar", ex);
				}
			}
		}
	}
}
