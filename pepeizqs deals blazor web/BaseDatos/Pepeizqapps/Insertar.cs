using Dapper;

namespace BaseDatos.Pepeizqapps
{
	public static class Insertar
	{
		public static async Task<int> Ejecutar(string titulo, string contenido)
		{
			List<string> campos = new List<string>();
			List<string> valores = new List<string>();
			DynamicParameters p = new DynamicParameters();

			campos.Add("titulo");
			valores.Add("@titulo");
			p.Add("@titulo", titulo);

			campos.Add("contenido");
			valores.Add("@contenido");
			p.Add("@contenido", contenido);

			campos.Add("fecha");
			valores.Add("@fecha");
			p.Add("@fecha", DateTime.Now);

			string sql = $@"INSERT INTO noticias ({string.Join(",", campos)})
                VALUES ({string.Join(",", valores)});
                SELECT SCOPE_IDENTITY();";

			try
			{
				return await Herramientas.BaseDatosPepeizqapps.Select(async conexion =>
				{
					return await conexion.ExecuteScalarAsync<int>(sql, p);
				});
			}
			catch (Exception ex)
			{
				Errores.Insertar.Mensaje("Noticias Insertar pepeizqapps", ex);
				return 0;
			}
		}
	}
}
