#nullable disable

using Dapper;

namespace BaseDatos.Mantenimiento
{
	public static class Indices
	{
		public static async Task Ejecutar(IConfiguration configuracion)
		{
			string sqlMantenimiento = "EXEC dbo.MantenimientoIndices;";

			try
			{
				await Herramientas.Conexiones.Pepedeals.Select(async (conexion) =>
				{
					return await conexion.ExecuteAsync(sqlMantenimiento, commandTimeout: 0);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje2("Indices Base Datos", ex, true, sqlMantenimiento);
			}
		}
	}
}
