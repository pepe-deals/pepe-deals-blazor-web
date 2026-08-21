//https://store.epicgames.com/graphql?operationName=searchStoreQuery&variables={%22allowCountries%22:%22ES%22,%22category%22:%22games/edition/base|addons|games/edition%22,%22count%22:40,%22country%22:%22ES%22,%22effectiveDate%22:%22[,2024-11-19T19:59:57.141Z]%22,%22keywords%22:%22%22,%22locale%22:%22en-GB%22,%22onSale%22:true,%22sortBy%22:%22relevancy,viewableDate%22,%22sortDir%22:%22DESC,DESC%22,%22start%22:40,%22tag%22:%22%22,%22withPrice%22:true}&extensions={%22persistedQuery%22:{%22version%22:1,%22sha256Hash%22:%227d58e12d9dd8cb14c84a3ff18d360bf9f0caa96bf218f2c5fda68ba88d68a437%22}}

#nullable disable

using Dapper;
using Herramientas;
using Juegos;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs.EpicGames
{
	public class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "epicgamesstore",
				Nombre = "Epic Games Store",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/epic_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/epic_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/epic_icono.ico",
				Color = "#101014",
				AdminUso = true,
				UsuarioUso = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa }
			};

			return tienda;
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			List<int> idsBorrar = new List<int>();

			string busqueda = "SELECT * FROM temporalepictienda";

			IEnumerable<dynamic> filas = await Herramientas.BaseDatos.Select(async conexion =>
			{
				return await conexion.QueryAsync(busqueda);
			});

			if (filas?.Count() > 0)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				foreach (var fila in filas)
				{
					string contenido = fila.contenido;

					if (string.IsNullOrEmpty(contenido) || contenido.Contains("<title>404 Not Found</title>") || contenido == "null" || contenido == "elements\":[]")
					{
						idsBorrar.Add(fila.id);
						continue;
					}

					EpicGamesStorePrincipal principal = null;

					try
					{
						principal = JsonSerializer.Deserialize<EpicGamesStorePrincipal>(contenido);
					}
					catch (Exception ex)
					{
						idsBorrar.Add(fila.id);

						BaseDatos.Errores.Insertar.Mensaje("Epic Games Tienda Id: " + fila.id.ToString(), ex, false);
					}

					if (principal?.Datos?.Catalogo?.Busqueda?.Juegos?.Count > 0)
					{
						foreach (var juego in principal?.Datos?.Catalogo?.Busqueda?.Juegos)
						{
							if (juego.Precio?.PrecioTotal?.PrecioFmt != null)
							{
								if (string.IsNullOrEmpty(juego.Precio.PrecioTotal.PrecioFmt.PrecioRebajado) == false)
								{
									string textoPrecioRebajado = juego.Precio.PrecioTotal.PrecioFmt.PrecioRebajado;

									if (string.IsNullOrEmpty(textoPrecioRebajado) == false)
									{
										textoPrecioRebajado = textoPrecioRebajado.Replace("€", null);
										textoPrecioRebajado = textoPrecioRebajado.Replace(",", ".");
										textoPrecioRebajado = textoPrecioRebajado.Trim();
									}

									string textoPrecioBase = juego.Precio.PrecioTotal.PrecioFmt.PrecioBase;

									if (string.IsNullOrEmpty(textoPrecioBase) == false)
									{
										textoPrecioBase = textoPrecioBase.Replace("€", null);
										textoPrecioBase = textoPrecioBase.Replace(",", ".");
										textoPrecioBase = textoPrecioBase.Trim();
									}

									if (string.IsNullOrEmpty(textoPrecioRebajado) == false && string.IsNullOrEmpty(textoPrecioBase) == false)
									{
										decimal precioRebajado = decimal.Parse(textoPrecioRebajado);
										decimal precioBase = decimal.Parse(textoPrecioBase);

										int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

										string slug = null;

										if (juego.Enlaces2?.Mapeos?.Count > 0)
										{
											slug = juego.Enlaces2.Mapeos[0].Slug;
										}

										if (string.IsNullOrEmpty(slug) == true)
										{
											if (juego.Enlaces?.Count > 0)
											{
												slug = juego.Enlaces[0].Slug;
											}
										}

										if (string.IsNullOrEmpty(slug) == true)
										{
											slug = juego.Enlace;
										}

										if (descuento > 0 && string.IsNullOrEmpty(slug) == false && juego.Imagenes != null)
										{
											if (juego.Imagenes.Count > 0)
											{
												string nombre = juego.Nombre;
												nombre = WebUtility.HtmlDecode(nombre);

												string enlace = "https://store.epicgames.com/p/" + slug;

												string imagen = juego.Imagenes[0].Enlace;

												JuegoPrecio oferta = new JuegoPrecio
												{
													Nombre = nombre,
													Enlace = enlace,
													Imagen = imagen,
													Moneda = JuegoMoneda.Euro,
													Precio = precioRebajado,
													Descuento = descuento,
													Tienda = Generar().Id,
													DRM = JuegoDRM.Epic,
													FechaDetectado = DateTime.Now,
													FechaActualizacion = DateTime.Now
												};

												ofertas.Add(oferta);

												bool actualizarBaseDatos = true;

												if (juego.Categorias?.Count > 0)
												{
													foreach (var categoria in juego.Categorias)
													{
														if (string.IsNullOrEmpty(categoria.Path) == false && categoria.Path.Contains("addon") == true)
														{
															oferta.Enlace = "https://store.epicgames.com/p/" + juego.Enlace;
															actualizarBaseDatos = false;
															break;
														}
													}
												}

												if (actualizarBaseDatos == true && string.IsNullOrEmpty(juego.Namespace) == false)
												{
													await APIs.EpicGames.Juego.ActualizarBaseDatos(juego.Namespace, slug);
												}

												string limpieza = "DELETE FROM temporalepictienda WHERE id=@id";

												try
												{
													await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
													{
														return await conexion.ExecuteAsync(limpieza, new { fila.id }, transaction: sentencia);
													});
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

	public class EpicGamesStorePrincipal
	{
		[JsonPropertyName("data")]
		public EpicGamesStoreData Datos { get; set; }
	}

	public class EpicGamesStoreData
	{
		[JsonPropertyName("Catalog")]
		public EpicGamesStoreCatalog Catalogo { get; set; }
	}

	public class EpicGamesStoreCatalog
	{
		[JsonPropertyName("searchStore")]
		public EpicGamesStoreSearch Busqueda { get; set; }
	}

	public class EpicGamesStoreSearch
	{
		[JsonPropertyName("elements")]
		public List<EpicGamesStoreJuego> Juegos { get; set; }
	}

	public class EpicGamesStoreJuego
	{
		[JsonPropertyName("title")]
		public string Nombre { get; set; }

		[JsonPropertyName("namespace")]
		public string Namespace { get; set; }

		[JsonPropertyName("keyImages")]
		public List<EpicGamesStoreJuegoImagen> Imagenes { get; set; }

		[JsonPropertyName("offerMappings")]
		public List<EpicGamesStoreJuegoEnlace> Enlaces { get; set; }

		[JsonPropertyName("price")]
		public EpicGamesStoreJuegoPrecio Precio { get; set; }

		[JsonPropertyName("productSlug")]
		public string Enlace { get; set; }

        [JsonPropertyName("catalogNs")]
        public EpicGamesStoreJuegoEnlaceMapeo Enlaces2 { get; set; }

		[JsonPropertyName("items")]
		public List<EpicGamesStoreJuegoItem> Items { get; set; }

		[JsonPropertyName("categories")]
		public List<EpicGamesStoreJuegoCategoria> Categorias { get; set; }
	}

	public class EpicGamesStoreJuegoImagen
	{
		[JsonPropertyName("type")]
		public string Tipo { get; set; }

		[JsonPropertyName("url")]
		public string Enlace { get; set; }
	}

	public class EpicGamesStoreJuegoEnlace
	{
		[JsonPropertyName("pageSlug")]
		public string Slug { get; set; }
	}

	public class EpicGamesStoreJuegoPrecio
	{
		[JsonPropertyName("totalPrice")]
		public EpicGamesStoreJuegoPrecio2 PrecioTotal { get; set; }
	}

	public class EpicGamesStoreJuegoPrecio2
	{
		[JsonPropertyName("fmtPrice")]
		public EpicGamesStoreJuegoPrecio3 PrecioFmt { get; set; }
	}

	public class EpicGamesStoreJuegoPrecio3
	{
		[JsonPropertyName("originalPrice")]
		public string PrecioBase { get; set; }

		[JsonPropertyName("discountPrice")]
		public string PrecioRebajado { get; set; }
	}

    public class EpicGamesStoreJuegoEnlaceMapeo
    {
        [JsonPropertyName("mappings")]
        public List<EpicGamesStoreJuegoEnlace> Mapeos { get; set; }
    }

	public class EpicGamesStoreJuegoItem
	{
		[JsonPropertyName("namespace")]
		public string Namespace { get; set; }
	}

	public class EpicGamesStoreJuegoCategoria
	{
		[JsonPropertyName("path")]
		public string Path { get; set; }
	}
}
