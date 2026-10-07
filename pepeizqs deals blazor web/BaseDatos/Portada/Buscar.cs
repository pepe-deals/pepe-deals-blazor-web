#nullable disable

using Dapper;
using Juegos;
using Servicios;
using System.Data;
using System.Text.Json;
using Tareas.Minimos;
using Tiendas2;

namespace BaseDatos.Portada
{
	public static class Buscar
	{
		public static async Task<List<JuegoMinimoTarea>> BuscarMinimos(TiendaTipo tipo, TiendaRegion region, string tienda = null)
		{
			DynamicParameters parametros = new DynamicParameters();

			string precioMinimosHistoricos = string.Empty;
			string adicionalMinimosHistoricos1 = string.Empty;
			string cruceAdicional = string.Empty;
			string campoOficial = string.Empty;
			string condicionesOficial = string.Empty;

			if (tipo == TiendaTipo.Oficial && region == TiendaRegion.Europa)
			{
				precioMinimosHistoricos = "precioMinimosHistoricos";
			}
			else if (tipo == TiendaTipo.Oficial && region == TiendaRegion.EstadosUnidos)
			{
				precioMinimosHistoricos = "precioMinimosHistoricosUS";
			}
			else if (tipo == TiendaTipo.NoOficial && region == TiendaRegion.Europa)
			{
				precioMinimosHistoricos = "preciosHistoricosNoOficialesEU";
				campoOficial = "precioMinimosHistoricos";

				adicionalMinimosHistoricos1 = $", j.{campoOficial}, pmh2.DRM as DRMOficial";
			}
			else if (tipo == TiendaTipo.NoOficial && region == TiendaRegion.EstadosUnidos)
			{
				precioMinimosHistoricos = "preciosHistoricosNoOficialesUS";
				campoOficial = "precioMinimosHistoricosUS";

				adicionalMinimosHistoricos1 = $", j.{campoOficial}, pmh2.DRM as DRMOficial";
			}
			else if (tipo == TiendaTipo.Marketplace && region == TiendaRegion.Europa)
			{
				precioMinimosHistoricos = "preciosHistoricosMarketplacesEU";
				campoOficial = "precioMinimosHistoricos";

				adicionalMinimosHistoricos1 = $", j.{campoOficial}, pmh2.DRM as DRMOficial";
			}
			else if (tipo == TiendaTipo.Marketplace && region == TiendaRegion.EstadosUnidos)
			{
				precioMinimosHistoricos = "preciosHistoricosMarketplacesUS";
				campoOficial = "precioMinimosHistoricosUS";

				adicionalMinimosHistoricos1 = $", j.{campoOficial}, pmh2.DRM as DRMOficial";
			}

			if (string.IsNullOrEmpty(precioMinimosHistoricos) == true)
			{
				return null;
			}

			if (string.IsNullOrEmpty(campoOficial) == false)
			{
				cruceAdicional = $@"
					CROSS APPLY OPENJSON(j.{campoOficial})
					WITH (
						DRM INT '$.DRM'
					) AS pmh2";

				condicionesOficial = $@"
					AND j.{campoOficial} IS NOT NULL
					AND j.{campoOficial} <> 'null'
					AND ISJSON(j.{campoOficial}) = 1
					AND pmh.DRM = pmh2.DRM";
			}

			string busqueda = @$"SELECT j.id, j.{precioMinimosHistoricos} {adicionalMinimosHistoricos1}, pmh.DRM as DRMElegido
				FROM juegos j
				CROSS APPLY OPENJSON(j.{precioMinimosHistoricos})
				WITH (
					FechaActualizacion DATETIME2 '$.FechaActualizacion',
					FechaTermina DATETIME2 '$.FechaTermina',
					DRM INT '$.DRM',
					Tienda NVARCHAR(50) '$.Tienda'
				) AS pmh
				{cruceAdicional}
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
				  {condicionesOficial}
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

		public static async Task<List<Juego>> Destacados(bool noOficial, bool marketplace, TiendaRegion region, int cantidadJuegos, int minimoReseñas, List<int> excluirJuegosIds = null, List<int> excluirSteamIds = null, bool ocultarBundles = true, int ocultarBundlesCantidad = 6, bool ocultarGratis = true, bool ocultarSuscripciones = true, int ocultarSuscripcionesCantidad = 6)
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

			string ConstruirCandidatos(string tablaOrigen, string columnaPrecio)
			{
				string exclusionSteam = excluirSteamIds?.Count > 0
					? $"AND NOT EXISTS (SELECT 1 FROM @excluirSteam WHERE Id = jg.idSteam AND JSON_VALUE(j.{columnaPrecio}, '$[0].DRM') = '0')"
					: string.Empty;

				return @$"SELECT j.idMaestra, j.{columnaPrecio} AS PrecioJson, jg.idSteam
					FROM {tablaOrigen} j 
					INNER JOIN dbo.juegos jg ON jg.id = j.idMaestra
					CROSS APPLY OPENJSON(j.{columnaPrecio}, '$[0]') WITH (
						Precio float '$.Precio',
						Descuento int '$.Descuento',
						DRM int '$.DRM',
						FechaTermina datetime2 '$.FechaTermina',
						FechaActualizacion datetime2 '$.FechaActualizacion'
					) precioMin
					WHERE jg.tipo = 0 {exclusionJuegos} {exclusionSteam} AND 
						year(getdate()) < year(JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoSteam')) + 11 AND
						precioMin.Precio >= 1.99 AND 
						{(noOficial == false && marketplace == false ? "precioMin.Descuento > 0 AND" : "")} 
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
						(jg.ocultarPortada IS NULL OR jg.ocultarPortada = 'false')";
			}

			string candidatosBase = ConstruirCandidatos(tabla, precioMinimosHistoricos);

			if (noOficial == true)
			{
				string tablaNoOficial = region == TiendaRegion.EstadosUnidos ? "seccionMinimosNoOficialesUS" : "seccionMinimosNoOficialesEU";
				string columnaNoOficial = region == TiendaRegion.EstadosUnidos ? "preciosHistoricosNoOficialesUS" : "preciosHistoricosNoOficialesEU";

				string candidatosNoOficial = ConstruirCandidatos(tablaNoOficial, columnaNoOficial);

				candidatosBase = $"{candidatosBase} UNION ALL {candidatosNoOficial}";
			}

			if (marketplace == true)
			{
				string tablaMarketplace = region == TiendaRegion.EstadosUnidos ? "seccionMinimosMarketplacesUS" : "seccionMinimosMarketplacesEU";
				string columnaMarketplace = region == TiendaRegion.EstadosUnidos ? "preciosHistoricosMarketplacesUS" : "preciosHistoricosMarketplacesEU";
				
				string candidatosMarketplace = ConstruirCandidatos(tablaMarketplace, columnaMarketplace);
				
				candidatosBase = $"{candidatosBase} UNION ALL {candidatosMarketplace}";
			}

			string cabecera;

			if (cantidadJuegos == 6)
			{
				cabecera = @$";DECLARE @base TABLE (idMaestra int, PrecioJson nvarchar(max), idSteam int, Resenas bigint);
							   DECLARE @primero TABLE (idMaestra int, PrecioJson nvarchar(max), idSteam int);

				INSERT INTO @base
				SELECT x.idMaestra, x.PrecioJson, x.idSteam,
					CONVERT(bigint, REPLACE(JSON_VALUE(jg.analisis, '$.Cantidad'),',',''))
				FROM ({candidatosBase}) x
				INNER JOIN dbo.juegos jg ON jg.id = x.idMaestra;

				INSERT INTO @primero
				SELECT TOP (1) idMaestra, PrecioJson, idSteam
				FROM @base
				ORDER BY CASE WHEN Resenas >= 10000 THEN 0 ELSE 1 END, NEWID();

						WITH Candidatos AS (
							SELECT idMaestra, PrecioJson, idSteam FROM @primero
							UNION ALL
							SELECT idMaestra, PrecioJson, idSteam FROM (
								SELECT TOP (5) idMaestra, PrecioJson, idSteam
								FROM @base
								WHERE idMaestra NOT IN (SELECT idMaestra FROM @primero)
								ORDER BY NEWID()
							) resto
						)";
			}
			else
			{
				cabecera = @$";WITH CandidatosBase AS (
					{candidatosBase}
				),
				Candidatos AS (
					SELECT TOP ({cantidadJuegos}) idMaestra, PrecioJson, idSteam
					FROM CandidatosBase
					ORDER BY NEWID()
				)";
			}

			string busqueda = @$"{cabecera}
			SELECT c.idMaestra, jg.nombre,
				JSON_VALUE(jg.imagenes, '$.Logo') as logo, 
				JSON_VALUE(jg.imagenes, '$.Library_1920x620') as fondo, 
				JSON_VALUE(jg.imagenes, '$.Header_460x215') as header, 
				JSON_VALUE(jg.media, '$.Videos[0].Micro') as video,
				c.PrecioJson AS {precioMinimosHistoricos}, c.idSteam
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

						if (string.IsNullOrEmpty(fila.precioMinimosHistoricos) == false)
						{
							juego.PrecioMinimosHistoricos = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricos);
						}

						if (string.IsNullOrEmpty(fila.precioMinimosHistoricosUS) == false)
						{
							juego.PrecioMinimosHistoricosUS = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricosUS);
						}

						if (string.IsNullOrEmpty(fila.preciosHistoricosNoOficialesEU) == false)
						{
							juego.PreciosHistoricosNoOficialesEU = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.preciosHistoricosNoOficialesEU);
						}

						if (string.IsNullOrEmpty(fila.preciosHistoricosNoOficialesUS) == false)
						{
							juego.PreciosHistoricosNoOficialesUS = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.preciosHistoricosNoOficialesUS);
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

		public static async Task<List<Juego>> Minimos(bool noOficial, bool marketplace, TiendaRegion region, int tipo, int posicion = 0, List<string> categorias = null, List<string> drms = null, int cantidadReseñas = 199, List<int> excluirJuegosIds = null, List<int> excluirSteamIds = null, List<int> excluirGogIds = null)
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

			string filtroTipo = string.Empty;

			if (tipo == 2)
			{
				filtroTipo = " AND CONVERT(datetime2, JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoSteam')) > DATEADD(DAY,-30,GetDate())";
			}

			string orden = string.Empty;

			if (tipo == 0 || tipo == 3)
			{
				orden = " ORDER BY Fecha DESC";
			}
			else if (tipo == 1)
			{
				orden = @" ORDER BY CASE
					WHEN analisis = 'null' OR analisis IS NULL THEN 0 ELSE CONVERT(int, REPLACE(JSON_VALUE(analisis, '$.Cantidad'),',',''))
				END DESC";
			}
			else if (tipo == 2)
			{
				orden = " ORDER BY FechaLanzamiento DESC";
			}


			string ConstruirBusqueda(string tablaOrigen, string columnaPrecio)
			{
				return @$"SELECT j.idMaestra, jg.nombre, jg.imagenes, j.{columnaPrecio}, CASE WHEN ISJSON(jg.media) = 1 THEN JSON_VALUE(jg.media, '$.Videos[0].Micro') END as video, jg.etiquetas,
				jg.idSteam, precioMin.FechaDetectado AS Fecha, jg.idGog, jg.analisis, CONVERT(datetime2, JSON_VALUE(jg.caracteristicas, '$.FechaLanzamientoSteam')) as FechaLanzamiento
				FROM {tablaOrigen} j
				INNER JOIN dbo.juegos jg ON jg.id = j.idMaestra
				CROSS APPLY OPENJSON(j.{columnaPrecio}, '$[0]') WITH (
					Descuento int '$.Descuento',
					DRM int '$.DRM',
					FechaDetectado datetime2 '$.FechaDetectado'
				) precioMin
				WHERE CASE WHEN ISJSON(jg.analisis) = 1
					THEN TRY_CONVERT(bigint, REPLACE(JSON_VALUE(jg.analisis, '$.Cantidad'),',',''))
				END >= @cantidadAnalisis {(noOficial == false && marketplace == false ? "AND precioMin.Descuento > 0" : "")} AND (jg.MayorEdad <> 'true' OR jg.MayorEdad IS NULL) {categoria} {drm} {exclusionJuegos} {exclusionSteam} {exclusionGog} {filtroTipo}";
			}

			int limite = posicion + 100;

			string Rama(string tablaOrigen, string columnaPrecio)
			{
				return $"SELECT TOP ({limite}) * FROM ({ConstruirBusqueda(tablaOrigen, columnaPrecio)}) AS r {orden}";
			}

			List<string> ramas = new List<string>();
			ramas.Add(Rama(tabla, precioMinimosHistoricos));

			if (noOficial == true)
			{
				string tablaNoOficial = region == TiendaRegion.EstadosUnidos ? "seccionMinimosNoOficialesUS" : "seccionMinimosNoOficialesEU";
				string columnaNoOficial = region == TiendaRegion.EstadosUnidos ? "preciosHistoricosNoOficialesUS" : "preciosHistoricosNoOficialesEU";

				ramas.Add(Rama(tablaNoOficial, columnaNoOficial));
			}

			if (marketplace == true)
			{
				string tablaMarketplace = region == TiendaRegion.EstadosUnidos ? "seccionMinimosMarketplacesUS" : "seccionMinimosMarketplacesEU";
				string columnaMarketplace = region == TiendaRegion.EstadosUnidos ? "preciosHistoricosMarketplacesUS" : "preciosHistoricosMarketplacesEU";

				ramas.Add(Rama(tablaMarketplace, columnaMarketplace));
			}

			string union = string.Join(" UNION ALL ", ramas.Select((r, i) => $"SELECT * FROM ({r}) AS rama{i}"));

			string subconsultas = @"
			(
				SELECT b.id, b.bundleTipo
				FROM bundles b
				INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
				WHERE bj.juegoId = p.idMaestra
					AND b.fechaEmpieza <= GETDATE()
					AND b.fechaTermina >= GETDATE()
				FOR JSON PATH
			) AS BundlesActuales,
			(
				SELECT b.id, b.bundleTipo
				FROM bundles b
				INNER JOIN bundlesJuegos bj ON bj.bundleId = b.id
				WHERE bj.juegoId = p.idMaestra
					AND b.fechaTermina < GETDATE()
				FOR JSON PATH
			) AS BundlesPasados,
			(
				SELECT g.gratis
				FROM gratis g
				WHERE g.juegoId = p.idMaestra
					AND g.fechaEmpieza <= GETDATE()
					AND g.fechaTermina >= GETDATE()
				FOR JSON PATH
			) AS GratisActuales,
			(
				SELECT g.gratis
				FROM gratis g
				WHERE g.juegoId = p.idMaestra
					AND g.fechaTermina < GETDATE()
				FOR JSON PATH
			) AS GratisPasados,
			(
				SELECT s.suscripcion
				FROM suscripciones s
				WHERE s.juegoId = p.idMaestra
					AND s.FechaEmpieza <= GETDATE()
					AND s.FechaTermina >= GETDATE()
				FOR JSON PATH
			) AS SuscripcionesActuales,
			(
				SELECT s.suscripcion
				FROM suscripciones s
				WHERE s.juegoId = p.idMaestra
					AND s.FechaTermina < GETDATE()
				FOR JSON PATH
			) AS SuscripcionesPasados";

			string busqueda = $@"SELECT p.*, {subconsultas}
			FROM (
				SELECT * FROM ({union}) AS resultado
				{orden}
				OFFSET {posicion} ROWS
				FETCH NEXT 100 ROWS ONLY
			) AS p
			{orden}";


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

					if (string.IsNullOrEmpty(fila.precioMinimosHistoricos) == false)
					{
						juego.PrecioMinimosHistoricos = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricos);
					}

					if (string.IsNullOrEmpty(fila.precioMinimosHistoricosUS) == false)
					{
						juego.PrecioMinimosHistoricosUS = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricosUS);
					}

					if (string.IsNullOrEmpty(fila.preciosHistoricosNoOficialesEU) == false)
					{
						juego.PreciosHistoricosNoOficialesEU = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.preciosHistoricosNoOficialesEU);
					}

					if (string.IsNullOrEmpty(fila.preciosHistoricosNoOficialesUS) == false)
					{
						juego.PreciosHistoricosNoOficialesUS = JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.preciosHistoricosNoOficialesUS);
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
