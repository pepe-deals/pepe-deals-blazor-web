#nullable disable

using Herramientas;
using Juegos;
using System.Net;
using Tiendas2;

namespace APIs.Loaded
{

	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "loaded",
				Nombre = "Loaded",
				Tipo = TiendaTipo.NoOficial,
				ImagenLogo = "/imagenes/tiendas/muvegames_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/muvegames_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/muvegames_icono.ico",
				Color = "#558205",
				AdminEnseñar = true,
				AdminInteractuar = true,
				Regiones = new List<TiendaRegion> { TiendaRegion.Europa }
			};

			return tienda;
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);

			var resultados = await Herramientas.Impact.ObtenerCatalogo("12138");

			if (resultados?.Count > 0)
			{
				List<JuegoPrecio> ofertas = new List<JuegoPrecio>();

				foreach (var resultado in resultados)
				{
					if (resultado.Disponibilidad == "InStock")
					{	
						if (resultado.Descuento.HasValue == true && resultado.Descuento.Value > 0)
						{
							string nombre = WebUtility.HtmlDecode(resultado.Nombre);

							string enlaceJuego = resultado.Url;

							string imagen = resultado.ImagenUrl;

							JuegoDRM drm = JuegoDRM2.Traducir(resultado.Plataforma, Generar().Id);

							JuegoPrecio oferta = new JuegoPrecio
							{
								Nombre = nombre,
								Enlace = enlaceJuego,
								Imagen = imagen,
								Moneda = JuegoMoneda.Euro,
								Precio = resultado.PrecioActual.Value,
								Descuento = resultado.Descuento.Value,
								Tienda = Generar().Id,
								DRM = drm,
								FechaDetectado = DateTime.Now,
								FechaActualizacion = DateTime.Now
							};

							if (drm == JuegoDRM.Steam)
							{
								ofertas.Add(oferta);
							}
						}
					}
				}

				BaseDatos.Errores.Insertar.Mensaje(Generar().Id, resultados.Count.ToString() + " " + ofertas.Count.ToString());

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
