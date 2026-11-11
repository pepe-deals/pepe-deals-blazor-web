#nullable disable

using Dapper;

namespace BaseDatos.Errores
{
    public static class Limpiar
    {
		public static async void Ejecutar()
        {
			await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
			{
				return await conexion.ExecuteAsync("TRUNCATE TABLE errores", transaction: sentencia);
			});
		}
    }
}
