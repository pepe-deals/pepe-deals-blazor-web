#nullable disable

using Dapper;
using Gratis2;
using Juegos;

namespace BaseDatos.Gratis
{
	public static class Buscar
	{
		public static async Task<List<JuegoGratis>> Actuales(GratisTipo tipo = GratisTipo.Desconocido)
		{
			string busqueda = @"SELECT sub.*
FROM(
	SELECT *, gratis AS Tipo

	FROM gratis

	WHERE GETDATE() BETWEEN fechaEmpieza AND fechaTermina
) AS sub";

			if (tipo != GratisTipo.Desconocido)
			{
				busqueda = busqueda + " WHERE sub.gratis=" + (int)tipo ;
			}

			busqueda = busqueda + " ORDER BY DATEPART(MONTH,sub.fechaTermina), DATEPART(DAY,sub.fechaTermina)";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoGratis>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis Actuales", ex);
			}

			return null;
		}

        public static async Task<List<JuegoGratis>> Año(string año)
        {
			string busqueda = @"SELECT sub.*
FROM (
    SELECT *, gratis AS Tipo
    FROM gratis
    WHERE YEAR(fechaEmpieza) = @año
      AND GETDATE() > fechaTermina
) AS sub
ORDER BY sub.nombre DESC;
";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoGratis>(busqueda, new { año })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis Año", ex);
			}

			return null;
		}

		public static async Task<JuegoGratis> UnJuego(string juegoId)
		{
			string busqueda = @"SELECT TOP 1 sub.*
FROM (
    SELECT *, gratis AS Tipo
    FROM gratis
    WHERE juegoId = @juegoId
) AS sub
ORDER BY sub.ID DESC;";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<JuegoGratis>(busqueda, new { juegoId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis Uno", ex);
			}

			return null;
		}

		public static async Task<JuegoGratis> UnGratis(string id)
		{
			string busqueda = @"SELECT sub.*
FROM (
    SELECT *, gratis AS Tipo
    FROM gratis
    WHERE id = @id
) AS sub;";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<JuegoGratis>(busqueda, new { id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis Uno", ex);
			}

			return null;
		}

		public static async Task<List<JuegoGratis>> Ultimos(int cantidad)
        {
			string busqueda = @"SELECT sub.*
FROM (
    SELECT TOP (@cantidad) *, gratis AS Tipo
    FROM gratis
    ORDER BY id DESC
) AS sub;
";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoGratis>(busqueda, new { cantidad })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Gratis Ultimos", ex);
			}

			return null;
		}
    }
}
