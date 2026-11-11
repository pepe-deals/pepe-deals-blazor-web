#nullable disable

using Dapper;
using Tiendas2;

namespace BaseDatos.Admin
{
	public static class Actualizar
	{
		public static async Task Tiendas(TiendaRegion region, string tienda, DateTime fecha, int cantidad)
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
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync($"UPDATE {tabla} SET fecha=@fecha, mensaje=@mensaje WHERE id=@id", new { id = tienda, fecha = fecha, mensaje = cantidad }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Tiendas", ex, false);
			}
		}

		public static async Task TiendasValorAdicional(TiendaRegion region, string tienda, string valor, int cantidad)
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
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync($"UPDATE adminTiendas SET {valor}=@cantidad WHERE id=@id", new { id = tienda, cantidad }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Tiendas Valor Adicional", ex);
			}
		}

		public static async Task TareaUso(string id, DateTime fecha)
		{
			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("UPDATE adminTareas SET fecha=@fecha WHERE id=@id", new { id = id, fecha = fecha }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Tareas", ex);
			}
		}

		public static async Task Dato(string id, int contenido)
		{
			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("UPDATE adminDatos SET contenido=@contenido WHERE id=@id", new { id = id, contenido = contenido }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Datos", ex);
			}
		}

		public static async Task Fecha(string id, DateTime fecha)
		{
			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("UPDATE adminFechas SET fecha=@fecha WHERE id=@id", new { id = id, fecha = fecha }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Actualizar Admin Fechas", ex);
			}
		}
	}
}
