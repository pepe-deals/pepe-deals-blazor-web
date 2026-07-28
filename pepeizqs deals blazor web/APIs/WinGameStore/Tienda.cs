#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.WinGameStore
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "wingamestore",
				Nombre = "WinGameStore",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/wingamestore_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/wingamestore_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/wingamestore_icono.webp",
				Color = "#265c92",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			return enlace + "?ars=pepeizqdeals";
		}

		public static async Task BuscarOfertas(TiendaRegion region) 
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			string html = await Decompiladores.Estandar("https://www.macgamestore.com/affiliate/feeds/p_C1B2A3.json");

			if (string.IsNullOrEmpty(html) == false)
			{
				List<WinGameStoreJuego> juegos = JsonSerializer.Deserialize<List<WinGameStoreJuego>>(html);

                if (juegos?.Count > 0)
                {
					List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

                    foreach (WinGameStoreJuego juego in juegos)
                    {
                        bool buscar = true;

                        if (string.IsNullOrEmpty(juego.PaisesRestringidos) == false)
                        {
                            List<string> listaPaisesRestringidos = new List<string>();

                            string[] datosPartidos = juego.PaisesRestringidos.Split(',');
                            listaPaisesRestringidos.AddRange(datosPartidos);

                            if (listaPaisesRestringidos.Count > 0)
                            {
                                foreach (var pais in listaPaisesRestringidos)
                                {
                                    if (region == TiendaRegion.Europa && pais.ToLower() == "es")
                                    {
                                        buscar = false;
                                        break;
                                    }
									else if (region == TiendaRegion.EstadosUnidos && pais.ToLower() == "us")
									{
										buscar = false;
										break;
									}
								}
                            }
                        }

                        if (string.IsNullOrEmpty(juego.PaisesAprobados) == false)
                        {
                            List<string> listaPaisesAprobados = new List<string>();

                            string[] datosPartidos = juego.PaisesAprobados.Split(',');
                            listaPaisesAprobados.AddRange(datosPartidos);

                            if (listaPaisesAprobados.Count > 0)
                            {
                                bool encontrado = false;
                                foreach (var pais in listaPaisesAprobados)
                                {
                                    if (region == TiendaRegion.Europa && pais.ToLower() == "es")
                                    {
                                        encontrado = true;
                                        break;
                                    }
									else if (region == TiendaRegion.EstadosUnidos && pais.ToLower() == "us")
									{
										encontrado = true;
										break;
									}
								}

                                if (encontrado == false)
                                {
                                    buscar = false;
                                }
                            }
                        }

                        if (buscar == true)
                        {
                            decimal precioBase = decimal.Parse(juego.PrecioBase);
                            decimal precioRebajado = decimal.Parse(juego.PrecioRebajado);

                            int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

                            if (descuento > 0)
                            {
                                string nombre = WebUtility.HtmlDecode(juego.Nombre);

                                string enlace = juego.Enlace;

                                enlace = enlace.Replace("?ars=pepeizqdeals", null);

                                string imagen = juego.Imagen;

                                JuegoDRM drm = JuegoDRM2.Traducir(juego.DRM, Generar().Id);

                                JuegoPrecio oferta = new JuegoPrecio
                                {
                                    Nombre = nombre,
                                    Enlace = enlace,
                                    Imagen = imagen,
                                    Moneda = JuegoMoneda.Dolar,
                                    Precio = precioRebajado,
                                    Descuento = descuento,
                                    Tienda = Generar().Id,
                                    DRM = drm,
                                    FechaDetectado = DateTime.Now,
                                    FechaActualizacion = DateTime.Now
                                };

                                ofertas.Add(oferta);
							}
                        }
                    }

					if (ofertas?.Count > 0)
					{
						int juegos2 = 0;

						int tamaño = 500;
						var lotes = ofertas
							.Select((oferta, indice) => new { oferta, indice })
							.GroupBy(x => x.indice / tamaño)
							.Select(g => g.Select(x => x.oferta).ToList())
							.ToList();

						foreach (var lote in lotes)
						{
							try
							{
								await BaseDatos.Tiendas.Comprobar.Resto(region, lote);
							}
							catch (Exception ex)
							{
								BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
							}

							juegos2 += lote.Count;

							try
							{
								await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, juegos2);
							}
							catch (Exception ex)
							{
								BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
							}
						}
					}
				}
            }
		}
	}

	public class WinGameStoreJuego
	{
		[JsonPropertyName("title")]
		public string Nombre { get; set; }

		[JsonPropertyName("url")]
		public string Enlace { get; set; }

		[JsonPropertyName("current_price")]
		public string PrecioRebajado { get; set; }

		[JsonPropertyName("retail_price")]
		public string PrecioBase { get; set; }

		[JsonPropertyName("drm")]
		public string DRM { get; set; }

		[JsonPropertyName("badge")]
		public string Imagen { get; set; }

		[JsonPropertyName("whitelist")]
		public string PaisesAprobados { get; set; }

		[JsonPropertyName("blacklist")]
		public string PaisesRestringidos { get; set; }
	}
}
