#nullable disable

using Dapper;
using Herramientas;

namespace BaseDatos.Divisas
{
	public static class Insertar
	{
		public static async Task Ejecutar(Divisa divisa)
		{
			string sqlAñadir = "INSERT INTO divisas " +
                     "(id, cantidad, fecha) VALUES " +
                     "(@id, @cantidad, @fecha) ";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlAñadir, new
					{
						id = divisa.Id,
						cantidad = divisa.Cantidad,
						fecha = divisa.Fecha
					}, transaction: sentencia);
				});
			}
			catch (Exception ex) 
			{
				BaseDatos.Errores.Insertar.Mensaje("Divisas Insertar " + divisa.Id, ex);
			}
		}
	}
}
