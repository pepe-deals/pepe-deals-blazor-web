#nullable disable

using Dapper;
using Microsoft.Data.SqlClient;

namespace BaseDatos.CorreosEnviar
{
	public static class Insertar
	{
		public static async Task Ejecutar(string html, string titulo, string correoDesde, string correoHacia, DateTime fecha, CorreoPendienteTipo tipo, string json = null, SqlConnection conexion = null)
		{
			string columnasExtra = string.IsNullOrEmpty(json) ? "" : ", json";
			string valoresExtra = string.IsNullOrEmpty(json) ? "" : ", @json";

			string sql = @"
					INSERT INTO correosEnviar 
					(html, titulo, correoDesde, correoHacia, tipo, fecha" + columnasExtra + @") 
					VALUES 
					(@html, @titulo, @correoDesde, @correoHacia, @tipo, @fecha" + valoresExtra + @");
				";

			try
			{
				var parametros = new
				{
					html,
					titulo,
					correoDesde,
					correoHacia,
					tipo,
					fecha,
					json
				};

				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sql, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje2("Correos Enviar Insertar", ex, false, sql);
			}
		}

		public static async Task Registro(string html, string titulo, string correoHacia)
		{
			string sql = @"
					INSERT INTO correosRegistro 
					(html, titulo, correoHacia) 
					VALUES 
					(@html, @titulo, @correoHacia);
				";

			try
			{
				var parametros = new
				{
					html,
					titulo,
					correoHacia
				};

				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sql, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje2("Correos Registro Insertar", ex, false);
			}
		}
	}
}
