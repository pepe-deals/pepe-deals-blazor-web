#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Xml.Serialization;
using Tiendas2;

namespace APIs.JoyBuggy
{
    public static class Tienda
    {
        public static Tiendas2.Tienda Generar()
        {
            Tiendas2.Tienda tienda = new Tiendas2.Tienda
            {
                Id = "joybuggy",
                Nombre = "JoyBuggy",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/joybuggy_logo.webp",
                Imagen300x80 = "/imagenes/tiendas/joybuggy_300x80.webp",
                ImagenIcono = "/imagenes/tiendas/joybuggy_icono.ico",
                Color = "#39f2d3",
                AdminEnseñar = true,
                AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

            return tienda;
        }

        public static string Referido(string enlace)
        {
            return enlace + "?ref=253";
        }

        public static async Task BuscarOfertas(TiendaRegion region)
        {
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			string enlace = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				enlace = "https://www.joybuggy.com/modules/xmlfeeds/api/xml.php?id=70";
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				enlace = "https://www.joybuggy.com/modules/xmlfeeds/api/xml.php?id=59";
			}

            string html = await Decompiladores.Estandar(enlace);

            if (string.IsNullOrEmpty(html) == false)
            {
                html = html.Replace("g:title", "title");
                html = html.Replace("g:link", "link");
                html = html.Replace("g:image_link", "image_link");
                html = html.Replace("g:id", "id");
                html = html.Replace("g:sale_price", "sale_price");
                html = html.Replace("g:price", "price");
				html = html.Replace("g:availability", "availability");
				html = html.Replace("g:delivery_time", "delivery_time");

				XmlSerializer xml = new XmlSerializer(typeof(JoyBuggyCanal));
                JoyBuggyCanal listaJuegos = null;

                using (TextReader lector = new StringReader(html))
                {
                    listaJuegos = (JoyBuggyCanal)xml.Deserialize(lector);
                }

				if (listaJuegos?.Datos?.Juegos?.Count > 0)
				{
					List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

					foreach (JoyBuggyJuego juego in listaJuegos.Datos.Juegos)
					{
						if (string.IsNullOrEmpty(juego.Disponibilidad) == false && juego.Disponibilidad == "in stock")
						{
							if (string.IsNullOrEmpty(juego.PrecioBase) == false && string.IsNullOrEmpty(juego.PrecioRebajado) == false)
							{
								string tempPrecioBase = juego.PrecioBase;
								tempPrecioBase = tempPrecioBase.Replace("EUR", null);
								tempPrecioBase = tempPrecioBase.Replace("USD", null);
								tempPrecioBase = tempPrecioBase.Trim();

								decimal precioBase = decimal.Parse(tempPrecioBase);

								string tempPrecioRebajado = juego.PrecioRebajado;
								tempPrecioRebajado = tempPrecioRebajado.Replace("EUR", null);
								tempPrecioRebajado = tempPrecioRebajado.Replace("USD", null);
								tempPrecioRebajado = tempPrecioRebajado.Trim();

								decimal precioRebajado = decimal.Parse(tempPrecioRebajado);

								int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

								if (descuento > 0)
								{
									string nombre = WebUtility.HtmlDecode(juego.Nombre);

									string enlaceJuego = juego.Enlace;

									if (enlaceJuego.Contains("?") == true)
									{
										int int1 = enlaceJuego.IndexOf("?");
										enlaceJuego = enlaceJuego.Remove(int1, enlaceJuego.Length - int1);
									}

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
    public class JoyBuggyCanal
    {
        [XmlElement("channel")]
        public JoyBuggyJuegos Datos { get; set; }
    }

    public class JoyBuggyJuegos
    {
        [XmlElement("item")]
        public List<JoyBuggyJuego> Juegos { get; set; }

        [XmlElement("created_at")]
        public string Creado { get; set; }
    }

    public class JoyBuggyJuego
    {
        [XmlElement("title")]
        public string Nombre { get; set; }

        [XmlElement("link")]
        public string Enlace { get; set; }

        [XmlElement("Currency_saleprice")]
        public string PrecioRebajado { get; set; }

        [XmlElement("Currency_nodiscount")]
        public string PrecioBase { get; set; }

        [XmlElement("id")]
        public string ID { get; set; }

        [XmlElement("image_link")]
        public string Imagen { get; set; }

        [XmlElement("Platform")]
        public string DRM { get; set; }

		[XmlElement("delivery_time")]
		public string Disponibilidad { get; set; }
	}
}
