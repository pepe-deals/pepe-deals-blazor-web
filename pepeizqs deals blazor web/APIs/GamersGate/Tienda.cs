#nullable disable

using Herramientas;
using Juegos;
using System.Globalization;
using System.Net;
using System.Xml;
using Tiendas2;

namespace APIs.GamersGate
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "gamersgate",
				Nombre = "GamersGate",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/gamersgate_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/gamersgate_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/gamersgate_icono.ico",
				Color = "#232A3E",
				AdminUso = true,
				UsuarioUso = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			return enlace + "?aff=6704538";
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			string enlace = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				enlace = "https://www.gamersgate.com/feeds/products?country=DEU";
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				enlace = "https://www.gamersgate.com/feeds/products?country=USA";
			}

			if (string.IsNullOrEmpty(enlace) == true)
			{
				return;
			}

			string html = await Decompiladores.Estandar(enlace);

			if (string.IsNullOrEmpty(html) == false)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				using (var reader = XmlReader.Create(new StringReader(html)))
				{
					while (reader.Read() == true)
					{
						if (reader.NodeType == XmlNodeType.Element && reader.Name == "item")
						{
							string nombre = null;
							string enlaceJuego = null;
							string precioRebajadoTexto = null;
							string precioBaseTexto = null;
							string imagen = null;
							string drm = null;
							string fecha = null;

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
											enlaceJuego = reader.ReadElementContentAsString();
											break;
										case "price":
											precioRebajadoTexto = reader.ReadElementContentAsString();
											break;
										case "srp":
											precioBaseTexto = reader.ReadElementContentAsString();
											break;
										case "boximg":
											imagen = reader.ReadElementContentAsString();
											break;
										case "drm":
											drm = reader.ReadElementContentAsString();
											break;
										case "discount_end":
											fecha = reader.ReadElementContentAsString();
											break;
										default:
											reader.Skip();
											break;
									}
								}
							}

							if (string.IsNullOrEmpty(precioBaseTexto) == false && string.IsNullOrEmpty(precioRebajadoTexto) == false &&
								decimal.TryParse(precioBaseTexto, out decimal precioBase) == true &&
								decimal.TryParse(precioRebajadoTexto, out decimal precioRebajado) == true)
							{
								int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

								if (descuento > 0)
								{
									nombre = WebUtility.HtmlDecode(nombre);

									if (string.IsNullOrEmpty(enlaceJuego) == false)
									{
										enlaceJuego = enlaceJuego.Replace("/en-us/", "/");
										enlaceJuego = enlaceJuego.Replace("/es/", "/");
									}

									JuegoDRM juegoDRM = JuegoDRM2.Traducir(drm, Generar().Id);

									JuegoPrecio oferta = new JuegoPrecio
									{
										Nombre = nombre,
										Enlace = enlaceJuego,
										Imagen = imagen,
										Moneda = region == TiendaRegion.EstadosUnidos ? JuegoMoneda.Dolar : JuegoMoneda.Euro,
										Precio = precioRebajado,
										Descuento = descuento,
										Tienda = Generar().Id,
										DRM = juegoDRM,
										FechaDetectado = DateTime.Now,
										FechaActualizacion = DateTime.Now
									};

									if (string.IsNullOrEmpty(fecha) == false)
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
