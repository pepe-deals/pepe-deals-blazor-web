#nullable disable

using Herramientas;
using Herramientas.Afiliados;
using Juegos;
using System.Net;
using Tiendas2;

namespace APIs.Driffle
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "driffle",
				Nombre = "Driffle",
				Tipo = TiendaTipo.Marketplace,
				ImagenLogo = "/imagenes/tiendas/driffle_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/driffle_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/driffle_icono.ico",
				Color = "#558205",
				AdminUso = true,
				UsuarioUso = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa }
			};

			return tienda;
		}

		public static string Referido(string enlace)
		{
			return Daisycon.CrearEnlaceAfiliado(enlace);
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			string enlace = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				enlace = Daisycon.DriffleEuropa;
			}

			if (string.IsNullOrEmpty(enlace) == true)
			{
				return;
			}

			string html = await Decompiladores.Estandar(enlace);

			if (string.IsNullOrEmpty(html) == false)
			{
				var resultados = await Daisycon.ObtenerFeed(enlace);

				if (resultados?.Info?.TotalProductos > 0)
				{
					List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

					foreach (var programa in resultados.Programas)
					{
						foreach (var resultado in programa.Productos)
						{
							if (resultado.Info?.Descuento > 0 && resultado.Info?.EnStock == true && resultado.Info?.Moneda == "EUR" && resultado.Info?.PlataformaActivacion == "Steam" && (resultado.Info?.Region == "Global" || resultado.Info?.Region == "Europe") && resultado.Info?.Nombre.Contains("Steam Gift") == false)
							{
								string nombre = WebUtility.HtmlDecode(resultado.Info?.Nombre);
								nombre = nombre.Replace("- Steam - Digital Key", null);
								nombre = nombre.Replace("(Global) (PC)", null);
								nombre = nombre.Replace("(Europe) (PC)", null);
								nombre = nombre.Replace("(Global) (PC / Mac)", null);
								nombre = nombre.Replace("(Europe) (PC / Mac)", null);
								nombre = nombre.Replace("(Global) (PC / Linux)", null);
								nombre = nombre.Replace("(Europe) (PC / Linux)", null);
								nombre = nombre.Replace("(Global) (PC / Mac / Linux)", null);
								nombre = nombre.Replace("(Europe) (PC / Mac / Linux)", null);
								nombre = nombre.Trim();

								if (nombre.EndsWith(" DLC") == true)
								{
									nombre = nombre.Replace("DLC", null);
									nombre = nombre.Trim();
								}

								string enlaceJuego = Daisycon.LimpiarEnlace(resultado.Info?.Url, "https://driffle.com");

								string imagen = resultado.Info?.ImagenUrl;

								JuegoPrecio oferta = new JuegoPrecio
								{
									Nombre = nombre,
									Enlace = enlaceJuego,
									Imagen = imagen,
									Precio = resultado.Info?.PrecioActual.Value ?? 0,
									Descuento = resultado.Info?.Descuento.Value ?? 0,
									Tienda = Generar().Id,
									DRM = JuegoDRM.Steam,
									FechaDetectado = DateTime.Now,
									FechaActualizacion = DateTime.Now,
									Moneda = JuegoMoneda.Euro
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
}
