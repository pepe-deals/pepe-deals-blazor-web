#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using System.Net.Http.Headers;
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
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa, TiendaRegion.EstadosUnidos }
			};

			return tienda;
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			HttpClient cliente = new HttpClient();
			cliente.BaseAddress = new Uri("https://www.nexus.gg/");
			cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			cliente.DefaultRequestHeaders.Add("x-algolia-api-key", "3bb19265d0e5978fa8b4af145f8209a5");
			cliente.DefaultRequestHeaders.Add("x-algolia-application-id", "KZU3CQXW7F");

			HttpRequestMessage peticion = new HttpRequestMessage(HttpMethod.Post, "https://kzu3cqxw7f-dsn.algolia.net/1/indexes/prod_test_release_date/query?x-algolia-agent=Algolia for JavaScript (4.13.1); Browser");
			peticion.Content = new StringContent(@"{""query"":"""",""filters"":""skuType:\""game\"" AND discounted:true AND discountedPrice:0 TO 97 AND (requiresPermission:false)"",""facets"":[""developer"",""publisher"",""type"",""platform"",""operatingSystem"",""tags"",""discounted"",""discountedPrice"",""preorder"",""comingSoon"",""id"",""categoryName"",""skuType"",""requiresPermission""],""hitsPerPage"":40,""page"":0,""maxValuesPerFacet"":1000}");

			HttpResponseMessage respuesta = await cliente.SendAsync(peticion);

			string html = string.Empty;

			try
			{
				html = await respuesta.Content.ReadAsStringAsync();
			}
			catch { }

			if (string.IsNullOrEmpty(html) == false)
			{
				NexusJuegos juegos = JsonSerializer.Deserialize<NexusJuegos>(html);

				if (juegos?.Resultados?.Count > 0)
				{
					List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

					foreach (var juego in juegos.Resultados)
					{
						decimal precioRebajado = juego.PrecioRebajado;
						decimal precioBase = juego.PrecioBase;

						int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

						if (descuento > 0)
						{
							bool drmValido = false;

							if (juego.DRMs.Count > 0)
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

	public class NexusJuegos
	{
		[JsonPropertyName("hits")]
		public List<NexusJuego> Resultados { get; set; }
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
	}
}
