#nullable disable

using Dapper;
using Herramientas;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.Boosteroid
{
	public static class Streaming
	{
		public static Streaming2.Streaming Generar()
		{
			Streaming2.Streaming boosteroid = new Streaming2.Streaming
			{
				Id = Streaming2.StreamingTipo.Boosteroid,
				Nombre = "Boosteroid",
				ImagenLogo = "/imagenes/streaming/boosteroid_logo.webp",
				ImagenIcono = "/imagenes/streaming/boosteroid_icono.webp",
				PrecioMinimo = 7.49,
				EnlaceCuota = "https://cloud.boosteroid.com/profile/account/billing-plans"
			};

			return boosteroid;
		}

		public static async Task Buscar()
		{
			await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, "boosteroid", DateTime.Now, 0);

			JsonSerializerOptions opciones = new JsonSerializerOptions
			{
				UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
			};

			int cantidad = 0;

			int i = 1;
			while (i < 100)
			{
				string html = await Decompiladores.GZipFormato3("https://cloud.boosteroid.com/api/v1/public/applications?orderBy=popularity&collection=1&page=" + i.ToString());

				if (string.IsNullOrEmpty(html) == false)
				{
					BoosteroidDatos datos = JsonSerializer.Deserialize<BoosteroidDatos>(html, opciones);

					if (datos != null)
					{
						if (datos.Juegos == null)
						{
							break;
						}
						else
						{
							if (datos.Juegos.Count == 0)
							{
								break;
							}
							else
							{
								foreach (var juego in datos.Juegos)
								{
									List<string> drms = new List<string>();

									if (juego.DRMs != null)
									{
										if (string.IsNullOrEmpty(juego.DRMs.Steam) == false)
										{
											drms.Add("Steam");
										}

										if (string.IsNullOrEmpty(juego.DRMs.EpicGames) == false)
										{
											drms.Add("Epic Games");
										}

										if (string.IsNullOrEmpty(juego.DRMs.BattleNet) == false)
										{
											drms.Add("Battle.net");
										}
									}

									List<int> drms2 = new List<int>();

									if (drms?.Count > 0)
									{
										foreach (var drm in drms)
										{
											var drmTraducido = Juegos.JuegoDRM2.Traducir(drm);

											if (drmTraducido != Juegos.JuegoDRM.NoEspecificado)
											{
												drms2.Add((int)drmTraducido);
											}
										}
									}

									if (drms.Count > 0)
									{
										DateTime fecha = DateTime.Now;
										fecha = fecha + TimeSpan.FromDays(1);

										bool encontrado = false;

										string sqlBuscar = "SELECT 1 FROM streamingboosteroid WHERE id=@id";

										try
										{
											encontrado = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
											{
												return await conexion.QueryFirstOrDefaultAsync<bool>(sqlBuscar, new { id = juego.Id });
											});
										}
										catch (Exception ex)
										{
											BaseDatos.Errores.Insertar.Mensaje("Boosteroid 1", ex);
										}

										if (encontrado == true)
										{
											cantidad += 1;
											await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, "boosteroid", DateTime.Now, cantidad);

											string sqlActualizar = "UPDATE streamingboosteroid " +
																"SET fecha=@fecha, drms=@drms, drms2=@drms2 WHERE id=@id";

											try
											{
												await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
												{
													return await conexion.ExecuteAsync(sqlActualizar, new { 
														id = juego.Id, 
														fecha,
														drms = JsonSerializer.Serialize(drms),
														drms2 = JsonSerializer.Serialize(drms2)
													}, transaction: sentencia);
												});
											}
											catch (Exception ex)
											{
												BaseDatos.Errores.Insertar.Mensaje("Boosteroid 2", ex);
											}
										}
										else 
										{
											string sqlInsertar = "INSERT INTO streamingboosteroid " +
															"(id, nombre, drms, fecha, drms2) VALUES " +
															"(@id, @nombre, @drms, @fecha, @drms2) ";

											try
											{
												await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
												{
													return await conexion.ExecuteAsync(sqlInsertar, new
													{
														id = juego.Id,
														nombre = juego.Nombre,
														drms = JsonSerializer.Serialize(drms),
														fecha,
														drms2 = JsonSerializer.Serialize(drms2)
													}, transaction: sentencia);
												});
											}
											catch (Exception ex)
											{
												BaseDatos.Errores.Insertar.Mensaje("Boosteroid 3", ex);
											}
										}
									}
								}
							}
						}
					}
				}

				i += 1;
			}
		}
	}

	public class BoosteroidDatos
	{
		[JsonPropertyName("data")]
		public List<BoosteroidDatosJuego> Juegos { get; set; }
	}

	public class BoosteroidDatosJuego
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("icon")]
		public string Imagen { get; set; }

		[JsonPropertyName("stores")]
		public BoosteroidDatosJuegoTiendas DRMs { get; set; }
	}

	public class BoosteroidDatosJuegoTiendas
	{
		[JsonPropertyName("steam")]
		public string Steam { get; set; }

		[JsonPropertyName("epic-games-store")]
		public string EpicGames { get; set; }

		[JsonPropertyName("battlenet")]
		public string BattleNet { get; set; }
	}
}
