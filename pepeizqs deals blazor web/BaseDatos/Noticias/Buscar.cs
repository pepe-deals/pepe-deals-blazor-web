#nullable disable

using Dapper;
using Noticias;

namespace BaseDatos.Noticias
{
	public static class Buscar
	{
		public static async Task<Noticia> UnaNoticia(int id)
		{
			string sql = "SELECT * FROM noticias WHERE id = @id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Noticia>(sql, new { id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticia Una", ex);
			}

			return null;
		}

		public static async Task<Noticia> UnaNoticiaTitulos(int id)
		{
			string sql = "SELECT id, tituloEn, tituloEs FROM noticias WHERE id = @id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Noticia>(sql, new { id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticia Titulos", ex);
			}

			return null;
		}

		public static async Task<List<Noticia>> Actuales(List<NoticiaTipo> tipos = null, int ultimosDias = 0)
		{
			string busqueda = "SELECT * FROM noticias WHERE (GETDATE() BETWEEN fechaEmpieza AND fechaTermina)";

			if (ultimosDias > 0)
			{
				busqueda = busqueda + " AND (GETDATE()-" + ultimosDias.ToString() + " < fechaEmpieza)";
			}

			if (tipos?.Count > 0)
			{
				busqueda = busqueda + " AND (noticiaTipo IN (" + string.Join(",", tipos.Select(t => (int)t)) + "))";
			}

			busqueda = busqueda + " ORDER BY id DESC";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Noticia>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actuales", ex);
			}

			return null;
		}

		public static async Task<List<Noticia>> Año(string año)
		{
			string busqueda = "SELECT * FROM noticias WHERE YEAR(fechaEmpieza) = " + año + " AND GETDATE() > fechaTermina ORDER BY id DESC";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Noticia>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Año", ex);
			}

			return null;
		}

		public static async Task<List<Noticia>> Ultimas(int cantidad)
		{
			string busqueda = "SELECT TOP " + cantidad.ToString() + " * FROM noticias ORDER BY id DESC";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Noticia>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Ultimas", ex);
			}

			return null;
		}

		public static async Task<List<Noticia>> Ultimas(int cantidad, NoticiaTipo tipo)
		{
			string busqueda = "SELECT TOP " + cantidad.ToString() + " * FROM noticias WHERE noticiaTipo=@noticiaTipo ORDER BY id DESC";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Noticia>(busqueda, new { noticiaTipo = tipo })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Ultimas", ex);
			}

			return null;
		}

		public static async Task<List<Noticia>> Aleatorias()
		{
			string busqueda = @"SELECT TOP 50 id, tituloEn FROM noticias ORDER BY NEWID()";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Noticia>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Aleatorias", ex);
			}

			return null;
		}
	}
}
