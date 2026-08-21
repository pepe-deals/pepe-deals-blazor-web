#nullable disable

using Herramientas;
using Juegos;
using Microsoft.VisualBasic;
using System.Net;
using Tiendas2;

namespace APIs.Noctre
{
	public static class Tienda
	{
		public static Tiendas2.Tienda Generar()
		{
			Tiendas2.Tienda tienda = new Tiendas2.Tienda
			{
				Id = "noctre",
				Nombre = "Noctre",
				Tipo = TiendaTipo.Oficial,
				ImagenLogo = "/imagenes/tiendas/noctre_logo.webp",
				Imagen300x80 = "/imagenes/tiendas/noctre_300x80.webp",
				ImagenIcono = "/imagenes/tiendas/noctre_icono.webp",
				Color = "#007aff",
				AdminUso = false,
				UsuarioUso = false
			};

			return tienda;
		}

		public static async Task BuscarOfertas(TiendaRegion region)
		{
			await BaseDatos.Admin.Actualizar.Tiendas(region, Generar().Id, DateTime.Now, 0);
			int juegos2 = 0;

			int i = 1;
			while (i < 100)
			{
				string html = await Decompiladores.Estandar("https://www.noctre.com/specials?page=" + i.ToString());

				if (string.IsNullOrEmpty(html) == false)
				{
					if (html.Contains(Strings.ChrW(34) + "card card-block" + Strings.ChrW(34)) == false)
					{
						break;
					}

					int j = 0;
					while (j < 24)
					{
						if (html.Contains(Strings.ChrW(34) + "card card-block" + Strings.ChrW(34)) == true)
						{
							int int1 = html.IndexOf(Strings.ChrW(34) + "card card-block" + Strings.ChrW(34));
							string temp1 = html.Remove(0, int1 + 5);

							html = temp1;

							int int2 = temp1.IndexOf("</a>");
							string temp2 = temp1.Remove(int2, temp1.Length - int2);

							decimal precioBase = 0;

							if (temp2.Contains("<s>€") == true)
							{
								int int3 = temp2.IndexOf("<s>€");
								string temp3 = temp2.Remove(0, int3 + 4);

								int int4 = temp3.IndexOf("</s>");
								string temp4 = temp3.Remove(int4, temp3.Length - int4);

								precioBase = decimal.Parse(temp4);
							}

							decimal precioRebajado = 0;

							if (temp2.Contains(Strings.ChrW(34) + ">€") == true)
							{
								int int3 = temp2.IndexOf(Strings.ChrW(34) + ">€");
								string temp3 = temp2.Remove(0, int3 + 3);

								int int4 = temp3.IndexOf("</div>");
								string temp4 = temp3.Remove(int4, temp3.Length - int4);

								precioRebajado = decimal.Parse(temp4);
							}

							if (precioBase > 0 && precioRebajado > 0)
							{
								int descuento = Calculadora.SacarDescuento(precioBase, precioRebajado);

								if (descuento > 0)
								{
									string nombre = string.Empty;

									if (temp2.Contains("alt=") == true)
									{
										int int3 = temp2.IndexOf("alt=");
										string temp3 = temp2.Remove(0, int3 + 5);

										int int4 = temp3.IndexOf(Strings.ChrW(34));
										string temp4 = temp3.Remove(int4, temp3.Length - int4);

										nombre = temp4;
									}

									string imagen = string.Empty;

									if (temp2.Contains("<img data-src=") == true)
									{
										int int3 = temp2.IndexOf("<img data-src=");
										string temp3 = temp2.Remove(0, int3 + 15);

										int int4 = temp3.IndexOf(Strings.ChrW(34));
										string temp4 = temp3.Remove(int4, temp3.Length - int4);

										imagen = "https://www.noctre.com" + temp4;
									}

									string enlace = string.Empty;

									if (temp2.Contains("<a href=") == true)
									{
										int int3 = temp2.IndexOf("<a href=");
										string temp3 = temp2.Remove(0, int3 + 9);

										int int4 = temp3.IndexOf(Strings.ChrW(34));
										string temp4 = temp3.Remove(int4, temp3.Length - int4);

										enlace = "https://www.noctre.com" + temp4;
									}

									Juegos.JuegoDRM drm = Juegos.JuegoDRM.NoEspecificado;

									if (temp2.Contains("/img/platform/steam.svg") == true)
									{
										drm = Juegos.JuegoDRM.Steam;
									}
									else if (temp2.Contains("/img/platform/epic-games.svg") == true)
									{
										drm = Juegos.JuegoDRM.Epic;
									}

									if (ComprobarRegionValida(enlace) == true)
									{
										JuegoPrecio oferta = new JuegoPrecio
										{
											Nombre = WebUtility.HtmlDecode(nombre),
											Enlace = enlace,
											Imagen = imagen,
											Moneda = JuegoMoneda.Euro,
											Precio = precioRebajado,
											Descuento = descuento,
											Tienda = Generar().Id,
											DRM = drm,
											FechaDetectado = DateTime.Now,
											FechaActualizacion = DateTime.Now
										};

										try
										{
											await BaseDatos.Tiendas.Comprobar.Resto(region, oferta);
										}
										catch (Exception ex)
										{
											BaseDatos.Errores.Insertar.Mensaje(Generar().Id, ex);
										}

										juegos2 += 1;

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

						j += 1;
					}
				}

				i += 1;
			}
		}

		public static bool ComprobarRegionValida(string enlace)
		{
			if (enlace.Contains("/SMSLS6498/") ||
				enlace.Contains("/SMSLS6497/") ||
				enlace.Contains("/SMSLS6931/") ||
				enlace.Contains("/SMSLS6493/") ||
				enlace.Contains("/SMJVP7132/"))
			{
				return false;
			}

			return true;
		}
	}
}
