#nullable disable

using Dapper;

namespace BaseDatos.Portapapeles
{
	public static class Buscar
	{
		public static async Task<bool> YaExiste(string id)
		{
			try
			{
				string sql = "SELECT COUNT(*) FROM portapapeles WHERE id=@id";

				int resultado = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.ExecuteScalarAsync<int>(sql, new { id });
				});

				return resultado > 0;
			}
			catch (Exception ex) 
			{
				BaseDatos.Errores.Insertar.Mensaje("Portapapeles Ya Existe", ex);				
			}

			return false;
		}

		public static async Task<string> Contenido(string id)
		{
			try
			{
				string sql = "SELECT contenido FROM portapapeles WHERE id=@id";

				string contenido = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.ExecuteScalarAsync<string>(sql, new { id });
				});

				return contenido ?? string.Empty;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Portapapeles Buscar Contenido", ex);
			}

			return null;
		}
	}
}
