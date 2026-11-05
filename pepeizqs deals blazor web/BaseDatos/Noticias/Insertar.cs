#nullable disable

using Dapper;
using Noticias;
using System.Net;

namespace BaseDatos.Noticias
{
	public static class Insertar
	{
		public static async Task<int> Ejecutar(Noticia noticia)
		{
			List<string> campos = new List<string>();
			List<string> valores = new List<string>();
			DynamicParameters p = new DynamicParameters();

			campos.Add("noticiaTipo");
			valores.Add("@noticiaTipo");
			p.Add("@noticiaTipo", noticia.NoticiaTipo);

			campos.Add("fechaEmpieza");
			valores.Add("@fechaEmpieza");
			p.Add("@fechaEmpieza", noticia.FechaEmpieza);

			campos.Add("fechaTermina");
			valores.Add("@fechaTermina");
			p.Add("@fechaTermina", noticia.FechaTermina);

			campos.Add("tituloEn");
			valores.Add("@tituloEn");
			p.Add("@tituloEn", WebUtility.HtmlDecode(noticia.TituloEn));

			campos.Add("tituloEs");
			valores.Add("@tituloEs");
			p.Add("@tituloEs", WebUtility.HtmlDecode(noticia.TituloEs));

			campos.Add("contenidoEn");
			valores.Add("@contenidoEn");
			p.Add("@contenidoEn", noticia.ContenidoEn);

			campos.Add("contenidoEs");
			valores.Add("@contenidoEs");
			p.Add("@contenidoEs", noticia.ContenidoEs);

			if (!string.IsNullOrEmpty(noticia.Imagen))
			{
				campos.Add("imagen");
				valores.Add("@imagen");
				p.Add("@imagen", noticia.Imagen);
			}

			if (!string.IsNullOrEmpty(noticia.Enlace))
			{
				campos.Add("enlace");
				valores.Add("@enlace");
				p.Add("@enlace", noticia.Enlace);
			}

			if (!string.IsNullOrEmpty(noticia.Juegos))
			{
				campos.Add("juegos");
				valores.Add("@juegos");
				p.Add("@juegos", noticia.Juegos);
			}

			if (noticia.NoticiaTipo == NoticiaTipo.Bundles)
			{
				campos.Add("bundleTipo");
				valores.Add("@bundleTipo");
				p.Add("@bundleTipo", noticia.BundleTipo);
			}

			if (noticia.NoticiaTipo == NoticiaTipo.Gratis)
			{
				campos.Add("gratisTipo");
				valores.Add("@gratisTipo");
				p.Add("@gratisTipo", noticia.GratisTipo);
			}

			if (noticia.NoticiaTipo == NoticiaTipo.Suscripciones)
			{
				campos.Add("suscripcionTipo");
				valores.Add("@suscripcionTipo");
				p.Add("@suscripcionTipo", noticia.SuscripcionTipo);
			}

			if (noticia.BundleId > 0)
			{
				campos.Add("bundleId");
				valores.Add("@bundleId");
				p.Add("@bundleId", noticia.BundleId);
			}

			if (!string.IsNullOrEmpty(noticia.GratisIds))
			{
				campos.Add("gratisIds");
				valores.Add("@gratisIds");
				p.Add("@gratisIds", noticia.GratisIds);
			}

			if (!string.IsNullOrEmpty(noticia.SuscripcionesIds))
			{
				campos.Add("suscripcionesIds");
				valores.Add("@suscripcionesIds");
				p.Add("@suscripcionesIds", noticia.SuscripcionesIds);
			}

			string sql = $@"INSERT INTO noticias ({string.Join(",", campos)})
                VALUES ({string.Join(",", valores)});
                SELECT SCOPE_IDENTITY();";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.ExecuteScalarAsync<int>(sql, p);
				});
			}
			catch (Exception ex)
			{
				Errores.Insertar.Mensaje("Noticias Insertar", ex);
				return 0;
			}
		}
	}
}
