using Juegos;
using Suscripciones2;
using Tiendas2;

namespace APIs.GTAPlus
{
	public static class Suscripcion
	{
		public static Suscripciones2.Suscripcion Generar()
		{
			Suscripciones2.Suscripcion gtaplus = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.GTAPlus,
				Nombre = "GTA+",
				ImagenLogo = "/imagenes/suscripciones/gtaplus_logo.webp",
				ImagenIcono = "/imagenes/suscripciones/gtaplus_icono.webp",
				ImagenFondo = "/imagenes/suscripciones/gtaplus_fondo.webp",
				ColorDestacado = "rgba(246, 135, 30, 0.2)",
				Enlace = "https://store.steampowered.com/subscriptions/gtaplus",
				DRMDefecto = JuegoDRM.Steam,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = false,
				Precio = 8.99,
				AdminAñadir = false,
				UsuarioPuedeAbrir = true,
				SteamPaquete = 1229928
			};

			return gtaplus;
		}

		public static async Task Buscar()
		{
			await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, Generar().Id.ToString().ToLower(), DateTime.Now, 0);

			int juegos2 = 0;

			if (Generar().SteamPaquete > 0)
			{
				List<int> juegosIdsSteam = await BaseDatos.Suscripciones.Buscar.Steam(Generar().SteamPaquete);

				if (juegosIdsSteam.Count > 0)
				{
					List<Juegos.Juego> juegos = await BaseDatos.Juegos.Buscar.MultiplesJuegosSteam2(false, TiendaRegion.Europa, juegosIdsSteam);
			
					if (juegos?.Count > 0)
					{
						foreach (var juego in juegos)
						{
							bool insertar = true;
							var suscripciones = await BaseDatos.Suscripciones.Buscar.JuegoId(juego.Id);

							if (suscripciones?.Count > 0)
							{
								foreach (var suscripcion in suscripciones)
								{
									if (suscripcion.Tipo == SuscripcionTipo.GTAPlus)
									{
										insertar = false;

										suscripcion.FechaTermina = DateTime.Now + TimeSpan.FromDays(30);

										await BaseDatos.Suscripciones.Actualizar.FechaTermina(suscripcion);
									}
								}
							}

							juegos2 += 1;
							await BaseDatos.Admin.Actualizar.Tiendas(TiendaRegion.Europa, Generar().Id.ToString().ToLower(), DateTime.Now, juegos2);

							if (insertar == true)
							{
								JuegoSuscripcion juegoSuscripcion = new JuegoSuscripcion
								{
									JuegoId = juego.Id,
									DRM = JuegoDRM.Steam,
									Tipo = Suscripciones2.SuscripcionTipo.GTAPlus,
									Nombre = juego.Nombre,
									Enlace = "https://store.steampowered.com/app/" + juego.IdSteam,
									Imagen = juego.Imagenes.Header_460x215,
									FechaEmpieza = DateTime.Now,
									FechaTermina = DateTime.Now + TimeSpan.FromDays(30)
								};

								await BaseDatos.Suscripciones.Insertar.Ejecutar(juego.Id, juegoSuscripcion);
							}
						}
					}
				}
			}
		}
	}
}
