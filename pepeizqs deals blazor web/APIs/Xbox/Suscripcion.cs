//https://catalog.gamepass.com/subscriptions?subscription=all&market=US

#nullable disable

using Dapper;
using Herramientas;
using Juegos;
using Microsoft.VisualBasic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.Xbox
{
    public static class Suscripcion
    {
        public static Suscripciones2.Suscripcion Generar()
        {
            Suscripciones2.Suscripcion gamepass = new Suscripciones2.Suscripcion
            {
                Id = Suscripciones2.SuscripcionTipo.PCGamePass,
                Nombre = "PC Game Pass",
                ImagenLogo = "/imagenes/suscripciones/pcgamepass_logo.webp",
				ImagenIcono = "/imagenes/suscripciones/pcgamepass_icono.webp",
				ImagenFondo = "/imagenes/suscripciones/pcgamepass_fondo.webp",
				ColorDestacado = "rgba(46, 71, 3, 0.3)",
				Enlace = "https://www.xbox.com/xbox-game-pass/pc-game-pass",
                DRMDefecto = JuegoDRM.Microsoft,
                UsuarioEnlacesEspecificos = true,
                ParaSiempre = false,
                Precio = 14.99,
				AdminPendientes = true,
				TablaPendientes = "tiendamicrosoftstore",
				AdminAñadir = false,
				UsuarioPuedeAbrir = true
			};

            return gamepass;
        }

		public static async Task Buscar()
        {
			await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, Generar().Id.ToString().ToLower(), DateTime.Now, 0);

            int cantidad = 0;

			List<string> enlaces = [
				"https://catalog.gamepass.com/sigls/v2?id=fdd9e2a7-0fee-49f6-ad69-4354098401ff&language=en-us&market=US",
                "https://catalog.gamepass.com/sigls/v2?id=1d33fbb9-b895-4732-a8ca-a55c8b99fa2c&language=en-us&market=US"];

			foreach (var enlace2 in enlaces)
			{
                string html = await Decompiladores.GZipFormato3(enlace2);

                if (string.IsNullOrEmpty(html) == false)
                {
                    int int1 = html.IndexOf("{" + Strings.ChrW(34) + "id" + Strings.ChrW(34));
                    string temp1 = html.Remove(0, int1);
                    html = "[" + temp1;

                    List<XboxGamePassJuego> juegos = JsonSerializer.Deserialize<List<XboxGamePassJuego>>(html);

                    if (juegos?.Count > 0)
                    {
                        foreach (var juego in juegos)
                        {
                            string enlace = "https://www.microsoft.com/store/productid/" + juego.Id;

                            bool encontrado = false;

							string sqlBuscar = "SELECT idJuegos FROM tiendamicrosoftstore WHERE enlace=@enlace";

							try
							{
								string idJuegosTexto = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
								{
									return await conexion.QueryFirstOrDefaultAsync<string>(sqlBuscar, new { enlace });
								});

								if (string.IsNullOrEmpty(idJuegosTexto) == false)
								{
									encontrado = true;

									if (idJuegosTexto != "0")
									{
										cantidad += 1;
										await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, Generar().Id.ToString().ToLower(), DateTime.Now, cantidad);

										List<string> idJuegos = Herramientas.Listados.Generar(idJuegosTexto);

										if (idJuegos.Count > 0)
										{
											foreach (var id in idJuegos)
											{
												bool insertar = true;
												var suscripciones = await BaseDatos.Suscripciones.Buscar.JuegoId(int.Parse(id));

												if (suscripciones?.Count > 0)
												{
													foreach (var suscripcion in suscripciones)
													{
														if (suscripcion.Tipo == Suscripciones2.SuscripcionTipo.PCGamePass)
														{
															insertar = false;

															suscripcion.FechaTermina = DateTime.Now + TimeSpan.FromDays(1);

															await BaseDatos.Suscripciones.Actualizar.FechaTermina(suscripcion);
														}
													}
												}

												if (insertar == true)
												{
													DateTime nuevaFecha = DateTime.Now;
													nuevaFecha = nuevaFecha + TimeSpan.FromDays(1);

													JuegoSuscripcion nuevaSuscripcion = new JuegoSuscripcion
													{
														DRM = JuegoDRM.Microsoft,
														Nombre = id,
														FechaEmpieza = DateTime.Now,
														FechaTermina = nuevaFecha,
														JuegoId = int.Parse(id),
														Enlace = enlace,
														Tipo = Suscripciones2.SuscripcionTipo.PCGamePass
													};

													await BaseDatos.Suscripciones.Insertar.Ejecutar(int.Parse(id), nuevaSuscripcion);
												}
											}
										}
									}
								}
							}
							catch (Exception ex)
							{
								BaseDatos.Errores.Insertar.Mensaje("Gamepass", ex);
							}

							if (encontrado == false)
							{
								await BaseDatos.Suscripciones.Insertar.Temporal(Generar().Id.ToString().ToLower(), enlace);
							}
                        }
                    }
					else
					{
						BaseDatos.Errores.Insertar.Mensaje("Error GamePass", html);
					}
                }
            }
        }
    }

    public class XboxGamePassJuego
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
