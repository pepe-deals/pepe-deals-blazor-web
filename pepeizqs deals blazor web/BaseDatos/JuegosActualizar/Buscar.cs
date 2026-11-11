#nullable disable

using Dapper;

namespace BaseDatos.JuegosActualizar
{

	public static class Buscar
	{
		public static async Task<bool> Existe(int idJuego, int idPlataforma, string metodo)
		{
			string sql = "SELECT COUNT(*) FROM fichasActualizar " +
				   "WHERE idJuego=@idJuego AND idPlataforma=@idPlataforma AND metodo=@metodo";

			try
			{
				int valor = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.ExecuteScalarAsync<int>(sql, new
					{
						idJuego,
						idPlataforma,
						metodo
					});
				});

				return valor > 0;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("JuegosActualizar Buscar Existe", ex);	
			}

			return false;
		}

		public static async Task<List<JuegoActualizar>> Todos()
		{
			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoActualizar>("SELECT * FROM fichasActualizar")).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("JuegosActualizar Buscar Todos", ex);
			}

			return new List<JuegoActualizar>();
		}
	}

	public class JuegoActualizar
	{
		public int IdJuego { get; set; }
		public int IdPlataforma { get; set; }
		public string Metodo { get; set; }
	}
}
