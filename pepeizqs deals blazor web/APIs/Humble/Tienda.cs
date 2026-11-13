//https://www.humblebundle.com/store/api/recommend?recommendation_attempt=1&machine_name=americanfugitive_storefront
//https://www.humblebundle.com/store/api/search?sort=discount&filter=all&search=american&request=1
//https://www.humblebundle.com/api/v1/subscriptions/humble_monthly/subscription_products_with_gamekeys/
//https://www.humblebundle.com/store/api/lookup?products[]=sonic-mania&request=1

#nullable disable

using Dapper;
using Herramientas;
using Juegos;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Tiendas2;

namespace APIs.Humble
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "humblestore",
				Nombre = "Humble Store",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/humblestore_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/humblestore_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/humblestore_icono.webp",
				Color = "#ea9192",
				AdminUso = true,
				UsuarioUso = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa }
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			if (string.IsNullOrEmpty(enlace) == false)
			{
				if (Uri.TryCreate(enlace, UriKind.Absolute, out var uri) == false)
				{
					return enlace;
				}

				var slug = new Uri(enlace).AbsolutePath.Trim('/').Split('/').Last();
				var palabras = slug.Split('-', StringSplitOptions.RemoveEmptyEntries);

				string prodsku;
				if (palabras.Length > 1)
				{
					var titulo = string.Join(" ", palabras.Take(palabras.Length - 1).Select(Capitalizar));
					prodsku = $"{titulo} - {palabras[^1].ToUpperInvariant()}";
				}
				else if (palabras.Length == 1)
				{
					prodsku = Capitalizar(palabras[0]);
				}
				else
				{
					prodsku = string.Empty; 
				}

				return "https://humblebundleinc.sjv.io/c/1382810/2087718/25796"
					+ "?prodsku=" + Uri.EscapeDataString(prodsku)
					+ "&u=" + Uri.EscapeDataString(enlace)
					+ "&intsrc=CATF_18045";
			}
			else
			{
				return enlace;
			}
		}

		private static string Capitalizar(string palabra) => palabra.Length == 0 ? palabra : char.ToUpperInvariant(palabra[0]) + palabra[1..];

		public static Tiendas2.Tienda GenerarChoice()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "humblechoice",
				Nombre = "Humble Choice",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/humblechoice_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/humblechoice_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/humblestore_icono.webp",
				Color = "#ea9192",
				AdminUso = false,
				UsuarioUso = false,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa }
			};

			return tienda;
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			string busqueda = "SELECT contenido, enlace FROM temporalhumble";

			try
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				IEnumerable<dynamic> filas = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.QueryAsync(busqueda);
				});

				foreach (var fila in filas)
				{
					if (fila.contenido != null && string.IsNullOrEmpty((string)fila.contenido) == false)
					{
						string jsonReal = Regex.Unescape((string)fila.contenido);

						var opciones = new JsonSerializerOptions
						{
							PropertyNameCaseInsensitive = true
						};

						HumbleBusqueda nuevosJuegos = JsonSerializer.Deserialize<HumbleBusqueda>(jsonReal, opciones);

						if (nuevosJuegos?.Productos?.Count > 0)
						{
							foreach (var juego in nuevosJuegos.Productos)
							{
								string nombre = WebUtility.HtmlDecode(juego.Nombre);

								string imagen = WebUtility.HtmlDecode(juego.Imagen);

								string enlace = "https://www.humblebundle.com/store/" + juego.Enlace;

								if (juego.Precio?.PrecioBase?.Cantidad != null && juego.Precio?.PrecioRebajado?.Cantidad != null)
								{
									decimal precioRebajado = juego.Precio.PrecioRebajado.Cantidad;

									int descuento = Calculadora.SacarDescuento(juego.Precio.PrecioBase.Cantidad, precioRebajado);

									if (descuento > 0)
									{
										bool añadirChoice = true;

										//if (juego.CosasIncompatibles?.Count > 0)
										//{
										//	foreach (var cosa in juego.CosasIncompatibles)
										//	{
										//		if (cosa == "subscriber-discount-coupons")
										//		{
										//			añadirChoice = false;
										//		}
										//	}
										//}

										foreach (string drmTexto in juego.DRMs)
										{
											JuegoDRM drm = JuegoDRM2.Traducir(drmTexto, Generar().Id);

											JuegoPrecio oferta = new JuegoPrecio
											{
												Nombre = nombre,
												Enlace = enlace,
												Imagen = imagen,
												Moneda = JuegoMoneda.Euro,
												Precio = precioRebajado,
												Descuento = descuento,
												Tienda = Generar().Id,
												DRM = drm,
												FechaDetectado = DateTime.Now,
												FechaActualizacion = DateTime.Now
											};

											//if (juego.Precio.FechaTermina > 0)
											//{
											//	DateTime fechaTermina = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
											//	fechaTermina = fechaTermina.AddSeconds(juego.FechaTermina);
											//	fechaTermina = fechaTermina.ToLocalTime();

											//	oferta.FechaTermina = fechaTermina;
											//}

											ofertas.Add(oferta);



											//if (añadirChoice == true)
											//{
											//	decimal tempChoice = decimal.Parse(juego.PrecioRebajado.Cantidad.ToString()) * Convert.ToDecimal(DescuentoChoice(juego.DescuentoChoice));

											//	decimal precioChoice = decimal.Parse(juego.PrecioRebajado.Cantidad.ToString()) - tempChoice;
											//	precioChoice = Math.Round(precioChoice, 2);

											//	if (precioChoice < precioRebajado)
											//	{
											//		int descuentoChoice = Calculadora.SacarDescuento(decimal.Parse(juego.PrecioBase.Cantidad.ToString()), precioChoice);

											//		JuegoPrecio choice = new JuegoPrecio
											//		{
											//			Nombre = nombre,
											//			Enlace = enlace,
											//			Imagen = imagen,
											//			Moneda = JuegoMoneda.Euro,
											//			Precio = precioChoice,
											//			Descuento = descuentoChoice,
											//			Tienda = GenerarChoice().Id,
											//			DRM = drm,
											//			FechaDetectado = DateTime.Now,
											//			FechaActualizacion = DateTime.Now
											//		};

											//		if (juego.FechaTermina > 0)
											//		{
											//			DateTime fechaTermina = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
											//			fechaTermina = fechaTermina.AddSeconds(juego.FechaTermina);
											//			fechaTermina = fechaTermina.ToLocalTime();

											//			choice.FechaTermina = fechaTermina;
											//		}

											//		ofertas.Add(choice);
											//	}
											//}
										}

										if (ofertas?.Count > 0)
										{
											BaseDatos.Errores.Insertar.Mensaje("test 1", JsonSerializer.Serialize(ofertas[0]));
											BaseDatos.Errores.Insertar.Mensaje("test 2", JsonSerializer.Serialize(juego));
											break;
										}
									}
								}
							}
						}
					}

					//if (fila.enlace != null)
					//{
					//	string limpieza = "DELETE FROM temporalhumble WHERE enlace=@enlace";

					//	await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
					//	{
					//		return await conexion.ExecuteAsync(limpieza, new { enlace = fila.enlace }, transaction: sentencia);
					//	});
					//}
				}

				//if (ofertas?.Count > 0)
				//{
				//	int juegos2 = 0;

				//	int tamaño = 500;
				//	var lotes = ofertas
				//		.Select((oferta, indice) => new { oferta, indice })
				//		.GroupBy(x => x.indice / tamaño)
				//		.Select(g => g.Select(x => x.oferta).ToList())
				//		.ToList();

				//	foreach (var lote in lotes)
				//	{
				//		try
				//		{
				//			await BaseDatos.Tiendas.Comprobar.Resto(region, lote);
				//		}
				//		catch (Exception ex)
				//		{
				//			BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
				//		}

				//		juegos2 += lote.Count;

				//		try
				//		{
				//			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, juegos2);
				//		}
				//		catch (Exception ex)
				//		{
				//			BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
				//		}
				//	}
				//}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Humble Buscar Ofertas", ex);
			}
		}

		private static double DescuentoChoice(double cantidad)
		{
			double descuento = 0;

			if (cantidad == 0.1)
			{
				descuento = 0.2;
			}
			else if (cantidad == 0.05)
			{
				descuento = 0.15;
			}
			else if (cantidad == 0.03)
			{
				descuento = 0.13;
			}
			else if (cantidad == 0.02)
			{
				descuento = 0.12;
			}
			else if (cantidad == 0)
			{
				descuento = 0.10;
			}

			return descuento;
		}
	}

	public class HumbleBusqueda
	{
		[JsonPropertyName("products")]
		public List<HumbleProducto> Productos { get; set; }

		[JsonPropertyName("pagination")]
		public HumblePaginacion Paginacion { get; set; }

		[JsonPropertyName("success")]
		public bool Exito { get; set; }
	}

	public class HumblePaginacion
	{
		[JsonPropertyName("total")]
		public int Total { get; set; }

		[JsonPropertyName("page")]
		public int Pagina { get; set; }

		[JsonPropertyName("totalPages")]
		public int TotalPaginas { get; set; }
	}

	public class HumbleProducto
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("productKey")]
		public string Clave { get; set; }

		[JsonPropertyName("slug")]
		public string Slug { get; set; }

		[JsonPropertyName("url")]
		public string Enlace { get; set; }

		[JsonPropertyName("title")]
		public string Nombre { get; set; }

		[JsonPropertyName("thumbnail")]
		public string Imagen { get; set; }

		[JsonPropertyName("deliveryMethods")]
		public List<string> DRMs { get; set; }

		[JsonPropertyName("platforms")]
		public List<string> Sistemas { get; set; }

		[JsonPropertyName("displayPrice")]
		public HumblePrecioBloque Precio { get; set; }

		[JsonPropertyName("productType")]
		public string Tipo { get; set; }
	}

	public class HumblePrecioBloque
	{
		[JsonPropertyName("currentPrice")]
		public HumblePrecio PrecioRebajado { get; set; }

		[JsonPropertyName("basePrice")]
		public HumblePrecio PrecioBase { get; set; }

		[JsonPropertyName("isSale")]
		public bool EnOferta { get; set; }

		[JsonPropertyName("validUntil")]
		public string FechaTermina { get; set; }

		[JsonPropertyName("choiceDiscountEligible")]
		public bool ElegibleChoice { get; set; }
	}

	public class HumblePrecio
	{
		[JsonPropertyName("centAmount")]
		public int Centimos { get; set; }

		[JsonPropertyName("currencyCode")]
		public string Moneda { get; set; }

		[JsonPropertyName("fractionDigits")]
		public int Decimales { get; set; }

		[JsonIgnore]
		public decimal Cantidad => Centimos / (decimal)Math.Pow(10, Decimales);
	}
}
