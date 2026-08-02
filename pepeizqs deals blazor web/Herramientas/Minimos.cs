#nullable disable

using Juegos;
using Tiendas2;

namespace Herramientas
{
	public static class Minimos
	{
		public static List<JuegoPrecio> OrdenarPrecios(TiendaRegion region, List<JuegoPrecio> precios, JuegoDRM drm = JuegoDRM.NoEspecificado, bool verificarActivo = true)
		{
			List<JuegoPrecio> preciosOrdenados = new List<JuegoPrecio>();

			if (precios?.Count > 0)
			{
				foreach (JuegoPrecio precio in precios)
				{
					bool drmCorrecto = false;

					if (drm != JuegoDRM.NoEspecificado)
					{
						if (precio.DRM == drm)
						{
							drmCorrecto = true;
						}
					}
					else
					{
						drmCorrecto = true;
					}

					if (drmCorrecto == true && precio.Descuento > 0)
					{
						bool verificarActivo2 = true;

						if (verificarActivo == true)
						{
							verificarActivo2 = Herramientas.OfertaActiva.Verificar(precio);
						}

						if (verificarActivo2 == true)
						{
							JuegoPrecio nuevoPrecio = precio;

							if (nuevoPrecio != null)
							{
								if (region == TiendaRegion.Europa && nuevoPrecio.Moneda != Herramientas.JuegoMoneda.Euro && nuevoPrecio.PrecioCambiado == 0)
								{
									nuevoPrecio.PrecioCambiado = Herramientas.Divisas.CambioEuro(nuevoPrecio.Precio, nuevoPrecio.Moneda);
								}
								else if (region == TiendaRegion.EstadosUnidos && nuevoPrecio.Moneda != Herramientas.JuegoMoneda.Dolar && nuevoPrecio.PrecioCambiado == 0)
								{
									nuevoPrecio.PrecioCambiado = Herramientas.Divisas.CambioDolar(nuevoPrecio.Precio, nuevoPrecio.Moneda);
								}
							}

							bool verificacionFinal = true;

							if (preciosOrdenados.Count > 0)
							{
								foreach (var ordenado in preciosOrdenados)
								{
									if (ordenado.Enlace == nuevoPrecio.Enlace && ordenado.Tienda == nuevoPrecio.Tienda && ordenado.DRM == nuevoPrecio.DRM)
									{
										verificacionFinal = false;
										break;
									}
								}
							}

							if (verificacionFinal == true)
							{
								preciosOrdenados.Add(nuevoPrecio);
							}
						}
					}
				}
			}

			if (preciosOrdenados.Count > 0)
			{
				preciosOrdenados.Sort(delegate (JuegoPrecio p1, JuegoPrecio p2)
				{
					if (region == TiendaRegion.Europa)
					{
						decimal precio1 = 0;

						if (region == TiendaRegion.Europa && p1.Moneda != Herramientas.JuegoMoneda.Euro)
						{
							precio1 = p1.PrecioCambiado;
						}
						else
						{
							precio1 = p1.Precio;
						}

						decimal precio2 = 0;

						if (region == TiendaRegion.Europa && p2.Moneda != Herramientas.JuegoMoneda.Euro)
						{
							precio2 = p2.PrecioCambiado;
						}
						else
						{
							precio2 = p2.Precio;
						}

						if (precio1 == precio2)
						{
							return p2.FechaDetectado.CompareTo(p1.FechaDetectado);
						}
						else
						{
							return precio1.CompareTo(precio2);
						}
					}
					else if (region == TiendaRegion.EstadosUnidos)
					{
						decimal precio1 = 0;

						if (region == TiendaRegion.EstadosUnidos && p1.Moneda != Herramientas.JuegoMoneda.Dolar)
						{
							precio1 = p1.PrecioCambiado;
						}
						else
						{
							precio1 = p1.Precio;
						}

						decimal precio2 = 0;

						if (region == TiendaRegion.EstadosUnidos && p2.Moneda != Herramientas.JuegoMoneda.Dolar)
						{
							precio2 = p2.PrecioCambiado;
						}
						else
						{
							precio2 = p2.Precio;
						}

						if (precio1 == precio2)
						{
							return p2.FechaDetectado.CompareTo(p1.FechaDetectado);
						}
						else
						{
							return precio1.CompareTo(precio2);
						}
					}
					else
					{
						return p2.FechaDetectado.CompareTo(p1.FechaDetectado);
					}
				});
			}

			return preciosOrdenados;
		}

		public static string CogerMinimoDRM(TiendaRegion region, string idioma, JuegoDRM drm,
			int juegoId, string juegoNombre, List<JuegoPrecio> historicos, List<JuegoPrecio> actuales, List<JuegoHistorico> listaHistoricos)
		{
			string drmPreparado = null;
			List<JuegoPrecio> historicosDRM = new List<JuegoPrecio>();

			if (historicos?.Count > 0)
			{
				foreach (JuegoPrecio historico in historicos)
				{
					if (historico.DRM == drm)
					{
						historicosDRM.Add(historico);
					}
				}
			}

			if (historicosDRM.Count == 0)
			{
				if (actuales?.Count > 0)
				{
					historicosDRM = OrdenarPrecios(region, actuales, drm, true);
				}
			}
			else
			{
				historicosDRM = OrdenarPrecios(region, historicosDRM, drm, false);
			}

			if (historicosDRM.Count > 0)
			{
				if (historicosDRM[0] != null)
				{
					List<Tienda> tiendas = TiendasCargar.GenerarListado();
					string tiendaFinal = string.Empty;

					foreach (var tienda in tiendas)
					{
						if (tienda.Id == historicosDRM[0].Tienda)
						{
							tiendaFinal = tienda.Nombre;
							break;
						}
					}

					bool existeEnActuales = ExisteEnActuales(historicosDRM[0], actuales, drm);

					if (existeEnActuales && OfertaActiva.Verificar(historicosDRM[0]) == true && historicosDRM[0].Descuento > 0)
					{
						drmPreparado = drmPreparado + " " + string.Format(Idiomas.BuscarTexto(idioma, "String11", "Game"), tiendaFinal);
					}
					else
					{
						string mensaje = string.Empty;

						if (listaHistoricos?.Count > 0)
						{
							bool generarMensaje = false;
							JuegoHistorico ultimo = new JuegoHistorico();
							ultimo.Fecha = DateTime.Now - TimeSpan.FromDays(900);

							foreach (var historico in listaHistoricos)
							{
								if (historico.DRM == drm)
								{
									if (ultimo.Fecha < historico.Fecha)
									{
										ultimo = historico;
										generarMensaje = true;
									}
								}
							}

							if (generarMensaje == true)
							{
								mensaje = " " + string.Format(Idiomas.BuscarTexto(idioma, "String13", "Game"), Calculadora.DiferenciaTiempo(ultimo.Fecha, idioma), tiendaFinal);
							}
						}

						if (string.IsNullOrEmpty(mensaje) == true)
						{
							mensaje = " " + string.Format(Idiomas.BuscarTexto(idioma, "String13", "Game"), Calculadora.DiferenciaTiempo(historicosDRM[0].FechaDetectado, idioma), tiendaFinal);
						}

						drmPreparado = drmPreparado + mensaje;
					}

					if (listaHistoricos?.Count > 0)
					{
						int veces = ContarVecesHistorico(historicosDRM[0], listaHistoricos, drm, region);

						if (veces > 0)
						{
							if (veces == 1)
							{
								drmPreparado = drmPreparado + " (" +
									string.Format(Idiomas.BuscarTexto(idioma, "String85", "Game"),
									veces.ToString()) + ")";
							}
							else
							{
								drmPreparado = drmPreparado + " (" +
									string.Format(Idiomas.BuscarTexto(idioma, "String86", "Game"),
									veces.ToString()) + ")";
							}
						}
					}
				}
			}

			return drmPreparado;
		}

		public static bool ExisteEnActuales(JuegoPrecio historico, List<JuegoPrecio> actuales, JuegoDRM drm)
		{
			if (actuales?.Count == 0)
			{
				return false;
			}

			return actuales.Any(a =>
				a.Tienda == historico.Tienda && a.DRM == drm &&
				(a.Precio == historico.Precio || (historico.PrecioCambiado > 0 && a.PrecioCambiado == historico.PrecioCambiado)));
		}

		private static int ContarVecesHistorico(JuegoPrecio historico, List<JuegoHistorico> listaHistoricos, JuegoDRM drm, TiendaRegion region)
		{
			if (listaHistoricos?.Count == 0)
			{ 
				return 0;
			}

			int contador = 0;

			foreach (var historico2 in listaHistoricos)
			{
				if (historico2.DRM == drm)
				{
					bool precioCoincide = false;

					if (region == TiendaRegion.Europa)
					{
						precioCoincide = (historico2.Precio == historico.Precio && historico.Moneda == JuegoMoneda.Euro) ||
										 (historico2.Precio == historico.PrecioCambiado && historico.PrecioCambiado > 0 && historico.Moneda != JuegoMoneda.Euro);
					}
					else if (region == TiendaRegion.EstadosUnidos)
					{
						precioCoincide = (historico2.Precio == historico.Precio && historico.Moneda == JuegoMoneda.Dolar) ||
										 (historico2.Precio == historico.PrecioCambiado && historico.PrecioCambiado > 0 && historico.Moneda != JuegoMoneda.Dolar);
					}

					if (precioCoincide == true)
					{
						contador += 1;
					}
				}
			}

			return contador;
		}
	}
}
