#nullable disable

using Bundles2;
using Dapper;
using Juegos;
using Tiendas2;

namespace BaseDatos.RedesSociales
{
    public static class Buscar
    {
        public static async Task<List<Juego>> OfertasDelDia(TiendaRegion region, int drm)
        {
			string seccionMinimos = region == TiendaRegion.Europa ? "seccionMinimos" : "seccionMinimosUS";
			string precioMinimosHistoricos = region == TiendaRegion.Europa ? "precioMinimosHistoricos" : "precioMinimosHistoricosUS";

			string busqueda = $@"SELECT TOP 100 j.idMaestra, jg.nombre, j.{precioMinimosHistoricos}, precioMin.FechaDetectado AS Fecha, precioMin.DRM AS DRM, jg.analisis, CONVERT(datetime2, JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoSteam')) as FechaLanzamiento FROM {seccionMinimos} j
				INNER JOIN dbo.juegos jg ON jg.id = j.idMaestra
				CROSS APPLY OPENJSON(j.{precioMinimosHistoricos}, '$[0]') WITH (
					Precio float '$.Precio',
					Descuento int '$.Descuento',
					DRM int '$.DRM',
					FechaTermina datetime2 '$.FechaTermina',
					FechaActualizacion datetime2 '$.FechaActualizacion',
					FechaDetectado datetime2 '$.FechaDetectado'
				) precioMin
				WHERE precioMin.DRM = @drm
				AND precioMin.Descuento > 0
				AND CAST(precioMin.FechaDetectado AS date) = CAST(GETDATE() AS date)
				";

			if (drm == 0)
			{
				busqueda = busqueda + @" AND CONVERT(bigint, REPLACE(JSON_VALUE(jg.analisis, '$.Cantidad'),',','')) > 100";
			}

			busqueda = busqueda + @" ORDER BY CASE WHEN jg.analisis = 'null' OR jg.analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(jg.analisis, '$.Cantidad'),',','')) END DESC"
;
			if (string.IsNullOrEmpty(busqueda) == false)
			{
				try
				{
					return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(busqueda, new { drm })).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Redes Sociales Ofertas Dia " + drm.ToString(), ex);
				}
			}

			return null;
        }

		public static async Task<List<Juego>> OfertasUltimaSemana(TiendaRegion region, int drm)
		{
			string seccionMinimos = region == TiendaRegion.Europa ? "seccionMinimos" : "seccionMinimosUS";
			string precioMinimosHistoricos = region == TiendaRegion.Europa ? "precioMinimosHistoricos" : "precioMinimosHistoricosUS";

			string busqueda = $@"SELECT TOP 250 j.idMaestra, j.nombre, j.{precioMinimosHistoricos}, CONVERT(datetime2, JSON_VALUE(j.{precioMinimosHistoricos}, '$[0].FechaDetectado')) AS Fecha, JSON_VALUE(j.{precioMinimosHistoricos}, '$[0].DRM') AS DRM, j.analisis, j.imagenes, CONVERT(datetime2, JSON_VALUE(j.caracteristicas, '$.FechaLanzamientoSteam')) as FechaLanzamiento FROM {seccionMinimos} j
				WHERE JSON_VALUE(j.{precioMinimosHistoricos}, '$[0].DRM') = @drm
				AND JSON_VALUE(j.{precioMinimosHistoricos}, '$[0].Descuento') > 0
				AND tipo = 0
				AND CAST(CONVERT(datetime2, JSON_VALUE(j.{precioMinimosHistoricos}, '$[0].FechaDetectado')) AS date) >= CAST(DATEADD(day, -7, GETDATE()) AS date)
				";

			if (drm == 0)
			{
				busqueda = busqueda + @" AND CONVERT(bigint, REPLACE(JSON_VALUE(j.analisis, '$.Cantidad'),',','')) > 499";
			}

			busqueda = busqueda + @" ORDER BY CASE WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',','')) END DESC";

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				try
				{
					return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(busqueda, new { drm })).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Redes Sociales Ofertas Ultima Semana " + drm.ToString(), ex);
				}
			}

			return null;
		}

		public static async Task<List<Bundle>> BundlesUltimaSemana()
		{
			string busqueda = @"SELECT * FROM bundles WHERE (GETDATE() BETWEEN fechaEmpieza AND fechaTermina) AND fechaEmpieza >= CAST(DATEADD(day, -7, GETDATE()) AS date)
								ORDER BY nombre";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Bundle>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Redes Sociales Bundles Ultima Semana", ex);
			}

			return null;
		}

		public static async Task<List<JuegoGratis>> GratisUltimaSemana()
		{
			string busqueda = @"SELECT sub.*
				FROM(
					SELECT *, gratis AS Tipo

					FROM gratis

					WHERE fechaEmpieza >= CAST(DATEADD(day, -7, GETDATE()) AS date) AND GETDATE() BETWEEN fechaEmpieza AND fechaTermina
				) AS sub";

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
				BaseDatos.Errores.Insertar.Mensaje("Redes Sociales Gratis Ultima Semana", ex);
			}

			return null;
		}

		public static async Task<List<JuegoSuscripcion>> SuscripcionesUltimaSemana()
		{
			string busqueda = $@"
				SELECT sub.*,
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
					WHERE fechaEmpieza >= CAST(DATEADD(day, -7, GETDATE()) AS date)
				) AS sub
				INNER JOIN juegos j ON j.id = sub.juegoid
				WHERE 1=1
					AND (sub.suscripcion = 0 OR sub.suscripcion = 11 OR sub.suscripcion = 6)";

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
				BaseDatos.Errores.Insertar.Mensaje("Redes Sociales Suscripciones Ultima Semana", ex);
			}

			return null;
		}

	}
}