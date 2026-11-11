#nullable disable

using Dapper;

namespace BaseDatos.Errores
{
    public static class Insertar
	{
		public static async void Mensaje2(string seccion, Exception ex, bool reiniciar = true, string comandoUsado = null)
		{
			string sqlInsertar = @"
                INSERT INTO errores (seccion, mensaje, stacktrace, fecha {0})
                VALUES (@seccion, @mensaje, @stacktrace, @fecha {1});
            ";

			string columnasExtra = "";
			string valoresExtra = "";

			if (string.IsNullOrEmpty(comandoUsado) == false)
			{
				columnasExtra = ", sentenciaSQL";
				valoresExtra = ", @sentenciaSQL";
			}

			sqlInsertar = string.Format(sqlInsertar, columnasExtra, valoresExtra);

			await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
			{
				return await conexion.ExecuteAsync(sqlInsertar, new
				{
					seccion,
					mensaje = ex.Message,
					stacktrace = ex.StackTrace,
					fecha = DateTime.Now,
					sentenciaSQL = comandoUsado
				}, transaction: sentencia);
			});

			if (reiniciar == true)
			{
				WebApplicationBuilder builder = WebApplication.CreateBuilder();
				string piscinaApp = builder.Configuration.GetValue<string>("PoolWeb:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaApp != piscinaUsada)
				{
					Environment.Exit(1);
				}
			}
		}

		public static async void Mensaje(string seccion, Exception ex, bool reiniciar = true)
		{
			string sql = @"
                INSERT INTO errores
                (seccion, mensaje, stacktrace, fecha)
                VALUES (@seccion, @mensaje, @stacktrace, @fecha)
            ";

			await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
			{
				return await conexion.ExecuteAsync(sql, new
				{
					seccion,
					mensaje = ex.Message,
					stacktrace = ex.StackTrace,
					fecha = DateTime.Now
				}, transaction: sentencia);
			});

			if (reiniciar == true)
            {
				WebApplicationBuilder builder = WebApplication.CreateBuilder();
				string piscinaApp = builder.Configuration.GetValue<string>("PoolWeb:Contenido");
				string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

				if (piscinaApp != piscinaUsada)
                {
					Environment.Exit(1);
				}
			}  
        }

        public static async void Mensaje(string seccion, string mensaje, string enlace = null)
        {
			string sql = @"
				INSERT INTO errores
				(seccion, mensaje, stacktrace, fecha, enlace)
				VALUES (@seccion, @mensaje, @stacktrace, @fecha, @enlace);
			";

			await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
			{
				return await conexion.ExecuteAsync(sql, new
				{
					seccion,
					mensaje,
					stacktrace = "",
					fecha = DateTime.Now,
					enlace = (object)enlace ?? DBNull.Value
				}, transaction: sentencia);
			});
		}

		public static async void Mensaje(string seccion, DynamicParameters dynamicParams, string baseSql)
		{
			var paramField = dynamicParams.GetType().GetField("parameters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var paramDict = paramField?.GetValue(dynamicParams) as Dictionary<string, object>;

			string sqlResult = baseSql;

			if (paramDict != null)
			{
				foreach (var param in paramDict)
				{
					string paramName = param.Key;
					object paramValue = param.Value;

					string formattedValue = paramValue == null ? "NULL" :
						paramValue is string ? $"'{paramValue.ToString().Replace("'", "''")}'" :
						paramValue.ToString();

					sqlResult = sqlResult.Replace($"@{paramName}", formattedValue);
				}
			}

			string sql = @"
				INSERT INTO errores
				(seccion, mensaje, stacktrace, fecha)
				VALUES (@seccion, @mensaje, @stacktrace, @fecha);
			";

			await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
			{
				return await conexion.ExecuteAsync(sql, new
				{
					seccion,
					sqlResult,
					stacktrace = "",
					fecha = DateTime.Now
				}, transaction: sentencia);
			});
		}
	}
}
