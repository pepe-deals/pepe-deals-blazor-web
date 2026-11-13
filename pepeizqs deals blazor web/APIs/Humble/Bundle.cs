#nullable disable

using Herramientas;
using HtmlAgilityPack;
using Microsoft.VisualBasic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace APIs.Humble
{
	public static class Bundle
	{
		public static Bundles2.Bundle Generar()
		{
			Bundles2.Bundle bundle = new Bundles2.Bundle()
			{
				BundleTipo = Bundles2.BundleTipo.HumbleBundle,
				Tienda = "Humble Bundle",
				ImagenTienda = "/imagenes/bundles/humblebundle_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/humblestore_icono.webp",
				EnlaceBase = "humblebundle.com",
				Pick = false,
				ImagenesExtra = new List<string> { "/imagenes/bundles/humble_logo1.webp", "/imagenes/bundles/humble_logo2.webp" }
			};

			DateTime fechaEmpieza = DateTime.Now;
			fechaEmpieza = new DateTime(fechaEmpieza.Year, fechaEmpieza.Month, fechaEmpieza.Day, DateTime.Now.Hour, 0, 0);

			bundle.FechaEmpieza = fechaEmpieza;

			DateTime fechaTermina = DateTime.Now;
			fechaTermina = fechaTermina.AddDays(21);
			fechaTermina = fechaTermina.AddHours(10);
			fechaTermina = new DateTime(fechaTermina.Year, fechaTermina.Month, fechaTermina.Day, fechaTermina.Hour, 0, 0);

			bundle.FechaTermina = fechaTermina;

			return bundle;
		}

		public static string Referido(string enlace)
		{
			return Tienda.Referido(enlace);
		}

		public static async Task<Bundles2.Bundle> ExtraerDatos(Bundles2.Bundle bundle)
		{
            if (bundle.Enlace != "https://www.humblebundle.com/")
            {
				string html = await Decompiladores.Estandar(bundle.Enlace);

				if (string.IsNullOrEmpty(html) == false)
				{
					HtmlDocument doc = new HtmlDocument();
					doc.LoadHtml(html);

					bundle.Nombre = doc.DocumentNode.SelectSingleNode("//h1")?.InnerText.Trim();

					string imagen = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']")?.GetAttributeValue("content", "");

					bundle.Imagen = imagen;
					bundle.ImagenNoticia = imagen;

					string json = html.Replace("\\\"", "\"");

					MatchCollection tiers = Regex.Matches(json,
						@"""price"":\{""amount"":([\d.]+),""currencyCode"":""([A-Z]{3})""\},""totalMsrpValue"":\{""amount"":([\d.]+)");

					List<Bundles2.BundleTier> tiers2 = new List<Bundles2.BundleTier>();

					foreach (Match tier in tiers)
					{
						decimal precio = decimal.Parse(tier.Groups[1].Value, CultureInfo.InvariantCulture);					

						Bundles2.BundleTier tierNuevo = new Bundles2.BundleTier
						{
							Posicion = (tiers2.Count + 1),
							Precio = precio.ToString()
						};

						bool añadir = true;

						if (tiers.Count > 0)
						{
							foreach (var tier2 in tiers2)
							{
								if (tierNuevo.Precio == tier2.Precio)
								{
									añadir = false;
								}
							}
						}

						if (añadir == true)
						{
							tiers2.Add(tierNuevo);
						}
					}

					bundle.Tiers = tiers2;
				}
			}

			return bundle;
		}
	}
}
