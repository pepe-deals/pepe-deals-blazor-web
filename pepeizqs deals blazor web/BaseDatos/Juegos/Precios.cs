#nullable disable

using BaseDatos.Usuarios;
using Dapper;
using Herramientas;
using Juegos;
using System.Text;
using System.Text.Json;
using Tiendas2;

namespace BaseDatos.Juegos
{
	public static class Precios
	{
		public static async Task<(string sql, DynamicParameters parametros)> Comprobacion(TiendaTipo tiendaTipo, int id, int idSteam, List<JuegoPrecio> ofertasActuales, List<JuegoPrecio> ofertasHistoricas, List<JuegoHistorico> historicos = null, JuegoPrecio nuevaOferta = null,
			string slugGOG = null, string idGOG = null, string slugEpic = null, JuegoAnalisis reseñas = null, int indice = 0)
		{
			bool añadidoEnHistoricos = true;
			bool ultimaModificacion = false;
			bool añadir = true;
			bool huboCambioReal = false;

			#region Aplicar Codigo Descuento

			decimal RedondearHaciaAbajo(decimal valor)
			{
				return Math.Floor(valor * 100) / 100;
			}

			if (nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado == 0)
			{
				nuevaOferta.PrecioCambiado = Herramientas.Divisas.CambioEuro(nuevaOferta.Precio, nuevaOferta.Moneda);

				if (string.IsNullOrEmpty(nuevaOferta.CodigoTexto) == false && nuevaOferta.CodigoDescuento > 0)
				{
					decimal descuento = (decimal)nuevaOferta.CodigoDescuento / 100;
					nuevaOferta.PrecioCambiado = nuevaOferta.PrecioCambiado - (nuevaOferta.PrecioCambiado * descuento);
					nuevaOferta.PrecioCambiado = RedondearHaciaAbajo(nuevaOferta.PrecioCambiado);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(nuevaOferta.CodigoTexto) == false && nuevaOferta.CodigoDescuento > 0)
				{
					decimal descuento = (decimal)nuevaOferta.CodigoDescuento / 100;
					nuevaOferta.Precio = nuevaOferta.Precio - (nuevaOferta.Precio * descuento);
					nuevaOferta.Precio = RedondearHaciaAbajo(nuevaOferta.Precio);
				}
			}

			#endregion

			if (ofertasActuales?.Count > 0)
			{
				foreach (JuegoPrecio precio in ofertasActuales)
				{
					if (nuevaOferta.Enlace == precio.Enlace &&
						nuevaOferta.DRM == precio.DRM &&
						nuevaOferta.Tienda == precio.Tienda)
					{
						bool cambiarFechaDetectado = false;

						if (nuevaOferta.Moneda == JuegoMoneda.Euro && precio.Moneda == JuegoMoneda.Euro && (nuevaOferta.Precio < precio.Precio || nuevaOferta.Precio > precio.Precio))
						{
							cambiarFechaDetectado = true;
						}
						else if (nuevaOferta.Moneda != JuegoMoneda.Euro && precio.Moneda == JuegoMoneda.Euro && (nuevaOferta.PrecioCambiado < precio.Precio || nuevaOferta.PrecioCambiado > precio.Precio))
						{
							cambiarFechaDetectado = true;
						}
						else if (nuevaOferta.Moneda == JuegoMoneda.Euro && precio.Moneda != JuegoMoneda.Euro && (nuevaOferta.Precio < precio.PrecioCambiado || nuevaOferta.Precio > precio.PrecioCambiado))
						{
							cambiarFechaDetectado = true;
						}
						else if (nuevaOferta.Moneda != JuegoMoneda.Euro && precio.Moneda != JuegoMoneda.Euro && (nuevaOferta.PrecioCambiado < precio.PrecioCambiado || nuevaOferta.PrecioCambiado > precio.PrecioCambiado))
						{
							cambiarFechaDetectado = true;
						}

						DateTime tempFecha = precio.FechaDetectado;
						tempFecha = tempFecha.AddDays(21);

						if (tempFecha < nuevaOferta.FechaDetectado)
						{
							cambiarFechaDetectado = true;
						}

						if (cambiarFechaDetectado == true)
						{
							precio.FechaDetectado = nuevaOferta.FechaDetectado;
						}

						if (nuevaOferta.FechaTermina == default(DateTime))
						{
							huboCambioReal = true;
						}
						else if (precio.FechaTermina != nuevaOferta.FechaTermina ||
							precio.Descuento != nuevaOferta.Descuento ||
							precio.CodigoDescuento != nuevaOferta.CodigoDescuento ||
							precio.CodigoTexto != nuevaOferta.CodigoTexto ||
							precio.Moneda != nuevaOferta.Moneda ||
							(nuevaOferta.Moneda == JuegoMoneda.Euro && precio.Moneda == JuegoMoneda.Euro && precio.Precio != nuevaOferta.Precio) ||
							(nuevaOferta.Moneda != JuegoMoneda.Euro && precio.Moneda != JuegoMoneda.Euro && precio.PrecioCambiado != nuevaOferta.PrecioCambiado) ||
							(nuevaOferta.Moneda == JuegoMoneda.Euro && precio.Moneda != JuegoMoneda.Euro && precio.PrecioCambiado != nuevaOferta.Precio) ||
							(nuevaOferta.Moneda != JuegoMoneda.Euro && precio.Moneda == JuegoMoneda.Euro && precio.Precio != nuevaOferta.PrecioCambiado))
						{
							huboCambioReal = true;
						}

						if (nuevaOferta.Moneda != JuegoMoneda.Euro)
						{
							precio.PrecioCambiado = nuevaOferta.PrecioCambiado;
						}
						else
						{
							precio.PrecioCambiado = 0;
						}

						precio.Precio = nuevaOferta.Precio;
						precio.Descuento = nuevaOferta.Descuento;
						precio.FechaActualizacion = nuevaOferta.FechaActualizacion;

						if (nuevaOferta.FechaTermina != default(DateTime))
						{
							precio.FechaTermina = nuevaOferta.FechaTermina;
						}

						precio.CodigoDescuento = nuevaOferta.CodigoDescuento;
						precio.CodigoTexto = nuevaOferta.CodigoTexto;
						precio.Nombre = nuevaOferta.Nombre;
						precio.Imagen = nuevaOferta.Imagen;
						precio.Moneda = nuevaOferta.Moneda;
						precio.BundleSteam = nuevaOferta.BundleSteam;

						añadir = false;
						break;
					}
				}
			}
			else
			{
				ofertasActuales = new List<JuegoPrecio>();
			}

			if (añadir == true)
			{
				ofertasActuales.Add(nuevaOferta);

				huboCambioReal = true;
			}

			if (ofertasHistoricas?.Count > 0)
			{
				bool drmEncontrado = false;

				foreach (JuegoPrecio minimo in ofertasHistoricas)
				{
					if (nuevaOferta.DRM == minimo.DRM)
					{
						drmEncontrado = true;

						if (minimo.Moneda != JuegoMoneda.Euro && minimo.Precio > 0 && minimo.PrecioCambiado == 0)
						{
							minimo.PrecioCambiado = minimo.Precio;
						}

						if ((minimo.Moneda == JuegoMoneda.Euro && nuevaOferta.Moneda == JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.Precio > 0 && nuevaOferta.Precio < minimo.Precio) ||
							(minimo.Moneda != JuegoMoneda.Euro && nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.PrecioCambiado < minimo.PrecioCambiado) ||
							(minimo.Moneda == JuegoMoneda.Euro && nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && nuevaOferta.PrecioCambiado < minimo.Precio) ||
							(minimo.Moneda != JuegoMoneda.Euro && nuevaOferta.Moneda == JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.Precio < minimo.PrecioCambiado))
						{
							historicos = ComprobarHistoricos(TiendaRegion.Europa, historicos, nuevaOferta);

							bool notificar = false;

							if ((minimo.Moneda == JuegoMoneda.Euro && nuevaOferta.Moneda == JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.Precio > 0 && nuevaOferta.Precio + 0.1m < minimo.Precio) ||
								(minimo.Moneda != JuegoMoneda.Euro && nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.PrecioCambiado + 0.1m < minimo.PrecioCambiado) ||
								(minimo.Moneda == JuegoMoneda.Euro && nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && nuevaOferta.PrecioCambiado + 0.1m < minimo.Precio) ||
								(minimo.Moneda != JuegoMoneda.Euro && nuevaOferta.Moneda == JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.Precio + 0.1m < minimo.PrecioCambiado))
							{
								notificar = true;
							}

							ultimaModificacion = true;

							if (nuevaOferta.Moneda != JuegoMoneda.Euro)
							{
								minimo.PrecioCambiado = nuevaOferta.PrecioCambiado;
							}
							else
							{
								minimo.PrecioCambiado = 0;
							}

							minimo.Precio = nuevaOferta.Precio;
							minimo.Moneda = nuevaOferta.Moneda;
							minimo.Descuento = nuevaOferta.Descuento;
							minimo.FechaDetectado = nuevaOferta.FechaDetectado;
							minimo.FechaActualizacion = nuevaOferta.FechaActualizacion;
							minimo.FechaTermina = nuevaOferta.FechaTermina;
							minimo.CodigoDescuento = nuevaOferta.CodigoDescuento;
							minimo.CodigoTexto = nuevaOferta.CodigoTexto;
							minimo.Nombre = nuevaOferta.Nombre;
							minimo.Imagen = nuevaOferta.Imagen;
							minimo.Enlace = nuevaOferta.Enlace;
							minimo.Tienda = nuevaOferta.Tienda;
							minimo.BundleSteam = nuevaOferta.BundleSteam;

							//------------------------------------------

							if (notificar == true && tiendaTipo == TiendaTipo.Oficial)
							{
								List<string> usuariosInteresados = await BaseDatos.Usuarios.Buscar.ListaUsuariosTienenDeseado(id, nuevaOferta.DRM);

								if (usuariosInteresados?.Count > 0)
								{
									foreach (var usuarioInteresado in usuariosInteresados)
									{
										if (await Usuarios.Buscar.UsuarioTieneJuego(usuarioInteresado, id, nuevaOferta.DRM) == false)
										{
											try
											{
												DeseadosDatos datosDeseados = null;

												string datosDeseadosTexto = await BaseDatos.Usuarios.Buscar.OpcionStringRegion(TiendaRegion.Europa, usuarioInteresado, "WishlistData");

												if (string.IsNullOrEmpty(datosDeseadosTexto) == true)
												{
													datosDeseados = new DeseadosDatos();
												}
												else
												{
													try
													{
														datosDeseados = JsonSerializer.Deserialize<DeseadosDatos>(datosDeseadosTexto);
													}
													catch
													{
														datosDeseados = new DeseadosDatos();
													}

													datosDeseados.Cantidad = datosDeseados.Cantidad + 1;
													datosDeseados.UltimoJuego = DateTime.Now;

													if (datosDeseados.Juegos == null)
													{
														datosDeseados.Juegos = new List<DeseadosDatosJuego>();
													}

													datosDeseados.Juegos.Add(new DeseadosDatosJuego
													{
														Id = id,
														Enlace = minimo.Enlace,
														Descuento = minimo.Descuento.ToString(),
														Precio = Herramientas.Precios.Euro(minimo.PrecioCambiado > 0 ? minimo.PrecioCambiado : minimo.Precio),
														Tienda = minimo.Tienda,
														DRM = minimo.DRM,
														Fecha = DateTime.Now
													});
												}

												await BaseDatos.Usuarios.Actualizar.Opcion("WishlistData", JsonSerializer.Serialize(datosDeseados), usuarioInteresado);
											}
											catch (Exception ex)
											{
												BaseDatos.Errores.Insertar.Mensaje("Actualizar Datos Deseados Minimo", ex);
											}

											try
											{
												string correo = await Usuarios.Buscar.UsuarioQuiereCorreos(TiendaRegion.Europa, usuarioInteresado, "NotificationLows");

												if (string.IsNullOrEmpty(correo) == false)
												{
													try
													{
														await Herramientas.Correos.DeseadoMinimo.Nuevo(usuarioInteresado, id, minimo, correo);
													}
													catch (Exception ex)
													{
														BaseDatos.Errores.Insertar.Mensaje("Enviar Correo Minimo 2", ex);
													}
												}
											}
											catch (Exception ex)
											{
												BaseDatos.Errores.Insertar.Mensaje("Enviar Correo Minimo 1", ex);
											}

											try
											{
												bool enviarPush = await BaseDatos.Usuarios.Buscar.OpcionBoolRegion(TiendaRegion.Europa, usuarioInteresado, "NotificationPushLows");

												if (enviarPush == true)
												{
													try
													{
														decimal precioNotificar = minimo.Precio;

														if (minimo.PrecioCambiado > 0)
														{
															precioNotificar = minimo.PrecioCambiado;
														}

														var notificaciones = ServiciosGlobales.ServiceProvider.GetRequiredService<NotificacionesPush>();
														await notificaciones.EnviarNotificacion(usuarioInteresado, minimo.Nombre + " - " + Herramientas.Precios.Euro(precioNotificar), minimo.Enlace);
													}
													catch (Exception ex)
													{
														BaseDatos.Errores.Insertar.Mensaje("Enviar Push Minimo 2", ex);
													}
												}
											}
											catch (Exception ex)
											{
												BaseDatos.Errores.Insertar.Mensaje("Enviar Push Minimo 1", ex);
											}
										}
									}
								}
							}
						}
						else
						{
							if ((minimo.Moneda == JuegoMoneda.Euro && nuevaOferta.Moneda == JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.Precio > 0 && decimal.Round(nuevaOferta.Precio, 2) == decimal.Round(minimo.Precio, 2)) ||
								(minimo.Moneda != JuegoMoneda.Euro && nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && decimal.Round(nuevaOferta.PrecioCambiado, 2) == decimal.Round(minimo.PrecioCambiado, 2)) ||
								(minimo.Moneda == JuegoMoneda.Euro && nuevaOferta.Moneda != JuegoMoneda.Euro && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && decimal.Round(nuevaOferta.PrecioCambiado, 2) == decimal.Round(minimo.Precio, 2)) ||
								(minimo.Moneda != JuegoMoneda.Euro && nuevaOferta.Moneda == JuegoMoneda.Euro && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && decimal.Round(nuevaOferta.Precio, 2) == decimal.Round(minimo.PrecioCambiado, 2)))
							{
								int cantidadHistoricos = historicos?.Count ?? 0;
								historicos = ComprobarHistoricos(TiendaRegion.Europa, historicos, nuevaOferta);

								if (historicos.Count > cantidadHistoricos)
								{
									añadidoEnHistoricos = true;
								}
								else
								{
									añadidoEnHistoricos = false;
								}

								ultimaModificacion = true;

								minimo.Imagen = nuevaOferta.Imagen;
								minimo.Enlace = nuevaOferta.Enlace;
								minimo.Tienda = nuevaOferta.Tienda;
								minimo.Moneda = nuevaOferta.Moneda;
								minimo.Descuento = nuevaOferta.Descuento;

								minimo.FechaActualizacion = nuevaOferta.FechaActualizacion;
								minimo.FechaTermina = nuevaOferta.FechaTermina;

								DateTime tempFecha = nuevaOferta.FechaDetectado;
								tempFecha = tempFecha.AddDays(30);

								bool cambiarFechaDetectado = false;

								if (tempFecha < minimo.FechaDetectado)
								{
									cambiarFechaDetectado = true;
								}

								if (cambiarFechaDetectado == true)
								{
									minimo.FechaDetectado = nuevaOferta.FechaDetectado;
								}
							}
						}
					}
				}

				if (drmEncontrado == false)
				{
					ofertasHistoricas.Add(nuevaOferta);

					huboCambioReal = true;
				}
			}
			else
			{
				ofertasHistoricas = new List<JuegoPrecio>();
				ofertasHistoricas.Add(nuevaOferta);
			}

			DateTime? ahora = null;

			if (ultimaModificacion == true)
			{
				ahora = DateTime.Now;
			}

			huboCambioReal = huboCambioReal || ultimaModificacion || (string.IsNullOrEmpty(slugGOG) == false) || (string.IsNullOrEmpty(slugEpic) == false);

			if (huboCambioReal == false)
			{
				return (null, null);
			}

			StringBuilder sql = new StringBuilder();
			sql.Append("UPDATE juegos SET ");

			if (tiendaTipo == TiendaTipo.Oficial)
			{
				sql.Append($"precioActualesTiendas=@precioActualesTiendas{indice}, ");
				sql.Append($"precioMinimosHistoricos=@precioMinimosHistoricos{indice} ");

				if (añadidoEnHistoricos == true)
				{
					sql.Append($", historicos=@historicos{indice} ");
				}

				if (string.IsNullOrEmpty(slugGOG) == false)
				{
					sql.Append($", idGog=@idGog{indice}, slugGOG=@slugGOG{indice} ");
				}

				if (string.IsNullOrEmpty(slugEpic) == false)
				{
					sql.Append($", slugEpic=@slugEpic{indice} ");
				}
			}
			else if (tiendaTipo == TiendaTipo.NoOficial)
			{
				sql.Append($"preciosActualesNoOficialesEU=@preciosActualesNoOficialesEU{indice}, ");
				sql.Append($"preciosHistoricosNoOficialesEU=@preciosHistoricosNoOficialesEU{indice} ");
			}
			else if (tiendaTipo == TiendaTipo.Marketplace)
			{
				sql.Append($"preciosActualesMarketplacesEU=@preciosActualesMarketplacesEU{indice}, ");
				sql.Append($"preciosHistoricosMarketplacesEU=@preciosHistoricosMarketplacesEU{indice} ");
			}

			if (ahora != null)
			{
				sql.Append($", ultimaModificacion=@ultimaModificacion{indice} ");
			}

			sql.Append($"WHERE id=@id{indice};");

			ofertasActuales = ofertasActuales
				.Where(p => p.FechaActualizacion >= DateTime.Now.AddMonths(-3))
				.ToList();

			DynamicParameters parametros = new DynamicParameters();
			parametros.Add($"@id{indice}", id);

			if (tiendaTipo == TiendaTipo.Oficial)
			{
				parametros.Add($"@precioActualesTiendas{indice}", JsonSerializer.Serialize(ofertasActuales));
				parametros.Add($"@precioMinimosHistoricos{indice}", JsonSerializer.Serialize(ofertasHistoricas));

				if (añadidoEnHistoricos == true)
				{
					parametros.Add($"@historicos{indice}", JsonSerializer.Serialize(historicos));
				}

				if (string.IsNullOrEmpty(slugGOG) == false) 
				{ 
					parametros.Add($"@idGog{indice}", idGOG); 
					parametros.Add($"@slugGOG{indice}", slugGOG); 
				}

				if (string.IsNullOrEmpty(slugEpic) == false)
				{
					parametros.Add($"@slugEpic{indice}", slugEpic);
				}
			}
			else if (tiendaTipo == TiendaTipo.NoOficial)
			{
				parametros.Add($"@preciosActualesNoOficialesEU{indice}", JsonSerializer.Serialize(ofertasActuales));
				parametros.Add($"@preciosHistoricosNoOficialesEU{indice}", JsonSerializer.Serialize(ofertasHistoricas));
			}
			else if (tiendaTipo == TiendaTipo.Marketplace)
			{
				parametros.Add($"@preciosActualesMarketplacesEU{indice}", JsonSerializer.Serialize(ofertasActuales));
				parametros.Add($"@preciosHistoricosMarketplacesEU{indice}", JsonSerializer.Serialize(ofertasHistoricas));
			}

			if (ahora != null)
			{
				parametros.Add($"@ultimaModificacion{indice}", ahora);
			}

			return (sql.ToString(), parametros);
		}

		public static async Task<(string sql, DynamicParameters parametros)> ComprobacionUS(TiendaTipo tiendaTipo, int id, int idSteam, List<JuegoPrecio> ofertasActualesUS, List<JuegoPrecio> ofertasHistoricasUS, List<JuegoHistorico> historicosUS = null, JuegoPrecio nuevaOferta = null,
			string slugGOG = null, string idGOG = null, string slugEpic = null, JuegoAnalisis reseñas = null, int indice = 0)
		{
			bool añadidoEnHistoricos = true;
			bool ultimaModificacion = false;
			bool añadir = true;
			bool huboCambioReal = false;

			#region Aplicar Codigo Descuento

			decimal RedondearHaciaAbajo(decimal valor)
			{
				return Math.Floor(valor * 100) / 100;
			}

			if (nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado == 0)
			{
				nuevaOferta.PrecioCambiado = Herramientas.Divisas.CambioDolar(nuevaOferta.Precio, nuevaOferta.Moneda);

				if (string.IsNullOrEmpty(nuevaOferta.CodigoTexto) == false && nuevaOferta.CodigoDescuento > 0)
				{
					decimal descuento = (decimal)nuevaOferta.CodigoDescuento / 100;
					nuevaOferta.PrecioCambiado = nuevaOferta.PrecioCambiado - (nuevaOferta.PrecioCambiado * descuento);
					nuevaOferta.PrecioCambiado = RedondearHaciaAbajo(nuevaOferta.PrecioCambiado);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(nuevaOferta.CodigoTexto) == false && nuevaOferta.CodigoDescuento > 0)
				{
					decimal descuento = (decimal)nuevaOferta.CodigoDescuento / 100;
					nuevaOferta.Precio = nuevaOferta.Precio - (nuevaOferta.Precio * descuento);
					nuevaOferta.Precio = RedondearHaciaAbajo(nuevaOferta.Precio);
				}
			}

			#endregion

			if (ofertasActualesUS?.Count > 0)
			{
				foreach (JuegoPrecio precio in ofertasActualesUS)
				{
					if (nuevaOferta.Enlace == precio.Enlace &&
						nuevaOferta.DRM == precio.DRM &&
						nuevaOferta.Tienda == precio.Tienda)
					{
						bool cambiarFechaDetectado = false;

						if (nuevaOferta.Moneda == JuegoMoneda.Dolar && precio.Moneda == JuegoMoneda.Dolar && (nuevaOferta.Precio < precio.Precio || nuevaOferta.Precio > precio.Precio))
						{
							cambiarFechaDetectado = true;
						}
						else if (nuevaOferta.Moneda != JuegoMoneda.Dolar && precio.Moneda == JuegoMoneda.Dolar && (nuevaOferta.PrecioCambiado < precio.Precio || nuevaOferta.PrecioCambiado > precio.Precio))
						{
							cambiarFechaDetectado = true;
						}
						else if (nuevaOferta.Moneda == JuegoMoneda.Dolar && precio.Moneda != JuegoMoneda.Dolar && (nuevaOferta.Precio < precio.PrecioCambiado || nuevaOferta.Precio > precio.PrecioCambiado))
						{
							cambiarFechaDetectado = true;
						}
						else if (nuevaOferta.Moneda != JuegoMoneda.Dolar && precio.Moneda != JuegoMoneda.Dolar && (nuevaOferta.PrecioCambiado < precio.PrecioCambiado || nuevaOferta.PrecioCambiado > precio.PrecioCambiado))
						{
							cambiarFechaDetectado = true;
						}

						DateTime tempFecha = precio.FechaDetectado;
						tempFecha = tempFecha.AddDays(21);

						if (tempFecha < nuevaOferta.FechaDetectado)
						{
							cambiarFechaDetectado = true;
						}

						if (cambiarFechaDetectado == true)
						{
							precio.FechaDetectado = nuevaOferta.FechaDetectado;
						}

						if (nuevaOferta.FechaTermina == default(DateTime))
						{
							huboCambioReal = true;
						}
						else if (precio.FechaTermina != nuevaOferta.FechaTermina || precio.Descuento != nuevaOferta.Descuento ||
							precio.CodigoDescuento != nuevaOferta.CodigoDescuento || precio.CodigoTexto != nuevaOferta.CodigoTexto || precio.Moneda != nuevaOferta.Moneda ||
							(nuevaOferta.Moneda == JuegoMoneda.Dolar && precio.Moneda == JuegoMoneda.Dolar && precio.Precio != nuevaOferta.Precio) ||
							(nuevaOferta.Moneda != JuegoMoneda.Dolar && precio.Moneda != JuegoMoneda.Dolar && precio.PrecioCambiado != nuevaOferta.PrecioCambiado) ||
							(nuevaOferta.Moneda == JuegoMoneda.Dolar && precio.Moneda != JuegoMoneda.Dolar && precio.PrecioCambiado != nuevaOferta.Precio) ||
							(nuevaOferta.Moneda != JuegoMoneda.Dolar && precio.Moneda == JuegoMoneda.Dolar && precio.Precio != nuevaOferta.PrecioCambiado))
						{
							huboCambioReal = true;
						}

						if (nuevaOferta.Moneda != JuegoMoneda.Dolar)
						{
							precio.PrecioCambiado = nuevaOferta.PrecioCambiado;
						}
						else
						{
							precio.PrecioCambiado = 0;
						}

						precio.Precio = nuevaOferta.Precio;
						precio.Descuento = nuevaOferta.Descuento;
						precio.FechaActualizacion = nuevaOferta.FechaActualizacion;

						if (nuevaOferta.FechaTermina != default(DateTime))
						{
							precio.FechaTermina = nuevaOferta.FechaTermina;
						}

						precio.CodigoDescuento = nuevaOferta.CodigoDescuento;
						precio.CodigoTexto = nuevaOferta.CodigoTexto;
						precio.Nombre = nuevaOferta.Nombre;
						precio.Imagen = nuevaOferta.Imagen;
						precio.Moneda = nuevaOferta.Moneda;
						precio.BundleSteam = nuevaOferta.BundleSteam;

						añadir = false;
						break;
					}
				}
			}
			else
			{
				ofertasActualesUS = new List<JuegoPrecio>();
			}

			if (añadir == true)
			{
				ofertasActualesUS.Add(nuevaOferta);

				huboCambioReal = true;
			}

			if (ofertasHistoricasUS?.Count > 0)
			{
				bool drmEncontrado = false;

				foreach (JuegoPrecio minimo in ofertasHistoricasUS)
				{
					if (nuevaOferta.DRM == minimo.DRM)
					{
						drmEncontrado = true;

						if ((minimo.Moneda == JuegoMoneda.Dolar && nuevaOferta.Moneda == JuegoMoneda.Dolar && nuevaOferta.Precio > 0 && minimo.Precio > 0 && nuevaOferta.Precio < minimo.Precio) ||
							(minimo.Moneda != JuegoMoneda.Dolar && nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.PrecioCambiado < minimo.PrecioCambiado) ||
							(minimo.Moneda == JuegoMoneda.Dolar && nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && nuevaOferta.PrecioCambiado < minimo.Precio) ||
							(minimo.Moneda != JuegoMoneda.Dolar && nuevaOferta.Moneda == JuegoMoneda.Dolar && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.Precio < minimo.PrecioCambiado))
						{
							historicosUS = ComprobarHistoricos(TiendaRegion.EstadosUnidos, historicosUS, nuevaOferta);

							bool notificar = false;

							if ((minimo.Moneda == JuegoMoneda.Dolar && nuevaOferta.Moneda == JuegoMoneda.Dolar && nuevaOferta.Precio > 0 && minimo.Precio > 0 && nuevaOferta.Precio + 0.1m < minimo.Precio) ||
								(minimo.Moneda != JuegoMoneda.Dolar && nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.PrecioCambiado + 0.1m < minimo.PrecioCambiado) ||
								(minimo.Moneda == JuegoMoneda.Dolar && nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && nuevaOferta.PrecioCambiado + 0.1m < minimo.Precio) ||
								(minimo.Moneda != JuegoMoneda.Dolar && nuevaOferta.Moneda == JuegoMoneda.Dolar && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && nuevaOferta.Precio + 0.1m < minimo.PrecioCambiado))
							{
								notificar = true;
							}

							ultimaModificacion = true;

							if (nuevaOferta.Moneda != JuegoMoneda.Dolar)
							{
								minimo.PrecioCambiado = nuevaOferta.PrecioCambiado;
							}
							else
							{
								minimo.PrecioCambiado = 0;
							}

							minimo.Precio = nuevaOferta.Precio;
							minimo.Moneda = nuevaOferta.Moneda;
							minimo.Descuento = nuevaOferta.Descuento;
							minimo.FechaDetectado = nuevaOferta.FechaDetectado;
							minimo.FechaActualizacion = nuevaOferta.FechaActualizacion;
							minimo.FechaTermina = nuevaOferta.FechaTermina;
							minimo.CodigoDescuento = nuevaOferta.CodigoDescuento;
							minimo.CodigoTexto = nuevaOferta.CodigoTexto;
							minimo.Nombre = nuevaOferta.Nombre;
							minimo.Imagen = nuevaOferta.Imagen;
							minimo.Enlace = nuevaOferta.Enlace;
							minimo.Tienda = nuevaOferta.Tienda;
							minimo.BundleSteam = nuevaOferta.BundleSteam;

							//------------------------------------------

							if (notificar == true && tiendaTipo == TiendaTipo.Oficial)
							{
								List<string> usuariosInteresados = await BaseDatos.Usuarios.Buscar.ListaUsuariosTienenDeseado(id, nuevaOferta.DRM);

								if (usuariosInteresados?.Count > 0)
								{
									foreach (var usuarioInteresado in usuariosInteresados)
									{
										if (await Usuarios.Buscar.UsuarioTieneJuego(usuarioInteresado, id, nuevaOferta.DRM) == false)
										{
											try
											{
												DeseadosDatos datosDeseados = null;

												string datosDeseadosTexto = await BaseDatos.Usuarios.Buscar.OpcionStringRegion(TiendaRegion.EstadosUnidos, usuarioInteresado, "WishlistData");

												if (string.IsNullOrEmpty(datosDeseadosTexto) == true)
												{
													datosDeseados = new DeseadosDatos();
												}
												else
												{
													try
													{
														datosDeseados = JsonSerializer.Deserialize<DeseadosDatos>(datosDeseadosTexto);
													}
													catch
													{
														datosDeseados = new DeseadosDatos();
													}

													datosDeseados.Cantidad = datosDeseados.Cantidad + 1;
													datosDeseados.UltimoJuego = DateTime.Now;

													if (datosDeseados.Juegos == null)
													{
														datosDeseados.Juegos = new List<DeseadosDatosJuego>();
													}

													datosDeseados.Juegos.Add(new DeseadosDatosJuego
													{
														Id = id,
														Enlace = minimo.Enlace,
														Descuento = minimo.Descuento.ToString(),
														Precio = Herramientas.Precios.Dolar(minimo.PrecioCambiado > 0 ? minimo.PrecioCambiado : minimo.Precio),
														Tienda = minimo.Tienda,
														DRM = minimo.DRM,
														Fecha = DateTime.Now
													});
												}

												await BaseDatos.Usuarios.Actualizar.Opcion("WishlistData", JsonSerializer.Serialize(datosDeseados), usuarioInteresado);
											}
											catch (Exception ex)
											{
												BaseDatos.Errores.Insertar.Mensaje("Actualizar Datos Deseados Minimo", ex);
											}

											try
											{
												string correo = await Usuarios.Buscar.UsuarioQuiereCorreos(TiendaRegion.EstadosUnidos, usuarioInteresado, "NotificationLows");

												if (string.IsNullOrEmpty(correo) == false)
												{
													try
													{
														await Herramientas.Correos.DeseadoMinimo.Nuevo(usuarioInteresado, id, minimo, correo);
													}
													catch (Exception ex)
													{
														BaseDatos.Errores.Insertar.Mensaje("Enviar Correo Minimo 2", ex);
													}
												}
											}
											catch (Exception ex)
											{
												BaseDatos.Errores.Insertar.Mensaje("Enviar Correo Minimo 1", ex);
											}

											try
											{
												bool enviarPush = await BaseDatos.Usuarios.Buscar.OpcionBoolRegion(TiendaRegion.EstadosUnidos, usuarioInteresado, "NotificationPushLows");

												if (enviarPush == true)
												{
													try
													{
														decimal precioNotificar = minimo.Precio;

														if (minimo.PrecioCambiado > 0)
														{
															precioNotificar = minimo.PrecioCambiado;
														}

														var notificaciones = ServiciosGlobales.ServiceProvider.GetRequiredService<NotificacionesPush>();
														await notificaciones.EnviarNotificacion(usuarioInteresado, minimo.Nombre + " - " + Herramientas.Precios.Dolar(precioNotificar), minimo.Enlace);
													}
													catch (Exception ex)
													{
														BaseDatos.Errores.Insertar.Mensaje("Enviar Push Minimo 2", ex);
													}
												}
											}
											catch (Exception ex)
											{
												BaseDatos.Errores.Insertar.Mensaje("Enviar Push Minimo 1", ex);
											}
										}
									}
								}
							}
						}
						else
						{
							if ((minimo.Moneda == JuegoMoneda.Dolar && nuevaOferta.Moneda == JuegoMoneda.Dolar && nuevaOferta.Precio > 0 && minimo.Precio > 0 && decimal.Round(nuevaOferta.Precio, 2) == decimal.Round(minimo.Precio, 2)) ||
							   (minimo.Moneda != JuegoMoneda.Dolar && nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado > 0 && minimo.PrecioCambiado > 0 && decimal.Round(nuevaOferta.PrecioCambiado, 2) == decimal.Round(minimo.PrecioCambiado, 2)) ||
							   (minimo.Moneda == JuegoMoneda.Dolar && nuevaOferta.Moneda != JuegoMoneda.Dolar && nuevaOferta.PrecioCambiado > 0 && minimo.Precio > 0 && decimal.Round(nuevaOferta.PrecioCambiado, 2) == decimal.Round(minimo.Precio, 2)) ||
							   (minimo.Moneda != JuegoMoneda.Dolar && nuevaOferta.Moneda == JuegoMoneda.Dolar && nuevaOferta.Precio > 0 && minimo.PrecioCambiado > 0 && decimal.Round(nuevaOferta.Precio, 2) == decimal.Round(minimo.PrecioCambiado, 2)))
							{
								int cantidadHistoricos = historicosUS?.Count ?? 0;
								historicosUS = ComprobarHistoricos(TiendaRegion.EstadosUnidos, historicosUS, nuevaOferta);

								if (historicosUS.Count > cantidadHistoricos)
								{
									añadidoEnHistoricos = true;
								}
								else
								{
									añadidoEnHistoricos = false;
								}

								ultimaModificacion = true;

								minimo.Imagen = nuevaOferta.Imagen;
								minimo.Enlace = nuevaOferta.Enlace;
								minimo.Tienda = nuevaOferta.Tienda;
								minimo.Moneda = nuevaOferta.Moneda;
								minimo.Descuento = nuevaOferta.Descuento;

								minimo.FechaActualizacion = nuevaOferta.FechaActualizacion;
								minimo.FechaTermina = nuevaOferta.FechaTermina;

								DateTime tempFecha = nuevaOferta.FechaDetectado;
								tempFecha = tempFecha.AddDays(30);

								bool cambiarFechaDetectado = false;

								if (tempFecha < minimo.FechaDetectado)
								{
									cambiarFechaDetectado = true;
								}

								if (cambiarFechaDetectado == true)
								{
									minimo.FechaDetectado = nuevaOferta.FechaDetectado;
								}
							}
						}
					}
				}

				if (drmEncontrado == false)
				{
					ofertasHistoricasUS.Add(nuevaOferta);

					huboCambioReal = true;
				}
			}
			else
			{
				ofertasHistoricasUS = new List<JuegoPrecio>();
				ofertasHistoricasUS.Add(nuevaOferta);
			}

			DateTime? ahora = null;

			if (ultimaModificacion == true)
			{
				ahora = DateTime.Now;
			}

			huboCambioReal = huboCambioReal || ultimaModificacion || (string.IsNullOrEmpty(slugGOG) == false) || (string.IsNullOrEmpty(slugEpic) == false);

			if (huboCambioReal == false)
			{
				return (null, null);
			}

			var sql = new StringBuilder();
			sql.Append("UPDATE juegos SET ");

			if (tiendaTipo == TiendaTipo.Oficial)
			{
				sql.Append($"precioActualesTiendasUS=@precioActualesTiendasUS{indice}, ");
				sql.Append($"precioMinimosHistoricosUS=@precioMinimosHistoricosUS{indice} ");

				if (añadidoEnHistoricos == true)
				{
					sql.Append($", historicosUS=@historicosUS{indice} ");
				}

				if (string.IsNullOrEmpty(slugGOG) == false)
				{
					sql.Append($", idGog=@idGog{indice}, slugGOG=@slugGOG{indice} ");
				}

				if (string.IsNullOrEmpty(slugEpic) == false)
				{
					sql.Append($", slugEpic=@slugEpic{indice} ");
				}
			}
			else if (tiendaTipo == TiendaTipo.NoOficial)
			{
				sql.Append($"preciosActualesNoOficialesUS=@preciosActualesNoOficialesUS{indice}, ");
				sql.Append($"preciosHistoricosNoOficialesUS=@preciosHistoricosNoOficialesUS{indice} ");
			}
			else if (tiendaTipo == TiendaTipo.Marketplace)
			{
				sql.Append($"preciosActualesMarketplacesUS=@preciosActualesMarketplacesUS{indice}, ");
				sql.Append($"preciosHistoricosMarketplacesUS=@preciosHistoricosMarketplacesUS{indice} ");
			}

			if (ahora != null)
			{
				sql.Append($", ultimaModificacion=@ultimaModificacion{indice} ");
			}

			sql.Append($"WHERE id=@id{indice};");

			ofertasActualesUS = ofertasActualesUS
				.Where(p => p.FechaActualizacion >= DateTime.Now.AddMonths(-3))
				.ToList();

			DynamicParameters parametros = new DynamicParameters();
			parametros.Add($"@id{indice}", id);

			if (tiendaTipo == TiendaTipo.Oficial)
			{
				parametros.Add($"@precioActualesTiendasUS{indice}", JsonSerializer.Serialize(ofertasActualesUS));
				parametros.Add($"@precioMinimosHistoricosUS{indice}", JsonSerializer.Serialize(ofertasHistoricasUS));

				if (añadidoEnHistoricos == true)
				{
					parametros.Add($"@historicosUS{indice}", JsonSerializer.Serialize(historicosUS));
				}

				if (string.IsNullOrEmpty(slugGOG) == false)
				{
					parametros.Add($"@idGog{indice}", idGOG);
					parametros.Add($"@slugGOG{indice}", slugGOG);
				}

				if (string.IsNullOrEmpty(slugEpic) == false)
				{
					parametros.Add($"@slugEpic{indice}", slugEpic);
				}
			}
			else if (tiendaTipo == TiendaTipo.NoOficial)
			{
				parametros.Add($"@preciosActualesNoOficialesUS{indice}", JsonSerializer.Serialize(ofertasActualesUS));
				parametros.Add($"@preciosHistoricosNoOficialesUS{indice}", JsonSerializer.Serialize(ofertasHistoricasUS));
			}
			else if (tiendaTipo == TiendaTipo.Marketplace)
			{
				parametros.Add($"@preciosActualesMarketplacesUS{indice}", JsonSerializer.Serialize(ofertasActualesUS));
				parametros.Add($"@preciosHistoricosMarketplacesUS{indice}", JsonSerializer.Serialize(ofertasHistoricasUS));
			}

			if (ahora != null)
			{
				parametros.Add($"@ultimaModificacion{indice}", ahora);
			}

			return (sql.ToString(), parametros);
		}

		private static List<JuegoHistorico> ComprobarHistoricos(TiendaRegion region, List<JuegoHistorico> historicos, JuegoPrecio nuevaOferta)
		{
			if (historicos == null)
			{
				historicos = new List<JuegoHistorico>();
			}

			if (historicos.Count == 0)
			{
				JuegoHistorico nuevoHistorico = new JuegoHistorico();
				nuevoHistorico.DRM = nuevaOferta.DRM;
				nuevoHistorico.Tienda = nuevaOferta.Tienda;
				nuevoHistorico.Fecha = nuevaOferta.FechaDetectado;

				if (region == TiendaRegion.Europa)
				{
					if (nuevaOferta.Moneda != JuegoMoneda.Euro)
					{
						nuevoHistorico.Precio = nuevaOferta.PrecioCambiado;
					}
					else
					{
						nuevoHistorico.Precio = nuevaOferta.Precio;
					}
				}
				else if (region == TiendaRegion.EstadosUnidos)
				{
					if (nuevaOferta.Moneda != JuegoMoneda.Dolar)
					{
						nuevoHistorico.Precio = nuevaOferta.PrecioCambiado;
					}
					else
					{
						nuevoHistorico.Precio = nuevaOferta.Precio;
					}
				}

				historicos.Add(nuevoHistorico);
			}
			else if (historicos.Count > 0)
			{
				JuegoHistorico nuevoHistorico = new JuegoHistorico();
				nuevoHistorico.DRM = nuevaOferta.DRM;
				nuevoHistorico.Tienda = nuevaOferta.Tienda;
				nuevoHistorico.Fecha = nuevaOferta.FechaDetectado;

				if (region == TiendaRegion.Europa)
				{
					if (nuevaOferta.Moneda != JuegoMoneda.Euro)
					{
						if (nuevaOferta.PrecioCambiado > 0)
						{
							nuevoHistorico.Precio = nuevaOferta.PrecioCambiado;
						}
						else
						{
							nuevoHistorico.Precio = nuevaOferta.Precio;
						}
					}
					else
					{
						nuevoHistorico.Precio = nuevaOferta.Precio;
					}
				}
				else if (region == TiendaRegion.EstadosUnidos)
				{
					if (nuevaOferta.Moneda != JuegoMoneda.Dolar)
					{
						if (nuevaOferta.PrecioCambiado > 0)
						{
							nuevoHistorico.Precio = nuevaOferta.PrecioCambiado;
						}
						else
						{
							nuevoHistorico.Precio = nuevaOferta.Precio;
						}
					}
					else
					{
						nuevoHistorico.Precio = nuevaOferta.Precio;
					}
				}

				bool mismoDRM = false;
				decimal precioMasBajo2 = 1000000;
				DateTime fechaMasBajo2 = new DateTime();

				foreach (var historico in historicos)
				{
					if (historico.DRM == nuevoHistorico.DRM)
					{
						mismoDRM = true;

						if (precioMasBajo2 >= historico.Precio)
						{
							precioMasBajo2 = historico.Precio;
							fechaMasBajo2 = historico.Fecha;
						}
					}
				}

				if (mismoDRM == true)
				{
					if (precioMasBajo2 > nuevoHistorico.Precio)
					{
						historicos.Add(nuevoHistorico);
					}

					if (decimal.Round(precioMasBajo2, 2) == decimal.Round(nuevoHistorico.Precio, 2))
					{
						DateTime historico2 = fechaMasBajo2;
						historico2 = historico2.AddDays(22);

						if (historico2 < nuevoHistorico.Fecha)
						{
							historicos.Add(nuevoHistorico);
						}
					}
				}
				else
				{
					historicos.Add(nuevoHistorico);
				}
			}

			return historicos;
		}
	}
}

public static class ServiciosGlobales
{
	public static IServiceProvider ServiceProvider { get; set; }
}