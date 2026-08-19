#nullable disable

using Microsoft.AspNetCore.Mvc;
using Noticias;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Security.Claims;
using Tiendas2;
using Juegos;

namespace Herramientas
{
    public class Rss : Controller
    {
        #region Noticias

        [ResponseCache(Duration = 300)]
        [HttpGet("rss-en.xml")]
        public async Task<IActionResult> GenerarEnRSS()
        {
            string dominio = "https://" + HttpContext.Request.Host.Value;

            SyndicationFeed feed = new SyndicationFeed("pepe's deals", "RSS in English from the web", new Uri(dominio), "RSSUrl", DateTime.Now)
            {
                Copyright = new TextSyndicationContent($"{DateTime.Now.Year}")
            };

            List<SyndicationItem> items = new List<SyndicationItem>();
            List<Noticia> noticias = await global::BaseDatos.Noticias.Buscar.Actuales();

            if (noticias?.Count > 0)
            {
                noticias = noticias.OrderBy(x => x.FechaEmpieza).Reverse().ToList();

                foreach (Noticia noticia in noticias)
                {
                    string enlace = dominio + "/news/" + noticia.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(noticia.TituloEn) + "/";

                    string titulo = noticia.TituloEn;
                    string contenido = noticia.ContenidoEn;
                    Uri enlaceUri = null;

                    if (enlace != null)
                    {
                        enlaceUri = new Uri(enlace);
                    }

                    SyndicationItem item = new SyndicationItem(titulo, contenido, enlaceUri, noticia.Id.ToString(), noticia.FechaEmpieza);

                    if (string.IsNullOrEmpty(noticia.Imagen) == false)
                    {
                        item.ElementExtensions.Add(new XElement("image", noticia.Imagen));
                    }

                    items.Add(item);
                }

                feed.Items = items;

                XmlWriterSettings opciones = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    NewLineHandling = NewLineHandling.Entitize,
                    NewLineOnAttributes = true,
                    Indent = true
                };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (XmlWriter xmlEscritor = XmlWriter.Create(stream, opciones))
                    {
                        Rss20FeedFormatter rssFormateador = new Rss20FeedFormatter(feed, false);
                        rssFormateador.WriteTo(xmlEscritor);
                        xmlEscritor.Flush();
                    }

                    return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
                }
            }

            return null;
        }

        [ResponseCache(Duration = 300)]
        [HttpGet("rss-es.xml")]
        public async Task<IActionResult> GenerarEsRSS()
        {
            string dominio = "https://" + HttpContext.Request.Host.Value;

            SyndicationFeed feed = new SyndicationFeed("pepe's deals", "RSS en Español de la web", new Uri(dominio), "RSSUrl", DateTime.Now)
            {
                Copyright = new TextSyndicationContent($"{DateTime.Now.Year}")
            };

            List<SyndicationItem> items = new List<SyndicationItem>();
            List<Noticia> noticias = await global::BaseDatos.Noticias.Buscar.Actuales();

            if (noticias?.Count > 0)
            {
                noticias = noticias.OrderBy(x => x.FechaEmpieza).Reverse().ToList();

                foreach (Noticia noticia in noticias)
                {
                    string enlace = dominio + "/news/" + noticia.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(noticia.TituloEs) + "/";

                    string titulo = noticia.TituloEs;
                    string contenido = noticia.ContenidoEs;
                    Uri enlaceUri = null;

                    if (enlace != null)
                    {
                        enlaceUri = new Uri(enlace);
                    }

                    SyndicationItem item = new SyndicationItem(titulo, contenido, enlaceUri, noticia.Id.ToString(), noticia.FechaEmpieza);

                    if (string.IsNullOrEmpty(noticia.Imagen) == false)
                    {
                        item.ElementExtensions.Add(new XElement("image", noticia.Imagen));
                    }

                    items.Add(item);
                }

                feed.Items = items;

                XmlWriterSettings opciones = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    NewLineHandling = NewLineHandling.Entitize,
                    NewLineOnAttributes = true,
                    Indent = true
                };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (XmlWriter xmlEscritor = XmlWriter.Create(stream, opciones))
                    {
                        Rss20FeedFormatter rssFormateador = new Rss20FeedFormatter(feed, false);
                        rssFormateador.WriteTo(xmlEscritor);
                        xmlEscritor.Flush();
                    }

                    return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
                }
            }

            return null;
        }

		[ResponseCache(Duration = 300)]
		[HttpGet("rss2/es/{categorias2}.xml")]
		public async Task<IActionResult> GenerarEsRSS2(string categorias2)
		{
			string dominio = "https://" + HttpContext.Request.Host.Value;

			SyndicationFeed feed = new SyndicationFeed("pepe's deals", "RSS en Español de la web", new Uri(dominio), "RSSUrl", DateTime.Now)
			{
				Copyright = new TextSyndicationContent($"{DateTime.Now.Year}")
			};

			List<SyndicationItem> items = new List<SyndicationItem>();

            List<NoticiaTipo> tipos = new List<NoticiaTipo>();

			if (string.IsNullOrEmpty(categorias2) == false)
			{
				string[] categorias = categorias2.Split(',');
				foreach (string categoria in categorias)
				{
					if (Enum.TryParse(categoria, true, out NoticiaTipo tipo) == true)
					{
						tipos.Add(tipo);
					}
				}
			}

			List<Noticia> noticias = await global::BaseDatos.Noticias.Buscar.Actuales(tipos, 5);

			if (noticias?.Count > 0)
			{
				noticias = noticias.OrderBy(x => x.FechaEmpieza).Reverse().ToList();

				foreach (Noticia noticia in noticias)
				{
					string enlace = dominio + "/news/" + noticia.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(noticia.TituloEs) + "/";

					string titulo = noticia.TituloEs;
					string contenido = noticia.ContenidoEs;
					Uri enlaceUri = null;

					if (enlace != null)
					{
						enlaceUri = new Uri(enlace);
					}

					SyndicationItem item = new SyndicationItem(titulo, contenido, enlaceUri, noticia.Id.ToString(), noticia.FechaEmpieza);

					if (string.IsNullOrEmpty(noticia.Imagen) == false)
					{
						item.ElementExtensions.Add(new XElement("image", noticia.Imagen));
					}

					items.Add(item);
				}

				feed.Items = items;

				XmlWriterSettings opciones = new XmlWriterSettings
				{
					Encoding = Encoding.UTF8,
					NewLineHandling = NewLineHandling.Entitize,
					NewLineOnAttributes = true,
					Indent = true
				};

				using (MemoryStream stream = new MemoryStream())
				{
					using (XmlWriter xmlEscritor = XmlWriter.Create(stream, opciones))
					{
						Rss20FeedFormatter rssFormateador = new Rss20FeedFormatter(feed, false);
						rssFormateador.WriteTo(xmlEscritor);
						xmlEscritor.Flush();
					}

					return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
				}
			}

			return null;
		}

		[ResponseCache(Duration = 300)]
        [HttpGet("news-rss")]
        public async Task<IActionResult> CogerNoticiasRSS()
        {
            string dominio = "https://" + HttpContext.Request.Host.Value;

            if (User.Identity.IsAuthenticated == true)
            {
                if (await global::BaseDatos.Usuarios.Buscar.RolDios(User.FindFirst(ClaimTypes.NameIdentifier).Value) == true)
                {
                    List<Noticia> noticias = await global::BaseDatos.Noticias.Buscar.Ultimas(10);

                    if (noticias.Count > 0)
                    {
                        foreach (var noticia in noticias)
                        {
                            if (noticia.Imagen.Contains("https://") == false)
                            {
                                noticia.Imagen = dominio + noticia.Imagen;
                            }
                        }

                        return Ok(noticias);
                    }
                }
            }

            return Redirect("~/");
        }

        #endregion

        [ResponseCache(Duration = 300)]
        [HttpGet("rss/{region2}/{drm}/{cantidadReseñas}")]
        public async Task<IActionResult> GenerarUltimasOfertas(string region2, string drm, int cantidadReseñas)
        {
            TiendaRegion region = TiendaRegion.Europa;

            if (region2 == "us")
            {
                region = TiendaRegion.EstadosUnidos;
            }

            string dominio = "https://" + HttpContext.Request.Host.Value;

            List<string> drmsUsar = new List<string>();

            foreach (var drm2 in Juegos.JuegoDRM2.GenerarListado())
            {
                foreach (var acepcion in drm2.Acepciones)
                {
                    if (drm.ToLower().Contains(acepcion) == true)
                    {
                        int posicion = Array.IndexOf(Enum.GetValues(typeof(Juegos.JuegoDRM)), drm2.Id);

                        drmsUsar.Add(posicion.ToString());
                    }
                }
            }

            if (drmsUsar.Count > 0)
            {
                if (cantidadReseñas < 200)
                {
                    cantidadReseñas = 199;
                }

                if (cantidadReseñas > 10000)
                {
                    cantidadReseñas = 9999;
                }

                SyndicationFeed feed = new SyndicationFeed("pepe's deals", "RSS Last Deals", new Uri(dominio), "RSSUrl", DateTime.Now)
                {
                    Copyright = new TextSyndicationContent($"{DateTime.Now.Year}")
                };

                List<SyndicationItem> items = new List<SyndicationItem>();

                List<Juegos.Juego> juegos = await global::BaseDatos.Portada.Buscar.Minimos(false, region, 3, 50, null, drmsUsar, cantidadReseñas);

                if (juegos.Count > 0)
                {
                    foreach (Juegos.Juego juego in juegos)
                    {
                        string enlace = dominio + "/game/" + juego.Id.ToString() + "/" + Herramientas.EnlaceAdaptador.Nombre(juego.Nombre) + "/";

                        string titulo = juego.Nombre;
                        string contenido = string.Empty;

                        if (region == TiendaRegion.Europa)
                        {
                            contenido = juego.PrecioMinimosHistoricos[0].Descuento.ToString() + "% - " + Herramientas.Precios.Euro(juego.PrecioMinimosHistoricos[0].Precio);
                        }
                        else if (region == TiendaRegion.EstadosUnidos)
                        {
                            contenido = juego.PrecioMinimosHistoricosUS[0].Descuento.ToString() + "% - " + Herramientas.Precios.Dolar(juego.PrecioMinimosHistoricosUS[0].Precio);
                        }

                        Uri enlaceUri = null;

                        if (enlace != null)
                        {
                            enlaceUri = new Uri(enlace);
                        }

                        SyndicationItem item = new SyndicationItem(titulo, contenido, enlaceUri);

                        if (string.IsNullOrEmpty(juego.Imagenes.Header_460x215) == false)
                        {
                            item.ElementExtensions.Add(new XElement("image", juego.Imagenes.Header_460x215));
                        }

                        items.Add(item);
                    }

                    feed.Items = items;

                    XmlWriterSettings opciones = new XmlWriterSettings
                    {
                        Encoding = Encoding.UTF8,
                        NewLineHandling = NewLineHandling.Entitize,
                        NewLineOnAttributes = true,
                        Indent = true
                    };

                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (XmlWriter xmlEscritor = XmlWriter.Create(stream, opciones))
                        {
                            Rss20FeedFormatter rssFormateador = new Rss20FeedFormatter(feed, false);
                            rssFormateador.WriteTo(xmlEscritor);
                            xmlEscritor.Flush();
                        }

                        return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
                    }
                }
            }

            return null;
        }

        [ResponseCache(Duration = 300)]
        [HttpGet("rss/{region2}/{categoria2}/{usuarioId}.xml")]
        public async Task<IActionResult> GenerarDeseados(string region2, string categoria2, string usuarioId)
        {
            TiendaRegion region = TiendaRegion.Europa;

            if (region2 == "us")
            {
                region = TiendaRegion.EstadosUnidos;
            }

            string dominio = "https://" + HttpContext.Request.Host.Value;

			SyndicationFeed feed = new SyndicationFeed("pepe's deals", "RSS Wishlisted Games", new Uri(dominio), "RSSUrl", DateTime.Now)
			{
				Copyright = new TextSyndicationContent($"{DateTime.Now.Year}")
			};

			List<SyndicationItem> items = new List<SyndicationItem>();

			List<JuegoDeseadoMostrar> juegos = await Deseados.LeerJuegos(false, region, usuarioId);

			if (juegos?.Count > 0)
			{
                juegos = juegos?.OrderByDescending(x => x.Precio.FechaDetectado).ThenBy(x => x.Nombre).ToList();

				foreach (JuegoDeseadoMostrar juego in juegos)
				{
                    bool mostrar = false;

                    if (categoria2 == "1" && juego.Historico == true)
					{
						mostrar = true;
					}
					else if (categoria2 == "0")
					{
						mostrar = true;
					}

					if (mostrar == true)
                    {
						string enlace = dominio + "/game/" + juego.Id.ToString() + "/" + EnlaceAdaptador.Nombre(juego.Nombre) + "/";

						string titulo = juego.Nombre;
						string contenido = string.Empty;

						decimal precioMostrar = juego.Precio.Precio;

						if (juego.Precio.PrecioCambiado > 0)
						{
							precioMostrar = juego.Precio.PrecioCambiado;
						}

						if (region == TiendaRegion.Europa)
						{
							contenido = juego.Precio.Descuento.ToString() + "% - " + Precios.Euro(precioMostrar);
						}
						else if (region == TiendaRegion.EstadosUnidos)
						{
							contenido = juego.Precio.Descuento.ToString() + "% - " + Precios.Dolar(precioMostrar);
						}

						Uri enlaceUri = null;

						if (enlace != null)
						{
							enlaceUri = new Uri(enlace);
						}

						SyndicationItem item = new SyndicationItem(titulo, contenido, enlaceUri);

						if (string.IsNullOrEmpty(juego.Imagen) == false)
						{
							item.ElementExtensions.Add(new XElement("image", juego.Imagen));
						}

						items.Add(item);
					}
				}

				feed.Items = items;

				XmlWriterSettings opciones = new XmlWriterSettings
				{
					Encoding = Encoding.UTF8,
					NewLineHandling = NewLineHandling.Entitize,
					NewLineOnAttributes = true,
					Indent = true
				};

				using (MemoryStream stream = new MemoryStream())
				{
					using (XmlWriter xmlEscritor = XmlWriter.Create(stream, opciones))
					{
						Rss20FeedFormatter rssFormateador = new Rss20FeedFormatter(feed, false);
						rssFormateador.WriteTo(xmlEscritor);
						xmlEscritor.Flush();
					}

					return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
				}
			}

            return null;
		}

		[ResponseCache(Duration = 300)]
		[HttpGet("rss2/{region2}/{noOficial2}/{marketplace2}/{categoria2}/{usuarioId}.xml")]
		public async Task<IActionResult> GenerarDeseados2(string region2, bool noOficial2, bool marketplace2, string categoria2, string usuarioId)
		{
			TiendaRegion region = TiendaRegion.Europa;

			if (region2 == "us")
			{
				region = TiendaRegion.EstadosUnidos;
			}

			string dominio = "https://" + HttpContext.Request.Host.Value;

			SyndicationFeed feed = new SyndicationFeed("pepe's deals", "RSS Wishlisted Games", new Uri(dominio), "RSSUrl", DateTime.Now)
			{
				Copyright = new TextSyndicationContent($"{DateTime.Now.Year}")
			};

			List<SyndicationItem> items = new List<SyndicationItem>();

			List<JuegoDeseadoMostrar> juegos = await Deseados.LeerJuegos(noOficial2, region, usuarioId);

			if (juegos?.Count > 0)
			{
				juegos = juegos?.OrderByDescending(x => x.Precio.FechaDetectado).ThenBy(x => x.Nombre).ToList();

				foreach (JuegoDeseadoMostrar juego in juegos)
				{
					bool mostrar = false;

					if (categoria2 == "1" && juego.Historico == true)
					{
						mostrar = true;
					}
					else if (categoria2 == "0")
					{
						mostrar = true;
					}

					if (mostrar == true)
					{
						string enlace = dominio + "/game/" + juego.Id.ToString() + "/" + EnlaceAdaptador.Nombre(juego.Nombre) + "/";

						string titulo = juego.Nombre;
						string contenido = string.Empty;

						decimal precioMostrar = juego.Precio.Precio;

						if (juego.Precio.PrecioCambiado > 0)
						{
							precioMostrar = juego.Precio.PrecioCambiado;
						}

						if (region == TiendaRegion.Europa)
						{
							contenido = juego.Precio.Descuento.ToString() + "% - " + Precios.Euro(precioMostrar);
						}
						else if (region == TiendaRegion.EstadosUnidos)
						{
							contenido = juego.Precio.Descuento.ToString() + "% - " + Precios.Dolar(precioMostrar);
						}

						Uri enlaceUri = null;

						if (enlace != null)
						{
							enlaceUri = new Uri(enlace);
						}

						SyndicationItem item = new SyndicationItem(titulo, contenido, enlaceUri);

						if (string.IsNullOrEmpty(juego.Imagen) == false)
						{
							item.ElementExtensions.Add(new XElement("image", juego.Imagen));
						}

						items.Add(item);
					}
				}

				feed.Items = items;

				XmlWriterSettings opciones = new XmlWriterSettings
				{
					Encoding = Encoding.UTF8,
					NewLineHandling = NewLineHandling.Entitize,
					NewLineOnAttributes = true,
					Indent = true
				};

				using (MemoryStream stream = new MemoryStream())
				{
					using (XmlWriter xmlEscritor = XmlWriter.Create(stream, opciones))
					{
						Rss20FeedFormatter rssFormateador = new Rss20FeedFormatter(feed, false);
						rssFormateador.WriteTo(xmlEscritor);
						xmlEscritor.Flush();
					}

					return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
				}
			}

			return null;
		}
	}
}
