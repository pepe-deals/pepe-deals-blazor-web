#nullable disable

using Dapper;
using Herramientas;
using System.Text.Json;
using Tiendas2;

namespace APIs.GOG
{
	public static class Streaming
	{
		public static Streaming2.Streaming Generar()
		{
			Streaming2.Streaming amazonluna = new Streaming2.Streaming
			{
				Id = Streaming2.StreamingTipo.AmazonLuna,
				Nombre = "Luna",
				ImagenLogo = "/imagenes/streaming/amazonluna_logo.webp",
				ImagenIcono = "/imagenes/streaming/amazonluna_icono.webp"
			};

			return amazonluna;
		}

		public static async Task Buscar()
		{
			await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, Generar().Id.ToString(), DateTime.Now, 0);

			int cantidad = 0;

			int i = 1;
			int limite = 20;
			while (i < limite + 1)
			{
				string enlace = "https://catalog.gog.com/v1/filtered-catalog?limit=48&order=desc:discount&productType=in:game,pack,dlc,extras&page=" + i.ToString() + "&pageId=2f70726f6d6f2f706c61792d6f6e2d6c756e61&sectionId=7efd2a6a-0831-43af-bc1f-616509912d24&countryCode=ES&locale=en-US&currencyCode=EUR";
                string html = await Decompiladores.Estandar(enlace);

				if (string.IsNullOrEmpty(html) == false)
				{
					GOGOfertas datos = null;

					try
					{
						datos = JsonSerializer.Deserialize<GOGOfertas>(html);
					}
					catch
					{
						BaseDatos.Errores.Insertar.Mensaje("Amazon Luna GOG", html, enlace);
                    }

                    if (datos?.Juegos?.Count > 0)
					{
						limite = datos.Paginas;

						foreach (var juego in datos.Juegos)
						{
							if (juego.Tipo == "game" || juego.Tipo == "pack")
							{
								DateTime fecha = DateTime.Now;
								fecha = fecha + TimeSpan.FromDays(1);

								bool? encontrado = false;

								string sqlBuscar = "SELECT 1 FROM streamingamazonluna WHERE nombreCodigo=@nombreCodigo";

								try
								{
									encontrado = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
									{
										var resultado = await conexion.QueryFirstOrDefaultAsync<bool?>(
											sqlBuscar,
											new { nombreCodigo = juego.Id }
										);

										return resultado ?? false;  
									});
								}
								catch (Exception ex)
								{
									BaseDatos.Errores.Insertar.Mensaje("Amazon Luna 1", ex);
								}

								List<string> drms = new List<string>();
								drms.Add("gog");

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

								if (encontrado == true)
								{
									cantidad += 1;
									await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, Generar().Id.ToString(), DateTime.Now, cantidad);

									string sqlActualizar = @"UPDATE streamingamazonluna 
													SET fecha=@fecha, drms=@drms, drms2=@drms2 WHERE nombreCodigo=@nombreCodigo";

									try
									{
										await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
										{
											return await conexion.ExecuteAsync(sqlActualizar, new
											{
												nombreCodigo = juego.Id,
												fecha,
												drms = JsonSerializer.Serialize(drms),
												drms2 = JsonSerializer.Serialize(drms2)
											}, transaction: sentencia);
										});
									}
									catch (Exception ex)
									{
										BaseDatos.Errores.Insertar.Mensaje("Amazon Luna 2", ex);
									}
								}
								else
								{
									string sqlInsertar = "INSERT INTO streamingamazonluna " +
														"(nombreCodigo, nombre, drms, fecha, drms2) VALUES " +
														"(@nombreCodigo, @nombre, @drms, @fecha, @drms2) ";

									try
									{
										await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
										{
											return await conexion.ExecuteAsync(sqlInsertar, new
											{
												nombreCodigo = juego.Id,
												nombre = juego.Nombre,
												drms = JsonSerializer.Serialize(drms),
												fecha,
												drms2 = JsonSerializer.Serialize(drms2)
											}, transaction: sentencia);
										});
									}
									catch (Exception ex)
									{
										BaseDatos.Errores.Insertar.Mensaje("Amazon Luna 3", ex);
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
}
