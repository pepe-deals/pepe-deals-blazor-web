//https://www.indiegala.com/games/ajax/on-sale/percentage-off/1
//https://www.indiegala.com/store_games_rss?&sale=true&page=1

#nullable disable

using Herramientas;
using Juegos;
using System.Globalization;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using Tiendas2;

namespace APIs.IndieGala
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "indiegala",
				Nombre = "IndieGala",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/indiegala_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/indiegala_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/indiegala_icono.webp",
				Color = "#ffccd4",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			return enlace + "?ref=pepeizq";
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			int juegos2 = 0;
			int totalPaginas = 10;

			int i = 1;
			while (i <= totalPaginas)
			{
				string html = await Decompiladores.Estandar("https://www.indiegala.com/store_games_rss?&sale=true&page=" + i.ToString());

				if (string.IsNullOrEmpty(html) == false)
				{
					if (html == "None")
					{
						break;
					}

					List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

					using (XmlReader reader = XmlReader.Create(new StringReader(html)))
					{
						while (reader.Read() == true)
						{
							if (i == 1 && reader.NodeType == XmlNodeType.Element && reader.Name == "totalPages")
							{
								string totalPaginasTexto = reader.ReadElementContentAsString();

								if (int.TryParse(totalPaginasTexto, out int paginas))
								{
									totalPaginas = paginas;
								}

								continue;
							}

							if (reader.NodeType == XmlNodeType.Element && reader.Name == "item")
							{
								string nombre = null;
								string enlace = null;
								string precioRebajado = null;
								string precioBase = null;
								string precioRebajadoUS = null;
								string precioBaseUS = null;
								string imagen = null;
								string drm = null;
								string fecha = null;
								string estado = null;
								string paisesStockAprobados = null;
								string paisesAprobados = null;
								string paisesRestringidos = null;

								while (reader.Read() == true)
								{
									if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "item")
									{
										break;
									}

									if (reader.NodeType == XmlNodeType.Element)
									{
										switch (reader.Name)
										{
											case "title":
												nombre = reader.ReadElementContentAsString();
												break;
											case "link":
												enlace = reader.ReadElementContentAsString();
												break;
											case "discountPriceEUR":
												precioRebajado = reader.ReadElementContentAsString();
												break;
											case "priceEUR":
												precioBase = reader.ReadElementContentAsString();
												break;
											case "discountPriceUSD":
												precioRebajadoUS = reader.ReadElementContentAsString();
												break;
											case "priceUSD":
												precioBaseUS = reader.ReadElementContentAsString();
												break;
											case "boximg":
												imagen = reader.ReadElementContentAsString();
												break;
											case "drminfo":
												drm = reader.ReadElementContentAsString();
												break;
											case "discountEnd":
												fecha = reader.ReadElementContentAsString();
												break;
											case "state":
												estado = reader.ReadElementContentAsString();
												break;
											case "regionStockAvailable":
												paisesStockAprobados = reader.ReadElementContentAsString();
												break;
											case "regionAvailable":
												paisesAprobados = reader.ReadElementContentAsString();
												break;
											case "notAvailableRegions":
												paisesRestringidos = reader.ReadElementContentAsString();
												break;
											default:
												reader.Skip();
												break;
										}
									}
								}

								bool buscar = true;

								if (string.IsNullOrEmpty(paisesRestringidos) == false)
								{
									List<string> listaPaisesRestringidos = paisesRestringidos.Split(',').Select(p => p.Trim()).ToList();

									foreach (string pais in listaPaisesRestringidos)
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

								if (buscar == true && string.IsNullOrEmpty(paisesStockAprobados) == false)
								{
									List<string> listaPaisesAprobados = paisesStockAprobados.Split(',').Select(p => p.Trim()).ToList();

									bool encontrado = listaPaisesAprobados.Any(pais =>
										(region == TiendaRegion.Europa && pais.ToLower() == "es") ||
										(region == TiendaRegion.EstadosUnidos && pais.ToLower() == "us")
									);

									if (encontrado == false)
									{
										buscar = false;
									}
								}

								if (buscar == true && string.IsNullOrEmpty(paisesAprobados) == false)
								{
									List<string> listaPaisesAprobados = paisesAprobados.Split(',').Select(p => p.Trim()).ToList();

									bool encontrado = listaPaisesAprobados.Any(pais =>
										(region == TiendaRegion.Europa && pais.ToLower() == "es") ||
										(region == TiendaRegion.EstadosUnidos && pais.ToLower() == "us")
									);

									if (encontrado == false)
									{
										buscar = false;
									}
								}

								if (buscar == true && estado == "available")
								{
									bool puedeAñadir = false;
									decimal precioRebajadoFinal = 0;
									decimal precioBaseFinal = 0;

									if (region == TiendaRegion.Europa &&
										decimal.TryParse(precioBase, out decimal precioBaseEur) == true &&
										decimal.TryParse(precioRebajado, out decimal precioRebajadoEur) == true)
									{
										precioBaseFinal = precioBaseEur;
										precioRebajadoFinal = precioRebajadoEur;
										puedeAñadir = true;
									}

									else if (region == TiendaRegion.EstadosUnidos &&
										decimal.TryParse(precioBaseUS, out decimal precioBaseUsd) == true &&
										decimal.TryParse(precioRebajadoUS, out decimal precioRebajadoUsd) == true)
									{
										precioBaseFinal = precioBaseUsd;
										precioRebajadoFinal = precioRebajadoUsd;
										puedeAñadir = true;
									}

									if (puedeAñadir == true)
									{
										int descuento = Calculadora.SacarDescuento(precioBaseFinal, precioRebajadoFinal);

										if (descuento > 0)
										{
											nombre = WebUtility.HtmlDecode(nombre ?? string.Empty);

											if (imagen.Contains("https://www.indiegalacdn.com/") == false)
											{
												imagen = "https://www.indiegalacdn.com/" + imagen;
											}

											JuegoDRM juegoDRM = JuegoDRM2.Traducir(drm, Generar().Id);

											if (nombre.Contains("(Epic)") == true)
											{
												juegoDRM = JuegoDRM.Epic;
											}
											else if (ComprobarEpicFalsos(enlace) == true)
											{
												juegoDRM = JuegoDRM.Epic;
											}
											else if (enlace.Contains("_epic") == true)
											{
												juegoDRM = JuegoDRM.Epic;
											}

											if (ComprobarElderFalsos(enlace) == true)
											{
												juegoDRM = JuegoDRM.ElderScrolls;
											}

											if (ComprobarRockstarFalsos(enlace) == true)
											{
												juegoDRM = JuegoDRM.Rockstar;
											}

											JuegoPrecio oferta = new JuegoPrecio
											{
												Nombre = nombre,
												Enlace = enlace,
												Imagen = imagen,
												Moneda = region == TiendaRegion.EstadosUnidos ? JuegoMoneda.Dolar : JuegoMoneda.Euro,
												Precio = precioRebajadoFinal,
												Descuento = descuento,
												Tienda = Generar().Id,
												DRM = juegoDRM,
												FechaDetectado = DateTime.Now,
												FechaActualizacion = DateTime.Now
											};

											if (string.IsNullOrEmpty(fecha) == false && fecha != "None")
											{
												if (DateTime.TryParse(fecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaTermina))
												{
													oferta.FechaTermina = fechaTermina;
												}
											}

											ofertas.Add(oferta);
										}
									}
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

				i += 1;
			}
		}

		private static bool ComprobarEpicFalsos(string enlaceJuego)
		{
			List<string> enlaces = [
				"1286680_pass",
				"1286680_pre",
				"1286680_deluxe_pre",
				"1496590",
				"1496590_deluxe",
				"578650",
				"629820",
				"9b06280a",
				"f69bbded"
			];

			foreach (var enlace in enlaces)
			{
				if (enlaceJuego.Contains("/" + enlace) == true)
				{
					return true;
				}
			}

			return false;
		}

		private static bool ComprobarElderFalsos(string enlaceJuego)
		{
			List<string> enlaces = [
				"2662630_elder"
			];

			foreach (var enlace in enlaces)
			{
				if (enlaceJuego.Contains("/" + enlace) == true)
				{
					return true;
				}
			}

			return false;
		}

		private static bool ComprobarRockstarFalsos(string enlaceJuego)
		{
			List<string> enlaces = [
				"1174180",
				"1174180_ultimate"
			];

			foreach (var enlace in enlaces)
			{
				if (enlaceJuego.Contains("/" + enlace) == true)
				{
					return true;
				}
			}

			return false;
		}
	}

	#region Clases

	[XmlRoot("rss")]
	public class IndieGalaRSS
	{
		[XmlElement("channel")]
		public IndieGalaCanal Canal { get; set; }
	}

	public class IndieGalaCanal
	{
		[XmlElement("totalPages")]
		public int TotalPaginas { get; set; }

		[XmlElement("totalGames")]
		public int TotalJuegos { get; set; }

		[XmlElement("browse")]
		public IndieGalaJuegos Buscador { get; set; }
	}

	public class IndieGalaJuegos
	{
		[XmlElement("item")]
		public List<IndieGalaJuego> Juegos { get; set; }
	}

	public class IndieGalaJuego
	{
		[XmlElement("title")]
		public string Nombre { get; set; }

		[XmlElement("link")]
		public string Enlace { get; set; }

		[XmlElement("discountPercentEUR")]
		public string Descuento { get; set; }

		[XmlElement("discountPriceEUR")]
		public string PrecioRebajado { get; set; }

		[XmlElement("priceEUR")]
		public string PrecioBase { get; set; }

		[XmlElement("discountPercentUSD")]
		public string DescuentoUS { get; set; }

		[XmlElement("discountPriceUSD")]
		public string PrecioRebajadoUS { get; set; }

		[XmlElement("priceUSD")]
		public string PrecioBaseUS { get; set; }

		[XmlElement("boximg")]
		public string Imagen { get; set; }

		[XmlElement("drminfo")]
		public string DRM { get; set; }

		[XmlElement("discountEnd")]
		public string Fecha { get; set; }

		[XmlElement("state")]
		public string Estado { get; set; }

		[XmlElement("regionStockAvailable")]
		public string PaisesStockAprobados { get; set; }

		[XmlElement("regionAvailable")]
		public string PaisesAprobados { get; set; }

		[XmlElement("notAvailableRegions")]
		public string PaisesRestringidos { get; set; }
	}

	#endregion
}
