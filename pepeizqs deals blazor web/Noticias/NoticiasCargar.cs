#nullable disable

using Bundles2;

namespace Noticias
{
	public enum NoticiaTipo
	{
		Ofertas,
		Bundles,
		Gratis,
		Suscripciones,
		Eventos,
		Sorteos,
		Rumores,
		Otros,
		Web,
		Patreon,
		Despedidas,
		Desconocido
	}

	public static class NoticiasCargar
	{
        public static List<NoticiaMostrar> CargarNoticiasMostrar()
		{
			List<NoticiaMostrar> noticias = new List<NoticiaMostrar>();

            NoticiaMostrar bundles = new NoticiaMostrar
            {
                Tipo = NoticiaTipo.Bundles,
                Mostrar = true
            };

            noticias.Add(bundles);

            NoticiaMostrar gratis = new NoticiaMostrar
            {
                Tipo = NoticiaTipo.Gratis,
                Mostrar = true
            };

            noticias.Add(gratis);

            NoticiaMostrar suscripciones = new NoticiaMostrar
            {
                Tipo = NoticiaTipo.Suscripciones,
                Mostrar = true
            };

            noticias.Add(suscripciones);

            NoticiaMostrar rumores = new NoticiaMostrar
            {
                Tipo = NoticiaTipo.Rumores,
                Mostrar = false
            };

            noticias.Add(rumores);

            NoticiaMostrar web = new NoticiaMostrar
            {
                Tipo = NoticiaTipo.Web,
                Mostrar = true
            };

            noticias.Add(web);

			NoticiaMostrar patreon = new NoticiaMostrar
			{
				Tipo = NoticiaTipo.Patreon,
				Mostrar = false
			};

			noticias.Add(patreon);

			NoticiaMostrar despedidas = new NoticiaMostrar
			{
				Tipo = NoticiaTipo.Despedidas,
				Mostrar = false
			};

			noticias.Add(despedidas);

			NoticiaMostrar otros = new NoticiaMostrar
			{
				Tipo = NoticiaTipo.Otros,
				Mostrar = true
			};

			noticias.Add(otros);

			return noticias;
		}

		public static List<NoticiaTipo> CargarNoticiasTipo()
		{
			List<NoticiaTipo> tipos = Enum.GetValues(typeof(NoticiaTipo)).Cast<NoticiaTipo>().ToList();

			return tipos;
		}

		public static string Traduccion(NoticiaTipo tipo, string idioma)
		{
			List<NoticiaTipo> tipos = CargarNoticiasTipo();

			int posicion = 1;
			foreach (var tipo2 in tipos) 
			{ 
				if (tipo2 == tipo)
				{
					return Herramientas.Idiomas.BuscarTexto(idioma, "Type" + posicion.ToString(), "News");
				}

				posicion += 1;
			}

			return null;
		}

		public static string Traduccion(string tipo, string idioma)
		{
			List<NoticiaTipo> tipos = CargarNoticiasTipo();

			int posicion = 1;
			foreach (var tipo2 in tipos)
			{
				if (Herramientas.Idiomas.BuscarTexto(idioma, "Type" + posicion.ToString(), "News") == tipo)
				{
					return Herramientas.Idiomas.BuscarTexto("es", "Type" + posicion.ToString(), "News");
				}

				posicion += 1;
			}

			return null;
		}
	}
}
