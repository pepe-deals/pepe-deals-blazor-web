#nullable disable

using Dapper;
using Juegos;
using Suscripciones2;

namespace BaseDatos.Suscripciones
{
    public static class Buscar
    {
		public static async Task<List<JuegoSuscripcion>> Actuales(SuscripcionTipo tipo = SuscripcionTipo.Desconocido, int orden = 0, string nombreBusqueda = null, int cantidad = 0)
        {
			string cantidadTexto = string.Empty;

			if (cantidad > 0)
			{
				cantidadTexto = $"TOP {cantidad}";
			}

			string busqueda = $@"
				SELECT {cantidadTexto}
					sub.*,
					(
						SELECT g.gratis
						FROM gratis g
						WHERE g.juegoId = j.id
						  AND g.fechaEmpieza <= GETDATE()
						  AND g.fechaTermina >= GETDATE()
						FOR JSON PATH
					) AS GratisActuales,
					(
						SELECT g.gratis
						FROM gratis g
						WHERE g.juegoId = j.id
						  AND g.fechaTermina < GETDATE()
						FOR JSON PATH
					) AS GratisPasados,
					(
						SELECT s.suscripcion
						FROM suscripciones s
						WHERE s.juegoId = j.id
						  AND s.FechaEmpieza <= GETDATE()
						  AND s.FechaTermina >= GETDATE()
						FOR JSON PATH
					) AS SuscripcionesActuales,
					(
						SELECT s.suscripcion
						FROM suscripciones s
						WHERE s.juegoId = j.id
						  AND s.FechaTermina < GETDATE()
						FOR JSON PATH
					) AS SuscripcionesPasados,
				j.*
				FROM (
					SELECT *, suscripcion AS Tipo
					FROM suscripciones
					WHERE GETDATE() BETWEEN fechaEmpieza AND fechaTermina
				) AS sub
				INNER JOIN juegos j ON j.id = sub.juegoid
				WHERE 1=1";

			if (tipo != SuscripcionTipo.Desconocido)
			{
				busqueda += " AND sub.suscripcion = " + (int)tipo;
			}

			if (string.IsNullOrEmpty(nombreBusqueda) == false)
			{
				busqueda += $@"AND j.nombre COLLATE Latin1_General_CI_AI
      LIKE '%{nombreBusqueda}%'";
			}

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

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					var resultado = await conexion.QueryAsync<JuegoSuscripcion, Juego, JuegoSuscripcion>(
						busqueda,
						(suscripcion, juego) =>
						{
							suscripcion.Juego = juego;
							return suscripcion;
						},
						splitOn: "Id"
					);

					return resultado.ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Actuales", ex);
			}

			return null;
		}

		public static async Task<List<JuegoSuscripcion>> Año(string año)
		{
			string busqueda = @"
SELECT sub.*
FROM (
    SELECT *, suscripcion AS Tipo
    FROM suscripciones
    WHERE YEAR(fechaEmpieza) = @Año
      AND GETDATE() > fechaTermina
) AS sub
ORDER BY sub.Nombre DESC";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoSuscripcion>(busqueda, new { año })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Año", ex);
			}

			return null;
		}

		public static async Task<JuegoSuscripcion> Id(int id)
		{
			string busqueda = @"
				SELECT sub.*
				FROM (
					SELECT *, suscripcion AS Tipo
					FROM suscripciones
					WHERE id = @Id
				) AS sub";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<JuegoSuscripcion>(busqueda, new { Id = id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Id", ex);
			}

			return null;
		}

		public static async Task<List<JuegoSuscripcion>> JuegoId(int id)
		{
			string busqueda = @"
				SELECT sub.*
				FROM (
					SELECT *, suscripcion AS Tipo
					FROM suscripciones
					WHERE juegoId = @JuegoId
				) AS sub";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoSuscripcion>(busqueda, new { JuegoId = id })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones JuegoId", ex);
			}

			return null;
		}

		public static async Task<List<JuegoSuscripcion>> UltimasAñadidas()
		{
			string busqueda = @"
				SELECT 
					sub.*,
					j.*,
					(
						SELECT g.gratis
						FROM gratis g
						WHERE g.juegoId = j.id
						  AND g.fechaEmpieza <= GETDATE()
						  AND g.fechaTermina >= GETDATE()
						FOR JSON PATH
					) AS GratisActuales,
					(
						SELECT g.gratis
						FROM gratis g
						WHERE g.juegoId = j.id
						  AND g.fechaTermina < GETDATE()
						FOR JSON PATH
					) AS GratisPasados,
					(
						SELECT s.suscripcion
						FROM suscripciones s
						WHERE s.juegoId = j.id
						  AND s.FechaEmpieza <= GETDATE()
						  AND s.FechaTermina >= GETDATE()
						FOR JSON PATH
					) AS SuscripcionesActuales,
					(
						SELECT s.suscripcion
						FROM suscripciones s
						WHERE s.juegoId = j.id
						  AND s.FechaTermina < GETDATE()
						FOR JSON PATH
					) AS SuscripcionesPasados
				FROM (
					SELECT *, suscripcion AS Tipo
					FROM suscripciones
					WHERE fechaEmpieza >= DATEADD(day, -30, GETDATE())
				) AS sub
				INNER JOIN juegos j ON j.id = sub.juegoid
				ORDER BY fechaEmpieza DESC";
			
			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					var resultado = await conexion.QueryAsync<JuegoSuscripcion, Juego, JuegoSuscripcion>(
						busqueda,
						(suscripcion, juego) =>
						{
							suscripcion.Juego = juego;
							return suscripcion;
						},
						splitOn: "Id"
					);

					return resultado.ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Ultimas Añadidas", ex);
			}

			return null;
		}

		public static async Task<List<JuegoSuscripcion>> Ultimos(string cantidad)
		{
			string busqueda = $@"
				SELECT TOP {cantidad} sub.*
				FROM (
					SELECT *, suscripcion AS Tipo
					FROM suscripciones
				) AS sub
				ORDER BY id DESC";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoSuscripcion>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Ultimos", ex);
			}

			return null;
		}

		public static async Task<List<int>> Steam(int idPaquete)
		{
			string busqueda = @"
				SELECT idJuego FROM tiendasteamsuscripciones
				WHERE idPaquete=@idPaquete";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<int>(busqueda, new { idPaquete })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Steam", ex);
			}

			return null;
		}
	}
}
