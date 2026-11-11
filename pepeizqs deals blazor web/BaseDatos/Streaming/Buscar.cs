#nullable disable

using Dapper;
using Juegos;
using System.Text.Json;

namespace BaseDatos.Streaming
{
    public static class Buscar
    {
		public static async Task<List<JuegoDRM>> DRMs(string tabla, int idJuego)
        {
			string busqueda = "SELECT drms FROM streaming" + tabla + " WHERE idJuego = " + idJuego.ToString() + " AND fecha > DATEADD(DAY, -7, CAST(GETDATE() as date))";

			try
			{
				var resultados = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<string>(busqueda, new { idJuego })).ToList();
				});

				List<JuegoDRM> listaDRMs = new List<JuegoDRM>();

				foreach (var drmsTexto in resultados)
				{
					if (!string.IsNullOrEmpty(drmsTexto))
					{
						var drms = JsonSerializer.Deserialize<List<string>>(drmsTexto);

						foreach (var drm in drms)
						{
							listaDRMs.Add(JuegoDRM2.Traducir(drm));
						}
					}
				}

				return listaDRMs;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Streaming DRMs", ex);
			}

			return null;
        }

		public static async Task<List<JuegoStreaming>> BuscarJuegos(string tabla, int orden, string nombreBusqueda = null)
		{
			if (string.IsNullOrEmpty(tabla) == false)
			{
				string busqueda = $@"SELECT
  j.id,
  j.nombre,
  j.imagenes,
  j.idSteam,
  j.idGOG,
  s.drms
FROM {tabla} s
INNER JOIN juegos j ON s.idJuego = j.id
WHERE s.idJuego <> 0 AND j.nombre IS NOT NULL AND LEN(LTRIM(RTRIM(j.nombre))) >= 1";

				if (string.IsNullOrEmpty(nombreBusqueda) == false)
				{
					busqueda += $@"AND j.nombre COLLATE Latin1_General_CI_AI
      LIKE '%{nombreBusqueda}%'";
				}

busqueda += $@"GROUP BY j.id, j.nombre, j.imagenes, j.idSteam, j.idGOG, j.analisis, s.drms";

				if (orden == 0)
				{
					busqueda += $@"
ORDER BY CASE
WHEN j.analisis = 'null' OR j.analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(j.analisis, '$.Cantidad'),',',''))
END DESC";
				}
				else if (orden == 1)
				{
					busqueda += $@"
ORDER BY j.Nombre";
				}
				else if (orden == 2)
				{
					busqueda += $@"
ORDER BY j.Nombre DESC";
				}

				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoStreaming>(busqueda)).ToList();
				});
			}

			return null;
		}
    }
}
