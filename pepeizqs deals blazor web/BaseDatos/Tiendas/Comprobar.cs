#nullable disable

using Dapper;
using Juegos;
using System.Text;
using System.Text.Json;
using Tiendas2;

namespace BaseDatos.Tiendas
{
	public static class Comprobar
	{
		public static async Task Steam(TiendaRegion region, List<JuegoPrecio> ofertas, List<JuegoReseñas> reseñas, bool rapido)
		{
			if (ofertas == null || ofertas.Count == 0)
			{
				return;
			}

			List<int> idsSteam = new List<int>();
			var mapaOfertas = new Dictionary<int, JuegoPrecio>();
			var mapaReseñas = new Dictionary<int, JuegoReseñas>();

			foreach (var oferta in ofertas)
			{
				string idSteam2 = APIs.Steam.Juego.LimpiarID(oferta.Enlace);

				if (string.IsNullOrEmpty(idSteam2) == false)
				{
					idSteam2 = APIs.Steam.Tienda.IdsEspeciales(idSteam2);

					if (int.TryParse(idSteam2, out int idSteam) && idSteam > 0)
					{
						idsSteam.Add(idSteam);
						mapaOfertas[idSteam] = oferta;
					}
				}
			}

			if (reseñas != null && reseñas.Count > 0)
			{
				foreach (var reseña in reseñas)
				{
					mapaReseñas[reseña.IdSteam] = reseña;
				}
			}

			if (idsSteam.Count == 0)
			{
				return;
			}

			string buscarJuegos = "SELECT id, precioMinimosHistoricos, precioActualesTiendas, idSteam, historicos, fechaSteamAPIComprobacion FROM juegos WHERE idSteam IN @idsSteam";

			if (region == TiendaRegion.EstadosUnidos)
			{
				buscarJuegos = "SELECT id, precioMinimosHistoricosUS, precioActualesTiendasUS, idSteam, historicosUS, fechaSteamAPIComprobacion FROM juegos WHERE idSteam IN @idsSteam";
			}

			try
			{
				var datos = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<dynamic>(buscarJuegos, new { idsSteam })).ToList();
				});

				var juegosEncontrados = datos.Cast<dynamic>().ToList();

				var updates = new List<(string sql, DynamicParameters parametros)>();
				int indice = 0;

				foreach (var dato in juegosEncontrados)
				{
					int idSteam = dato.idSteam;
					var oferta = mapaOfertas[idSteam];
					var reseña = mapaReseñas.ContainsKey(idSteam) ? mapaReseñas[idSteam] : null;

					Juego juego = JuegoCrear.Generar();

					if (reseña != null && juego != null)
					{
						if (string.IsNullOrEmpty(reseña.Cantidad) == false && string.IsNullOrEmpty(reseña.Porcentaje) == false)
						{
							JuegoAnalisis nuevasReseñas = new JuegoAnalisis();
							nuevasReseñas.Cantidad = reseña.Cantidad;
							nuevasReseñas.Porcentaje = reseña.Porcentaje;

							juego.Analisis = nuevasReseñas;
						}
					}

					juego.IdSteam = idSteam;

					string fechaAPI = dato.fechaSteamAPIComprobacion;

					if (string.IsNullOrEmpty(fechaAPI) == false)
					{
						bool actualizarAPI = DateTime.Now.Subtract(DateTime.Parse(fechaAPI)) > TimeSpan.FromDays(91);

						if (actualizarAPI == true)
						{
							await BaseDatos.JuegosActualizar.Insertar.Ejecutar(juego.Id, juego.IdSteam, "SteamAPI");
						}

						int id = dato.id ?? 0;

						if (id > 0)
						{
							if (region == TiendaRegion.Europa)
							{
								List<JuegoPrecio> ofertasHistoricas = null;

								if (string.IsNullOrEmpty(dato.precioMinimosHistoricos) == false)
								{
									ofertasHistoricas = JsonSerializer.Deserialize<List<JuegoPrecio>>(dato.precioMinimosHistoricos) ?? new List<JuegoPrecio>();

									if (ofertasHistoricas.Count == 0)
									{
										ofertasHistoricas.Add(oferta);
									}
								}

								List<JuegoPrecio> ofertasActuales = null;

								if (string.IsNullOrEmpty(dato.precioActualesTiendas) == false)
								{
									ofertasActuales = JsonSerializer.Deserialize<List<JuegoPrecio>>(dato.precioActualesTiendas) ?? new List<JuegoPrecio>();

									if (ofertasActuales.Count == 0)
									{
										ofertasActuales.Add(oferta);
									}
								}

								List<JuegoHistorico> historicos = null;

								if (string.IsNullOrEmpty(dato.historicos) == false)
								{
									historicos = JsonSerializer.Deserialize<List<JuegoHistorico>>(dato.historicos) ?? new List<JuegoHistorico>();
								}

								var resultado = await Juegos.Precios.Comprobacion(TiendaTipo.Oficial, id, idSteam, ofertasActuales, ofertasHistoricas, historicos, oferta, null, null, null, juego.Analisis, indice);
								
								if (resultado.Item1 != null && resultado.Item2 != null)
								{
									updates.Add((resultado.Item1, resultado.Item2));
									indice += 1;
								}
							}
							else if (region == TiendaRegion.EstadosUnidos)
							{
								List<JuegoPrecio> ofertasHistoricasUS = null;

								if (string.IsNullOrEmpty(dato.precioMinimosHistoricosUS) == false)
								{
									ofertasHistoricasUS = JsonSerializer.Deserialize<List<JuegoPrecio>>(dato.precioMinimosHistoricosUS) ?? new List<JuegoPrecio>();

									if (ofertasHistoricasUS.Count == 0)
									{
										ofertasHistoricasUS.Add(oferta);
									}
								}

								List<JuegoPrecio> ofertasActualesUS = null;

								if (string.IsNullOrEmpty(dato.precioActualesTiendasUS) == false)
								{
									ofertasActualesUS = JsonSerializer.Deserialize<List<JuegoPrecio>>(dato.precioActualesTiendasUS) ?? new List<JuegoPrecio>();

									if (ofertasActualesUS.Count == 0)
									{
										ofertasActualesUS.Add(oferta);
									}
								}

								List<JuegoHistorico> historicosUS = null;

								if (string.IsNullOrEmpty(dato.historicosUS) == false)
								{
									historicosUS = JsonSerializer.Deserialize<List<JuegoHistorico>>(dato.historicosUS) ?? new List<JuegoHistorico>();
								}

								var resultado = await Juegos.Precios.ComprobacionUS(TiendaTipo.Oficial, id, idSteam, ofertasActualesUS, ofertasHistoricasUS, historicosUS, oferta, null, null, null, juego.Analisis, indice);
								
								if (resultado.Item1 != null && resultado.Item2 != null)
								{
									updates.Add((resultado.Item1, resultado.Item2));
									indice += 1;
								}
							}
						}
					}
				}

				if (indice > 0)
				{
					foreach (var grupo in updates.Chunk(100))
					{
						StringBuilder sqlBatch = new StringBuilder();
						DynamicParameters parametrosBatch = new DynamicParameters();

						int indiceGlobal = 0;

						foreach (var (sql, parametros2) in grupo)
						{
							Dictionary<string, string> mapeoParametros = new Dictionary<string, string>();

							foreach (var paramName in parametros2.ParameterNames)
							{
								string paramBaseNombre = System.Text.RegularExpressions.Regex.Replace(paramName, @"\d+$", "");
								string nuevoParamName = paramBaseNombre + indiceGlobal;

								mapeoParametros[paramName] = nuevoParamName;

								var paramValue = parametros2.Get<dynamic>(paramName);
								parametrosBatch.Add(nuevoParamName, paramValue);
							}

							string sqlNormalizado = sql;
							foreach (var kvp in mapeoParametros)
							{
								sqlNormalizado = sqlNormalizado.Replace(kvp.Key, kvp.Value);
							}

							sqlBatch.Append(sqlNormalizado);
							indiceGlobal++;
						}

						await Herramientas.BaseDatos.RestoOperaciones(async (conexion, transaccion) =>
						{
							return await conexion.ExecuteAsync(sqlBatch.ToString(), parametrosBatch, transaction: transaccion);
						});
					}
				}

				var idsSteamEncontrados = juegosEncontrados.Select(d => (int)d.idSteam).ToList();
				var idsSteamNoEncontrados = idsSteam.Where(id => !idsSteamEncontrados.Contains(id)).ToList();

				if (rapido == false && idsSteamNoEncontrados.Count > 0)
				{
					foreach (var idSteam in idsSteamNoEncontrados)
					{
						var oferta = mapaOfertas[idSteam];
						var reseña = mapaReseñas.ContainsKey(idSteam) ? mapaReseñas[idSteam] : null;
						string idSteam2 = idSteam.ToString();

						try
						{
							Juego juego = await APIs.Steam.Juego.CargarDatosJuego(idSteam2);

							if (juego != null)
							{
								if (reseña != null)
								{
									if (string.IsNullOrEmpty(reseña.Cantidad) == false && string.IsNullOrEmpty(reseña.Porcentaje) == false)
									{
										JuegoAnalisis nuevasReseñas = new JuegoAnalisis();
										nuevasReseñas.Cantidad = reseña.Cantidad;
										nuevasReseñas.Porcentaje = reseña.Porcentaje;

										juego.Analisis = nuevasReseñas;
									}
								}

								if (region == TiendaRegion.Europa)
								{
									juego.PrecioActualesTiendas ??= new List<JuegoPrecio>();
									juego.PrecioMinimosHistoricos ??= new List<JuegoPrecio>();

									if (juego.PrecioActualesTiendas.Count == 0)
									{
										juego.PrecioActualesTiendas.Add(oferta);
										juego.PrecioMinimosHistoricos.Add(oferta);
									}
								}
								else if (region == TiendaRegion.EstadosUnidos)
								{
									juego.PrecioActualesTiendasUS ??= new List<JuegoPrecio>();
									juego.PrecioMinimosHistoricosUS ??= new List<JuegoPrecio>();

									if (juego.PrecioActualesTiendasUS.Count == 0)
									{
										juego.PrecioActualesTiendasUS.Add(oferta);
										juego.PrecioMinimosHistoricosUS.Add(oferta);
									}
								}

								await Juegos.Insertar.Ejecutar(juego, region);
							}
						}
						catch { }
					}
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Tiendas Comprobar Steam", ex);
			}
		}

		public static async Task Resto(TiendaRegion region, JuegoPrecio oferta, string idGog = null, string slugGOG = null, string slugEpic = null)
		{
			bool encontrado = false;

			string precioMinimosHistoricos = "precioMinimosHistoricos";
			string precioActualesTiendas = "precioActualesTiendas";

			if (region == TiendaRegion.EstadosUnidos)
			{
				precioMinimosHistoricos = "precioMinimosHistoricosUS";
				precioActualesTiendas = "precioActualesTiendasUS";
			}

			string esquema = $"tienda{oferta.Tienda}";
			string sqlBuscar = $@"
SELECT j.id,
       j.{precioMinimosHistoricos},
       j.{precioActualesTiendas},
       j.idSteam,
       j.historicos,
       j.analisis
FROM {esquema} t
CROSS APPLY (
    SELECT TRY_CAST(value AS INT) AS numero
    FROM STRING_SPLIT(t.idJuegos, ',')
) ids
JOIN juegos j ON j.id = ids.numero
WHERE t.enlace = @Enlace
  AND (t.descartado = 'no' OR t.descartado IS NULL)
  AND ids.numero IS NOT NULL
  AND ids.numero != 0;
";

			List<dynamic> resultados = new List<dynamic>();

			try
			{
				resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync(sqlBuscar, new
					{
						Enlace = oferta.Enlace
					})).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Tiendas Comprobar Resto 1", ex);
			}

			if (resultados?.Count > 0)
			{
				var updates = new List<(string sql, DynamicParameters parametros)>();
				int indice = 0;

				foreach (var fila in resultados)
				{
					encontrado = true;

					int id = fila.id ?? 0;
					int idSteam = fila.idSteam ?? 0;

					if (region == TiendaRegion.Europa)
					{
						var ofertasHistoricas = string.IsNullOrEmpty(fila.precioMinimosHistoricos) ? new List<JuegoPrecio>() : JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricos);

						var ofertasActuales = string.IsNullOrEmpty(fila.precioActualesTiendas) ? new List<JuegoPrecio>() : JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioActualesTiendas);

						var historicos = string.IsNullOrEmpty(fila.historicos) ? new List<JuegoHistorico>() : JsonSerializer.Deserialize<List<JuegoHistorico>>(fila.historicos);

						JuegoAnalisis reseñas = null;
						if (!string.IsNullOrEmpty(fila.analisis) && fila.analisis != "null")
						{
							reseñas = JsonSerializer.Deserialize<JuegoAnalisis>(fila.analisis);
						}

						if (ofertasActuales?.Count == 0)
						{
							ofertasActuales.Add(oferta);
						}

						if (id > 0)
						{
							var resultado = await Juegos.Precios.Comprobacion(TiendaTipo.Oficial, id, idSteam, ofertasActuales, ofertasHistoricas, historicos, oferta, slugGOG, idGog, slugEpic, reseñas, indice);

							if (resultado.Item1 != null && resultado.Item2 != null)
							{
								updates.Add((resultado.Item1, resultado.Item2));
								indice += 1;
							}
						}
					}
					else if (region == TiendaRegion.EstadosUnidos)
					{
						var ofertasHistoricasUS = string.IsNullOrEmpty(fila.precioMinimosHistoricosUS) ? new List<JuegoPrecio>() : JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricosUS);

						var ofertasActualesUS = string.IsNullOrEmpty(fila.precioActualesTiendasUS) ? new List<JuegoPrecio>() : JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioActualesTiendasUS);

						var historicosUS = string.IsNullOrEmpty(fila.historicosUS) ? new List<JuegoHistorico>() : JsonSerializer.Deserialize<List<JuegoHistorico>>(fila.historicosUS);

						JuegoAnalisis reseñas = null;
						if (!string.IsNullOrEmpty(fila.analisis) && fila.analisis != "null")
						{
							reseñas = JsonSerializer.Deserialize<JuegoAnalisis>(fila.analisis);
						}

						if (id > 0)
						{
							var resultado = await Juegos.Precios.ComprobacionUS(TiendaTipo.Oficial, id, idSteam, ofertasActualesUS, ofertasHistoricasUS, historicosUS, oferta, slugGOG, idGog, slugEpic, reseñas, indice);

							if (resultado.Item1 != null && resultado.Item2 != null)
							{
								updates.Add((resultado.Item1, resultado.Item2));
								indice += 1;
							}
						}
					}
				}

				if (indice > 0)
				{
					foreach (var grupo in updates.Chunk(100))
					{
						StringBuilder sqlBatch = new StringBuilder();
						DynamicParameters parametrosBatch = new DynamicParameters();

						int indiceGlobal = 0;

						foreach (var (sql, parametros2) in grupo)
						{
							Dictionary<string, string> mapeoParametros = new Dictionary<string, string>();

							foreach (var paramName in parametros2.ParameterNames)
							{
								string paramBaseNombre = System.Text.RegularExpressions.Regex.Replace(paramName, @"\d+$", "");
								string nuevoParamName = paramBaseNombre + indiceGlobal;

								mapeoParametros[paramName] = nuevoParamName;

								var paramValue = parametros2.Get<dynamic>(paramName);
								parametrosBatch.Add(nuevoParamName, paramValue);
							}

							string sqlNormalizado = sql;
							foreach (var kvp in mapeoParametros)
							{
								sqlNormalizado = sqlNormalizado.Replace(kvp.Key, kvp.Value);
							}

							sqlBatch.Append(sqlNormalizado);
							indiceGlobal++;
						}

						await Herramientas.BaseDatos.RestoOperaciones(async (conexion, transaccion) =>
						{
							return await conexion.ExecuteAsync(sqlBatch.ToString(), parametrosBatch, transaction: transaccion);
						});
					}
				}
			}

			// -----------------------------------------------------------------

			if (encontrado == false)
			{
				string sqlInsertar = $@"
					IF NOT EXISTS (SELECT 1 FROM {esquema} WHERE enlace = @Enlace)
					BEGIN
						DECLARE @nuevaId NVARCHAR(MAX); 

						SELECT @nuevaId = id 
						FROM juegos 
						WHERE nombreCodigo = @NombreCodigo;

						IF @nuevaId IS NULL SET @nuevaId = 0;

						INSERT INTO {esquema} (enlace, nombre, imagen, idJuegos, descartado)
						VALUES (@Enlace, @Nombre, @Imagen, @nuevaId, 'no');
					END;
					";

				try
				{
					await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(sqlInsertar, new
						{
							oferta.Enlace,
							oferta.Nombre,
							oferta.Imagen,
							NombreCodigo = Herramientas.Buscador.LimpiarNombre(oferta.Nombre)
						}, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Tiendas Comprobar Resto 2 - " + sqlInsertar, ex);
				}
			}
		}

		public static async Task Resto(TiendaRegion region, List<JuegoPrecio> ofertas)
		{
			if (ofertas == null || ofertas.Count == 0)
			{
				return;
			}

			TiendaTipo tiendaTipo = TiendasCargar.GenerarListado().Find(t => t.Id == ofertas[0].Tienda)?.Tipo ?? TiendaTipo.Oficial;

			string precioMinimosHistoricos = string.Empty;
			string precioActualesTiendas = string.Empty;

			if (tiendaTipo == TiendaTipo.Oficial && region == TiendaRegion.Europa)
			{
				precioMinimosHistoricos = "precioMinimosHistoricos";
				precioActualesTiendas = "precioActualesTiendas";
			}
			else if (tiendaTipo == TiendaTipo.Oficial && region == TiendaRegion.EstadosUnidos)
			{
				precioMinimosHistoricos = "precioMinimosHistoricosUS";
				precioActualesTiendas = "precioActualesTiendasUS";
			}
			else if (tiendaTipo == TiendaTipo.NoOficial && region == TiendaRegion.Europa)
			{
				precioMinimosHistoricos = "preciosHistoricosNoOficialesEU";
				precioActualesTiendas = "preciosActualesNoOficialesEU";
			}
			else if (tiendaTipo == TiendaTipo.NoOficial && region == TiendaRegion.EstadosUnidos)
			{
				precioMinimosHistoricos = "preciosHistoricosNoOficialesUS";
				precioActualesTiendas = "preciosActualesNoOficialesUS";
			}

			if (string.IsNullOrEmpty(precioMinimosHistoricos) == false && string.IsNullOrEmpty(precioActualesTiendas) == false)
			{
				var ofertasPorTienda = ofertas.GroupBy(o => o.Tienda).ToList();

				foreach (var grupoTienda in ofertasPorTienda)
				{
					string esquema = $"tienda{grupoTienda.Key}";

					List<JuegoPrecio> enlacesUnicos = grupoTienda.DistinctBy(o => o.Enlace).ToList();
					List<string> enlaces = enlacesUnicos.Select(o => o.Enlace).ToList();

					const int maxParametrosPorChunk = 2000;

					List<string[]> chunksEnlaces = enlaces.Chunk(maxParametrosPorChunk).ToList();

					Dictionary<string, JuegoPrecio> mapaOfertas = enlacesUnicos.ToDictionary(o => o.Enlace, o => o);
					HashSet<string> enlacesEncontrados = new HashSet<string>();

					var estadosPorJuego = new Dictionary<int, (
						List<JuegoPrecio> ofertasActuales,
						List<JuegoPrecio> ofertasHistoricas,
						List<JuegoHistorico> historicos,
						JuegoAnalisis reseñas,
						int idSteam
					)>();

					var updates = new Dictionary<int, (string sql, DynamicParameters parametros)>();
					int indice = 0;

					foreach (var chunkEnlaces in chunksEnlaces)
					{
						string placeholders = string.Join(",", chunkEnlaces.Select((_, i) => $"@enlace{i}"));

						string sqlBuscar = $@"
						SELECT j.id,
							   j.{precioMinimosHistoricos},
							   j.{precioActualesTiendas},
							   j.idSteam,
							   j.historicos,
							   j.analisis,
							   t.enlace
						FROM {esquema} t
						CROSS APPLY (
							SELECT TRY_CAST(value AS INT) AS numero
							FROM STRING_SPLIT(t.idJuegos, ',')
						) ids
						JOIN juegos j ON j.id = ids.numero
						WHERE t.enlace IN ({placeholders})
						    AND (t.descartado = 'no' OR t.descartado IS NULL)
							AND ids.numero IS NOT NULL
							AND ids.numero != 0
						";

						DynamicParameters parametros = new DynamicParameters();
						for (int i = 0; i < chunkEnlaces.Length; i++)
						{
							parametros.Add($"@enlace{i}", chunkEnlaces[i]);
						}

						List<dynamic> resultados = new List<dynamic>();

						try
						{
							resultados = await Herramientas.BaseDatos.Select(async conexion =>
							{
								return (await conexion.QueryAsync(sqlBuscar, parametros)).ToList();
							});
						}
						catch (Exception ex)
						{
							BaseDatos.Errores.Insertar.Mensaje("Tiendas Comprobar Resto 1 (" + grupoTienda.Key + " - " + ofertas.Count.ToString() + ")", ex);
							continue;
						}

						if (resultados?.Count > 0)
						{
							foreach (var fila in resultados)
							{
								string enlace = fila.enlace;
								enlacesEncontrados.Add(enlace);

								if (mapaOfertas.ContainsKey(enlace) == false)
								{
									continue;
								}

								JuegoPrecio oferta = mapaOfertas[enlace];
								int id = fila.id ?? 0;
								int idSteam = fila.idSteam ?? 0;

								if (region == TiendaRegion.Europa)
								{
									List<JuegoPrecio> ofertasHistoricasEU = null;
									List<JuegoPrecio> ofertasActualesEU = null;
									List<JuegoHistorico> historicosEU = null;
									JuegoAnalisis reseñas = null;

									if (estadosPorJuego.TryGetValue(id, out var estadoPrevio))
									{
										ofertasActualesEU = estadoPrevio.ofertasActuales;
										ofertasHistoricasEU = estadoPrevio.ofertasHistoricas;
										historicosEU = estadoPrevio.historicos;
										reseñas = estadoPrevio.reseñas;
										idSteam = estadoPrevio.idSteam;
									}
									else
									{
										if (tiendaTipo == TiendaTipo.Oficial)
										{
											ofertasHistoricasEU = string.IsNullOrEmpty(fila.precioMinimosHistoricos) || fila.precioMinimosHistoricos == "null"
												? new List<JuegoPrecio>()
												: JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricos);

											ofertasActualesEU = string.IsNullOrEmpty(fila.precioActualesTiendas) || fila.precioActualesTiendas == "null"
												? new List<JuegoPrecio>()
												: JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioActualesTiendas);

											historicosEU = string.IsNullOrEmpty(fila.historicos) || fila.historicos == "null"
												? new List<JuegoHistorico>()
												: JsonSerializer.Deserialize<List<JuegoHistorico>>(fila.historicos);
										}
										else if (tiendaTipo == TiendaTipo.NoOficial)
										{
											ofertasHistoricasEU = string.IsNullOrEmpty(fila.preciosHistoricosNoOficialesEU) || fila.preciosHistoricosNoOficialesEU == "null"
												? new List<JuegoPrecio>()
												: JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.preciosHistoricosNoOficialesEU);

											ofertasActualesEU = string.IsNullOrEmpty(fila.preciosActualesNoOficialesEU) || fila.preciosActualesNoOficialesEU == "null"
												? new List<JuegoPrecio>()
												: JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.preciosActualesNoOficialesEU);
										}

										if (string.IsNullOrEmpty(fila.analisis) == false && fila.analisis != "null")
										{
											reseñas = JsonSerializer.Deserialize<JuegoAnalisis>(fila.analisis);
										}
									}

									if (id > 0)
									{
										var resultado = await Juegos.Precios.Comprobacion(tiendaTipo, id, idSteam, ofertasActualesEU, ofertasHistoricasEU, historicosEU, oferta, null, null, null, reseñas, indice);

										if (resultado.Item1 != null && resultado.Item2 != null)
										{
											if (tiendaTipo == TiendaTipo.Oficial)
											{
												var ofertasActualesActualizado = JsonSerializer.Deserialize<List<JuegoPrecio>>(resultado.Item2.Get<string>($"@precioActualesTiendas{indice}"));

												var ofertasHistoricasActualizado = JsonSerializer.Deserialize<List<JuegoPrecio>>(resultado.Item2.Get<string>($"@precioMinimosHistoricos{indice}"));

												var historicosActualizado = resultado.Item2.ParameterNames.Contains($"@historicos{indice}")
													? JsonSerializer.Deserialize<List<JuegoHistorico>>(resultado.Item2.Get<string>($"@historicos{indice}"))
													: historicosEU;

												estadosPorJuego[id] = (ofertasActualesActualizado, ofertasHistoricasActualizado, historicosActualizado, reseñas, idSteam);
											}
											else if (tiendaTipo == TiendaTipo.NoOficial)
											{
												var ofertasActualesActualizado = JsonSerializer.Deserialize<List<JuegoPrecio>>(resultado.Item2.Get<string>($"@preciosActualesNoOficialesEU{indice}"));

												var ofertasHistoricasActualizado = JsonSerializer.Deserialize<List<JuegoPrecio>>(resultado.Item2.Get<string>($"@preciosHistoricosNoOficialesEU{indice}"));

												estadosPorJuego[id] = (ofertasActualesActualizado, ofertasHistoricasActualizado, null, reseñas, idSteam);
											}

											updates[id] = (resultado.Item1, resultado.Item2);
											indice += 1;
										}
									}
								}
								else if (region == TiendaRegion.EstadosUnidos)
								{
									List<JuegoPrecio> ofertasHistoricasUS = null;
									List<JuegoPrecio> ofertasActualesUS = null;
									List<JuegoHistorico> historicosUS = null;
									JuegoAnalisis reseñas = null;

									if (estadosPorJuego.TryGetValue(id, out var estadoPrevio))
									{
										ofertasActualesUS = estadoPrevio.ofertasActuales;
										ofertasHistoricasUS = estadoPrevio.ofertasHistoricas;
										historicosUS = estadoPrevio.historicos;
										reseñas = estadoPrevio.reseñas;
										idSteam = estadoPrevio.idSteam;
									}
									else
									{
										if (tiendaTipo == TiendaTipo.Oficial)
										{
											ofertasHistoricasUS = string.IsNullOrEmpty(fila.precioMinimosHistoricosUS) || fila.precioMinimosHistoricosUS == "null"
												? new List<JuegoPrecio>()
												: JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioMinimosHistoricosUS);

											ofertasActualesUS = string.IsNullOrEmpty(fila.precioActualesTiendasUS) || fila.precioActualesTiendasUS == "null"
												? new List<JuegoPrecio>()
												: JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.precioActualesTiendasUS);

											historicosUS = string.IsNullOrEmpty(fila.historicosUS) || fila.historicosUS == "null"
												? new List<JuegoHistorico>()
												: JsonSerializer.Deserialize<List<JuegoHistorico>>(fila.historicosUS);
										}
										else if (tiendaTipo == TiendaTipo.NoOficial)
										{
											ofertasHistoricasUS = string.IsNullOrEmpty(fila.preciosHistoricosNoOficialesUS) || fila.preciosHistoricosNoOficialesUS == "null"
												? new List<JuegoPrecio>()
												: JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.preciosHistoricosNoOficialesUS);

											ofertasActualesUS = string.IsNullOrEmpty(fila.preciosActualesNoOficialesUS) || fila.preciosActualesNoOficialesUS == "null"
												? new List<JuegoPrecio>()
												: JsonSerializer.Deserialize<List<JuegoPrecio>>(fila.preciosActualesNoOficialesUS);
										}

										if (string.IsNullOrEmpty(fila.analisis) == false && fila.analisis != "null")
										{
											reseñas = JsonSerializer.Deserialize<JuegoAnalisis>(fila.analisis);
										}
									}

									if (id > 0)
									{
										var resultado = await Juegos.Precios.ComprobacionUS(tiendaTipo, id, idSteam, ofertasActualesUS, ofertasHistoricasUS, historicosUS, oferta, null, null, null, reseñas, indice);

										if (resultado.Item1 != null && resultado.Item2 != null)
										{
											if (tiendaTipo == TiendaTipo.Oficial)
											{
												var ofertasActualesActualizado = JsonSerializer.Deserialize<List<JuegoPrecio>>(resultado.Item2.Get<string>($"@precioActualesTiendasUS{indice}"));

												var ofertasHistoricasActualizado = JsonSerializer.Deserialize<List<JuegoPrecio>>(resultado.Item2.Get<string>($"@precioMinimosHistoricosUS{indice}"));

												var historicosActualizado = resultado.Item2.ParameterNames.Contains($"@historicosUS{indice}")
													? JsonSerializer.Deserialize<List<JuegoHistorico>>(resultado.Item2.Get<string>($"@historicosUS{indice}"))
													: historicosUS;

												estadosPorJuego[id] = (ofertasActualesActualizado, ofertasHistoricasActualizado, historicosActualizado, reseñas, idSteam);
											}
											else if (tiendaTipo == TiendaTipo.NoOficial)
											{
												var ofertasActualesActualizado = JsonSerializer.Deserialize<List<JuegoPrecio>>(resultado.Item2.Get<string>($"@preciosActualesNoOficialesUS{indice}"));

												var ofertasHistoricasActualizado = JsonSerializer.Deserialize<List<JuegoPrecio>>(resultado.Item2.Get<string>($"@preciosHistoricosNoOficialesUS{indice}"));

												estadosPorJuego[id] = (ofertasActualesActualizado, ofertasHistoricasActualizado, null, reseñas, idSteam);
											}

											updates[id] = (resultado.Item1, resultado.Item2);
											indice += 1;
										}
									}
								}
							}
						}
					}	

					if (indice > 0)
					{
						foreach (var grupo in updates.Values.Chunk(100))
						{
							StringBuilder sqlBatch = new StringBuilder();
							DynamicParameters parametrosBatch = new DynamicParameters();

							int indiceGlobal = 0;

							foreach (var (sql, parametros2) in grupo)
							{
								Dictionary<string, string> mapeoParametros = new Dictionary<string, string>();

								foreach (var paramName in parametros2.ParameterNames)
								{
									string paramBaseNombre = System.Text.RegularExpressions.Regex.Replace(paramName, @"\d+$", "");
									string nuevoParamName = paramBaseNombre + indiceGlobal;

									mapeoParametros[paramName] = nuevoParamName;

									var paramValue = parametros2.Get<dynamic>(paramName);
									parametrosBatch.Add(nuevoParamName, paramValue);
								}

								string sqlNormalizado = sql;
								foreach (var kvp in mapeoParametros)
								{
									sqlNormalizado = sqlNormalizado.Replace(kvp.Key, kvp.Value);
								}

								sqlBatch.Append(sqlNormalizado);
								indiceGlobal++;
							}

							try
							{
								await Herramientas.BaseDatos.RestoOperaciones(async (conexion, transaccion) =>
								{
									return await conexion.ExecuteAsync(sqlBatch.ToString(), parametrosBatch, transaction: transaccion);
								});
							}
							catch
							{
								string sqlDebug = DapperSentencias.ConstruirSqlDebug(sqlBatch.ToString(), parametrosBatch);
								
								BaseDatos.Errores.Insertar.Mensaje("Error en sentencia SQL", sqlDebug);
							}
						}
					}

					var enlacesNoEncontrados = mapaOfertas.Where(kv => enlacesEncontrados.Contains(kv.Key) == false).ToList();

					if (enlacesNoEncontrados.Count > 0)
					{
						foreach (var kvp in enlacesNoEncontrados)
						{
							var oferta = kvp.Value;

							string sqlMergear = $@"MERGE INTO {esquema} AS mergeador
								USING (SELECT @Enlace AS enlace) AS fuente
								ON mergeador.enlace = fuente.enlace
								WHEN NOT MATCHED THEN
									INSERT (enlace, nombre, imagen)
									VALUES (@Enlace, @Nombre, @Imagen);";

							try
							{
								await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
								{
									return await conexion.ExecuteAsync(sqlMergear, new
									{
										oferta.Enlace,
										oferta.Nombre,
										oferta.Imagen,
										NombreCodigo = Herramientas.Buscador.LimpiarNombre(oferta.Nombre)
									}, transaction: sentencia);
								});
							}
							catch (Exception ex)
							{
								BaseDatos.Errores.Insertar.Mensaje("Tiendas Comprobar Resto 2 - " + sqlMergear, ex);
							}
						}
					}
				}
			}
		}
	}
}