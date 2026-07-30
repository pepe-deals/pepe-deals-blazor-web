#nullable disable

using Dapper;
using Juegos;
using System.Data;
using System.Text.Json;
using Tareas.Minimos;
using Tiendas2;

namespace BaseDatos.Portada
{
	public static class Buscar
	{
		public static async Task<List<JuegoMinimoTarea>> BuscarMinimos(TiendaRegion region, string tienda = null)
		{
			DynamicParameters parametros = new DynamicParameters();

			string precioMinimosHistoricos = "precioMinimosHistoricos";

			if (region == TiendaRegion.EstadosUnidos)
			{
				precioMinimosHistoricos = "precioMinimosHistoricosUS";
			}

			string busqueda = @$"SELECT j.id, j.{precioMinimosHistoricos}, pmh.DRM as DRMElegido
				FROM juegos j
				CROSS APPLY OPENJSON(j.{precioMinimosHistoricos})
				WITH (
					FechaActualizacion DATETIME2 '$.FechaActualizacion',
					FechaTermina DATETIME2 '$.FechaTermina',
					DRM INT '$.DRM',
					Tienda NVARCHAR(50) '$.Tienda'
				) AS pmh
				WHERE j.ultimaModificacion >= DATEADD(day, -3, GETDATE())
				  AND j.analisis IS NOT NULL
				  AND j.analisis <> 'null'
				  AND ISJSON(j.analisis) = 1
				  AND JSON_VALUE(j.analisis, '$.Cantidad') IS NOT NULL
				  AND TRY_CONVERT(bigint, REPLACE(JSON_VALUE(j.analisis, '$.Cantidad'), ',', '')) > 99
				  AND j.nombre IS NOT NULL
				  AND j.imagenes IS NOT NULL
				  AND (j.mayorEdad = 'false' OR j.mayorEdad IS NULL)
				  AND (j.freeToPlay = 'false' OR j.freeToPlay IS NULL)
				  AND j.{precioMinimosHistoricos} IS NOT NULL
				  AND j.{precioMinimosHistoricos} <> 'null'
				  AND ISJSON(j.{precioMinimosHistoricos}) = 1
				  AND (
						(pmh.FechaActualizacion >= DATEADD(hour, -24, GETDATE()) AND (pmh.Tienda = 'steam' OR pmh.Tienda = 'steambundles')) OR
						(pmh.FechaActualizacion >= DATEADD(hour, -25, GETDATE()) AND (pmh.Tienda = 'humblestore' OR pmh.Tienda = 'humblechoice')) OR
						(pmh.FechaActualizacion >= DATEADD(hour, -48, GETDATE()) AND pmh.Tienda = 'epicgamesstore') OR
						(pmh.FechaActualizacion >= DATEADD(hour, -12, GETDATE()))    
					  )
				AND (
					pmh.FechaTermina IS NULL
					OR pmh.FechaTermina = '0001-01-01'
					OR pmh.FechaTermina > GETDATE()
				)";

			if (string.IsNullOrEmpty(tienda) == false)
			{
				busqueda = busqueda + $" AND pmh.Tienda=@Tienda";
				parametros.Add("Tienda", tienda);
			}

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<JuegoMinimoTarea>(busqueda, parametros)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Buscar Minimos", ex, false);
			}

			return null;
		}

		public static async Task<List<Juego>> Destacados(TiendaRegion region, int cantidadJuegos, int minimoReseñas, List<int> excluirJuegosIds = null, List<int> excluirSteamIds = null, bool ocultarBundles = true, int ocultarBundlesCantidad = 6, bool ocultarGratis = true, bool ocultarSuscripciones = true, int ocultarSuscripcionesCantidad = 6)
		{
			string tabla = "seccionMinimos";

			if (region == TiendaRegion.EstadosUnidos)
			{
				tabla = "seccionMinimosUS";
			}

			string precioMinimosHistoricos = "precioMinimosHistoricos";

			if (region == TiendaRegion.EstadosUnidos)
			{
				precioMinimosHistoricos = "precioMinimosHistoricosUS";
			}

			DynamicParameters parametros = new DynamicParameters();

			string exclusionJuegos = string.Empty;
			string exclusionSteam = string.Empty;

			if (excluirJuegosIds?.Count > 0)
			{
				DataTable tablaJuegos = CrearDataTable(excluirJuegosIds);
				parametros.Add("excluirJuegos", tablaJuegos.AsTableValuedParameter("dbo.ListaIdsNumericos"));
				exclusionJuegos = $"AND NOT EXISTS (SELECT 1 FROM @excluirJuegos WHERE Id = j.idMaestra)";
			}

			if (excluirSteamIds?.Count > 0)
			{
				DataTable tablaSteam = CrearDataTable(excluirSteamIds);
				parametros.Add("excluirSteam", tablaSteam.AsTableValuedParameter("dbo.ListaIdsNumericos"));
				exclusionSteam = $"AND NOT EXISTS (SELECT 1 FROM @excluirSteam WHERE Id = jg.idSteam AND JSON_VALUE(j.{precioMinimosHistoricos}, '$[0].DRM') = '0')";
			}

			string busqueda = @$";WITH Candidatos AS (
				SELECT TOP {cantidadJuegos} j.idMaestra, j.{precioMinimosHistoricos}, jg.idSteam
				FROM {tabla} j 
				INNER JOIN dbo.juegos jg ON jg.id = j.idMaestra
				CROSS APPLY OPENJSON(j.{precioMinimosHistoricos}, '$[0]') WITH (
					Precio float '$.Precio',
					Descuento int '$.Descuento',
					DRM int '$.DRM',
					FechaTermina datetime2 '$.FechaTermina',
					FechaActualizacion datetime2 '$.FechaActualizacion'
				) precioMin
				WHERE jg.tipo = 0 {exclusionJuegos} {exclusionSteam} AND 
					year(getdate()) < year(JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoSteam')) + 11 AND
					precioMin.Precio >= 1.99 AND 
					precioMin.Descuento > 0 AND 
					precioMin.DRM = 0 AND 
					(
						(YEAR(precioMin.FechaTermina) > 2020 AND precioMin.FechaTermina > GETDATE())
						OR
						(
							NOT (YEAR(precioMin.FechaTermina) > 2020 AND precioMin.FechaTermina > GETDATE())
							AND precioMin.FechaActualizacion > DATEADD(HOUR,-24,GetDate())
						)
					) AND 
					(CONVERT(bigint, REPLACE(JSON_VALUE(jg.analisis, '$.Cantidad'),',','')) >= {minimoReseñas}) AND 
					{(ocultarBundles == true ? $"NOT EXISTS (SELECT 1 FROM bundles b INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id WHERE bj.JuegoId = j.idMaestra AND b.fechaTermina > DATEADD(MONTH, -{ocultarBundlesCantidad}, GETDATE())) AND " : "")} 
					{(ocultarGratis == true ? "NOT EXISTS (SELECT 1 FROM gratis WHERE gratis.juegoId = j.idMaestra AND gratis.DRM = 0) AND " : "")}
					{(ocultarSuscripciones == true ? @$"NOT EXISTS (SELECT 1 FROM suscripciones WHERE suscripciones.juegoId = j.idMaestra AND suscripciones.DRM = 0 AND suscripciones.fechaTermina > DATEADD(MONTH, -{ocultarSuscripcionesCantidad}, GETDATE())) AND " : "")}
					(jg.ocultarPortada IS NULL OR jg.ocultarPortada = 'false')
				ORDER BY NEWID()
			)
			SELECT c.idMaestra, jg.nombre,
				JSON_VALUE(jg.imagenes, '$.Logo') as logo, 
				JSON_VALUE(jg.imagenes, '$.Library_1920x620') as fondo, 
				JSON_VALUE(jg.imagenes, '$.Header_460x215') as header, 
				JSON_VALUE(jg.media, '$.Videos[0].Micro') as video,
				c.{precioMinimosHistoricos}, c.idSteam
			FROM Candidatos c
			INNER JOIN dbo.juegos jg ON jg.id = c.idMaestra;";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					var filas = await conexion.QueryAsync(busqueda, parametros);

					var juegos = filas.Select(fila =>
					{
						Juego juego = new Juego
						{
							Id = fila.idMaestra,
							IdMaestra = fila.idMaestra,
							Nombre = fila.nombre,
							IdSteam = fila.idSteam
						};

						if (string.IsNullOrEmpty(fila.logo) == false || string.IsNullOrEmpty(fila.fondo) == false || string.IsNullOrEmpty(fila.header) == false)
						{
							juego.Imagenes = new JuegoImagenes
							{
								Logo = fila.logo,
								Library_1920x620 = fila.fondo,
								Header_460x215 = fila.header
							};
						}

						if (region == TiendaRegion.Europa)
						{
							if (string.IsNullOrEmpty(fila.precioMinimosHistoricos) == false)
							{
								juego.PrecioMinimosHistoricos =
									JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricos);
							}
						}
						else if (region == TiendaRegion.EstadosUnidos)
						{
							if (string.IsNullOrEmpty(fila.precioMinimosHistoricosUS) == false)
							{
								juego.PrecioMinimosHistoricosUS =
									JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricosUS);
							}
						}

						if (string.IsNullOrEmpty(fila.video) == false)
						{
							juego.Media = new JuegoMedia
							{
								Videos = new List<JuegoMediaVideo>
								{
									new JuegoMediaVideo { Micro = fila.video }
								}
							};
						}

						return juego;
					}).ToList();

					return juegos;

				}).ContinueWith(t => t.Result.ToList());
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Portada Destacados", ex, false);
			}

			return null;
		}

		public static async Task<List<Juego>> Minimos(TiendaRegion region, int tipo, int posicion = 0, List<string> categorias = null, List<string> drms = null, int cantidadReseñas = 199, List<int> excluirJuegosIds = null, List<int> excluirSteamIds = null, List<int> excluirGogIds = null)
		{
			string tabla = "seccionMinimos";

			if (region == TiendaRegion.EstadosUnidos)
			{
				tabla = "seccionMinimosUS";
			}

			string precioMinimosHistoricos = "precioMinimosHistoricos";

			if (region == TiendaRegion.EstadosUnidos)
			{
				precioMinimosHistoricos = "precioMinimosHistoricosUS";
			}

			DynamicParameters parametros = new DynamicParameters();
			parametros.Add("cantidadAnalisis", cantidadReseñas);

			string categoria = string.Empty;

			if (categorias?.Count > 0)
			{
				List<int> categoriasValidas = new List<int>();

				foreach (var valor in categorias)
				{
					if (int.TryParse(valor, out int categoriaId))
					{
						categoriasValidas.Add(categoriaId);
					}
				}

				if (categoriasValidas.Count > 0)
				{
					DataTable tablaCategorias = CrearDataTable(categoriasValidas);
					parametros.Add("categorias", tablaCategorias.AsTableValuedParameter("dbo.ListaIdsNumericos"));
					categoria = " AND jg.tipo IN (SELECT Id FROM @categorias)";
				}
			}

			string drm = string.Empty;

			if (drms?.Count > 0)
			{
				List<int> drmsValidos = new List<int>();

				foreach (var valor in drms)
				{
					if (int.TryParse(valor, out int drmId))
					{
						drmsValidos.Add(drmId);
					}
				}

				if (drmsValidos.Count > 0)
				{
					DataTable tablaDrms = CrearDataTable(drmsValidos);
					parametros.Add("drms", tablaDrms.AsTableValuedParameter("dbo.ListaIdsNumericos"));
					drm = " AND precioMin.DRM IN (SELECT Id FROM @drms)";
				}
			}

			string exclusionJuegos = string.Empty;
			string exclusionSteam = string.Empty;
			string exclusionGog = string.Empty;

			if (excluirJuegosIds?.Count > 0)
			{
				DataTable tablaJuegos = CrearDataTable(excluirJuegosIds);
				parametros.Add("excluirJuegos", tablaJuegos.AsTableValuedParameter("dbo.ListaIdsNumericos"));
				exclusionJuegos = $"AND NOT EXISTS (SELECT 1 FROM @excluirJuegos WHERE Id = j.idMaestra)";
			}

			if (excluirSteamIds?.Count > 0)
			{
				DataTable tablaSteam = CrearDataTable(excluirSteamIds);
				parametros.Add("excluirSteam", tablaSteam.AsTableValuedParameter("dbo.ListaIdsNumericos"));
				exclusionSteam = $"AND NOT EXISTS (SELECT 1 FROM @excluirSteam WHERE Id = jg.idSteam AND precioMin.DRM = 0)";
			}

			if (excluirGogIds?.Count > 0)
			{
				DataTable tablaGog = CrearDataTable(excluirGogIds);
				parametros.Add("excluirGog", tablaGog.AsTableValuedParameter("dbo.ListaIdsNumericos"));
				exclusionGog = $"AND NOT EXISTS (SELECT 1 FROM @excluirGog WHERE Id = jg.idGog AND precioMin.DRM = 8)";
			}

			string busqueda = @$"SELECT j.idMaestra, jg.nombre, jg.imagenes, j.{precioMinimosHistoricos}, JSON_VALUE(jg.media, '$.Videos[0].Micro') as video, jg.etiquetas,
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
	) AS SuscripcionesPasados, jg.idSteam, precioMin.FechaDetectado AS Fecha, jg.idGog, jg.analisis, CONVERT(datetime2, JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoSteam')) as FechaLanzamiento FROM {tabla} j
		INNER JOIN dbo.juegos jg ON jg.id = j.idMaestra
		CROSS APPLY OPENJSON(j.{precioMinimosHistoricos}, '$[0]') WITH (
			Descuento int '$.Descuento',
			DRM int '$.DRM',
			FechaDetectado datetime2 '$.FechaDetectado'
		) precioMin
		WHERE CONVERT(bigint, REPLACE(JSON_VALUE(jg.analisis, '$.Cantidad'),',','')) >= @cantidadAnalisis AND precioMin.Descuento > 0 AND (jg.MayorEdad <> 'true' OR jg.MayorEdad IS NULL) {categoria} {drm} {exclusionJuegos} {exclusionSteam} {exclusionGog}";

			if (tipo == 0)
			{
				busqueda = busqueda + " ORDER BY Fecha DESC";
			}
			else if (tipo == 1)
			{
				busqueda = busqueda + @" ORDER BY CASE
									WHEN jg.analisis = 'null' OR jg.analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(jg.analisis, '$.Cantidad'),',',''))
								 END DESC";
			}
			else if (tipo == 2)
			{
				busqueda = busqueda + " AND CONVERT(datetime2, JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoSteam')) > DATEADD(DAY,-30,GetDate()) ORDER BY CONVERT(datetime2, JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoSteam')) DESC";
			}
			else if (tipo == 3)
			{
				busqueda = busqueda + $" ORDER BY precioMin.FechaDetectado DESC";
			}

			busqueda = busqueda + @$" OFFSET {posicion} ROWS
								FETCH NEXT 100 ROWS ONLY";

			try
			{
				var filas = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryAsync(busqueda, parametros);
				});

				List<Juego> resultados = new List<Juego>();

				foreach (var fila in filas)
				{
					Juego juego = new Juego
					{
						Id = fila.idMaestra,
						IdMaestra = fila.idMaestra,
						Nombre = fila.nombre,
						IdSteam = fila.idSteam,
						IdGog = fila.idGog,
						Caracteristicas = fila.FechaLanzamiento != null ? new JuegoCaracteristicas { FechaLanzamientoSteam = fila.FechaLanzamiento } : null,
						Etiquetas = string.IsNullOrEmpty(fila.etiquetas) == false ? JsonSerializer.Deserialize<List<string>>(fila.etiquetas) : null
					};

					if (string.IsNullOrEmpty(fila.imagenes) == false)
					{
						juego.Imagenes = JsonSerializer.Deserialize<JuegoImagenes>(fila.imagenes);
					}

					if (region == TiendaRegion.Europa)
					{
						if (string.IsNullOrEmpty(fila.precioMinimosHistoricos) == false)
						{
							juego.PrecioMinimosHistoricos = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricos);
						}
					}
					else if (region == TiendaRegion.EstadosUnidos)
					{
						if (string.IsNullOrEmpty(fila.precioMinimosHistoricosUS) == false)
						{
							juego.PrecioMinimosHistoricosUS = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricosUS);
						}
					}

					if (string.IsNullOrEmpty(fila.video) == false)
					{
						juego.Media = new JuegoMedia
						{
							Videos = new List<JuegoMediaVideo> { new JuegoMediaVideo { Micro = fila.video } }
						};
					}

					if (string.IsNullOrEmpty(fila.BundlesActuales) == false)
					{
						juego.BundlesActuales = JsonSerializer.Deserialize<List<JuegoBundlesActuales>>(fila.BundlesActuales);
					}

					if (string.IsNullOrEmpty(fila.BundlesPasados) == false)
					{
						juego.BundlesPasados = JsonSerializer.Deserialize<List<JuegoBundlesPasados>>(fila.BundlesPasados);
					}

					if (string.IsNullOrEmpty(fila.GratisActuales) == false)
					{
						juego.GratisActuales = JsonSerializer.Deserialize<List<JuegoGratisActuales>>(fila.GratisActuales);
					}

					if (string.IsNullOrEmpty(fila.GratisPasados) == false)
					{
						juego.GratisPasados = JsonSerializer.Deserialize<List<JuegoGratisPasados>>(fila.GratisPasados);
					}

					if (string.IsNullOrEmpty(fila.SuscripcionesActuales) == false)
					{
						juego.SuscripcionesActuales = JsonSerializer.Deserialize<List<JuegoSuscripcionActuales>>(fila.SuscripcionesActuales);
					}

					if (string.IsNullOrEmpty(fila.SuscripcionesPasados) == false)
					{
						juego.SuscripcionesPasados = JsonSerializer.Deserialize<List<JuegoSuscripcionPasados>>(fila.SuscripcionesPasados);
					}

					if (string.IsNullOrEmpty(fila.analisis) == false)
					{
						juego.Analisis = JsonSerializer.Deserialize<JuegoAnalisis>(fila.analisis);
					}

					resultados.Add(juego);
				}

				return resultados;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Portada Minimos", ex, false);
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

		public static async Task<List<Juego>> Proximamente(int cantidadJuegos, List<string> categorias = null, List<string> drms = null)
		{
			string busqueda = @"SELECT TOP @cantidadJuegos id, nombre, imagenes, precioMinimosHistoricos, JSON_VALUE(media, '$.Videos[0].Micro') as video, idSteam, idGog, CONVERT(datetime2, JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam')) as FechaLanzamiento FROM juegos 
                                    WHERE ISJSON(caracteristicas) > 0 AND DATEDIFF(DAY, JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam'), GETDATE()) < 0
ORDER BY CONVERT(datetime2, JSON_VALUE(caracteristicas, '$.FechaLanzamientoSteam'))";

			busqueda = busqueda.Replace("@cantidadJuegos", cantidadJuegos.ToString());

			try
			{
				var filas = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(busqueda)).ToList();
				});

				List<Juego> resultados = new List<Juego>();

				foreach (var fila in filas)
				{
					Juego juego = new Juego
					{
						Id = fila.id,
						IdMaestra = fila.id,
						Nombre = fila.nombre,
						IdSteam = fila.idSteam,
						IdGog = fila.idGog,
						Caracteristicas = fila.FechaLanzamiento != null ? new JuegoCaracteristicas { FechaLanzamientoSteam = fila.FechaLanzamiento } : null
					};

					if (string.IsNullOrEmpty(fila.imagenes) == false)
					{
						juego.Imagenes = JsonSerializer.Deserialize<JuegoImagenes>(fila.imagenes);
					}

					if (string.IsNullOrEmpty(fila.precioMinimosHistoricos) == false)
					{
						juego.PrecioMinimosHistoricos = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricos);
					}

					if (string.IsNullOrEmpty(fila.video) == false)
					{
						juego.Media = new JuegoMedia
						{
							Videos = new List<JuegoMediaVideo> { new JuegoMediaVideo { Micro = fila.video } }
						};
					}

					resultados.Add(juego);
				}

				return resultados;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Portada Proximamente", ex, false);
			}

			return null;
		}
	}
}
