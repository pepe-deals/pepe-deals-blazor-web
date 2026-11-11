#nullable disable

using Dapper;

namespace BaseDatos.Recompensas
{
    public class RecompensaHistorial
    {
        public string UsuarioId { get; set; }
        public int Coins { get; set; }
        public string Razon { get; set; }
        public DateTime Fecha { get; set; }
    }

    public static class Historial
    {
		public static async Task Insertar(string usuarioId, int coins, string razon, DateTime fecha)
        {
            try
            {
				string insertar = "INSERT INTO recompensasHistorial " +
				   "(usuarioId, coins, razon, fecha) VALUES " +
				   "(@usuarioId, @coins, @razon, @fecha) ";

				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(insertar, new
					{
						usuarioId,
						coins,
						razon,
						fecha
					}, transaction: sentencia);
				});
			}
            catch (Exception ex)
            {
				BaseDatos.Errores.Insertar.Mensaje("Recompensas Insertar", ex);
			}
        }

        public static async Task<List<RecompensaHistorial>> Leer(string usuarioId = null)
        {
			try
			{
				string busqueda = "SELECT TOP 30 * FROM recompensasHistorial";
				
				if (string.IsNullOrEmpty(usuarioId) == false)
				{
					busqueda = busqueda + " WHERE usuarioId = @usuarioId";
				}

				busqueda = busqueda + " ORDER BY fecha DESC";

				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<RecompensaHistorial>(busqueda, new { usuarioId })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Recompensas Leer", ex);
			}
			
            return null;
        }
    }
}
