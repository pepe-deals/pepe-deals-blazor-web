#nullable disable

using Microsoft.VisualBasic;
using Tiendas2;

namespace Herramientas
{
	public static class Buscador
	{
		public static string LimpiarNombre(string nombre, bool quitarEspacio = false)
		{
			if (string.IsNullOrEmpty(nombre) == false)
			{
				if (quitarEspacio == true)
				{
					nombre = nombre.Replace(" ", null);
				}

				nombre = nombre.Replace("&#39;", "'");
				nombre = nombre.Replace("&amp;", "&");
				nombre = nombre.Replace("&quot;", Strings.ChrW(34).ToString());

				List<string> caracteres = new List<string>
				{
					"!", "#", "$", "%", "&", "'", "(", ")", "*", "+", ",", "-", ".", "/",
					":", ";", "<", "=", ">", "?", "@", "[", "\\", "]", "^", "_", "`", "{", "|", "}", "~", "€", "‚", "ƒ", "„",
					"…", "†", "‡", "ˆ", "‰", "Š", "‹", "Œ", "Ž", "‘", "’", "“", "”", "•", "˜", "™", "š", "›", "œ", "ž", "Ÿ", "¡",
					"¢", "£", "¤", "¥", "¦", "§", "¨", "©", "ª", "«", "¬", "®", "¯", "°", "±", "²", "³", "´", "µ", "¶", "·", "¸",
					"¹", "º", "»", "¼", "½", "¾", "¿", "Æ", "Ç", "Ð", "×", "Ø", "Þ", "ß", "æ", "ç", "÷", "ø", "þ",
					Strings.ChrW(34).ToString(), Strings.ChrW(127).ToString(), Strings.ChrW(129).ToString(), Strings.ChrW(141).ToString(),
					Strings.ChrW(143).ToString(), Strings.ChrW(144).ToString(), Strings.ChrW(150).ToString(), Strings.ChrW(151).ToString(),
					Strings.ChrW(157).ToString(), Strings.ChrW(160).ToString(), Strings.ChrW(173).ToString(), Strings.ChrW(8211).ToString()
				};

				foreach (string caracter in caracteres)
				{
					nombre = nombre.Replace(caracter, null);
				}

				nombre = nombre.Replace("À", "A");
				nombre = nombre.Replace("Á", "A");
				nombre = nombre.Replace("Â", "A");
				nombre = nombre.Replace("Ã", "A");
				nombre = nombre.Replace("Ä", "A");
				nombre = nombre.Replace("Å", "A");
				nombre = nombre.Replace("à", "a");
				nombre = nombre.Replace("á", "a");
				nombre = nombre.Replace("â", "a");
				nombre = nombre.Replace("ã", "a");
				nombre = nombre.Replace("ä", "a");
				nombre = nombre.Replace("å", "a");

				nombre = nombre.Replace("È", "E");
				nombre = nombre.Replace("É", "E");
				nombre = nombre.Replace("Ê", "E");
				nombre = nombre.Replace("Ë", "E");
				nombre = nombre.Replace("è", "e");
				nombre = nombre.Replace("é", "e");
				nombre = nombre.Replace("ê", "e");
				nombre = nombre.Replace("ë", "e");

				nombre = nombre.Replace("Ì", "I");
				nombre = nombre.Replace("Í", "I");
				nombre = nombre.Replace("Î", "I");
				nombre = nombre.Replace("Ï", "I");
				nombre = nombre.Replace("ì", "i");
				nombre = nombre.Replace("í", "i");
				nombre = nombre.Replace("î", "i");
				nombre = nombre.Replace("ï", "i");

				nombre = nombre.Replace("Ò", "O");
				nombre = nombre.Replace("Ó", "O");
				nombre = nombre.Replace("Ô", "O");
				nombre = nombre.Replace("Õ", "O");
				nombre = nombre.Replace("Ö", "O");
				nombre = nombre.Replace("ð", "o");
				nombre = nombre.Replace("ò", "o");
				nombre = nombre.Replace("ó", "o");
				nombre = nombre.Replace("ô", "o");
				nombre = nombre.Replace("õ", "o");
				nombre = nombre.Replace("ö", "o");
				nombre = nombre.Replace("ō", "o");

				nombre = nombre.Replace("Ù", "U");
				nombre = nombre.Replace("Ú", "U");
				nombre = nombre.Replace("Û", "U");
				nombre = nombre.Replace("Ü", "U");
				nombre = nombre.Replace("ù", "u");
				nombre = nombre.Replace("ú", "u");
				nombre = nombre.Replace("û", "u");
				nombre = nombre.Replace("ü", "u");

				nombre = nombre.Replace("Ñ", "N");
				nombre = nombre.Replace("ñ", "n");

				nombre = nombre.Replace("Ý", "Y");
				nombre = nombre.Replace("ý", "y");
				nombre = nombre.Replace("ÿ", "y");

				nombre = nombre.ToLower();
				nombre = nombre.Trim();

				return nombre;
			}

			return nombre;
		}

		public static string GenerarMensaje(bool noOficiales, TiendaRegion region, string idioma, Juegos.Juego juego, bool buscarBundles, bool buscarGratis, bool buscarSuscripciones)
		{
			string mensaje = string.Empty;

			if (string.IsNullOrEmpty(juego.FreeToPlay) == false)
			{
				if (juego.FreeToPlay.ToLower() == "true")
				{
					return Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage6", "Header");
				}
			}

			if (buscarGratis == true)
			{
				if (juego.GratisActuales?.Count > 0)
				{
					foreach (var gratis in juego.GratisActuales)
					{
						return string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage5", "Header"), Gratis2.GratisCargar.DevolverGratis(gratis.gratis).Nombre);
					}
				}
				else
				{
					if (juego.Gratis?.Count > 0)
					{
						foreach (var gratis in juego.Gratis)
						{
							if (DateTime.Now >= gratis.FechaEmpieza && DateTime.Now <= gratis.FechaTermina)
							{
								return string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage5", "Header"), Gratis2.GratisCargar.DevolverGratis(gratis.Tipo).Nombre);
							}
						}
					}
				}
            }

			//--------------------------------------------------------------

			string mensajeComplementoTipo = string.Empty;
			string mensajeComplementoTexto = string.Empty;
			int mensajeComplementoCantidadBundles = 0;
			int mensajeComplementoCantidadSuscripciones = 0;

			if (buscarBundles == true)
			{
				if (juego.BundlesActuales?.Count > 0)
				{
					foreach (var bundle in juego.BundlesActuales)
					{
						mensajeComplementoTipo = "bundle";
						mensajeComplementoTexto = Bundles2.BundlesCargar.DevolverBundle(bundle.bundleTipo).Tienda;
						mensajeComplementoCantidadBundles = mensajeComplementoCantidadBundles + 1;
					}
				}
			}

			if (buscarSuscripciones == true)
			{
				if (juego.SuscripcionesActuales?.Count > 0)
				{
					foreach (var suscripcion in juego.SuscripcionesActuales)
					{
						mensajeComplementoTipo = "suscripcion";
						mensajeComplementoTexto = Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.suscripcion).Nombre;
						mensajeComplementoCantidadSuscripciones = mensajeComplementoCantidadSuscripciones + 1;
					}
				}
				else
				{
					if (juego.Suscripciones?.Count > 0)
					{
						foreach (var suscripcion in juego.Suscripciones)
						{
							if (DateTime.Now >= suscripcion.FechaEmpieza && DateTime.Now <= suscripcion.FechaTermina)
							{
								mensajeComplementoTipo = "suscripcion";
								mensajeComplementoTexto = Suscripciones2.SuscripcionesCargar.DevolverSuscripcion(suscripcion.Tipo).Nombre;
								mensajeComplementoCantidadSuscripciones = mensajeComplementoCantidadSuscripciones + 1;
							}
						}
					}
				}
            }

			decimal minimoCantidad = 10000000;
			Juegos.JuegoPrecio minimoFinal = new Juegos.JuegoPrecio();

			if (region == TiendaRegion.Europa)
			{
				List<Juegos.JuegoPrecio> preciosActualesEU = new List<Juegos.JuegoPrecio>();
				
				if (juego.PrecioActualesTiendas?.Count > 0)
				{
					preciosActualesEU.AddRange(juego.PrecioActualesTiendas);
				}

				if (noOficiales == true && juego.PreciosActualesNoOficialesEU?.Count > 0)
				{
					preciosActualesEU.AddRange(juego.PreciosActualesNoOficialesEU);
				}

				if (preciosActualesEU?.Count > 0)
				{
					foreach (var oferta in preciosActualesEU)
					{
						bool drmAdecuado = true;

						if (oferta.DRM == Juegos.JuegoDRM.NoEspecificado)
						{
							drmAdecuado = false;
						}
						else if (oferta.DRM == Juegos.JuegoDRM.Microsoft)
						{
							drmAdecuado = false;
						}

						if (drmAdecuado == true)
						{
							if (Herramientas.OfertaActiva.Verificar(oferta) == true)
							{
								if (oferta.Descuento > 0)
								{
									decimal tempPrecio = oferta.Precio;

									if (oferta.PrecioCambiado > 0)
									{
										tempPrecio = oferta.PrecioCambiado;
									}

									if (oferta.Moneda != Herramientas.JuegoMoneda.Euro && oferta.PrecioCambiado == 0)
									{
										tempPrecio = Herramientas.Divisas.CambioEuro(tempPrecio, oferta.Moneda);
									}

									if (tempPrecio < minimoCantidad)
									{
										minimoCantidad = tempPrecio;
									}
								}
							}
						}
					}
				}
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				List<Juegos.JuegoPrecio> preciosActualesUS = new List<Juegos.JuegoPrecio>();

				if (juego.PrecioActualesTiendasUS?.Count > 0)
				{
					preciosActualesUS.AddRange(juego.PrecioActualesTiendasUS);
				}

				if (noOficiales == true && juego.PreciosActualesNoOficialesUS?.Count > 0)
				{
					preciosActualesUS.AddRange(juego.PreciosActualesNoOficialesUS);
				}

				if (preciosActualesUS?.Count > 0)
				{
					foreach (var oferta in juego.PrecioActualesTiendasUS)
					{
						bool drmAdecuado = true;

						if (oferta.DRM == Juegos.JuegoDRM.NoEspecificado)
						{
							drmAdecuado = false;
						}
						else if (oferta.DRM == Juegos.JuegoDRM.Microsoft)
						{
							drmAdecuado = false;
						}

						if (drmAdecuado == true)
						{
							if (Herramientas.OfertaActiva.Verificar(oferta) == true)
							{
								if (oferta.Descuento > 0)
								{
									decimal tempPrecio = oferta.Precio;

									if (oferta.PrecioCambiado > 0)
									{
										tempPrecio = oferta.PrecioCambiado;
									}

									if (oferta.Moneda != Herramientas.JuegoMoneda.Dolar && oferta.PrecioCambiado == 0)
									{
										tempPrecio = Herramientas.Divisas.CambioDolar(tempPrecio, oferta.Moneda);
									}

									if (tempPrecio < minimoCantidad)
									{
										minimoCantidad = tempPrecio;
									}
								}
							}
						}
					}
				}
			}

			if (minimoCantidad > 0 && minimoCantidad < 10000000)
			{
				string minimoCantidadTexto = string.Empty;

				if (region == TiendaRegion.Europa)
				{
					minimoCantidadTexto = Herramientas.Precios.Euro(minimoCantidad);
				}
				else if (region == TiendaRegion.EstadosUnidos)
				{
					minimoCantidadTexto = Herramientas.Precios.Dolar(minimoCantidad);
				}

				if (string.IsNullOrEmpty(mensajeComplementoTipo) == false)
				{
					if (mensajeComplementoTipo == "bundle")
					{
						if (mensajeComplementoCantidadBundles == 1)
						{
							return string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage3", "Header"), minimoCantidadTexto, mensajeComplementoTexto);
						}
						else if (mensajeComplementoCantidadBundles > 1)
						{
							return string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage9", "Header"), minimoCantidadTexto, mensajeComplementoCantidadBundles);
						}
					}
					else if (mensajeComplementoTipo == "suscripcion")
					{
						if (mensajeComplementoCantidadSuscripciones == 1)
						{
							return string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage4", "Header"), minimoCantidadTexto, mensajeComplementoTexto);
						}
						else if (mensajeComplementoCantidadSuscripciones > 1)
						{
							return string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage10", "Header"), minimoCantidadTexto, mensajeComplementoCantidadSuscripciones);
						}
					}
				}
				else
				{
					return string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage1", "Header"), minimoCantidadTexto);
				}
			}

			if (string.IsNullOrEmpty(mensajeComplementoTipo) == false)
			{
				if (mensajeComplementoTipo == "bundle")
				{
					if (mensajeComplementoCantidadBundles == 1)
					{
						return string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage7", "Header"), mensajeComplementoTexto);
					}
					else if (mensajeComplementoCantidadBundles > 1)
					{
						return string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage11", "Header"), mensajeComplementoCantidadBundles);
					}
				}
				else if (mensajeComplementoTipo == "suscripcion")
				{
					if (mensajeComplementoCantidadSuscripciones == 1)
					{
						return string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage8", "Header"), mensajeComplementoTexto);
					}
					else if (mensajeComplementoCantidadSuscripciones > 1)
					{
						return string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage12", "Header"), mensajeComplementoCantidadSuscripciones);
					}
				}
			}
				
			return Herramientas.Idiomas.BuscarTexto(idioma, "SearchMessage2", "Header");
		}
	}
}
