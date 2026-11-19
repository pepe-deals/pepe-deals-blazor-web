#nullable disable


using Dapper;

namespace BaseDatos.Pepeizqapps
{

	public static class Buscar
	{
		public static async Task<Noticia> UltimaActualizacion()
		{
			string sql = "SELECT TOP 1 id, titulo, fecha FROM noticias WHERE fecha >= DATEADD(DAY, -2, GETDATE()) ORDER BY fecha DESC, id DESC;";

			try
			{
				return await Herramientas.Conexiones.Pepeizqapps.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Noticia>(sql);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Ultima Actualizacion Pepeizqapps", ex);
			}

			return null;
		}
	}

	public class Noticia
	{
		public int Id { get; set; }
		public string Titulo { get; set; }
		public string Contenido { get; set; }
		public DateTime Fecha { get; set; }
		public string Imagen { get; set; }
	}
}
