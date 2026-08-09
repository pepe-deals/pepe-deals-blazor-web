using Juegos;

namespace APIs.Humble
{
	public static class Suscripcion
	{
		public static Suscripciones2.Suscripcion Generar()
		{
			Suscripciones2.Suscripcion humbleChoice = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.HumbleChoice,
				Nombre = "Humble Choice",
				ImagenLogo = "/imagenes/suscripciones/humblechoice.webp",
				ImagenIcono = "/imagenes/tiendas/humblestore_icono.webp",
				ImagenFondo = "/imagenes/suscripciones/humblechoice_fondo.webp",
				ColorDestacado = "rgba(201, 42, 46, 0.3)",
				Enlace = "https://www.humblebundle.com/membership/home",
				DRMDefecto = JuegoDRM.Steam,
				UsuarioEnlacesEspecificos = false,
				ParaSiempre = true,
				Precio = 12.99,
				AdminAñadir = true,
				UsuarioPuedeAbrir = true
			};

			DateTime fechaHumbleChoice = DateTime.Now;
			fechaHumbleChoice = fechaHumbleChoice.AddMonths(1);
			fechaHumbleChoice = new DateTime(fechaHumbleChoice.Year, fechaHumbleChoice.Month, 1, 19, 0, 0);

			int i = 1;
			while (i <= 7)
			{
				if (fechaHumbleChoice.DayOfWeek == DayOfWeek.Tuesday)
				{
					break;
				}
				else
				{
					fechaHumbleChoice = fechaHumbleChoice.AddDays(1);
				}

				i += 1;
			}

			fechaHumbleChoice = new DateTime(fechaHumbleChoice.Year, fechaHumbleChoice.Month, i, 19, 0, 0);

			humbleChoice.FechaSugerencia = fechaHumbleChoice;

			return humbleChoice;
		}

		public static string Referido(string enlace)
		{
			var slug = new Uri(enlace).AbsolutePath.Trim('/').Split('/').Last();
			var palabras = slug.Split('-', StringSplitOptions.RemoveEmptyEntries);

			string prodsku;
			if (palabras.Length > 1)
			{
				var titulo = string.Join(" ", palabras.Take(palabras.Length - 1).Select(Capitalizar));
				prodsku = $"{titulo} - {palabras[^1].ToUpperInvariant()}";
			}
			else
			{
				prodsku = Capitalizar(palabras[0]);
			}

			return "https://humblebundleinc.sjv.io/c/1382810/2059850/25796"
				+ "?prodsku=" + Uri.EscapeDataString(prodsku)
				+ "&u=" + Uri.EscapeDataString(enlace);
		}

		private static string Capitalizar(string palabra) => palabra.Length == 0 ? palabra : char.ToUpperInvariant(palabra[0]) + palabra[1..];

		public static Suscripciones2.Suscripcion GenerarAntiguo()
		{
			Suscripciones2.Suscripcion humbleMonthly = new Suscripciones2.Suscripcion
			{
				Id = Suscripciones2.SuscripcionTipo.HumbleMonthly,
				Nombre = "Humble Monthly",
				ImagenLogo = "/imagenes/suscripciones/humblemonthly.webp",
				ImagenIcono = "/imagenes/tiendas/humblestore_icono.webp",
				Enlace = "https://www.humblebundle.com/membership/home",
				DRMDefecto = JuegoDRM.Steam,
                UsuarioEnlacesEspecificos = false,
				ParaSiempre = true,
				AdminAñadir = false
			};

			DateTime fechaHumbleMonthly = DateTime.Now;
			fechaHumbleMonthly = fechaHumbleMonthly.AddMonths(1);
			fechaHumbleMonthly = new DateTime(fechaHumbleMonthly.Year, fechaHumbleMonthly.Month, 1, 19, 0, 0);

			int i = 1;
			while (i <= 7)
			{
				if (fechaHumbleMonthly.DayOfWeek == DayOfWeek.Tuesday)
				{
					break;
				}
				else
				{
					fechaHumbleMonthly = fechaHumbleMonthly.AddDays(1);
				}

				i += 1;
			}

			fechaHumbleMonthly = new DateTime(fechaHumbleMonthly.Year, fechaHumbleMonthly.Month, i, 19, 0, 0);

			humbleMonthly.FechaSugerencia = fechaHumbleMonthly;

			return humbleMonthly;
		}
	}
}
