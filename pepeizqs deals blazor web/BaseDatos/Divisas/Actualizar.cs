#nullable disable

using Dapper;
using Herramientas;

namespace BaseDatos.Divisas
{
	public static class Actualizar
	{
		public static async Task Ejecutar(Divisa divisa)
		{
			string sqlActualizar = "UPDATE divisas " +
                    "SET id=@id, cantidad=@cantidad, fecha=@fecha WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						id = divisa.Id,
						cantidad = divisa.Cantidad,
						fecha = divisa.Fecha
					}, transaction: sentencia);
				});
			}
			catch (Exception ex) 
			{
				BaseDatos.Errores.Insertar.Mensaje("Divisa Actualizar " + divisa.Id, ex);
			}
		}
	}
}
