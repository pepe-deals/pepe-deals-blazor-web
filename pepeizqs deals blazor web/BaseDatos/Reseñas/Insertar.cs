#nullable disable

using Dapper;

namespace BaseDatos.Reseñas
{
	public static class Insertar
	{
		public static async Task Ejecutar(int id, int positivos, int negativos, string idioma, string contenido)
		{
			try
			{
				bool existe = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM juegosAnalisis WHERE id=@id", new { id }) > 0;
				});

				string fechaCol = "fecha" + idioma;
				string positivosCol = "positivos" + idioma;
				string negativosCol = "negativos" + idioma;
				string contenidoCol = "contenido" + idioma;

				if (existe == false)
				{
					string sqlInsertar = $@"INSERT INTO juegosAnalisis (id, {positivosCol}, {negativosCol}, {fechaCol}, {contenidoCol}) VALUES (@id, @positivos, @negativos, @fecha, @contenido)";

					await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(sqlInsertar, new { id, positivos, negativos, fecha = DateTime.Now, contenido }, transaction: sentencia);
					});
				}
				else
				{
					string sqlActualizar = $@"UPDATE juegosAnalisis SET {positivosCol}=@positivos, {negativosCol}=@negativos, {fechaCol}=@fecha, {contenidoCol}=@contenido WHERE id=@id";

					await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(sqlActualizar, new { id, positivos, negativos, fecha = DateTime.Now, contenido }, transaction: sentencia);
					});
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Reseñas Insertar", ex);
			}
		}
	}
}
