#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.Nexus
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "nexus",
				Nombre = "Nexus",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/nexus_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/nexus_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/nexus_icono.webp",
				Color = "#3BB9AC",
				AdminUso = false,
				UsuarioUso = false,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			HttpClient cliente = new HttpClient();
			cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

			int pagina = 0;
			int tamañoPagina = 24;
			bool continuar = true;

			while (continuar == true)
			{
				string contenido = JsonSerializer.Serialize(new
				{
					query = "",
					skuType = "game",
					filters = new { skuIdSets = new int[0] },
					sort = "_discount",
					page = pagina,
					pageSize = tamañoPagina
				});

				HttpRequestMessage peticion = new HttpRequestMessage(HttpMethod.Post, "https://api.nexus.gg/v1/store/search/library");
				peticion.Version = HttpVersion.Version20;
				peticion.VersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

				peticion.Content = new StringContent(contenido, Encoding.UTF8);
				peticion.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

				peticion.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:157.0) Gecko/20100101 Firefox/157.0");
				peticion.Headers.TryAddWithoutValidation("Accept", "*/*");
				peticion.Headers.TryAddWithoutValidation("Accept-Language", "es-ES,es;q=0.9,en-US;q=0.8,en;q=0.7");
				peticion.Headers.TryAddWithoutValidation("Referer", "https://www.nexus.gg/");
				peticion.Headers.TryAddWithoutValidation("Origin", "https://www.nexus.gg");
				peticion.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
				peticion.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
				peticion.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "same-site");
				peticion.Headers.TryAddWithoutValidation("Priority", "u=0");

				string html = string.Empty;

				try
				{
					HttpResponseMessage respuesta = await cliente.SendAsync(peticion);
					html = await respuesta.Content.ReadAsStringAsync();
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
					break;
				}

				if (string.IsNullOrEmpty(html) == true)
				{
					break;
				}

				NexusJuegos juegos = null;

				try
				{
					juegos = JsonSerializer.Deserialize<NexusJuegos>(html);
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
					break;
				}

				if (juegos?.Resultados == null || juegos.Resultados.Count == 0)
				{
					break;
				}

				int ofertasPagina = 0;

				foreach (var juego in juegos.Resultados)
				{
					if (juego.RequierePermiso == true)
					{
						continue;
					}

					decimal precioRebajado = juego.PrecioRebajado;
					decimal precioBase = juego.PrecioBase;

					int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

					if (descuento > 0)
					{
						ofertasPagina += 1;

						bool drmValido = false;

						if (juego.DRMs?.Count > 0)
						{
							foreach (var drm in juego.DRMs)
							{
								if (drm.ToLower() == "steam")
								{
									drmValido = true;
									break;
								}
							}
						}

						if (drmValido == true)
						{
							string nombre = juego.Nombre;
							nombre = WebUtility.HtmlDecode(nombre);

							string enlace = "https://www.nexus.gg/pepeizq/" + juego.Slug;

							string imagen = string.Empty;

							JuegoPrecio oferta = new JuegoPrecio
							{
								Nombre = nombre,
								Enlace = enlace,
								Imagen = imagen,
								Moneda = JuegoMoneda.Dolar,
								Precio = precioRebajado,
								Descuento = descuento,
								Tienda = Generar().Id,
								DRM = JuegoDRM.Steam,
								FechaDetectado = DateTime.Now,
								FechaActualizacion = DateTime.Now
							};

							ofertas.Add(oferta);
						}
					}
				}

				if (ofertasPagina == 0)
				{
					break;
				}

				if (juegos.Resultados.Count < tamañoPagina)
				{
					break;
				}

				pagina += 1;
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

	public class NexusJuegos
	{
		[JsonPropertyName("skus")]
		public List<NexusJuego> Resultados { get; set; }

		[JsonPropertyName("totalSkus")]
		public int Total { get; set; }
	}

	public class NexusJuego
	{
		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("slug")]
		public string Slug { get; set; }

		[JsonPropertyName("discountedPrice")]
		public decimal PrecioRebajado { get; set; }

		[JsonPropertyName("normalPrice")]
		public decimal PrecioBase { get; set; }

		[JsonPropertyName("platform")]
		public List<string> DRMs { get; set; }

		[JsonPropertyName("requiresPermission")]
		public bool RequierePermiso { get; set; }
	}
}