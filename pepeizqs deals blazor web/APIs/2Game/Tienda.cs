#nullable disable

using Herramientas;
using Juegos;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tiendas2;

namespace APIs._2Game
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "2game",
				Nombre = "2Game",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/2game_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/2game_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/2game_icono.webp",
				Color = "#beb2f1",
				AdminUso = false,
				UsuarioUso = false
			};

			return tienda;
		}

        public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			int juegos2 = 0;

			int paginas = 10;

            int i = 1;
            while (i <= paginas)
            {
                string html = await Decompiladores.GZipFormato("https://2game.com/graphql?hash=2427175844&sort_1={%22bestsellers%22:%22DESC%22}&filter_1={%22price%22:{},%22special_price%22:{%22from%22:0.01},%22category_id%22:{%22eq%22:510}}&pageSize_1=48&currentPage_1=" + i.ToString() + "&popularBlockPageSize_1=12&storeCode=%22es_es%22");

				if (string.IsNullOrEmpty(html) == false)
				{
					_2GameResultados resultados = JsonSerializer.Deserialize<_2GameResultados>(html);

					if (resultados?.Datos?.Productos != null)
					{
						paginas = resultados.Datos.Productos.Paginas.TotalPaginas;

						List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

						foreach (var juego in resultados.Datos.Productos.Juegos)
						{
							if (juego.Precios.Datos.PrecioRebajado.Moneda.ToLower() == "eur" && juego.Precios.Datos.PrecioBase.Moneda.ToLower() == "eur")
							{
								decimal precioRebajado = juego.Precios.Datos.PrecioRebajado.Cantidad;
								decimal precioBase = juego.Precios.Datos.PrecioBase.Cantidad;

								int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

								if (descuento > 0)
								{
									string imagen = juego.Imagen.Enlace;

									string enlace = "https://2game.com" + juego.Enlace;
									enlace = enlace.Replace("/en_gb/", "/");
									enlace = enlace.Replace("/en_es/", "/");
									enlace = enlace.Replace("/es_es/", "/");

									JuegoPrecio oferta = new JuegoPrecio
									{
										Nombre = juego.Nombre,
										Enlace = enlace,
										Imagen = imagen,
										Moneda = JuegoMoneda.Euro,
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

				i += 1;
			}
		}
	}

	#region Clases

	public class _2GameResultados
	{
		[JsonPropertyName("data")]
		public _2GameDatos Datos { get; set; }
	}

	public class _2GameDatos
	{
		[JsonPropertyName("products")]
		public _2GameProductos Productos { get; set; }
	}

	public class _2GameProductos
	{
		[JsonPropertyName("page_info")]
		public _2GamePaginas Paginas { get; set; }

		[JsonPropertyName("items")]
		public List<_2GameJuego> Juegos { get; set; }
	}

	public class _2GamePaginas
	{
		[JsonPropertyName("current_page")]
		public int ActualPagina { get; set; }

		[JsonPropertyName("total_pages")]
		public int TotalPaginas { get; set; }
	}

	public class _2GameJuego
	{
		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("url")]
		public string Enlace { get; set; }

		[JsonPropertyName("main_image")]
		public _2GameJuegoImagen Imagen { get; set; }

		[JsonPropertyName("price_range")]
		public _2GameJuegoPrecios Precios { get; set; }
	}

	public class _2GameJuegoImagen
	{
		[JsonPropertyName("_full")]
		public string Enlace { get; set; }
	}

	public class _2GameJuegoPrecios
	{
		[JsonPropertyName("minimum_price")]
		public _2GameJuegoPreciosMinimo Datos { get; set; }
	}

	public class _2GameJuegoPreciosMinimo
	{
		[JsonPropertyName("final_price")]
		public _2GameJuegoPrecio PrecioRebajado { get; set; }

		[JsonPropertyName("regular_price")]
		public _2GameJuegoPrecio PrecioBase { get; set; }
	}

	public class _2GameJuegoPrecio
	{
		[JsonPropertyName("value")]
		public decimal Cantidad { get; set; }

        [JsonPropertyName("currency")]
        public string Moneda { get; set; }
    }

	#endregion
}
