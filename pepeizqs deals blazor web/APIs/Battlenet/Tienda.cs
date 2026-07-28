#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.Battlenet
{
    public class Tienda
    {
        public static Tiendas2.Tienda Generar()
        {
            Tiendas2.Tienda tienda = new Tiendas2.Tienda
            {
                Id = "battlenet",
                Nombre = "Battlenet Store",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/battlenet_logo.webp",
                Imagen300x80 = "/imagenes/tiendas/battlenet_300x80.webp",
                ImagenIcono = "/imagenes/tiendas/battlenet_icono.webp",
                Color = "#005aad",
                AdminEnseñar = true,
                AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

            return tienda;
        }

        private static List<string> ListaSlugs()
        {
            List<string> slugs = [
				"avowed",
				"call-of-duty-black-ops-4",
				"call-of-duty-black-ops-6",
				"call-of-duty-black-ops-cold-war",
				"call-of-duty-modern-warfare",
				"call-of-duty-modern-warfare-2-campaign-remastered",
				"call-of-duty-modern-warfare-ii", 
				"call-of-duty-modern-warfare-iii",
				"call-of-duty-vanguard",
				"crash-bandicoot-4",
				"diablo",
                "diablo_ii_resurrected",
				"diablo-iv",
				"doom-the-dark-ages",
				"sea-of-thieves",
				"the-outer-worlds-2",
				"tony-hawks-pro-skater-3-4",
				"warcraft-orcs-and-humans"];

            return slugs;
        }

        public static async Task BuscarOfertas(TiendaRegion region)
        {
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			int juegos2 = 0;

			List<string> listaSlugs = ListaSlugs();

			foreach (var slug in listaSlugs)
			{
				string enlace = string.Empty;

				if (region == TiendaRegion.Europa)
				{
					enlace = "https://eu.shop.battle.net/api/product/" + slug + "?platform=Web&locale=en-us";
				}
				else if (region == TiendaRegion.EstadosUnidos)
				{
					enlace = "https://us.shop.battle.net/api/product/" + slug + "?platform=Web&locale=en-us";
				}

				if (string.IsNullOrEmpty(enlace) == false)
				{
					string html = await Decompiladores.Estandar(enlace);

					if (string.IsNullOrEmpty(html) == false)
					{
						BattlenetJuego juegobattle = null;

						try
						{
							juegobattle = JsonSerializer.Deserialize<BattlenetJuego>(html);
						}
						catch { }

						if (juegobattle != null)
						{
							string nombreFamilia = juegobattle.Nombre;

							if (juegobattle.Productos?.Count > 0)
							{
								List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

								foreach (var producto in juegobattle.Productos)
								{
									if (producto.Precio.Precio != null)
									{
										string textoPrecioRebajado = producto.Precio.Precio.PrecioRebajado;

										if (string.IsNullOrEmpty(textoPrecioRebajado) == false)
										{
											textoPrecioRebajado = textoPrecioRebajado.Replace("€", null);
											textoPrecioRebajado = textoPrecioRebajado.Replace("EUR", null);
											textoPrecioRebajado = textoPrecioRebajado.Replace("$", null);
											textoPrecioRebajado = textoPrecioRebajado.Replace("USD", null);
											textoPrecioRebajado = textoPrecioRebajado.Trim();
										}

										string textoPrecioBase = producto.Precio.Precio.PrecioBase;

										if (string.IsNullOrEmpty(textoPrecioBase) == false)
										{
											textoPrecioBase = textoPrecioBase.Replace("€", null);
											textoPrecioBase = textoPrecioBase.Replace("EUR", null);
											textoPrecioBase = textoPrecioBase.Replace("$", null);
											textoPrecioBase = textoPrecioBase.Replace("USD", null);
											textoPrecioBase = textoPrecioBase.Trim();
										}

										if (string.IsNullOrEmpty(textoPrecioRebajado) == false && string.IsNullOrEmpty(textoPrecioBase) == false)
										{
											decimal precioRebajado = decimal.Parse(textoPrecioRebajado);
											decimal precioBase = decimal.Parse(textoPrecioBase);

											int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

											if (descuento > 0)
											{
												string nombre = nombreFamilia;

												if (string.IsNullOrEmpty(producto.Subnombre) == false)
												{
													if (producto.Subnombre != nombre)
													{
														nombre = nombre + " - " + producto.Subnombre;
													}
												}

												nombre = WebUtility.HtmlDecode(nombre);

												string enlaceJuego = "https://eu.shop.battle.net/en-us/product/" + slug + "?p=" + producto.Id;

												if (region == TiendaRegion.EstadosUnidos)
												{
													enlaceJuego = "https://us.shop.battle.net/en-us/product/" + slug + "?p=" + producto.Id;
												}

												string imagen = producto.Imagen;

												if (string.IsNullOrEmpty(imagen) == false)
												{
													if (imagen.Contains("https:") == false)
													{
														imagen = "https:" + imagen;
													}
												}

												JuegoDRM drm = JuegoDRM.BattleNet;

												JuegoPrecio oferta = new JuegoPrecio
												{
													Nombre = nombre,
													Enlace = enlaceJuego,
													Imagen = imagen,
													Moneda = JuegoMoneda.Euro,
													Precio = precioRebajado,
													Descuento = descuento,
													Tienda = Generar().Id,
													DRM = drm,
													FechaDetectado = DateTime.Now,
													FechaActualizacion = DateTime.Now
												};

												if (region == TiendaRegion.EstadosUnidos)
												{
													oferta.Moneda = JuegoMoneda.Dolar;
												}

												ofertas.Add(oferta);
											}
										}
									}
								}

								if (ofertas?.Count > 0)
								{
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
			}
		}
    }

    #region Clases

    public class BattlenetJuego
    {
        [JsonPropertyName("title")]
        public string Nombre { get; set; }

        [JsonPropertyName("products")]
        public List<BattlenetJuegoProducto> Productos { get; set; }
    }

    public class BattlenetJuegoProducto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("quantificationLabel")]
        public string Subnombre { get; set; }

        [JsonPropertyName("imageUrl")]
        public string Imagen { get; set; }

        [JsonPropertyName("priceInfo")]
        public BattlenetJuegoProductoPrecio Precio { get; set; }
    }

    public class BattlenetJuegoProductoPrecio
    {
        [JsonPropertyName("price")]
        public BattlenetJuegoProductoPrecio2 Precio { get; set; }
    }

    public class BattlenetJuegoProductoPrecio2
    {
        [JsonPropertyName("fullAmount")]
        public string PrecioBase { get; set; }

        [JsonPropertyName("discountAmount")]
        public string PrecioRebajado { get; set; }
    }

    #endregion
}
