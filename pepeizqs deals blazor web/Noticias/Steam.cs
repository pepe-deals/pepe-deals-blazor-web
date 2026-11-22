#nullable disable

using Juegos;
using System.Text.RegularExpressions;

namespace Noticias
{
	public static class Steam
	{
		public static async Task<Noticia> GenerarNoticia(Noticia noticia)
		{
			noticia.SteamEn = new NoticiaSteam();
			noticia.SteamEs = new NoticiaSteam();

			if (noticia.NoticiaTipo == NoticiaTipo.Bundles)
			{
				noticia.SteamEn.Titulo = string.Format(Herramientas.Idiomas.BuscarTexto("en", "Steam1", "NewsTemplates"), Bundles2.BundlesCargar.DevolverBundle(noticia.BundleTipo).Tienda);
				noticia.SteamEs.Titulo = string.Format(Herramientas.Idiomas.BuscarTexto("es", "Steam1", "NewsTemplates"), Bundles2.BundlesCargar.DevolverBundle(noticia.BundleTipo).Tienda);
			}
			else if (noticia.NoticiaTipo == NoticiaTipo.Gratis)
			{
				noticia.SteamEn.Titulo = string.Format(Herramientas.Idiomas.BuscarTexto("en", "Steam2", "NewsTemplates"), Gratis2.GratisCargar.DevolverGratis(noticia.GratisTipo).Nombre);
				noticia.SteamEs.Titulo = string.Format(Herramientas.Idiomas.BuscarTexto("es", "Steam2", "NewsTemplates"), Gratis2.GratisCargar.DevolverGratis(noticia.GratisTipo).Nombre);
			}
			else if (noticia.NoticiaTipo == NoticiaTipo.Suscripciones)
			{
				noticia.SteamEn.Titulo = string.Format(Herramientas.Idiomas.BuscarTexto("en", "Steam3", "NewsTemplates"), Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(noticia.SuscripcionTipo).Nombre);
				noticia.SteamEs.Titulo = string.Format(Herramientas.Idiomas.BuscarTexto("es", "Steam3", "NewsTemplates"), Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(noticia.SuscripcionTipo).Nombre);
			}
			else if (noticia.NoticiaTipo == NoticiaTipo.Web)
			{
				noticia.SteamEn.Titulo = Herramientas.Idiomas.BuscarTexto("en", "Steam4", "NewsTemplates");
				noticia.SteamEs.Titulo = Herramientas.Idiomas.BuscarTexto("es", "Steam4", "NewsTemplates");
			}
			else
			{
				noticia.SteamEn.Titulo = Herramientas.Idiomas.BuscarTexto("en", "Steam5", "NewsTemplates");
				noticia.SteamEs.Titulo = Herramientas.Idiomas.BuscarTexto("es", "Steam5", "NewsTemplates");
			}

			if (noticia.SteamEn.Titulo.Length > 80)
			{
				noticia.SteamEn.Titulo = noticia.SteamEn.Titulo.Remove(80, noticia.SteamEn.Titulo.Length - 80);
			}

			if (noticia.SteamEs.Titulo.Length > 80)
			{
				noticia.SteamEs.Titulo = noticia.SteamEs.Titulo.Remove(80, noticia.SteamEs.Titulo.Length - 80);
			}

			string subtituloEn = noticia.TituloEn;

			if (subtituloEn.Length > 120)
			{
				subtituloEn = subtituloEn.Remove(117, subtituloEn.Length - 117) + "...";
			}

			noticia.SteamEn.Subtitulo = subtituloEn;

			string subtituloEs = noticia.TituloEs;

			if (subtituloEs.Length > 120)
			{
				subtituloEs = subtituloEs.Remove(117, subtituloEs.Length - 117) + "...";
			}

			noticia.SteamEs.Subtitulo = subtituloEs;

			if (noticia.NoticiaTipo == NoticiaTipo.Bundles)
			{
				noticia.SteamEn.Contenido = await GenerarBundle(noticia.BundleId, "en", noticia.Id);
				noticia.SteamEs.Contenido = await GenerarBundle(noticia.BundleId, "es", noticia.Id);
			}
			else if (noticia.NoticiaTipo == NoticiaTipo.Gratis)
			{
				noticia.SteamEn.Contenido = await GenerarGratis(noticia.GratisIds, "en", noticia.Id);
				noticia.SteamEs.Contenido = await GenerarGratis(noticia.GratisIds, "es", noticia.Id);
			}
			else if (noticia.NoticiaTipo == NoticiaTipo.Suscripciones)
			{
				noticia.SteamEn.Contenido = await GenerarSuscripciones(noticia.SuscripcionesIds, "en", noticia.Id);
				noticia.SteamEs.Contenido = await GenerarSuscripciones(noticia.SuscripcionesIds, "es", noticia.Id);
			}
			else
			{
				noticia.SteamEn.Contenido = await GenerarMensaje(noticia.ContenidoEn, "en", noticia.Id);
				noticia.SteamEs.Contenido = await GenerarMensaje(noticia.ContenidoEs, "es", noticia.Id);
			}

			if (string.IsNullOrEmpty(noticia.Imagen) == false)
			{
				try
				{
					string imagenPostimage = await Herramientas.Ficheros.PostImage.SubirImagen(noticia.Imagen);

					if (string.IsNullOrEmpty(imagenPostimage) == false)
					{
						noticia.SteamEn.Contenido = "[p][img]" + imagenPostimage + "[/img][/p]" + noticia.SteamEn.Contenido;
						noticia.SteamEs.Contenido = "[p][img]" + imagenPostimage + "[/img][/p]" + noticia.SteamEs.Contenido;
					}
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Noticias Subir Imagen Steam", ex);
				}
			}

			return noticia;
		}

		private static async Task<string> GenerarBundle(int bundleId, string idioma, int noticiaId)
		{
			string contenido = string.Empty;
			Bundles2.Bundle bundle = await BaseDatos.Bundles.Buscar.UnBundle(bundleId);

			if (bundle?.Tiers?.Count > 0)
			{
				contenido = contenido + $@"[p][url=""https://pepe.deals/news/{noticiaId.ToString()}/"" style=""pill"" buttoncolor=""#293751""]{Herramientas.Idiomas.BuscarTexto(idioma, "Steam6", "NewsTemplates")}[/url][/p]";

				if (bundle.Pick == false)
				{
					contenido = contenido + "[list]";

					foreach (var tier in bundle.Tiers)
					{
						contenido = contenido + "[*][b]Tier " + tier.Posicion.ToString() + "[/b]: " + Herramientas.Precios.Euro(double.Parse(tier.Precio));
					}

					contenido = contenido + "[/list]";
				}
				else
				{
					contenido = contenido + "[list]";

					foreach (var tier in bundle.Tiers)
					{
						contenido = contenido + "[*][b]" + string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "Steam7", "NewsTemplates"), tier.CantidadJuegos.ToString()) + "[/b]: " + Herramientas.Precios.Euro(double.Parse(tier.Precio));
					}

					contenido = contenido + "[/list]";
				}

				if (bundle.Pick == false)
				{
					foreach (var tier in bundle.Tiers.OrderBy(b => b.Posicion))
					{
						contenido = contenido + $@"[p][table equalcells=""1"" colwidth=""185,450""][tr][th colwidth=""185""][p]Tier {tier.Posicion.ToString()}[/p][/th][th colwidth=""450""][p][/p][/th][/tr]";

						foreach (var juego in bundle.Juegos.OrderBy(b => b.Nombre))
						{
							if (juego.Juego == null)
							{
								juego.Juego = await BaseDatos.Juegos.Buscar.UnJuego(juego.JuegoId);
							}

							if (tier.Posicion == juego.Tier.Posicion)
							{
								string nombre = "[p]" + juego.Juego.Nombre + "[/p][p]" + juego.DRM.ToString() + "[/p]";
								string imagen = $@"[img src=""{juego.Juego.Imagenes.Header_460x215}""][/img]";

								contenido = contenido + $@"[tr][td colwidth=""185""]{imagen}[/td][td colwidth=""450""]{nombre}[/td][/tr]";
							}
						}

						contenido = contenido + "[/table][/p]";
					}
				}
				else
				{
					contenido = contenido + $@"[p][table equalcells=""1"" colwidth=""185,450""][tr][th colwidth=""185""][p]Tier 1[/p][/th][th colwidth=""450""][p][/p][/th][/tr]";

					foreach (var juego in bundle.Juegos.OrderBy(b => b.Nombre))
					{
						if (juego.Juego == null)
						{
							juego.Juego = await BaseDatos.Juegos.Buscar.UnJuego(juego.JuegoId);
						}

						string nombre = "[p]" + juego.Juego.Nombre + "[/p][p]" + juego.DRM.ToString() + "[/p]";
						string imagen = $@"[img src=""{juego.Juego.Imagenes.Header_460x215}""][/img]";

						contenido = contenido + $@"[tr][td colwidth=""185""]{imagen}[/td][td colwidth=""450""]{nombre}[/td][/tr]";
					}

					contenido = contenido + "[/table][/p]";
				}
			}

			return contenido;
		}

		private static async Task<string> GenerarGratis(string gratisIds, string idioma, int noticiaId)
		{
			List<string> gratisIds2 = Herramientas.Listados.Generar(gratisIds);
			string contenido = string.Empty;

			if (gratisIds2?.Count > 0)
			{
				contenido = contenido + $@"[p][url=""https://pepe.deals/news/{noticiaId.ToString()}/"" style=""pill"" buttoncolor=""#293751""]{Herramientas.Idiomas.BuscarTexto(idioma, "Steam8", "NewsTemplates")}[/url][/p]";
				contenido = contenido + $@"[p][table equalcells=""1"" colwidth=""185,450""][tr][th colwidth=""185""][/th][th colwidth=""450""][p][/p][/th][/tr]";

				foreach (var gratisId in gratisIds2)
				{
					JuegoGratis gratis = await BaseDatos.Gratis.Buscar.UnGratis(gratisId);

					if (gratis != null)
					{
						Juego juego = await BaseDatos.Juegos.Buscar.UnJuego(gratis.JuegoId);

						if (juego != null)
						{
							string nombre = "[p]" + juego.Nombre + "[/p]";
							string imagen = $@"[img src=""{juego.Imagenes.Header_460x215}""][/img]";

							contenido = contenido + $@"[tr][td colwidth=""185""]{imagen}[/td][td colwidth=""450""]{nombre}[/td][/tr]";
						}
					}
				}

				contenido = contenido + "[/table][/p]";
			}

			return contenido;
		}

		private static async Task<string> GenerarSuscripciones(string suscripcionesIds, string idioma, int noticiaId)
		{
			List<string> suscripcionesIds2 = Herramientas.Listados.Generar(suscripcionesIds);
			string contenido = string.Empty;

			if (suscripcionesIds2?.Count > 0)
			{
				contenido = contenido + $@"[p][url=""https://pepe.deals/news/{noticiaId.ToString()}/"" style=""pill"" buttoncolor=""#293751""]{Herramientas.Idiomas.BuscarTexto(idioma, "Steam9", "NewsTemplates")}[/url][/p]";
				contenido = contenido + $@"[p][table equalcells=""1"" colwidth=""185,450""][tr][th colwidth=""185""][/th][th colwidth=""450""][p][/p][/th][/tr]";

				foreach (var suscripcionId in suscripcionesIds2)
				{
					JuegoSuscripcion suscripcion = await BaseDatos.Suscripciones.Buscar.Id(int.Parse(suscripcionId));

					if (suscripcion != null)
					{
						Juego juego = await BaseDatos.Juegos.Buscar.UnJuego(suscripcion.JuegoId);

						if (juego != null)
						{
							string nombre = "[p]" + juego.Nombre + "[/p]";
							string imagen = $@"[img src=""{juego.Imagenes.Header_460x215}""][/img]";

							contenido = contenido + $@"[tr][td colwidth=""185""]{imagen}[/td][td colwidth=""450""]{nombre}[/td][/tr]";
						}
					}
				}

				contenido = contenido + "[/table][/p]";
			}

			return contenido;
		}

		private static async Task<string> GenerarMensaje(string contenidoHtml, string idioma, int noticiaId)
		{
			string contenido = string.Empty;

			contenido = contenido + $@"[p][url=""https://pepe.deals/news/{noticiaId.ToString()}/"" style=""pill"" buttoncolor=""#293751""]{Herramientas.Idiomas.BuscarTexto(idioma, "Steam10", "NewsTemplates")}[/url][/p]";

			contenidoHtml = Regex.Replace(contenidoHtml, @"<a[^>]*href=[""'](.*?)[""'][^>]*>(.*?)</a>", "[url=$1]$2[/url]", RegexOptions.IgnoreCase | RegexOptions.Singleline);

			contenidoHtml = Regex.Replace(contenidoHtml, @"<img[^>]*src=[""'](.*?)[""'][^>]*>", "[img]$1[/img]", RegexOptions.IgnoreCase | RegexOptions.Singleline);

			contenidoHtml = Regex.Replace(contenidoHtml, @"<ul[^>]*>(.*?)</ul>", "[list]$1[/list]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			contenidoHtml = Regex.Replace(contenidoHtml, @"<li[^>]*>(.*?)</li>", "[*]$1", RegexOptions.IgnoreCase | RegexOptions.Singleline);

			contenidoHtml = Regex.Replace(contenidoHtml, @"<p[^>]*>(.*?)</p>", "[p]$1[/p]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
						
			string anterior;
			do
			{
				anterior = contenidoHtml;
				contenidoHtml = Regex.Replace(
					contenidoHtml,
					@"<div[^>]*>((?:(?!<div)[\s\S])*?)</div>",
					"[p]$1[/p]", 
					RegexOptions.IgnoreCase
				);
			} while (contenidoHtml != anterior);


			contenidoHtml = contenidoHtml.Replace("\n", null);

			contenido = contenido + contenidoHtml;

			return contenido;
		}
	}

	public class PostImagesRespuesta
	{
		public string status { get; set; }
		public string url { get; set; }
	}
}
