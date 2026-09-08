#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using Tiendas2;

namespace APIs.Gameseal
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "gameseal",
				Nombre = "Gameseal",
				Tipo = TiendaTipo.NoOficial,
				ImagenLogo = "/imagenes/tiendas/gameseal_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/gameseal_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/gameseal_icono.webp",
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

			List<ImpactCatalogItem> resultados = new List<ImpactCatalogItem>();

			if (region == TiendaRegion.Europa)
			{
				resultados = await Herramientas.Impact.ObtenerCatalogo("18613");
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				resultados = await Herramientas.Impact.ObtenerCatalogo("24509");
			}

			if (resultados?.Count > 0)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				foreach (var resultado in resultados)
				{
					if (string.IsNullOrEmpty(resultado.Nombre) == false)
					{
						JuegoDRM drmJuego = JuegoDRM.NoEspecificado;

						if (region == TiendaRegion.Europa && resultado.Nombre.Contains("Steam Key - EU") == true || resultado.Nombre.Contains("Steam Key - GLOBAL") == true)
						{
							drmJuego = JuegoDRM.Steam;
						}
						else if (region == TiendaRegion.EstadosUnidos && resultado.Nombre.Contains("Steam Key - NA") == true || resultado.Nombre.Contains("Steam Key - GLOBAL") == true)
						{
							drmJuego = JuegoDRM.Steam;
						}

						if (drmJuego != JuegoDRM.NoEspecificado)
						{
							if (string.IsNullOrEmpty(resultado.Disponibilidad) == false && resultado.Disponibilidad == "InStock")
							{
								string nombre = WebUtility.HtmlDecode(resultado.Nombre);
								nombre = nombre.Replace("Steam Key - EU/US", null);
								nombre = nombre.Replace("Steam Key - EU/NA", null);
								nombre = nombre.Replace("Steam Key - EU", null);
								nombre = nombre.Replace("Steam Key - GLOBAL", null);
								nombre = nombre.Replace("Steam Key - NA/LATIN AMERICA", null);
								nombre = nombre.Replace("Steam Key - NA", null);
								nombre = nombre.Replace("(PC)", null);
								nombre = nombre.Replace("(MAC)", null);
								nombre = nombre.Replace("(DLC)", null);
								nombre = nombre.Trim();

								string enlaceJuego = resultado.Url;
								enlaceJuego = enlaceJuego.Replace("%3Fcurrency%3DUSD&intsrc=APIG_24509", null);
								enlaceJuego = enlaceJuego.Replace("%3Fcurrency%3DEUR&intsrc=APIG_18613", null);
								enlaceJuego = enlaceJuego.Replace("&intsrc=APIG_24509", null);
								enlaceJuego = enlaceJuego.Replace("&intsrc=APIG_18613", null);

								string imagen = resultado.ImagenUrl;

								JuegoPrecio oferta = new JuegoPrecio
								{
									Nombre = nombre,
									Enlace = enlaceJuego,
									Imagen = imagen,
									Moneda = JuegoMoneda.Euro,
									Precio = resultado.PrecioActual.Value,
									Descuento = 0,
									Tienda = Generar().Id,
									DRM = drmJuego,
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
