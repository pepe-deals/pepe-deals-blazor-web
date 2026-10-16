#nullable disable

using Dapper;
using Microsoft.VisualBasic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.GeforceNOW
{
    public static class Streaming
    {
        public static Streaming2.Streaming Generar()
        {
            Streaming2.Streaming geforcenow = new Streaming2.Streaming
            {
                Id = Streaming2.StreamingTipo.GeforceNOW,
                Nombre = "Geforce NOW",
                ImagenLogo = "/imagenes/streaming/geforcenow_logo.webp",
                ImagenIcono = "/imagenes/streaming/geforcenow_icono.webp",
				PrecioMinimo = 10.99,
				EnlaceCuota = "https://www.nvidia.com/geforce-now/premium-memberships/"
			};

            return geforcenow;
        }

        public static async Task Buscar()
        {
            await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, "geforcenow", DateTime.Now, 0);

			JsonSerializerOptions opciones = new JsonSerializerOptions
			{
				UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
			};

			int cantidad = 0;
            string cadena = string.Empty;

            int i = 0;
            while (i < 10)
            {
                HttpClient cliente = new HttpClient();
                cliente.BaseAddress = new Uri("https://www.nvidia.com/");
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string cadenaFinal = string.Empty;
                if (string.IsNullOrEmpty(cadena) == false)
                {
                    cadenaFinal = "after:" + Strings.ChrW(34) + cadena + Strings.ChrW(34) + " ";
                }

                HttpRequestMessage peticion = new HttpRequestMessage(HttpMethod.Post, "https://api-prod.nvidia.com/services/gfngames/v1/gameList");
                peticion.Content = new StringContent("{ apps(country:\"US\" language:\"en_US\" " + cadenaFinal + ") {\r\n        numberReturned\r\n        pageInfo {\r\n          endCursor\r\n          hasNextPage\r\n        }\r\n        items {\r\n        title\r\n        sortName\r\n        \r\n      variants{\r\n        appStore\r\n        publisherName\r\n          }\r\n        }\r\n}}",
                                                    Encoding.UTF8, "application/json");

                HttpResponseMessage respuesta = await cliente.SendAsync(peticion);

                string html = string.Empty;

                try
                {
                    html = await respuesta.Content.ReadAsStringAsync();
                }
                catch { }

                if (string.IsNullOrEmpty(html) == false)
                {
                    GeforceNOWDatos datos = JsonSerializer.Deserialize<GeforceNOWDatos>(html, opciones);

					if (datos?.Datos?.Info != null)
					{
						if (datos?.Datos?.Info?.SiguientePagina != null)
						{
							if (datos.Datos.Info.SiguientePagina.Existe == true)
							{
								cadena = datos.Datos.Info.SiguientePagina.Cadena;
							}
							else
							{
								i = 100;
							}
						}

						foreach (var juego in datos.Datos.Info.Juegos)
						{
							bool añadir = true;

							if (juego.Nombre.EndsWith(" Demo") == true)
							{
								añadir = false;
							}

							if (añadir == true)
							{
								List<string> drms = new List<string>();

								foreach (var drm in juego.DRMs)
								{
									drms.Add(drm.DRM);
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

								DateTime fecha = DateTime.Now;
								fecha = fecha + TimeSpan.FromDays(1);

								bool encontrado = false;

								string sqlBuscar = "SELECT 1 FROM streaminggeforcenow WHERE nombreCodigo=@nombreCodigo";

								try
								{
									encontrado = await Herramientas.BaseDatos.Select(async conexion =>
									{
										return await conexion.QueryFirstOrDefaultAsync<bool>(sqlBuscar, new { nombreCodigo = Herramientas.Buscador.LimpiarNombre(juego.Nombre, true) });
									});
								}
								catch (Exception ex)
								{
									BaseDatos.Errores.Insertar.Mensaje("Geforce NOW 1", ex);
								}

								if (encontrado == true)
								{
									cantidad += 1;
									await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, "geforcenow", DateTime.Now, cantidad);

									string sqlActualizar = "UPDATE streaminggeforcenow " +
															"SET fecha=@fecha, drms=@drms, drms2=@drms2 WHERE nombreCodigo=@nombreCodigo";

									try
									{
										await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
										{
											return await conexion.ExecuteAsync(sqlActualizar, new
											{
												nombreCodigo = Herramientas.Buscador.LimpiarNombre(juego.Nombre, true),
												fecha,
												drms = JsonSerializer.Serialize(drms),
												drms2 = JsonSerializer.Serialize(drms2)
											}, transaction: sentencia);
										});
									}
									catch (Exception ex)
									{
										BaseDatos.Errores.Insertar.Mensaje("Geforce NOW 2", ex);
									}
								}
								else
								{
									string sqlInsertar = "INSERT INTO streaminggeforcenow " +
														"(nombreCodigo, nombre, drms, fecha, drms2) VALUES " +
														"(@nombreCodigo, @nombre, @drms, @fecha, @drms2) ";

									try
									{
										await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
										{
											return await conexion.ExecuteAsync(sqlInsertar, new
											{
												nombreCodigo = Herramientas.Buscador.LimpiarNombre(juego.Nombre, true),
												nombre = juego.Nombre,
												drms = JsonSerializer.Serialize(drms),
												fecha,
												drms2 = JsonSerializer.Serialize(drms2)
											}, transaction: sentencia);
										});
									}
									catch (Exception ex)
									{
										BaseDatos.Errores.Insertar.Mensaje("Geforce NOW 3", ex);
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

    public class GeforceNOWDatos
    {
        [JsonPropertyName("data")]
        public GeforceNOWDatosApps Datos { get; set; }
    }

    public class GeforceNOWDatosApps
    {
        [JsonPropertyName("apps")]
        public GeforceNOWDatosInfo Info { get; set; }
    }

    public class GeforceNOWDatosInfo
    {
        [JsonPropertyName("pageInfo")]
        public GeforceNOWDatosInfoPagina SiguientePagina { get; set; }

        [JsonPropertyName("items")]
        public List<GeforceNOWDatosInfoJuego> Juegos { get; set; }
    }

    public class GeforceNOWDatosInfoPagina
    {
        [JsonPropertyName("endCursor")]
        public string Cadena { get; set; }

        [JsonPropertyName("hasNextPage")]
        public bool Existe { get; set; }
    }

    public class GeforceNOWDatosInfoJuego
    {
        [JsonPropertyName("title")]
        public string Nombre { get; set; }

        [JsonPropertyName("variants")]
        public List<GeforceNOWDatosInfoJuegoDRM> DRMs { get; set; }
    }

    public class GeforceNOWDatosInfoJuegoDRM
    {
        [JsonPropertyName("appStore")]
        public string DRM { get; set; }
    }
}
