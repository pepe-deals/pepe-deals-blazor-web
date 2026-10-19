#nullable disable

using Herramientas;
using Juegos;
using System.Xml;
using System.Xml.Linq;
using Tiendas2;

namespace APIs.Eneba
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "eneba",
				Nombre = "Eneba",
				Tipo = TiendaTipo.Marketplace,
				ImagenLogo = "/imagenes/tiendas/eneba_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/eneba_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/eneba_icono.ico",
				Color = "#558205",
				AdminUso = true,
				UsuarioUso = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa }
			};

			return tienda;
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			string enlace = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				enlace = "https://www.eneba.com/rss/products.xml?currency=EUR&country=ES";
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				enlace = "https://www.eneba.com/rss/products.xml?currency=USD&country=US";
			}

			if (string.IsNullOrEmpty(enlace) == true)
			{
				return;
			}

			List<EnebaOferta> resultados = await LeerFeed(enlace);

			if (resultados?.Count > 0)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				foreach (var resultado in resultados)
				{
					bool regionCorrecta = false;

					if (region == TiendaRegion.Europa && (resultado.Region == "global" || resultado.Region == "europe"))
					{
						regionCorrecta = true;
					}
					else if (region == TiendaRegion.EstadosUnidos && (resultado.Region == "global" || resultado.Region == "united_states"))
					{
						regionCorrecta = true;
					}

					if (regionCorrecta == true)
					{
						JuegoDRM juegoDRM = JuegoDRM2.Traducir(resultado.DRM, Generar().Id);

						if (juegoDRM == JuegoDRM.Steam && resultado.EnStock == "in stock")
						{
							string nombreJuego = resultado.TituloOriginal;
							nombreJuego = nombreJuego.Replace("Bethesda.net Key", null);
							nombreJuego = nombreJuego.Replace("Bethesda.net key", null);
							nombreJuego = nombreJuego.Replace("Bethesda key", null);
							nombreJuego = nombreJuego.Replace("Steam Key GLOBAL", null);
							nombreJuego = nombreJuego.Replace("Steam Key", null);
							nombreJuego = nombreJuego.Replace("Steam key", null);
							nombreJuego = (nombreJuego.EndsWith(" Key")) ? nombreJuego.Substring(0, nombreJuego.Length - 4).Trim() : nombreJuego;
							nombreJuego = (nombreJuego.EndsWith(" key")) ? nombreJuego.Substring(0, nombreJuego.Length - 4).Trim() : nombreJuego;
							nombreJuego = nombreJuego.Replace("(DLC)", null);
							nombreJuego = nombreJuego.Replace("(PC)", null);
							nombreJuego = nombreJuego.Replace("[VR]", null);
							nombreJuego = nombreJuego.Trim();

							string enlaceJuego = resultado.Enlace;
							enlaceJuego = enlaceJuego.Replace("?currency=EUR", null);
							enlaceJuego = enlaceJuego.Replace("?currency=USD", null);

							string precioJuego = resultado.Precio;
							precioJuego = precioJuego.Replace(" EUR", null);
							precioJuego = precioJuego.Replace(" USD", null);

							JuegoPrecio oferta = new JuegoPrecio
							{
								Nombre = nombreJuego,
								Imagen = resultado.Imagen,
								DRM = juegoDRM,
								Enlace = enlaceJuego,
								Descuento = 0,
								Precio = decimal.Parse(precioJuego, System.Globalization.CultureInfo.InvariantCulture),
								Moneda = JuegoMoneda.Euro,
								Tienda = Generar().Id,
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

		private static readonly XNamespace g = "http://base.google.com/ns/1.0";

		private const int ResultadosPorPagina = 5000;
		private const int MaximoPeticiones = 50; 
		private const int PausaEntrePeticionesMs = 1000;

		public static async Task<List<EnebaOferta>> LeerFeed(string enlaceBase)
		{
			List<EnebaOferta> ofertas = new List<EnebaOferta>();

			for (int peticion = 0; peticion < MaximoPeticiones; peticion++)
			{
				int desde = peticion * ResultadosPorPagina;
				string enlace = desde == 0 ? enlaceBase : $"{enlaceBase}&from={desde}";

				(List<EnebaOferta> pagina, int itemsLeidos) = await LeerPagina(enlace);
				ofertas.AddRange(pagina);

				if (itemsLeidos < ResultadosPorPagina)
				{
					break;
				}

				await Task.Delay(PausaEntrePeticionesMs);
			}

			return ofertas;
		}

		private static async Task<(List<EnebaOferta> ofertas, int itemsLeidos)> LeerPagina(string enlace)
		{
			List<EnebaOferta> ofertas = new List<EnebaOferta>();
			int itemsLeidos = 0;

			string html = await Herramientas.Decompiladores.Estandar(enlace);

			XmlNamespaceManager nsManager = new XmlNamespaceManager(new NameTable());
			nsManager.AddNamespace("g", g.NamespaceName);

			using XmlReader lector = XmlReader.Create(new StringReader(html), new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }, new XmlParserContext(null, nsManager, null, XmlSpace.None));

			lector.MoveToContent();

			while (lector.EOF == false)
			{
				if (lector.NodeType == XmlNodeType.Element && lector.LocalName == "item")
				{
					XElement item = (XElement)XNode.ReadFrom(lector);
					itemsLeidos += 1;

					EnebaOferta oferta = ConvertirItem(item);

					if (oferta != null)
					{
						ofertas.Add(oferta);
					}

					continue;
				}

				lector.Read();
			}

			return (ofertas, itemsLeidos);
		}

		private static EnebaOferta ConvertirItem(XElement item)
		{
			string Texto(XName nombre) => item.Element(nombre)?.Value.Trim();

			return new EnebaOferta
			{
				Id = Texto(g + "id"),
				Sku = Texto("sku"),
				Titulo = Texto("title"),
				TituloOriginal = Texto("original_title"),
				Enlace = Texto("link"),
				Imagen = Texto(g + "image_link"),
				EnStock = Texto(g + "availability"),
				Precio = Texto(g + "price"),
				Categoria = Texto(g + "google_product_category"),
				Tipo = Texto(g + "product_type"),
				DRM = Texto(g + "brand"),
				Region = Texto("region"),
				Condicion = Texto(g + "condition")
			};
		}
	}

	public class EnebaOferta
	{
		public string Id { get; set; }
		public string Sku { get; set; }
		public string Titulo { get; set; }
		public string TituloOriginal { get; set; }
		public string Enlace { get; set; }
		public string Imagen { get; set; }
		public string EnStock { get; set; }
		public string Precio { get; set; }
		public string Moneda { get; set; }
		public string Categoria { get; set; }
		public string Tipo { get; set; }
		public string DRM { get; set; }
		public string Region { get; set; }
		public string Condicion { get; set; }
	}
}
