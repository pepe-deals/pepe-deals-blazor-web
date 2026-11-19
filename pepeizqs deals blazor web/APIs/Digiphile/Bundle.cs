#nullable disable

using Herramientas;
using HtmlAgilityPack;
using System.Net;

namespace APIs.Digiphile
{
	public static class Bundle
	{
		public static Bundles2.Bundle Generar()
		{
			Bundles2.Bundle bundle = new Bundles2.Bundle()
			{
				BundleTipo = Bundles2.BundleTipo.Digiphile,
				Tienda = "Digiphile",
				ImagenTienda = "/imagenes/bundles/digiphile_300x80.webp",
				ImagenIcono = "/imagenes/bundles/digiphile_icono.ico",
				EnlaceBase = "digiphile.co",
				Pick = false
			};

			DateTime fechaEmpieza = DateTime.Now;
			fechaEmpieza = new DateTime(fechaEmpieza.Year, fechaEmpieza.Month, fechaEmpieza.Day, fechaEmpieza.Hour, 0, 0);

			bundle.FechaEmpieza = fechaEmpieza;

			DateTime fechaTermina = DateTime.Now;
			fechaTermina = fechaTermina.AddDays(35);
			fechaTermina = new DateTime(fechaTermina.Year, fechaTermina.Month, fechaTermina.Day, fechaTermina.Hour, 0, 0);

			bundle.FechaTermina = fechaTermina;

			return bundle;
		}

		public static async Task<Bundles2.Bundle> ExtraerDatos(Bundles2.Bundle bundle)
		{
			if (bundle.Enlace != "https://www.digiphile.co/")
			{
				string html = await Decompiladores.Estandar(bundle.Enlace);

				if (string.IsNullOrEmpty(html) == false)
				{
					HtmlDocument doc = new HtmlDocument();
					doc.LoadHtml(html);

					bundle.Nombre = WebUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("//h1")?.InnerText.Trim());

					string imagen = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']")?.GetAttributeValue("content", "");

					bundle.Imagen = imagen;
					bundle.ImagenNoticia = imagen;
				}
			}

			return bundle;
		}
	}
}