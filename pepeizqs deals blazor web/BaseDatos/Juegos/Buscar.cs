#nullable disable

using Dapper;
using Juegos;
using System.Data;
using Tiendas2;
using static pepeizqs_deals_blazor_web.Componentes.Cuenta.Cuenta.Juegos;
using static pepeizqs_deals_blazor_web.Componentes.Secciones.Minimos.Minimos;

namespace BaseDatos.Juegos
{
	public static class Buscar
	{
		public static async Task<Juego> UnJuego(int id)
		{
			return await UnJuego(id.ToString());
		}

		public static async Task<Juego> UnJuego(string id = null, string idSteam = null, string idGog = null, string idEpic = null)
		{
			if (id == "descartado")
			{
				return null;
			}

			string sqlBuscar = @"SELECT *,
	(
		SELECT g.gratis AS Tipo, g.enlace AS Enlace, g.drm AS DRM, g.FechaEmpieza AS FechaEmpieza, g.FechaTermina AS FechaTermina, g.id AS id
		FROM gratis g
		WHERE g.juegoId = j.id
		FOR JSON PATH
	) as gratis, 
	(
		SELECT s.*, s.suscripcion AS Tipo
		FROM suscripciones s
		WHERE s.juegoId = j.id
		FOR JSON PATH
	) as suscripciones,
    (
		SELECT b.id, b.bundleTipo
		FROM bundles b
		INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
		WHERE bj.juegoId = j.id
			AND b.fechaEmpieza <= GETDATE()
			AND b.fechaTermina >= GETDATE()
		FOR JSON PATH
	) AS BundlesActuales,
	(
		SELECT b.id, b.bundleTipo
		FROM bundles b
		INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
		WHERE bj.juegoId = j.id
			AND b.fechaTermina < GETDATE()
		FOR JSON PATH
	) AS BundlesPasados,
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
FROM juegos j";

			if (string.IsNullOrEmpty(id) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<Juego>(sqlBuscar + " WHERE id=@id", new { id });
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Uno Web", ex);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(idSteam) == false)
				{
					try
					{
						return await Herramientas.BaseDatos.Select(async conexion =>
						{
							return await conexion.QueryFirstOrDefaultAsync<Juego>(sqlBuscar + " WHERE idSteam=@idSteam", new { idSteam });
						});
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Juego Uno Steam", ex);
					}
				}
				else
				{
					if (string.IsNullOrEmpty(idGog) == false)
					{
						try
						{
							return await Herramientas.BaseDatos.Select(async conexion =>
							{
								return await conexion.QueryFirstOrDefaultAsync<Juego>(sqlBuscar + " WHERE slugGog=@slugGog", new { slugGog = idGog });
							});
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Juego Uno GOG", ex);
						}
					}
					else
					{
						if (string.IsNullOrEmpty(idEpic) == false)
						{
							try
							{
								return await Herramientas.BaseDatos.Select(async conexion =>
								{
									return await conexion.QueryFirstOrDefaultAsync<Juego>(sqlBuscar + " WHERE slugEpic=@slugEpic", new { slugEpic = idEpic });
								});
							}
							catch (Exception ex)
							{
								BaseDatos.Errores.Insertar.Mensaje("Juego Uno Epic", ex);
							}
						}
					}
				}
			}

			return null;
		}

		public static async Task<Juego> UnJuegoReducido(int id)
		{
			string busqueda = @"SELECT
    j.id, j.nombre, j.imagenes, j.precioMinimosHistoricos, j.precioActualesTiendas,
    j.tipo, j.analisis, j.idSteam, j.idGog, j.media, j.freeToPlay, j.etiquetas,
		(
			SELECT b.id, b.bundleTipo
			FROM bundles b
			INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
			WHERE bj.juegoId = j.id
				AND b.fechaEmpieza <= GETDATE()
				AND b.fechaTermina >= GETDATE()
			FOR JSON PATH
		) AS BundlesActuales,
		(
			SELECT b.id, b.bundleTipo
			FROM bundles b
			INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
			WHERE bj.juegoId = j.id
				AND b.fechaTermina < GETDATE()
			FOR JSON PATH
		) AS BundlesPasados,
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
	FROM juegos j
	WHERE id=@id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Juego>(busqueda, new { id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Uno Reducido", ex);
			}

			return null;
		}

		public static async Task<Juego> UnJuegoComparador(TiendaRegion region, int id)
		{
			string precioMinimosHistoricos = region switch
			{
				TiendaRegion.Europa => "precioMinimosHistoricos",
				TiendaRegion.EstadosUnidos => "precioMinimosHistoricosUS",
				_ => string.Empty
			};

			string precioActualesTiendas = region switch
			{
				TiendaRegion.Europa => "precioActualesTiendas",
				TiendaRegion.EstadosUnidos => "precioActualesTiendasUS",
				_ => string.Empty
			};

			string busqueda = $@"SELECT
    j.id, j.nombre, j.imagenes, j.{precioMinimosHistoricos}, j.{precioActualesTiendas},
    j.tipo, j.analisis, j.idSteam, j.idGog, j.idAmazon,
    j.exeEpic, j.exeUbisoft, j.freeToPlay, j.deck,
	(
		SELECT b.id, b.bundleTipo
		FROM bundles b
		INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
		WHERE bj.juegoId = j.id
			AND b.fechaEmpieza <= GETDATE()
			AND b.fechaTermina >= GETDATE()
		FOR JSON PATH
	) AS BundlesActuales,
	(
		SELECT b.id, b.bundleTipo
		FROM bundles b
		INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
		WHERE bj.juegoId = j.id
			AND b.fechaTermina < GETDATE()
		FOR JSON PATH
	) AS BundlesPasados,
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
FROM juegos j
WHERE id=@id AND j.tipo=0";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Juego>(busqueda, new { id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Uno Comparador", ex);
			}

			return null;
		}

		public static async Task<List<Juego>> MultiplesJuegos(List<string> ids)
        {
			if (ids?.Count == 0)
			{
				return null;
			}

			List<int> idsBaseDatos = ids.Where(id => string.IsNullOrWhiteSpace(id) == false).Select(j => int.Parse(j)).ToList();

			string sqlBuscar = @"SELECT 
        j.id, j.nombre, j.imagenes, j.precioMinimosHistoricos, j.precioActualesTiendas, j.media,
        j.tipo, j.analisis, j.idSteam, j.idGog, j.idAmazon,
        j.exeEpic, j.exeUbisoft, j.freeToPlay,
		(
			SELECT b.id, b.bundleTipo
			FROM bundles b
			INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
			WHERE bj.juegoId = j.id
				AND b.fechaEmpieza <= GETDATE()
				AND b.fechaTermina >= GETDATE()
			FOR JSON PATH
		) AS BundlesActuales,
        (
            SELECT g.gratis
            FROM gratis g
            WHERE g.juegoId = j.id
              AND g.fechaEmpieza <= GETDATE()
              AND g.fechaTermina >= GETDATE()
            FOR JSON PATH
        ) AS GratisActuales,
        (
            SELECT s.suscripcion
            FROM suscripciones s
            WHERE s.juegoId = j.id
              AND s.FechaEmpieza <= GETDATE()
              AND s.FechaTermina >= GETDATE()
            FOR JSON PATH
        ) AS SuscripcionesActuales
    FROM juegos j
    WHERE id IN @Ids
    ORDER BY CASE
        WHEN analisis = 'null' OR analisis IS NULL THEN 0 
        ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))
    END DESC";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
					(await conexion.QueryAsync<Juego>(sqlBuscar, new { Ids = idsBaseDatos })).ToList()
				);
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Multiples", ex);
				return null;
			}
        }

		public static async Task<List<Juego>> MultiplesJuegosReducido(List<string> ids)
		{
			if (ids?.Count == 0)
			{
				return null;
			}

			List<int> idsBaseDatos = ids.Select(j => int.Parse(j)).ToList();

			string sqlBuscar = @"SELECT 
					j.id, j.nombre, j.imagenes, 
					j.tipo, j.analisis, j.idSteam
				FROM juegos j
				WHERE id IN @Ids
				ORDER BY j.nombre";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
					(await conexion.QueryAsync<Juego>(sqlBuscar, new { Ids = idsBaseDatos })).ToList()
				);
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Multiples Reducido", ex);
				return null;
			}
		}

		public static async Task<List<Juego>> MultiplesJuegos(TiendaRegion region, List<JuegoDeseado> ids)
        {
			if (ids?.Count == 0)
			{
				return null;
			}

			string precioMinimosHistoricos = region switch
			{
				TiendaRegion.Europa => "precioMinimosHistoricos",
				TiendaRegion.EstadosUnidos => "precioMinimosHistoricosUS",
				_ => string.Empty
			};

			string precioActualesTiendas = region switch
			{
				TiendaRegion.Europa => "precioActualesTiendas",
				TiendaRegion.EstadosUnidos => "precioActualesTiendasUS",
				_ => string.Empty
			};

			var idsBaseDatos = ids.Select(j => int.Parse(j.IdBaseDatos)).ToList();

			string sqlBuscar = $@"SELECT 
        j.id, j.nombre, j.imagenes, j.{precioMinimosHistoricos}, j.{precioActualesTiendas}, j.media,
        j.tipo, j.analisis, j.idSteam, j.idGog, j.idAmazon,
        j.exeEpic, j.exeUbisoft, j.freeToPlay,
		(
			SELECT b.id, b.bundleTipo
			FROM bundles b
			INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
			WHERE bj.juegoId = j.id
				AND b.fechaEmpieza <= GETDATE()
				AND b.fechaTermina >= GETDATE()
			FOR JSON PATH
		) AS BundlesActuales,
        (
            SELECT g.gratis
            FROM gratis g
            WHERE g.juegoId = j.id
              AND g.fechaEmpieza <= GETDATE()
              AND g.fechaTermina >= GETDATE()
            FOR JSON PATH
        ) AS GratisActuales,
        (
            SELECT s.suscripcion
            FROM suscripciones s
            WHERE s.juegoId = j.id
              AND s.FechaEmpieza <= GETDATE()
              AND s.FechaTermina >= GETDATE()
            FOR JSON PATH
        ) AS SuscripcionesActuales
    FROM juegos j
    WHERE id IN @Ids
    ORDER BY CASE
        WHEN analisis = 'null' OR analisis IS NULL THEN 0 
        ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))
    END DESC";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
					(await conexion.QueryAsync<Juego>(sqlBuscar, new { Ids = idsBaseDatos })).ToList()
				);
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Multiples", ex);
				return null;
			}
        }

		public static async Task<List<Juego>> MultiplesJuegosSteam2(TiendaRegion region, List<int> ids)
		{
			if (ids == null || ids?.Count == 0)
			{
				return null;
			}

			string precioMinimosHistoricos = region switch
			{
				TiendaRegion.Europa => "precioMinimosHistoricos",
				TiendaRegion.EstadosUnidos => "precioMinimosHistoricosUS",
				_ => string.Empty
			};

			string precioActualesTiendas = region switch
			{
				TiendaRegion.Europa => "precioActualesTiendas",
				TiendaRegion.EstadosUnidos => "precioActualesTiendasUS",
				_ => string.Empty
			};

			string sqlBuscar = $@"SELECT 
        j.id, j.nombre, j.imagenes, j.{precioMinimosHistoricos}, j.{precioActualesTiendas}, j.media, j.etiquetas, 
        j.tipo, j.analisis, j.idSteam, j.idGog, j.idAmazon,
        j.exeEpic, j.exeUbisoft, j.freeToPlay,
		(
			SELECT b.id, b.bundleTipo
			FROM bundles b
			INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
			WHERE bj.juegoId = j.id
				AND b.fechaEmpieza <= GETDATE()
				AND b.fechaTermina >= GETDATE()
			FOR JSON PATH
		) AS BundlesActuales,
		(
			SELECT b.id, b.bundleTipo
			FROM bundles b
			INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
			WHERE bj.juegoId = j.id
				AND b.fechaTermina < GETDATE()
			FOR JSON PATH
		) AS BundlesPasados,
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
		FROM juegos j
		WHERE idSteam IN @Ids
		ORDER BY CASE
			WHEN analisis = 'null' OR analisis IS NULL THEN 0 
			ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))
		END DESC";

			try
			{
				const int loteTamaño = 2000;
				List<Juego> resultados = new List<Juego>();

				for (int i = 0; i < ids.Count; i += loteTamaño)
				{
					List<int> lote = ids.Skip(i).Take(loteTamaño).ToList();

					List<Juego> juegosDeLote = await Herramientas.BaseDatos.Select(async conexion =>
						(await conexion.QueryAsync<Juego>(sqlBuscar, new { Ids = lote })).ToList()
					);

					resultados.AddRange(juegosDeLote);
				}

				return resultados;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Multiples Steam", ex);
				return null;
			}
		}

		public static async Task<List<int>> MultiplesJuegosSteamOrdenado(List<int> ids)
		{
			if (ids?.Count == 0)
			{
				return null;
			}

			string sqlBuscar = @"SELECT idSteam FROM juegos WHERE idSteam IN @Ids 
ORDER BY CASE
WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))
END DESC";

			try
			{
				const int tamañoLote = 2000;
				List<int> resultados = new List<int>();

				var lotes = ids
					.Select((id, index) => new { id, index })
					.GroupBy(x => x.index / tamañoLote)
					.Select(g => g.Select(x => x.id).ToList())
					.ToList();

				foreach (var lote in lotes)
				{
					var loteResultados = await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<int>(sqlBuscar, new { Ids = lote })).ToList();
					});

					if (loteResultados != null)
					{
						resultados.AddRange(loteResultados);
					}
				}

				return resultados;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Multiples Steam Ordenado", ex);
			}

			return null;
		}

		public static async Task<List<Juego>> MultiplesJuegosGOG(TiendaRegion region, List<string> ids)
		{
			if (ids?.Count == 0)
			{
				return null;
			}

			string precioMinimosHistoricos = region switch
			{
				TiendaRegion.Europa => "precioMinimosHistoricos",
				TiendaRegion.EstadosUnidos => "precioMinimosHistoricosUS",
				_ => string.Empty
			};

			string precioActualesTiendas = region switch
			{
				TiendaRegion.Europa => "precioActualesTiendas",
				TiendaRegion.EstadosUnidos => "precioActualesTiendasUS",
				_ => string.Empty
			};

			var idsBaseDatos = ids.Select(j => int.Parse(j)).ToList();

			string sqlBuscar = $@"SELECT 
        j.id, j.nombre, j.imagenes, j.{precioMinimosHistoricos}, j.{precioActualesTiendas}, j.media,
        j.tipo, j.analisis, j.idSteam, j.idGog, j.idAmazon,
        j.exeEpic, j.exeUbisoft, j.freeToPlay,
		(
			SELECT b.id, b.bundleTipo
			FROM bundles b
			INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
			WHERE bj.juegoId = j.id
				AND b.fechaEmpieza <= GETDATE()
				AND b.fechaTermina >= GETDATE()
			FOR JSON PATH
		) AS BundlesActuales,
        (
            SELECT g.gratis
            FROM gratis g
            WHERE g.juegoId = j.id
              AND g.fechaEmpieza <= GETDATE()
              AND g.fechaTermina >= GETDATE()
            FOR JSON PATH
        ) AS GratisActuales,
        (
            SELECT s.suscripcion
            FROM suscripciones s
            WHERE s.juegoId = j.id
              AND s.FechaEmpieza <= GETDATE()
              AND s.FechaTermina >= GETDATE()
            FOR JSON PATH
        ) AS SuscripcionesActuales
    FROM juegos j
    WHERE idGOG IN @Ids
    ORDER BY CASE
        WHEN analisis = 'null' OR analisis IS NULL THEN 0 
        ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))
    END DESC";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
					(await conexion.QueryAsync<Juego>(sqlBuscar, new { Ids = idsBaseDatos })).ToList()
				);
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Multiples GOG", ex);
				return null;
			}
		}

		public static async Task<List<JuegoUsuario>> MultiplesJuegosUsuario(List<JuegoUsuario> juegos, JuegoDRM drm, List<string> ids)
		{
			bool cogerNumero = false;
			string campo = string.Empty;

			if (drm == JuegoDRM.Steam)
			{
				cogerNumero = true;
				campo = "idSteam";
			}
			else if (drm == JuegoDRM.GOG)
			{
				cogerNumero = true;
				campo = "idGOG";
			}
			else if (drm == JuegoDRM.Amazon)
			{
				campo = "idAmazon";
			}
			else if (drm == JuegoDRM.Epic)
			{
				campo = "exeEpic";
			}
			else if (drm == JuegoDRM.Ubisoft)
			{
				campo = "exeUbisoft";
			}
			else if (drm == JuegoDRM.EA)
			{
				campo = "exeEA";
			}

			if (string.IsNullOrEmpty(campo) == false)
			{
				if (ids != null)
				{
					string sqlBuscar = string.Empty;

					if (ids.Count > 0)
					{
						sqlBuscar = "SELECT id, nombre, JSON_VALUE(imagenes, '$.Capsule_231x87'), " + campo + " FROM juegos WHERE " + campo + " IN (";

						int i = 0;
						while (i < ids.Count)
						{
							if (i == 0)
							{
								sqlBuscar = sqlBuscar + "'" + ids[i] + "'";
							}
							else
							{
								sqlBuscar = sqlBuscar + ", '" + ids[i] + "'";
							}

							i += 1;
						}

						sqlBuscar = sqlBuscar + ")";
					}

					if (string.IsNullOrEmpty(sqlBuscar) == false)
					{
						try
						{
							var resultados = await Herramientas.BaseDatos.Select(async conexion =>
							{
								return (await conexion.QueryAsync<(int id, string nombre, string imagen, object drmValor)>(sqlBuscar)).ToList();
							});

							foreach (var fila in resultados)
							{
								var existente = juegos?.FirstOrDefault(j => j.Id == fila.id);

								string drmId = null;

								if (fila.drmValor != null && fila.drmValor is not DBNull)
								{
									drmId = cogerNumero ? fila.drmValor.ToString() : (string)fila.drmValor;
								}

								if (existente != null)
								{
									existente.DRMs.Add(new JuegoUsuarioDRM
									{
										DRM = drm,
										Id = drmId
									});

									continue;
								}

								var nuevo = new JuegoUsuario
								{
									Id = fila.id,
									Nombre = fila.nombre,
									Imagen = fila.imagen,
									DRMs = new List<JuegoUsuarioDRM>
									{
										new JuegoUsuarioDRM
										{
											DRM = drm,
											Id = drmId
										}
									}
								};

								juegos.Add(nuevo);
							}
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Juego Multiples Usuario", ex);
						}
					}
				}
			}

			return juegos;
		}

		public static async Task<List<Juego>> Nombre2(TiendaRegion region, string nombre, int cantidadJuegos = 10, bool reducido = false, bool añadirCurators = false, int cantidadCurators = 10, bool añadirBundles = false, int cantidadBundles = 10)
		{
			string precioMinimosHistoricos = region switch
			{
				TiendaRegion.Europa => "precioMinimosHistoricos",
				TiendaRegion.EstadosUnidos => "precioMinimosHistoricosUS",
				_ => string.Empty
			};

			string precioActualesTiendas = region switch
			{
				TiendaRegion.Europa => "precioActualesTiendas",
				TiendaRegion.EstadosUnidos => "precioActualesTiendasUS",
				_ => string.Empty
			};

			string busquedaJuegos = $@"SELECT TOP (@cantidadJuegos) 
				j.id, j.nombre, j.imagenes, j.{precioMinimosHistoricos}, j.{precioActualesTiendas},
				j.tipo, j.analisis, j.idSteam, j.idGog, j.idAmazon,
				j.exeEpic, j.exeUbisoft, j.freeToPlay,
				(
					SELECT b.id, b.bundleTipo
					FROM bundles b
					INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
					WHERE bj.juegoId = j.id
						AND b.fechaEmpieza <= GETDATE()
						AND b.fechaTermina >= GETDATE()
					FOR JSON PATH
				) AS BundlesActuales,
				(
					SELECT g.gratis
					FROM gratis g
					WHERE g.juegoId = j.id
					  AND g.fechaEmpieza <= GETDATE()
					  AND g.fechaTermina >= GETDATE()
					FOR JSON PATH
				) AS GratisActuales,
				(
					SELECT s.suscripcion
					FROM suscripciones s
					WHERE s.juegoId = j.id
					  AND s.FechaEmpieza <= GETDATE()
					  AND s.FechaTermina >= GETDATE()
					FOR JSON PATH
				) AS SuscripcionesActuales
			FROM juegos j
			WHERE 1=1";

			if (reducido == true)
			{
				busquedaJuegos = @"SELECT TOP (@cantidadJuegos) 
								j.id, j.nombre, j.imagenes, j.tipo, j.nombreCodigo
							FROM juegos j
							WHERE 1=1";
			}

			string busquedaCurators = string.Empty;

			if (añadirCurators == true)
			{
				busquedaCurators = $@"SELECT TOP (@cantidadCurators)
                                    c.id, c.nombre, 
									JSON_QUERY(CONCAT('{{""Header_460x215"":""', c.imagen, '""}}')) AS imagenes,
                                    NULL AS {precioMinimosHistoricos}, 
                                    NULL AS {precioActualesTiendas},
                                    5 AS tipo, NULL AS analisis, 
                                    NULL AS idSteam, NULL AS idGog, NULL AS idAmazon,
                                    c.slug AS exeEpic, NULL AS exeUbisoft, NULL AS freeToPlay,
                                    NULL AS BundlesActuales,
                                    NULL AS GratisActuales,
                                    NULL AS SuscripcionesActuales
                                FROM curators c
                                WHERE 1=1";
			}

			string busquedaBundles = string.Empty;

			if (añadirBundles == true)
			{
				busquedaBundles = $@"SELECT TOP (@cantidadBundles)
                                    b.id, b.nombre, 
									JSON_QUERY(CONCAT('{{""Header_460x215"":""', b.imagenNoticia, '""}}')) AS imagenes,
                                    NULL AS {precioMinimosHistoricos}, 
                                    NULL AS {precioActualesTiendas},
                                    2 AS tipo, NULL AS analisis, 
                                    b.bundleTipo AS idSteam, NULL AS idGog, NULL AS idAmazon,
                                    NULL AS exeEpic, NULL AS exeUbisoft, NULL AS freeToPlay,
                                    NULL AS BundlesActuales,
                                    NULL AS GratisActuales,
                                    NULL AS SuscripcionesActuales
                                FROM bundles b
                                WHERE 1=1";
			}

			DynamicParameters parametros = new DynamicParameters();
			parametros.Add("cantidadJuegos", cantidadJuegos);

			if (añadirCurators == true)
			{
				parametros.Add("cantidadCurators", cantidadCurators);
			}

			if (añadirBundles == true)
			{
				parametros.Add("cantidadBundles", cantidadBundles);
			}

			string condicionesJuegos = "";
			string condicionesCurators = "";
			string condicionesBundles = "";

			string[] palabras = nombre.Split(" ");
			int i = 0;

			foreach (var palabra in palabras)
			{
				if (string.IsNullOrEmpty(palabra) == false)
				{
					string palabraLimpia = Herramientas.Buscador.LimpiarNombre(palabra, true);

					string parametro = $"p{i}";
					condicionesJuegos += $" AND j.nombreCodigo LIKE '%' + @{parametro} + '%'";

					if (añadirCurators == true)
					{
						condicionesCurators += $" AND c.nombre LIKE '%' + @{parametro} + '%'";
					}

					if (añadirBundles == true)
					{
						condicionesBundles += $" AND b.nombre LIKE '%' + @{parametro} + '%'";
					}

					parametros.Add(parametro, palabraLimpia, DbType.String); 
					i++;
				}
			}

			string busquedaFinal = string.Empty;

			if (string.IsNullOrEmpty(busquedaCurators) == true && string.IsNullOrEmpty(busquedaBundles) == true)
			{
				busquedaFinal = $@"{busquedaJuegos}{condicionesJuegos}
                    ORDER BY CASE 
                        WHEN j.analisis = 'null' OR j.analisis IS NULL THEN 0 
                        ELSE CONVERT(int, REPLACE(JSON_VALUE(j.analisis, '$.Cantidad'),',',''))
                    END DESC";
			}
			else
			{
				busquedaFinal = $@"
					SELECT * FROM (
						SELECT TOP (@cantidadJuegos) * FROM (
							{busquedaJuegos}{condicionesJuegos}
						) AS Juegos
						ORDER BY CASE 
							WHEN analisis = 'null' OR analisis IS NULL THEN 0 
							ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))
						END DESC
					) AS JuegosTop
    
					UNION ALL
    
					SELECT * FROM (
						SELECT TOP (@cantidadCurators) * FROM (
							{busquedaCurators}{condicionesCurators}
						) AS Curators
						ORDER BY id DESC
					) AS CuratorsTop

					UNION ALL
    
					SELECT * FROM (
						SELECT TOP (@cantidadBundles) * FROM (
							{busquedaBundles}{condicionesBundles}
						) AS Bundles
						ORDER BY id DESC
					) AS BundlesTop";
			}

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Juego>(busquedaFinal, parametros)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Nombre", ex);
			}

			return null;
		}

		public static async Task<(List<Juego> juegos, int excluidos)> Nombre3(bool noOficiales, TiendaRegion region, string nombre, int cantidadJuegos = 10,
			int cantidadCurators = 10, int cantidadBundles = 10, List<int> excluirJuegosIds = null)
		{
			string precioMinimosHistoricos = region switch
			{
				TiendaRegion.Europa => "precioMinimosHistoricos",
				TiendaRegion.EstadosUnidos => "precioMinimosHistoricosUS",
				_ => string.Empty
			};

			string precioActualesTiendas = region switch
			{
				TiendaRegion.Europa => "precioActualesTiendas",
				TiendaRegion.EstadosUnidos => "precioActualesTiendasUS",
				_ => string.Empty
			};

			string noOficialesSelect = string.Empty;
			string noOficialesSelectNull = string.Empty;

			if (noOficiales == true)
			{
				noOficialesSelect = region switch
				{
					TiendaRegion.Europa => "j.preciosHistoricosNoOficialesEU, j.preciosActualesNoOficialesEU,",
					TiendaRegion.EstadosUnidos => "j.preciosHistoricosNoOficialesUS, j.preciosActualesNoOficialesUS,",
					_ => string.Empty
				};

				noOficialesSelectNull = region switch
				{
					TiendaRegion.Europa => "NULL AS preciosHistoricosNoOficialesEU, NULL AS preciosActualesNoOficialesEU,",
					TiendaRegion.EstadosUnidos => "NULL AS preciosHistoricosNoOficialesUS, NULL AS preciosActualesNoOficialesUS,",
					_ => string.Empty
				};
			}

			string busquedaJuegos = $@"SELECT TOP (@cantidadJuegos) 
				j.id, j.nombre, j.imagenes, j.{precioMinimosHistoricos}, j.{precioActualesTiendas}, {noOficialesSelect}
				j.tipo, j.analisis, j.idSteam, j.idGog, j.idAmazon,
				j.exeEpic, j.exeUbisoft, j.freeToPlay,
				(
					SELECT b.id, b.bundleTipo
					FROM bundles b
					INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
					WHERE bj.juegoId = j.id
						AND b.fechaEmpieza <= GETDATE()
						AND b.fechaTermina >= GETDATE()
					FOR JSON PATH
				) AS BundlesActuales,
				(
					SELECT g.gratis
					FROM gratis g
					WHERE g.juegoId = j.id
					  AND g.fechaEmpieza <= GETDATE()
					  AND g.fechaTermina >= GETDATE()
					FOR JSON PATH
				) AS GratisActuales,
				(
					SELECT s.suscripcion
					FROM suscripciones s
					WHERE s.juegoId = j.id
					  AND s.FechaEmpieza <= GETDATE()
					  AND s.FechaTermina >= GETDATE()
					FOR JSON PATH
				) AS SuscripcionesActuales
			FROM juegos j
			WHERE 1=1";

			string busquedaCurators = $@"SELECT TOP (@cantidadCurators)
                                    c.id, c.nombre, 
									JSON_QUERY(CONCAT('{{""Header_460x215"":""', c.imagen, '""}}')) AS imagenes,
                                    NULL AS {precioMinimosHistoricos}, 
                                    NULL AS {precioActualesTiendas},
									{noOficialesSelectNull}
                                    5 AS tipo, NULL AS analisis, 
                                    NULL AS idSteam, NULL AS idGog, NULL AS idAmazon,
                                    c.slug AS exeEpic, NULL AS exeUbisoft, NULL AS freeToPlay,
                                    NULL AS BundlesActuales,
                                    NULL AS GratisActuales,
                                    NULL AS SuscripcionesActuales
                                FROM curators c
                                WHERE 1=1";

			string busquedaBundles = $@"SELECT TOP (@cantidadBundles)
                                    b.id, b.nombre, 
									JSON_QUERY(CONCAT('{{""Header_460x215"":""', b.imagenNoticia, '""}}')) AS imagenes,
                                    NULL AS {precioMinimosHistoricos}, 
                                    NULL AS {precioActualesTiendas},
									{noOficialesSelectNull}
                                    2 AS tipo, NULL AS analisis, 
                                    b.bundleTipo AS idSteam, NULL AS idGog, NULL AS idAmazon,
                                    NULL AS exeEpic, NULL AS exeUbisoft, NULL AS freeToPlay,
                                    NULL AS BundlesActuales,
                                    NULL AS GratisActuales,
                                    NULL AS SuscripcionesActuales
                                FROM bundles b
                                WHERE 1=1";

			DynamicParameters parametros = new DynamicParameters();
			parametros.Add("cantidadJuegos", cantidadJuegos);
			parametros.Add("cantidadCurators", cantidadCurators);
			parametros.Add("cantidadBundles", cantidadBundles);

			string exclusionJuegos = string.Empty;

			if (excluirJuegosIds?.Count > 0)
			{
				DataTable tabla = CrearDataTable(excluirJuegosIds);
				parametros.Add("excluirJuegos", tabla.AsTableValuedParameter("dbo.ListaIdsNumericos"));

				exclusionJuegos = " AND j.id NOT IN (SELECT Id FROM @excluirJuegos)";
			}

			string condicionesJuegos = "";
			string condicionesCurators = "";
			string condicionesBundles = "";
			string[] palabras = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			int i = 0;

			foreach (var palabra in palabras)
			{
				string palabraLimpia = Herramientas.Buscador.LimpiarNombre(palabra, true);
				string parametro = $"p{i}";

				condicionesJuegos += $" AND j.nombreCodigo LIKE '%' + @{parametro} + '%'";
				condicionesCurators += $" AND c.nombre LIKE '%' + @{parametro} + '%'";
				condicionesBundles += $" AND b.nombre LIKE '%' + @{parametro} + '%'";

				parametros.Add(parametro, palabraLimpia);

				i += 1;
			}

			string busqueda = $@"
				SELECT * FROM (
					SELECT TOP (@cantidadJuegos) * FROM (
						{busquedaJuegos}{exclusionJuegos}{condicionesJuegos}
					) AS Juegos
					ORDER BY CASE 
						WHEN analisis = 'null' OR analisis IS NULL THEN 0 
						ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))
					END DESC
				) AS JuegosTop
 
				UNION ALL
 
				SELECT * FROM (
					SELECT TOP (@cantidadCurators) * FROM (
						{busquedaCurators}{condicionesCurators}
					) AS Curators
					ORDER BY id DESC
				) AS CuratorsTop
 
				UNION ALL
 
				SELECT * FROM (
					SELECT TOP (@cantidadBundles) * FROM (
						{busquedaBundles}{condicionesBundles}
					) AS Bundles
					ORDER BY id DESC
				) AS BundlesTop;
			";

			if (excluirJuegosIds?.Count > 0)
			{
				busqueda = busqueda + $@" SELECT COUNT(*) 
					FROM juegos j
					WHERE 1=1 {condicionesJuegos} AND j.id IN (SELECT Id FROM @excluirJuegos)";
			}

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					using var multi = await conexion.QueryMultipleAsync(busqueda, parametros);

					List<Juego> juegos = (await multi.ReadAsync<Juego>()).ToList();
					int excluidos = excluirJuegosIds?.Count > 0 ? await multi.ReadFirstAsync<int>() : 0;

					return (juegos, excluidos);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Nombre3", ex);
			}

			return (null, 0);
		}

		public static async Task<List<Juego>> NombreComparador(string nombre, int cantidadResultados = 10)
		{
			string busqueda = @"SELECT TOP (@cantidad) 
    j.id, j.nombre, j.imagenes, j.precioMinimosHistoricos, j.precioActualesTiendas,
    j.tipo, j.analisis, j.idSteam, j.idGog, j.idAmazon,
    j.exeEpic, j.exeUbisoft, j.freeToPlay,
	(
		SELECT b.id, b.bundleTipo
		FROM bundles b
		INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
		WHERE bj.juegoId = j.id
			AND b.fechaEmpieza <= GETDATE()
			AND b.fechaTermina >= GETDATE()
		FOR JSON PATH
	) AS BundlesActuales,
	(
		SELECT b.id, b.bundleTipo
		FROM bundles b
		INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
		WHERE bj.juegoId = j.id
			AND b.fechaTermina < GETDATE()
		FOR JSON PATH
	) AS BundlesPasados,
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
FROM juegos j
WHERE 1=1 AND j.tipo=0";

			string[] palabras = nombre.Split(" ");

			foreach (var palabra in palabras)
			{
				if (string.IsNullOrEmpty(palabra) == false)
				{
					string palabraLimpia = Herramientas.Buscador.LimpiarNombre(palabra, true);

					busqueda = busqueda + $" AND nombreCodigo LIKE '%{palabraLimpia}%'";
				}
			}

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				busqueda = busqueda + @" ORDER BY CASE 
WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))
END DESC";
			}

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Juego>(busqueda, new { cantidad = cantidadResultados })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Comparador", ex);
			}

			return new List<Juego>();
		}

		public static async Task<List<Juego>> Nombre(string nombre, int cantidad = 30, bool todo = true, int tipo = -1, bool logeado = false, bool prioridad = true)
		{
			if (string.IsNullOrEmpty(nombre) == false)
			{
				string busqueda = string.Empty;
				string busquedaTodo = "*";

				if (todo == false)
				{
					busquedaTodo = "id, nombre, imagenes, precioMinimosHistoricos, precioActualesTiendas, bundles, gratis, suscripciones, tipo, analisis, idSteam, idGog, idAmazon, exeEpic, exeUbisoft, freeToPlay";
				}

				if (nombre.Contains(" ") == true)
				{
					if (nombre.Contains("  ") == true)
					{
						nombre = nombre.Replace("  ", " ");
					}

					string[] palabras = nombre.Split(" ");

					int i = 0;
					foreach (var palabra in palabras)
					{
						if (string.IsNullOrEmpty(palabra) == false)
						{
							string palabraLimpia = Herramientas.Buscador.LimpiarNombre(palabra, true);

							if (palabraLimpia.Length > 0)
							{
								if (i == 0)
								{
									busqueda = "SELECT TOP " + cantidad + " " + busquedaTodo + " FROM juegos WHERE CHARINDEX('" + palabraLimpia + "', nombreCodigo) > 0 ";
								}
								else
								{
									bool buscar = true;

									if (palabra.ToLower() == "and")
									{
										buscar = false;
									}
									else if (palabra.ToLower() == "dlc")
									{
										buscar = false;
									}
									if (palabra.ToLower() == "expansion")
									{
										buscar = false;
									}

									if (buscar == true)
									{
										busqueda = busqueda + " AND CHARINDEX('" + palabraLimpia + "', nombreCodigo) > 0 ";
									}
								}

								i += 1;
							}
						}
					}
				}
				else
				{
					busqueda = "SELECT TOP " + cantidad + " " + busquedaTodo + " FROM juegos WHERE nombreCodigo LIKE '%" + Herramientas.Buscador.LimpiarNombre(nombre) + "%'";
				}

				if (tipo > -1)
				{
					busqueda = busqueda + " AND tipo = " + tipo.ToString();
				}

				if (logeado == false)
				{
					busqueda = busqueda + " AND (mayorEdad='false' OR mayorEdad IS NULL)";
				}

				if (string.IsNullOrEmpty(busqueda) == false)
				{
					busqueda = busqueda + " ORDER BY CASE\r\n WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))\r\n END DESC";
				}

				if (prioridad == true)
				{
					busqueda = busqueda + " OPTION (MAXDOP 8);";
				}

				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(busqueda)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Nombre", ex);
				}
			}

			return null;
		}

		private static DataTable CrearDataTable(List<int> ids)
		{
			DataTable tabla = new DataTable();
			tabla.Columns.Add("Id", typeof(int));

			foreach (var id in ids)
			{
				tabla.Rows.Add(id);
			}

			return tabla;
		}

		public static async Task<List<Juego>> Minimos(bool noOficial, TiendaRegion region, int posicion = 0, int ordenar = 0, List<MostrarJuegoTienda> tiendas = null, List<MostrarJuegoDRM> drms = null, List<MostrarJuegoTipo> tipos = null, List<string> categorias = null, List<string> etiquetas = null, int? minimoDescuento = null, int? maximoPrecio = null, List<MostrarJuegoSteamDeck> deck = null, List<MostrarJuegoSteamOS> steamos = null, List<MostrarJuegoSteamMachine> machine = null, List<MostrarJuegoSteamFrame> frame = null, int lanzamiento = 0, int? minimoReseñas = 0, string nombreBusqueda = null, List<int> excluirJuegosIds = null, List<int> excluirSteamIds = null, List<int> excluirGogIds = null)
		{
			string tablaMinimos = region == TiendaRegion.Europa ? "seccionMinimos" : "seccionMinimosUS";
			string precioMinimosHistoricos = region == TiendaRegion.Europa ? "precioMinimosHistoricos" : "precioMinimosHistoricosUS";
			string columnaNoOficialNombre = region == TiendaRegion.Europa ? "preciosHistoricosNoOficialesEU" : "preciosHistoricosNoOficialesUS";

			DynamicParameters parametros = new DynamicParameters();
			parametros.Add("etiquetas2", etiquetas?.Count > 0 ? string.Join(",", etiquetas) : "");

			string exclusionJuegos = string.Empty;
			string exclusionSteam = string.Empty;
			string exclusionGog = string.Empty;

			if (excluirJuegosIds?.Count > 0)
			{
				DataTable tablaJuegos = CrearDataTable(excluirJuegosIds);
				parametros.Add("excluirJuegos", tablaJuegos.AsTableValuedParameter("dbo.ListaIdsNumericos"));
				exclusionJuegos = $" NOT EXISTS (SELECT 1 FROM @excluirJuegos WHERE Id = j.idMaestra)";
			}

			if (excluirSteamIds?.Count > 0)
			{
				DataTable tablaSteam = CrearDataTable(excluirSteamIds);
				parametros.Add("excluirSteam", tablaSteam.AsTableValuedParameter("dbo.ListaIdsNumericos"));
				exclusionSteam = $" NOT EXISTS (SELECT 1 FROM @excluirSteam WHERE Id = jg.idSteam AND pmh.DRM = '0')";
			}

			if (excluirGogIds?.Count > 0)
			{
				DataTable tablaGog = CrearDataTable(excluirGogIds);
				parametros.Add("excluirGog", tablaGog.AsTableValuedParameter("dbo.ListaIdsNumericos"));
				exclusionGog = $" NOT EXISTS (SELECT 1 FROM @excluirGog WHERE Id = jg.idGog AND pmh.DRM = '8')";
			}

			string dondeTiendas = string.Empty;

			#region Where

			if (tiendas?.Count > 0)
			{
				foreach (var tienda in tiendas)
				{
					if (tienda.Estado == true)
					{
						if (string.IsNullOrEmpty(dondeTiendas) == false)
						{
							dondeTiendas = dondeTiendas + " OR ";
						}

						dondeTiendas = dondeTiendas + "pmh.Tienda = '" + tienda.TiendaId + "'";
					}
				}
			}

			if (string.IsNullOrEmpty(dondeTiendas) == false)
			{
				dondeTiendas = " (" + dondeTiendas + ")";
			}

			string dondeDRMs = string.Empty;

			if (drms?.Count > 0)
			{
				foreach (var drm in drms)
				{
					if (drm.Estado == true)
					{
						if (string.IsNullOrEmpty(dondeDRMs) == false)
						{
							dondeDRMs = dondeDRMs + " OR ";
						}

						dondeDRMs = dondeDRMs + "pmh.DRM = '" + ((int)drm.DRMId).ToString() + "'";
					}
				}
			}

			if (string.IsNullOrEmpty(dondeDRMs) == false)
			{
				dondeDRMs = " (" + dondeDRMs + ")";
			}

			string dondeTipos = string.Empty;

			if (tipos?.Count > 0)
			{
				foreach (var tipo in tipos)
				{
					if (tipo.Estado == true)
					{
						if (string.IsNullOrEmpty(dondeTipos) == false)
						{
							dondeTipos = dondeTipos + " OR ";
						}

						dondeTipos = dondeTipos + "jg.tipo = '" + ((int)tipo.Tipo).ToString() + "'";
					}
				}
			}

			if (string.IsNullOrEmpty(dondeTipos) == false)
			{
				dondeTipos = " (" + dondeTipos + ")";
			}

			string dondeCategorias = string.Empty;

			if (categorias?.Count > 0)
			{
				string categoriasFormateadas = "\"" + string.Join("\",\"", categorias) + "\"";
				dondeCategorias = $@" (
					SELECT COUNT(*) 
					FROM STRING_SPLIT(jg.categorias, ',') AS e1
					INNER JOIN STRING_SPLIT('{categoriasFormateadas}', ',') AS e2
					ON TRIM(e1.value) = TRIM(e2.value)
				) > 0";
			}

			string dondeEtiquetas = string.Empty;

			if (etiquetas?.Count > 0)
			{
				string etiquetasFormateadas = "\"" + string.Join("\",\"", etiquetas) + "\"";
				dondeEtiquetas = $@" (
					SELECT COUNT(*) 
					FROM STRING_SPLIT(jg.etiquetas, ',') AS e1
					INNER JOIN STRING_SPLIT('{etiquetasFormateadas}', ',') AS e2
					ON TRIM(e1.value) = TRIM(e2.value)
				) > 0";
			}

			string dondeMinimoDescuento = string.Empty;

			if (minimoDescuento == null)
			{
				minimoDescuento = 1;
			}

			if (minimoDescuento > 0)
			{
				dondeMinimoDescuento = "pmh.Descuento >= " + minimoDescuento.ToString();
			}

			if (string.IsNullOrEmpty(dondeMinimoDescuento) == false)
			{
				dondeMinimoDescuento = " (" + dondeMinimoDescuento + ")";
			}

			string dondeMaximoPrecio = string.Empty;

			if (maximoPrecio == null)
			{
				maximoPrecio = 90;
			}

			if (maximoPrecio > 0)
			{
				dondeMaximoPrecio = "pmh.Precio <= " + maximoPrecio.ToString();
			}

			if (string.IsNullOrEmpty(dondeMaximoPrecio) == false)
			{
				dondeMaximoPrecio = " (" + dondeMaximoPrecio + ")";
			}

			string dondeDeck = string.Empty;

			if (deck?.Count > 0)
			{
				foreach (var d in deck)
				{
					if (d.Estado == true)
					{
						if (string.IsNullOrEmpty(dondeDeck) == false)
						{
							dondeDeck = dondeDeck + " OR ";
						}

						dondeDeck = dondeDeck + "jg.deck = '" + ((int)d.Tipo).ToString() + "'";
					}
				}
			}

			if (string.IsNullOrEmpty(dondeDeck) == false)
			{
				dondeDeck = " (" + dondeDeck + ")";
			}

			string dondeSteamOS = string.Empty;

			if (steamos?.Count > 0)
			{
				foreach (var s in steamos)
				{
					if (s.Estado == true)
					{
						if (string.IsNullOrEmpty(dondeSteamOS) == false)
						{
							dondeSteamOS = dondeSteamOS + " OR ";
						}

						dondeSteamOS = dondeSteamOS + "jg.steamos = '" + ((int)s.Tipo).ToString() + "'";
					}
				}
			}

			if (string.IsNullOrEmpty(dondeSteamOS) == false)
			{
				dondeSteamOS = " (" + dondeSteamOS + ")";
			}

			string dondeSteamMachine = string.Empty;

			if (machine?.Count > 0)
			{
				foreach (var m in machine)
				{
					if (m.Estado == true)
					{
						if (string.IsNullOrEmpty(dondeSteamMachine) == false)
						{
							dondeSteamMachine = dondeSteamMachine + " OR ";
						}

						dondeSteamMachine = dondeSteamMachine + "jg.steammachine = '" + ((int)m.Tipo).ToString() + "'";
					}
				}
			}

			if (string.IsNullOrEmpty(dondeSteamMachine) == false)
			{
				dondeSteamMachine = " (" + dondeSteamMachine + ")";
			}

			string dondeSteamFrame = string.Empty;

			if (frame?.Count > 0)
			{
				foreach (var f in frame)
				{
					if (f.Estado == true)
					{
						if (string.IsNullOrEmpty(dondeSteamFrame) == false)
						{
							dondeSteamFrame = dondeSteamFrame + " OR ";
						}

						dondeSteamFrame = dondeSteamFrame + "jg.steamframe = '" + ((int)f.Tipo).ToString() + "'";
					}
				}
			}

			if (string.IsNullOrEmpty(dondeSteamFrame) == false)
			{
				dondeSteamFrame = " (" + dondeSteamFrame + ")";
			}

			#endregion

			string ConstruirBusqueda(string tablaOrigen, string columnaPrecio, bool esOficial)
			{
				string selectOficial = esOficial
					? $"j.{columnaPrecio} AS {precioMinimosHistoricos}"
					: $"NULL AS {precioMinimosHistoricos}";

				string selectNoOficial = esOficial
					? $"NULL AS {columnaNoOficialNombre}"
					: $"j.{columnaPrecio} AS {columnaNoOficialNombre}";

				string consulta = $@"SELECT j.idMaestra, jg.nombre, jg.imagenes, {selectOficial}, {selectNoOficial}, jg.Media,
					jg.tipo, jg.analisis, jg.idSteam, jg.idGog, jg.freeToPlay, jg.etiquetas,
					(
						SELECT b.id, b.bundleTipo
						FROM bundles b
						INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
						WHERE bj.juegoId = j.idMaestra
							AND b.fechaEmpieza <= GETDATE()
							AND b.fechaTermina >= GETDATE()
						FOR JSON PATH
					) AS BundlesActuales,
					(
						SELECT b.id, b.bundleTipo
						FROM bundles b
						INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
						WHERE bj.juegoId = j.idMaestra
							AND b.fechaTermina < GETDATE()
						FOR JSON PATH
					) AS BundlesPasados,
					(
						SELECT g.gratis
						FROM gratis g
						WHERE g.juegoId = j.idMaestra
							AND g.fechaEmpieza <= GETDATE()
							AND g.fechaTermina >= GETDATE()
						FOR JSON PATH
					) AS GratisActuales,
					(
						SELECT g.gratis
						FROM gratis g
						WHERE g.juegoId = j.idMaestra
							AND g.fechaTermina < GETDATE()
						FOR JSON PATH
					) AS GratisPasados,
					(
						SELECT s.suscripcion
						FROM suscripciones s
						WHERE s.juegoId = j.idMaestra
							AND s.FechaEmpieza <= GETDATE()
							AND s.FechaTermina >= GETDATE()
						FOR JSON PATH
					) AS SuscripcionesActuales,
					(
						SELECT s.suscripcion
						FROM suscripciones s
						WHERE s.juegoId = j.idMaestra
							AND s.FechaTermina < GETDATE()
						FOR JSON PATH
					) AS SuscripcionesPasados,
					CASE WHEN pmh.Precio IS NULL THEN 1000000 ELSE pmh.Precio END AS OrdenPrecio,
					CASE WHEN pmh.Descuento IS NULL THEN 0 ELSE pmh.Descuento END AS OrdenDescuento,
					CASE WHEN pmh.FechaDetectado IS NULL THEN DATEADD(YEAR, -20, CAST(GETDATE() as date)) ELSE pmh.FechaDetectado END AS OrdenFecha,
					CASE WHEN jg.analisis = 'null' OR jg.analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(jg.analisis, '$.Cantidad'),',','')) END AS OrdenReseñas,
					CASE WHEN jg.analisis = 'null' OR jg.analisis IS NULL THEN 0 ELSE CONVERT(int, JSON_VALUE(jg.analisis, '$.Porcentaje')) END AS OrdenPorcentaje,
					CASE WHEN jg.caracteristicas = 'null' OR jg.caracteristicas IS NULL THEN DATEADD(YEAR, -20, CAST(GETDATE() as date)) ELSE CAST(JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoSteam') AS date) END AS OrdenLanzamiento
				FROM {tablaOrigen} j
				LEFT JOIN dbo.juegos jg ON jg.id = j.idMaestra
				OUTER APPLY OPENJSON(j.{columnaPrecio}, '$[0]')
					WITH (
						Tienda         nvarchar(50) '$.Tienda',
						DRM            nvarchar(50) '$.DRM',
						Descuento      int          '$.Descuento',
						Precio         decimal(18,2) '$.Precio',
						FechaDetectado date         '$.FechaDetectado'
					) AS pmh";

				consulta = consulta + " WHERE " + string.Join(" AND ", new[] { dondeTiendas, dondeDRMs, dondeTipos, dondeCategorias, dondeEtiquetas, dondeMinimoDescuento, dondeMaximoPrecio, dondeDeck, dondeSteamOS, exclusionJuegos, exclusionSteam, exclusionGog, dondeSteamMachine, dondeSteamFrame }.Where(x => string.IsNullOrEmpty(x) == false));

				if (lanzamiento == 1)
				{
					consulta = consulta + " AND (JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoSteam') > DATEADD(MONTH, -6, CAST(GETDATE() as date)) OR JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoOriginal') > DATEADD(MONTH, -6, CAST(GETDATE() as date))) ";
				}

				if (lanzamiento == 2)
				{
					consulta = consulta + " AND (JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoSteam') > DATEADD(MONTH, -12, CAST(GETDATE() as date)) OR JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoOriginal') > DATEADD(MONTH, -12, CAST(GETDATE() as date))) ";
				}

				if (lanzamiento == 3)
				{
					consulta = consulta + " AND (JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoSteam') > DATEADD(MONTH, -24, CAST(GETDATE() as date)) OR JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoOriginal') > DATEADD(MONTH, -24, CAST(GETDATE() as date))) ";
				}

				if (minimoReseñas != null)
				{
					if (minimoReseñas > 0)
					{
						consulta = consulta + " AND jg.analisis IS NOT NULL and CONVERT(int, REPLACE(JSON_VALUE(jg.analisis, '$.Cantidad'),',','')) > " + minimoReseñas.ToString();
					}
				}

				if (string.IsNullOrEmpty(nombreBusqueda) == false)
				{
					consulta += $@" AND jg.nombre COLLATE Latin1_General_CI_AI LIKE '%{nombreBusqueda}%'";
				}

				return consulta;
			}

			string busqueda = ConstruirBusqueda(tablaMinimos, precioMinimosHistoricos, true);

			if (noOficial == true)
			{
				string tablaNoOficial = region == TiendaRegion.Europa ? "seccionMinimosNoOficialesEU" : "seccionMinimosNoOficialesUS";

				string busquedaNoOficial = ConstruirBusqueda(tablaNoOficial, columnaNoOficialNombre, false);

				busqueda = $"({busqueda}) UNION ALL ({busquedaNoOficial})";
			}

			#region Order

			if (ordenar == 0)
			{
				busqueda = busqueda + " ORDER BY OrdenReseñas DESC";
			}

			if (ordenar == 1)
			{
				busqueda = busqueda + " ORDER BY OrdenPorcentaje DESC";
			}

			if (ordenar == 2)
			{
				busqueda = busqueda + " ORDER BY nombre";
			}

			if (ordenar == 3)
			{
				busqueda = busqueda + " ORDER BY nombre DESC";
			}

			if (ordenar == 4)
			{
				busqueda = busqueda + " ORDER BY OrdenPrecio";
			}

			if (ordenar == 5)
			{
				busqueda = busqueda + " ORDER BY OrdenDescuento DESC";
			}

			if (ordenar == 6)
			{
				busqueda = busqueda + " ORDER BY OrdenFecha DESC";
			}

			if (ordenar == 7)
			{
				busqueda = busqueda + " ORDER BY OrdenLanzamiento DESC";
			}

			#endregion

			busqueda = busqueda + @$" OFFSET {posicion} ROWS
					FETCH NEXT 100 ROWS ONLY";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Juego>(busqueda, parametros)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Minimos", ex);
			}

			return null;
		}

		public static async Task<List<Juego>> MinimosStreaming(bool noOficial, TiendaRegion region, string tabla, JuegoDRM drm, int posicion = 0, int? minimoDescuento = null, decimal? maximoPrecio = null, int? minimoReseñas = 0, string nombreBusqueda = null, List<int> excluirJuegosIds = null, List<int> excluirSteamIds = null, List<int> excluirGogIds = null)
		{
			string tablaMinimos = region == TiendaRegion.Europa ? "seccionMinimos" : "seccionMinimosUS";
			string precioMinimosHistoricos = region == TiendaRegion.Europa ? "precioMinimosHistoricos" : "precioMinimosHistoricosUS";

			DynamicParameters parametros = new DynamicParameters();

			string exclusionJuegos = string.Empty;
			string exclusionSteam = string.Empty;
			string exclusionGog = string.Empty;

			if (excluirJuegosIds?.Count > 0)
			{
				DataTable tablaJuegos = CrearDataTable(excluirJuegosIds);
				parametros.Add("excluirJuegos", tablaJuegos.AsTableValuedParameter("dbo.ListaIdsNumericos"));
				exclusionJuegos = $" AND NOT EXISTS (SELECT 1 FROM @excluirJuegos WHERE Id = j.idMaestra)";
			}

			if (excluirSteamIds?.Count > 0)
			{
				DataTable tablaSteam = CrearDataTable(excluirSteamIds);
				parametros.Add("excluirSteam", tablaSteam.AsTableValuedParameter("dbo.ListaIdsNumericos"));
				exclusionSteam = $" AND NOT EXISTS (SELECT 1 FROM @excluirSteam WHERE Id = jg.idSteam AND pmh.DRM = '0')";
			}

			if (excluirGogIds?.Count > 0)
			{
				DataTable tablaGog = CrearDataTable(excluirGogIds);
				parametros.Add("excluirGog", tablaGog.AsTableValuedParameter("dbo.ListaIdsNumericos"));
				exclusionGog = $" AND NOT EXISTS (SELECT 1 FROM @excluirGog WHERE Id = jg.idGog AND pmh.DRM = '8')";
			}

			string dondeMinimoDescuento = string.Empty;

			if (minimoDescuento == null)
			{
				minimoDescuento = 1;
			}

			if (minimoDescuento > 0)
			{
				dondeMinimoDescuento = "pmh.Descuento >= " + minimoDescuento.ToString();
			}

			if (string.IsNullOrEmpty(dondeMinimoDescuento) == false)
			{
				dondeMinimoDescuento = " (" + dondeMinimoDescuento + ")";
			}

			string dondeMaximoPrecio = string.Empty;

			if (maximoPrecio == null)
			{
				maximoPrecio = 90;
			}

			if (maximoPrecio > 0)
			{
				dondeMaximoPrecio = "pmh.Precio <= " + maximoPrecio.ToString();
			}

			if (string.IsNullOrEmpty(dondeMaximoPrecio) == false)
			{
				dondeMaximoPrecio = " (" + dondeMaximoPrecio + ")";
			}

			string ConstruirBusqueda(string tablaOrigen, string columnaPrecio)
			{
				string consulta = $@"SELECT j.idMaestra, jg.nombre, jg.imagenes, j.{columnaPrecio}, jg.Media,
					jg.tipo, jg.analisis, jg.idSteam, jg.idGog, jg.freeToPlay, jg.etiquetas,
					(
						SELECT b.id, b.bundleTipo
						FROM bundles b
						INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
						WHERE bj.juegoId = j.idMaestra
							AND b.fechaEmpieza <= GETDATE()
							AND b.fechaTermina >= GETDATE()
						FOR JSON PATH
					) AS BundlesActuales,
					(
						SELECT b.id, b.bundleTipo
						FROM bundles b
						INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
						WHERE bj.juegoId = j.idMaestra
							AND b.fechaTermina < GETDATE()
						FOR JSON PATH
					) AS BundlesPasados,
					(
						SELECT g.gratis
						FROM gratis g
						WHERE g.juegoId = j.idMaestra
							AND g.fechaEmpieza <= GETDATE()
							AND g.fechaTermina >= GETDATE()
						FOR JSON PATH
					) AS GratisActuales,
					(
						SELECT g.gratis
						FROM gratis g
						WHERE g.juegoId = j.idMaestra
							AND g.fechaTermina < GETDATE()
						FOR JSON PATH
					) AS GratisPasados,
					(
						SELECT s.suscripcion
						FROM suscripciones s
						WHERE s.juegoId = j.idMaestra
							AND s.FechaEmpieza <= GETDATE()
							AND s.FechaTermina >= GETDATE()
						FOR JSON PATH
					) AS SuscripcionesActuales,
					(
						SELECT s.suscripcion
						FROM suscripciones s
						WHERE s.juegoId = j.idMaestra
							AND s.FechaTermina < GETDATE()
						FOR JSON PATH
					) AS SuscripcionesPasados,
					CASE
						WHEN jg.analisis = 'null' OR jg.analisis IS NULL THEN 0
						ELSE CONVERT(int, REPLACE(JSON_VALUE(jg.analisis, '$.Cantidad'),',',''))
					END AS OrdenReseñas
				FROM {tablaOrigen} j
				LEFT JOIN dbo.juegos jg ON jg.id = j.idMaestra
				OUTER APPLY OPENJSON(j.{columnaPrecio}, '$[0]')
					WITH (
						DRM       nvarchar(50)  '$.DRM',
						Descuento int           '$.Descuento',
						Precio    decimal(18,2) '$.Precio'
					) AS pmh
				WHERE jg.Tipo = 0 {exclusionJuegos} {exclusionSteam} {exclusionGog}
				AND EXISTS (
					SELECT 1
					FROM {tabla} sgn
					WHERE sgn.idJuego = j.idMaestra
						AND sgn.fecha BETWEEN DATEADD(DAY, -3, GETDATE()) AND DATEADD(DAY, 3, GETDATE())
						AND EXISTS (
							SELECT 1
							FROM OPENJSON(j.{columnaPrecio})
									WITH (DRM INT '$.DRM') p
							INNER JOIN OPENJSON(sgn.drms2) d
								ON d.value = p.DRM
						)
				)
				AND pmh.DRM = '" + ((int)drm).ToString() + "'";

				if (string.IsNullOrEmpty(dondeMinimoDescuento) == false || string.IsNullOrEmpty(dondeMaximoPrecio) == false)
				{
					consulta = consulta + " AND " + string.Join(" AND ", new[] { dondeMinimoDescuento, dondeMaximoPrecio }.Where(x => string.IsNullOrEmpty(x) == false));
				}

				if (minimoReseñas != null && minimoReseñas > 0)
				{
					consulta = consulta + " AND jg.analisis IS NOT NULL and CONVERT(int, REPLACE(JSON_VALUE(jg.analisis, '$.Cantidad'),',','')) > " + minimoReseñas.ToString();
				}

				if (string.IsNullOrEmpty(nombreBusqueda) == false)
				{
					consulta += $@" AND jg.nombre COLLATE Latin1_General_CI_AI LIKE '%{nombreBusqueda}%'";
				}

				return consulta;
			}

			string busqueda = ConstruirBusqueda(tablaMinimos, precioMinimosHistoricos);

			if (noOficial == true)
			{
				string tablaNoOficial = region == TiendaRegion.Europa ? "seccionMinimosNoOficialesEU" : "seccionMinimosNoOficialesUS";
				string columnaNoOficial = region == TiendaRegion.Europa ? "preciosHistoricosNoOficialesEU" : "preciosHistoricosNoOficialesUS";

				string busquedaNoOficial = ConstruirBusqueda(tablaNoOficial, columnaNoOficial);

				busqueda = $"({busqueda}) UNION ALL ({busquedaNoOficial})";
			}

			busqueda = busqueda + " ORDER BY OrdenReseñas DESC";

			busqueda = busqueda + @$" OFFSET {posicion} ROWS FETCH NEXT 100 ROWS ONLY";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Juego>(busqueda, parametros)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Minimos Streaming", ex);
			}

			return null;
		}

		public static async Task<List<Juego>> Ultimos(string tabla, int cantidad)
		{
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Juego>("SELECT TOP (" + cantidad + ") * FROM " + tabla + " ORDER BY id DESC")).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Ultimos", ex);
			}

			return null;
		}

		public static async Task<List<Juego>> DLCs(string idMaestro = null, JuegoTipo tipo = JuegoTipo.DLC)
		{
			string busqueda = null;

			if (string.IsNullOrEmpty(idMaestro) == false)
			{
				if (tipo == JuegoTipo.DLC)
				{
					busqueda = "SELECT * FROM juegos WHERE maestro='" + idMaestro + "' AND tipo='1' ORDER BY nombre DESC";
				}
				else if (tipo == JuegoTipo.Music)
				{
					busqueda = "SELECT * FROM juegos WHERE maestro='" + idMaestro + "' AND tipo='3' ORDER BY nombre DESC";
				}
			}
			else
			{
				busqueda = "SELECT * FROM juegos WHERE (maestro IS NULL AND tipo='1') OR (maestro='no' AND tipo='1') OR (maestro IS NULL AND tipo='3') OR (maestro='no' AND tipo='3') ORDER BY nombre DESC";
			}

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Juego>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego DLCs", ex);
			}

			return new List<Juego>();
		}

		public static async Task<int> DLCsCantidad()
		{
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QuerySingleAsync<int>("SELECT COUNT(*) FROM juegos WHERE (maestro IS NULL OR maestro = 'no') AND tipo IN ('1','3')");
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego DLCs Cantidad", ex);
			}

			return 0;
		}

		public static async Task<List<Juego>> Filtro(List<string> ids, int posicion = 0)
		{
			List<string> etiquetas = new List<string>();
			List<string> categorias = new List<string>();
			List<string> decks = new List<string>();
			List<string> sistemas = new List<string>();
			List<string> tipos = new List<string>();

			if (ids?.Count > 0)
			{
				foreach (var id in ids)
				{
					if (id.Contains("t") == true)
					{
						etiquetas.Add(id);
					}

					if (id.Contains("c") == true || id.Contains("a") == true)
					{
						categorias.Add(id);
					}

					if (id.Contains("d") == true)
					{
						decks.Add(id);
					}

					if (id.Contains("s") == true)
					{
						sistemas.Add(id);
					}

					if (id.Contains("i") == true)
					{
						tipos.Add(id);
					}
				}
			}

			DynamicParameters parametros = new DynamicParameters();

			string etiquetasTexto = string.Empty;

			if (etiquetas?.Count > 0)
			{
				int i = 0;
				List<string> condiciones = new List<string>();

				foreach (var etiqueta in etiquetas)
				{
					string etiqueta2 = etiqueta.Replace("t", null);

					if (etiquetasTexto.Contains(etiqueta2) == false)
					{
						string nombreParam = $"etq{i}";
						parametros.Add(nombreParam, "%\"" + etiqueta2 + "\"%");
						condiciones.Add($"j.etiquetas LIKE @{nombreParam}");
						etiquetasTexto += etiqueta2;
						i += 1;
					}
				}

				if (condiciones.Count > 0)
				{
					etiquetasTexto = " AND ISJSON(j.etiquetas) > 0 AND (" + string.Join(" AND ", condiciones) + ")";
				}
			}

			string categoriasTexto = string.Empty;

			if (categorias.Count > 0)
			{
				int i = 0;
				List<string> condiciones = new List<string>();

				foreach (var categoria in categorias)
				{
					string categoria2 = categoria.Replace("c", null).Replace("a", null);

					if (categoriasTexto.Contains(categoria2) == false)
					{
						string nombreParam = $"cat{i}";
						parametros.Add(nombreParam, "%\"" + categoria2 + "\"%");
						condiciones.Add($"j.categorias LIKE @{nombreParam}");
						categoriasTexto += categoria2;
						i += 1;
					}
				}

				if (condiciones.Count > 0)
				{
					categoriasTexto = " AND ISJSON(j.categorias) > 0 AND (" + string.Join(" AND ", condiciones) + ")";
				}
			}

			string deckTexto = string.Empty;

			if (decks.Count > 0)
			{
				int i = 0;
				List<string> condiciones = new List<string>();

				foreach (var deck in decks)
				{
					string deck2 = deck.Replace("d", null);

					if (int.TryParse(deck2, out int deckId) == false)
					{
						continue;
					}

					if (deckTexto.Contains(deck2) == false)
					{
						string nombreParam = $"deck{i}";
						parametros.Add(nombreParam, deckId);
						condiciones.Add($"j.deck = @{nombreParam}");
						deckTexto += deck2;
						i += 1;
					}
				}

				if (condiciones.Count > 0)
				{
					deckTexto = " AND (" + string.Join(" AND ", condiciones) + ")";
				}
			}

			string sistemasTexto = string.Empty;

			if (sistemas.Count > 0)
			{
				List<string> condiciones = new List<string>();
				int i = 0;

				foreach (var sistema in sistemas)
				{
					string sistema2 = sistema.Replace("s", null);
					string nombreSistema = null;

					if (sistema2 == "1") nombreSistema = "Windows";
					if (sistema2 == "2") nombreSistema = "Mac";
					if (sistema2 == "3") nombreSistema = "Linux";

					if (nombreSistema != null)
					{
						string nombreParam = $"sis{i}";
						parametros.Add(nombreParam, "%\"" + nombreSistema + "\":true%");
						condiciones.Add($"j.caracteristicas LIKE @{nombreParam}");
						i += 1;
					}
				}

				if (condiciones.Count > 0)
				{
					sistemasTexto = " AND (" + string.Join(" AND ", condiciones) + ")";
				}
			}

			string tiposTexto = string.Empty;

			if (tipos.Count > 0)
			{
				int i = 0;
				List<string> condiciones = new List<string>();

				foreach (var tipo in tipos)
				{
					string tipo2 = tipo.Replace("i", null);

					if (int.TryParse(tipo2, out int tipoId) == false)
					{
						continue;
					}

					if (tiposTexto.Contains(tipo2) == false)
					{
						string nombreParam = $"tipo{i}";
						parametros.Add(nombreParam, tipoId);
						condiciones.Add($"j.tipo = @{nombreParam}");
						tiposTexto += tipo2;
						i += 1;
					}
				}

				if (condiciones.Count > 0)
				{
					tiposTexto = " AND (" + string.Join(" AND ", condiciones) + ")";
				}
			}

			string busqueda = @"SELECT j.id, j.nombre, j.imagenes, j.precioMinimosHistoricos, j.precioActualesTiendas,
				j.tipo, j.analisis, j.idSteam, j.idGog, j.media, j.freeToPlay,
				(
					SELECT b.id, b.bundleTipo
					FROM bundles b
					INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
					WHERE bj.juegoId = j.id
						AND b.fechaEmpieza <= GETDATE()
						AND b.fechaTermina >= GETDATE()
					FOR JSON PATH
				) AS BundlesActuales,
				(
					SELECT b.id, b.bundleTipo
					FROM bundles b
					INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
					WHERE bj.juegoId = j.id
						AND b.fechaTermina < GETDATE()
					FOR JSON PATH
				) AS BundlesPasados,
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
				) AS SuscripcionesPasados, CONVERT(bigint, REPLACE(JSON_VALUE(j.analisis, '$.Cantidad'),',','')) AS Cantidad FROM juegos j " + Environment.NewLine +
				"WHERE ISJSON(analisis) > 0 " + etiquetasTexto + " " + categoriasTexto + " " + deckTexto + " " + sistemasTexto + " " + tiposTexto +
				" ORDER BY Cantidad DESC";

			busqueda = busqueda + @$" OFFSET {posicion} ROWS
								FETCH NEXT 50 ROWS ONLY";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Juego>(busqueda, parametros)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Filtro", ex);
			}

			return null;
		}
		public static async Task<List<Juego>> Duplicados()
		{
			string busqueda = @"SELECT * FROM juegos
 WHERE idSteam > 0 AND idSteam IN
    (SELECT idSteam FROM juegos GROUP BY idSteam HAVING COUNT(*) > 1)
    ORDER BY idSteam ";

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(busqueda)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Duplicados", ex);
				}
			}

			return new List<Juego>();
		}

		public static async Task<List<Juego>> BundleSteam(string id)
		{
			if (string.IsNullOrEmpty(id) == true)
			{
				return null;
			}

			string sql = @"
				SELECT j.idSteam, j.imagenes, j.precioActualesTiendas
				FROM juegos j
				WHERE j.id IN (
					SELECT value 
					FROM STRING_SPLIT(
						(SELECT idjuegos FROM tiendasteambundles WHERE enlace = @enlaceSteam),
						','
					)
				)";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Juego>(sql, new { enlaceSteam = id })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Bundle Steam", ex);
			}

			return null;
		}

        public static async Task<List<Juego>> Aleatorios(bool fechaAPISteam = false)
        {
			string sqlBase = @"SELECT TOP 300 id, nombre FROM juegos ORDER BY NEWID()";

			string sqlFecha = @"
				SELECT TOP 300 
					id, 
					nombre, 
					idSteam, 
					fechaSteamAPIComprobacion,
					JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam') AS FechaLanzamientoSteam
				FROM juegos
				WHERE idSteam > 0
				ORDER BY NEWID()";

			if (fechaAPISteam == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<Juego>(sqlBase)).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Juego Aleatorios 1", ex);
				}
			}

			try
			{
				var resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(sqlFecha)).ToList();
				});

				List<Juego> juegos = new List<Juego>();

				foreach (var resultado in resultados)
				{
					Juego juego = new Juego
					{
						Id = resultado.id,
						Nombre = resultado.nombre,
						IdSteam = resultado.idSteam,
						Caracteristicas = null
					};

					if (resultado.fechaSteamAPIComprobacion != null)
					{
						juego.FechaSteamAPIComprobacion = DateTime.Parse(resultado.fechaSteamAPIComprobacion);
					}

					if (resultado.FechaLanzamientoSteam != null)
					{
						juego.Caracteristicas = new JuegoCaracteristicas
						{
							FechaLanzamientoSteam = DateTime.Parse(resultado.FechaLanzamientoSteam)
						};
					}

					juegos.Add(juego);
				}

				return juegos;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Juego Aleatorios 2", ex);
			}

			return new List<Juego>();
		}
    }
}
