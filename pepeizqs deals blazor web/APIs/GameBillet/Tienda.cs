//Europe: https://www.gamebillet.com/product/jsonfeed?store=eu&guid=39A6D2B7-A4EF-4E8B-AA19-350B89788365
//UK: https://www.gamebillet.com/product/jsonfeed?store=uk&guid=39A6D2B7-A4EF-4E8B-AA19-350B89788365
//US / CA / ROW: https://www.gamebillet.com/product/jsonfeed?store=us&guid=39A6D2B7-A4EF-4E8B-AA19-350B89788365
//China: https://www.gamebillet.com/product/jsonfeed?store=cn&guid=39A6D2B7-A4EF-4E8B-AA19-350B89788365
//Brazil: https://www.gamebillet.com/product/jsonfeed?store=sa&guid=39A6D2B7-A4EF-4E8B-AA19-350B89788365

#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.GameBillet
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "gamebillet",
				Nombre = "GameBillet",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/gamebillet_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/gamebillet_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/gamebillet_icono.webp",
				Color = "#F1652A",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			return enlace + "?affiliate=64e186aa-fb0e-436f-a000-069090c06fe9";
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			string enlace = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				enlace = "https://www.gamebillet.com/product/jsonfeed?store=eu&guid=39A6D2B7-A4EF-4E8B-AA19-350B89788365";
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				enlace = "https://www.gamebillet.com/product/jsonfeed?store=us&guid=39A6D2B7-A4EF-4E8B-AA19-350B89788365";
			}

			if (string.IsNullOrEmpty(enlace) == false)
			{
				string html = await Decompiladores.Estandar(enlace);

				if (string.IsNullOrEmpty(html) == false)
				{
					GameBilletProductos juegos = JsonSerializer.Deserialize<GameBilletProductos>(html);

					if (juegos?.Juegos?.Count > 0)
					{
						List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

						foreach (GameBilletJuego juego in juegos.Juegos)
						{
							if (string.IsNullOrEmpty(juego.PrecioBase) == false && string.IsNullOrEmpty(juego.PrecioRebajado) == false)
							{
								string sPrecioBase = juego.PrecioBase;
								sPrecioBase = sPrecioBase.Replace("€", null);
								sPrecioBase = sPrecioBase.Replace("$", null);

								string sPrecioRebajado = juego.PrecioRebajado;
								sPrecioRebajado = sPrecioRebajado.Replace("€", null);
								sPrecioRebajado = sPrecioRebajado.Replace("$", null);

								decimal precioBase = decimal.Parse(sPrecioBase);
								decimal precioRebajado = decimal.Parse(sPrecioRebajado);

								int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

								if (descuento > 0)
								{
									JuegoDRM drm1 = JuegoDRM.NoEspecificado;

									if (string.IsNullOrEmpty(juego.DRM1) == false)
									{
										drm1 = JuegoDRM2.Traducir(juego.DRM1, Generar().Id);
									}

									JuegoDRM drm2 = JuegoDRM.NoEspecificado;

									if (string.IsNullOrEmpty(juego.DRM2) == false)
									{
										drm2 = JuegoDRM2.Traducir(juego.DRM2, Generar().Id);
									}

									JuegoDRM drm3 = JuegoDRM.NoEspecificado;

									if (string.IsNullOrEmpty(juego.DRM3) == false)
									{
										drm3 = JuegoDRM2.Traducir(juego.DRM3, Generar().Id);
									}

									if (drm1 != JuegoDRM.NoEspecificado || drm2 != JuegoDRM.NoEspecificado || drm3 != JuegoDRM.NoEspecificado)
									{
										JuegoDRM drmFinal = drm1;

										if (drm2 != JuegoDRM.NoEspecificado)
										{
											drmFinal = drm2;
										}

										if (drm3 != JuegoDRM.NoEspecificado)
										{
											drmFinal = drm3;
										}

										JuegoPrecio oferta = new JuegoPrecio
										{
											Nombre = WebUtility.HtmlDecode(juego.Nombre),
											Enlace = juego.Enlace,
											Imagen = "nada",
											Moneda = JuegoMoneda.Euro,
											Precio = precioRebajado,
											Descuento = descuento,
											Tienda = Generar().Id,
											DRM = drmFinal,
											FechaDetectado = DateTime.Now,
											FechaActualizacion = DateTime.Now
										};

										if (region == TiendaRegion.EstadosUnidos)
										{
											oferta.Moneda = JuegoMoneda.Dolar;
										}

										if (juego.TieneCodigoDescuento == true)
										{
											oferta.CodigoDescuento = (int)juego.CodigoDescuentoPorcentaje;
											oferta.CodigoTexto = juego.CodigoDescuento;
										}

										ofertas.Add(oferta);
									}
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
	}

	#region Clases

	public class GameBilletProductos
	{
		[JsonPropertyName("product")]
		public List<GameBilletJuego> Juegos { get; set; }
	}

	public class GameBilletJuego
	{
		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("url")]
		public string Enlace { get; set; }

		[JsonPropertyName("price")]
		public string PrecioBase { get; set; }

		[JsonPropertyName("special_price")]
		public string PrecioRebajado { get; set; }

		[JsonPropertyName("sku")]
		public string Id { get; set; }

		[JsonPropertyName("filters1")]
		public string DRM1 { get; set; }

		[JsonPropertyName("filters2")]
		public string DRM2 { get; set; }

		[JsonPropertyName("filters3")]
		public string DRM3 { get; set; }

		[JsonPropertyName("hasCoupon")]
		public bool TieneCodigoDescuento { get; set; }

		[JsonPropertyName("discountName")]
		public string CodigoDescuento { get; set; }

		[JsonPropertyName("discountPercentage")]
		public double CodigoDescuentoPorcentaje { get; set; }
	}

	#endregion
}
