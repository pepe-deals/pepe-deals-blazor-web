#nullable disable

using Dapper;

namespace BaseDatos.Pendientes
{
	public static class Insertar
	{
        public static async void AñadirId(string nombre, string ids)
        {
			string insertar = "INSERT INTO juegosIDs (nombre, ids, nombreCodigo) " +
					   "SELECT @nombre, @ids, @nombreCodigo " +
					   "WHERE NOT EXISTS (SELECT 1 FROM juegosIDs WHERE nombre = @nombre)";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(insertar, new
					{
						nombre,
						ids,
						nombreCodigo = Herramientas.Buscador.LimpiarNombre(nombre, true)
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Insertar", ex);
			}
		}
    }
}
