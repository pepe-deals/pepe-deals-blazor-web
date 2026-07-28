#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using Tiendas2;

namespace APIs.Gamesporium
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "gamesporium",
				Nombre = "Gamesporium",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/gamesporium_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/gamesporium_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/gamesporium_icono.webp",
				Color = "#141414",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			if (enlace.Contains("?") == true)
			{
				return enlace + "&utm_source=affiliate&utm_medium=pepedeals";
			}
			else
			{
				return enlace + "?utm_source=affiliate&utm_medium=pepedeals";
			}
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			string enlace = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				enlace = "https://feed.mulwi.com/f/b74893-2/feed.xml";
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				enlace = "https://feed.mulwi.com/f/b74893-2/general_us.xml";
			}

			if (string.IsNullOrEmpty(enlace) == true)
			{
				return;
			}

			string html = await Decompiladores.Estandar(enlace);

			if (string.IsNullOrEmpty(html) == false)
			{
				XmlReaderSettings opciones = new XmlReaderSettings
				{
					ConformanceLevel = ConformanceLevel.Document,
					IgnoreWhitespace = true,
					IgnoreComments = true,
					DtdProcessing = DtdProcessing.Ignore,
					XmlResolver = null
				};

				GamesporiumJuegos listaJuegos = new GamesporiumJuegos();
				listaJuegos.Juegos = new List<GamesporiumJuego>();

				using (var lector = XmlReader.Create(new StringReader(html), opciones))
				{
					while (lector.Read() == true)
					{
						if (lector.NodeType == XmlNodeType.Element && lector.Name == "product")
						{
							GamesporiumJuego juego = new GamesporiumJuego();

							using (XmlReader juegoLector = lector.ReadSubtree())
							{
								while (juegoLector.Read())
								{
									if (juegoLector.NodeType == XmlNodeType.Element)
									{
										string LeerValorElemento(XmlReader lector)
										{
											if (lector.IsEmptyElement == true)
											{
												return string.Empty;
											}

											while (lector.Read() && lector.NodeType != XmlNodeType.Text && lector.NodeType != XmlNodeType.CDATA)
											{
											}

											return (lector.NodeType == XmlNodeType.Text || lector.NodeType == XmlNodeType.CDATA)
												? lector.Value?.Trim() ?? string.Empty
												: string.Empty;
										}

										switch (juegoLector.Name)
										{
											case "ProductName":
												juego.Nombre = LeerValorElemento(juegoLector);
												break;

											case "ProductURL":
												juego.Enlace = LeerValorElemento(juegoLector);
												break;

											case "ImageURL":
												juego.Imagen = LeerValorElemento(juegoLector);
												break;

											case "Currency":
												juego.Moneda = LeerValorElemento(juegoLector);
												break;

											case "CompareAtPrice":
												juego.PrecioRebajado = LeerValorElemento(juegoLector);
												break;

											case "Price":
												juego.PrecioBase = LeerValorElemento(juegoLector);
												break;

											case "DRM":
												juego.DRM = LeerValorElemento(juegoLector);
												break;

											case "WhitelistCountries":
												juego.PaisesAprobados = LeerValorElemento(juegoLector);
												break;

											case "StockStatus":
												juego.StockEstado = LeerValorElemento(juegoLector);
												break;
										}
									}
								}
							}

							listaJuegos.Juegos.Add(juego);
						}
					}
				}

				if (listaJuegos?.Juegos?.Count > 0)
				{
					List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

					foreach (GamesporiumJuego juego in listaJuegos.Juegos)
					{
						bool buscar = true;

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
									if (region == TiendaRegion.Europa && pais.ToLower().Trim() == "es")
									{
										encontrado = true;
										break;
									}
									else if (region == TiendaRegion.EstadosUnidos && pais.ToLower().Trim() == "us")
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

						if (region == TiendaRegion.Europa && juego.Moneda?.ToLower() != "eur")
						{
							buscar = false;
						}
						else if (region == TiendaRegion.EstadosUnidos && juego.Moneda?.ToLower() != "usd")
						{
							buscar = false;
						}

						if (juego.StockEstado?.ToLower() == "false")
						{
							buscar = false;
						}

						if (string.IsNullOrEmpty(juego.PrecioRebajado) == true)
						{
							buscar = false;
						}

						if (string.IsNullOrEmpty(juego.PrecioBase) == true)
						{
							buscar = false;
						}

						if (buscar == true)
						{
							decimal precioBase = decimal.Parse(juego.PrecioBase);
							decimal precioRebajado = decimal.Parse(juego.PrecioRebajado);

							int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

							if (descuento > 0)
							{
								string nombre = WebUtility.HtmlDecode(juego.Nombre);

								string enlaceJuego = juego.Enlace;

								string imagen = juego.Imagen;

								bool esInt = int.TryParse(juego.DRM, out _);

								if (esInt == false)
								{
									JuegoDRM drm = JuegoDRM2.Traducir(juego.DRM, Generar().Id);

									JuegoPrecio oferta = new JuegoPrecio
									{
										Nombre = nombre,
										Enlace = enlaceJuego,
										Imagen = imagen,
										Precio = precioRebajado,
										Descuento = descuento,
										Tienda = Generar().Id,
										DRM = drm,
										FechaDetectado = DateTime.Now,
										FechaActualizacion = DateTime.Now
									};

									if (region == TiendaRegion.Europa)
									{
										oferta.Moneda = JuegoMoneda.Euro;
									}
									else if (region == TiendaRegion.EstadosUnidos)
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

	[XmlRoot("rss")]
	public class GamesporiumJuegos
	{
		[XmlArray("products")]
		[XmlArrayItem("product")]
		public List<GamesporiumJuego> Juegos { get; set; }
	}

	public class GamesporiumJuego
	{
		[XmlElement("ProductName")]
		public string Nombre { get; set; }

		[XmlElement("ProductURL")]
		public string Enlace { get; set; }

		[XmlElement("ImageURL")]
		public string Imagen { get; set; }

		[XmlElement("Currency")]
		public string Moneda { get; set; }

		[XmlElement("CompareAtPrice")]
		public string PrecioRebajado { get; set; }

		[XmlElement("Price")]
		public string PrecioBase { get; set; }

		[XmlElement("DRM")]
		public string DRM { get; set; }

		[XmlElement("WhitelistCountries")]
		public string PaisesAprobados { get; set; }

		[XmlElement("StockStatus")]
		public string StockEstado { get; set; }
	}
}
