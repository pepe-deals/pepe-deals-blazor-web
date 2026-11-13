using Dapper;
using System.Text.RegularExpressions;

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

			string imagenDetectada = Regex.Match(contenido ?? "",
				@"https?://[^\s""'<>)]+?\.(?:jpe?g|png|webp|avif|bmp)(?:\?[^\s""'<>)]*)?", RegexOptions.IgnoreCase).Value;

			if (string.IsNullOrEmpty(imagenDetectada) == false && imagenDetectada.Length <= 256)
			{
				campos.Add("imagen");
				valores.Add("@imagen");
				p.Add("@imagen", imagenDetectada);
			}

			string sql = $@"INSERT INTO noticias ({string.Join(",", campos)})
                VALUES ({string.Join(",", valores)});
                SELECT SCOPE_IDENTITY();";

			try
			{
				return await Herramientas.Conexiones.Pepeizqapps.Select(async conexion =>
				{
					return await conexion.ExecuteScalarAsync<int>(sql, p);
				});
			}
			catch (Exception ex)
			{
				Errores.Insertar.Mensaje("Noticias Insertar - pepeizq's apps", ex);
				return 0;
			}
		}
	}
}
