using Dapper;
using System.Text.RegularExpressions;

namespace BaseDatos.Pepeizqapps
{
	public static class Insertar
	{
		public static async Task<int> Actualizacion(string titulo, string contenido)
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
				Errores.Insertar.Mensaje("Actualización Insertar - pepeizq's apps", ex);
				return 0;
			}
		}

		public static async Task<int> ImporteCoste(string concepto, decimal cantidad, DateTime fecha)
		{
			List<string> campos = new List<string>();
			List<string> valores = new List<string>();
			DynamicParameters p = new DynamicParameters();

			campos.Add("concepto");
			valores.Add("@concepto");
			p.Add("@concepto", concepto);

			campos.Add("importe");
			valores.Add("@importe");
			p.Add("@importe", cantidad);

			campos.Add("fecha");
			valores.Add("@fecha");
			p.Add("@fecha", fecha);

			string sql = $@"INSERT INTO costespepedeals ({string.Join(",", campos)})
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
				Errores.Insertar.Mensaje("Costes Insertar - pepeizq's apps", ex);
				return 0;
			}
		}
	}
}
