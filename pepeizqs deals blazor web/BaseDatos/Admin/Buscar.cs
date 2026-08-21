#nullable disable

using Dapper;
using Streaming2;
using Suscripciones2;
using System.Drawing;
using Tiendas2;

namespace BaseDatos.Admin
{
	public static class Buscar
	{
		public static async Task<int> Dato(string id)
		{
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<int>("SELECT contenido FROM adminDatos WHERE id=@id", new { id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Buscar Dato", ex);
			}

			return 0;
		}

		public static async Task<DateTime?> Fecha(string id)
		{
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<DateTime?>("SELECT fecha FROM adminFechas WHERE id=@id", new { id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Buscar Fecha", ex);
			}

			return null;
		}

		public static async Task<DateTime> Tarea(string id)
		{
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<DateTime>("SELECT fecha FROM adminTareas WHERE id=@id", new { id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Buscar Tarea", ex);
			}

			return new DateTime();
		}

		public static async Task<bool> TiendasPosibleUsar(TimeSpan tiempo, string tiendaId)
		{
			try
			{
				DateTime? fecha = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<DateTime?>("SELECT fecha FROM adminTiendas WHERE id=@id", new { id = tiendaId });
				});

				if (fecha == null)
				{
					return false;
				}

				return (DateTime.Now - fecha.Value) > tiempo;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tiendas Posible Usar", ex);
			}

			return false;
		}

		public static async Task<bool> TiendasPosibleUsarUS(TimeSpan tiempo, string tiendaId)
		{
			try
			{
				DateTime? fecha = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<DateTime?>("SELECT fecha FROM adminTiendasUS WHERE id=@id", new { id = tiendaId });
				});

				if (fecha == null)
				{
					return false;
				}

				return (DateTime.Now - fecha.Value) > tiempo;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tiendas Posible Usar US", ex);
			}

			return false;
		}

		public static async Task<List<AdminTarea>> TiendasEnUso(TimeSpan tiempo)
		{
			try
			{
				List<AdminTarea> tiendas = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<AdminTarea>("SELECT id, fecha FROM adminTiendas UNION ALL SELECT id, fecha FROM adminTiendasUS ORDER BY fecha DESC")).ToList();
				});

				tiendas = tiendas.Where(t =>
				{
					foreach (Tienda tienda2 in TiendasCargar.GenerarListado())
					{
						if (tienda2.Id == t.Id && tienda2.AdminUso == false)
						{
							return false;
						}
					}

					foreach (Suscripcion suscripcion2 in SuscripcionesCargar.GenerarListado())
					{
						if (suscripcion2.Id.ToString() == t.Id && suscripcion2.AdminAñadir == true)
						{
							return false;
						}
					}

					foreach (Streaming2.Streaming streaming2 in StreamingCargar.GenerarListado())
					{
						if (streaming2.Id.ToString() == t.Id)
						{
							return false;
						}
					}

					return true;
				}).ToList();

				List<AdminTarea> tiendasEnUso = new List<AdminTarea>();

				foreach (var tienda in tiendas)
				{
					if ((DateTime.Now - tienda.Fecha) < tiempo)
					{
						tiendasEnUso.Add(tienda);
					}
				}

				return tiendasEnUso;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tiendas En Uso", ex);
			}

			return null;
		}

		public static async Task<int> TiendasValorAdicional(TiendaRegion region, string id, string valor)
		{
			string tabla = string.Empty;

			if (region == TiendaRegion.Europa)
			{
				tabla = "adminTiendas";
			}
			else if (region == TiendaRegion.EstadosUnidos)
			{
				tabla = "adminTiendasUS";
			}

			try
			{
				var fila = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync($"SELECT * FROM {tabla} WHERE id=@id", new { id });
				});

				if (fila == null)
				{
					return 0;
				}

				var diccionario = (IDictionary<string, object>)fila;

				if (diccionario.ContainsKey(valor) && diccionario[valor] != null)
				{
					return Convert.ToInt32(diccionario[valor]);
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tiendas Valor Adicional", ex);
			}

			return 0;
		}

		public static async Task<List<AdminTarea>> TareasTiendas()
		{
			try
			{
				string sql = null;

				foreach (Tienda tienda in TiendasCargar.GenerarListado())
				{
					if (tienda.AdminUso == true)
					{
						if (string.IsNullOrEmpty(sql) == true)
						{
							sql = $"SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendas WHERE id='{tienda.Id}'";

						}
						else
						{
							sql = sql + Environment.NewLine + $"UNION ALL SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendas WHERE id='{tienda.Id}'";
						}
					}
				}

				if (string.IsNullOrEmpty(sql) == false)
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<AdminTarea>(sql)).ToList();
					});
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tareas Tiendas", ex);
			}

			return null;
		}

		public static async Task<List<AdminTarea>> TareasTiendasUS()
		{
			try
			{
				string sql = null;

				foreach (Tienda tienda in TiendasCargar.GenerarListado())
				{
					if (tienda.AdminUso == true)
					{
						if (string.IsNullOrEmpty(sql) == true)
						{
							sql = $"SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendasUS WHERE id='{tienda.Id}'";

						}
						else
						{
							sql = sql + Environment.NewLine + $"UNION ALL SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendasUS WHERE id='{tienda.Id}'";
						}
					}
				}

				if (string.IsNullOrEmpty(sql) == false)
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<AdminTarea>(sql)).ToList();
					});
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tareas Tiendas", ex);
			}

			return null;
		}

		public static async Task<List<AdminTarea>> TareasSuscripciones()
		{
			try
			{
				string sql = null;

				foreach (Suscripcion suscripcion in SuscripcionesCargar.GenerarListado())
				{
					if (suscripcion.AdminAñadir == false)
					{
						if (string.IsNullOrEmpty(sql) == true)
						{
							sql = $"SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendas WHERE id='{suscripcion.Id}'";

						}
						else
						{
							sql = sql + Environment.NewLine + $" UNION ALL SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendas WHERE id='{suscripcion.Id}'";
						}
					}
				}

				if (string.IsNullOrEmpty(sql) == false)
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<AdminTarea>(sql)).ToList();
					});
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tareas Suscripciones", ex);
			}

			return null;
		}

		public static async Task<List<AdminTarea>> TareasStreaming()
		{
			try
			{
				string sql = null;

				foreach (var streaming in StreamingCargar.GenerarListado())
				{
					if (string.IsNullOrEmpty(sql) == true)
					{
						sql = $"SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendas WHERE id='{streaming.Id}'";

					}
					else
					{
						sql = sql + Environment.NewLine + $"UNION ALL SELECT id, fecha, mensaje AS Cantidad, valorAdicional as Valor1, valorAdicional2 as Valor2 FROM adminTiendas WHERE id='{streaming.Id}'";
					}
				}

				if (string.IsNullOrEmpty(sql) == false)
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<AdminTarea>(sql)).ToList();
					});
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tareas Streaming", ex);
			}

			return null;
		}

		public static async Task<List<AdminTarea>> TareasUltimos60Segundos(List<AdminTarea> tareas)
		{
			try
			{
				var ids = tareas.Where(t => t.Fecha.AddSeconds(60) >= DateTime.Now).Select(t => t.Id).Distinct().ToList();

				if (ids.Count == 0)
				{
					return tareas;
				}

				string sql = string.Empty;

				if (ids?.Count > 0)
				{
					sql = "SELECT id, fecha, mensaje AS Cantidad, valorAdicional AS Valor1, valorAdicional2 AS Valor2 FROM adminTiendas WHERE id IN (";

					int i = 0;
					while (i < ids.Count)
					{
						if (i == 0)
						{
							sql = sql + "'" + ids[i] + "'";
						}
						else
						{
							sql = sql + ", '" + ids[i] + "'";
						}

						i += 1;
					}

					sql = sql + ")";
				}

				List<AdminTarea> nuevas = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<AdminTarea>(sql)).ToList();
				});

				var diccionarioNuevas = nuevas.ToDictionary(t => t.Id);

				List<AdminTarea> fusionadas = tareas.Select(t => diccionarioNuevas.TryGetValue(t.Id, out var actualizada) ? actualizada : t).ToList();

				return fusionadas;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tareas Ultimos 60 Segundos", ex);
			}

			return null;
		}

		public static async Task<bool> TareaPosibleUsar(string id, TimeSpan tiempo)
		{
			try
			{
				DateTime? fecha = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<DateTime?>("SELECT fecha FROM adminTareas WHERE id=@id", new { id });
				}); 

				if (fecha == null)
				{
					return true;
				}

				return (DateTime.Now - fecha.Value) >= tiempo;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tarea Posible Usar", ex);
			}

			return true;
		}

		public static async Task<bool> TiendasLibre()
		{
			try
			{
				int enUso = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM adminTiendas WHERE fecha > DATEADD(SECOND, -60, GETDATE())");
				}); 

				return enUso == 0;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Tiendas Libre", ex);
			}

			return true;
		}

		public static async Task<int> CantidadErrores()
		{
			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM errores");
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Admin Cantidad Errores", ex);
			}

			return 0;
		}
	}

	public class AdminTarea
	{
		public string Id { get; set; }
		public DateTime Fecha { get; set; }
		public int Cantidad { get; set; }
		public int Valor1 { get; set; }
		public int Valor2 { get; set; }
	}
}
