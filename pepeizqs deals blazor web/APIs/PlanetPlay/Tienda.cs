#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.PlanetPlay
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "planetplay",
				Nombre = "PlanetPlay",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/planetplay_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/planetplay_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/planetplay_icono.webp",
				Color = "#00CC7E",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			string enlace = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				enlace = "https://prod-ecs-api.planetplay.com/assets/fetch-new?offer=discount&country=es&page=";
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				enlace = "https://prod-ecs-api.planetplay.com/assets/fetch-new?offer=discount&country=us&page=";
			}

			int tope = 10;
			int juegos2 = 0;

			int i = 0;
			while (i < tope)
			{
				string html = await Decompiladores.Estandar(enlace + i.ToString());

				if (string.IsNullOrEmpty(html) == false)
				{
					PlanetPlayDatos datos = JsonSerializer.Deserialize<PlanetPlayDatos>(html);

					if (datos != null)
					{
						tope = datos.Total / 20;

						if (datos.Juegos?.Count > 0)
						{
							List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

							foreach (var juego in datos.Juegos)
							{
								decimal precioBase = juego.PrecioBase;
								decimal precioRebajado = juego.PrecioRebajado;

								int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

								if (descuento > 0)
								{
									string nombre = WebUtility.HtmlDecode(juego.Nombre);

									string enlaceJuego = juego.Enlace;

									bool parar = false;

									if (enlace.Contains("/store-mobile/games/") == true)
									{
										parar = true;
									}

									if (parar == false)
									{
										string imagen = juego.Imagen;

										JuegoDRM drm = JuegoDRM2.Traducir(juego.DRM, Generar().Id);

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

				i += 1;
			}
		}
	}

	public class PlanetPlayDatos
	{
		[JsonPropertyName("results")]
		public List<PlanetPlayJuego> Juegos { get; set; }

		[JsonPropertyName("total")]
		public int Total { get; set; }
	}

	public class PlanetPlayJuego
	{
		[JsonPropertyName("title")]
		public string Nombre { get; set; }

		[JsonPropertyName("url")]
		public string Enlace { get; set; }

		[JsonPropertyName("image")]
		public string Imagen { get; set; }

		[JsonPropertyName("price")]
		public decimal PrecioBase { get; set; }

		[JsonPropertyName("discountPrice")]
		public decimal PrecioRebajado { get; set; }

		[JsonPropertyName("platform")]
		public string DRM { get; set; }
	}
}
