using Dapper;
using Noticias;
using System.Text.Json;

namespace BaseDatos.Noticias
{
	public static class Actualizar
	{
		public static async Task TituloEn(string id, string titulo)
		{
			string actualizar = "UPDATE noticias " +
					"SET tituloEn=@tituloEn " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new { id, tituloEn = titulo }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actualizar TituloEn", ex);
			}
		}

		public static async Task TituloEs(string id, string titulo)
		{
			string actualizar = "UPDATE noticias " +
					"SET tituloEs=@tituloEs " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new { id, tituloEs = titulo }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actualizar TituloEs", ex);
			}
		}

		public static async Task Imagen(string id, string imagen)
		{
			string actualizar = "UPDATE noticias " +
					"SET imagen=@imagen " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new { id, imagen }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actualizar Imagen", ex);
			}
		}

		public static async Task Enlace(string id, string enlace)
        {
            string actualizar = "UPDATE noticias " +
                    "SET enlace=@enlace " +
                    "WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new { id, enlace }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actualizar Enlace", ex);
			}
        }

		public static async Task FechaTermina(string id, string nuevaFecha)
		{
			string actualizar = "UPDATE noticias " +
					"SET fechaTermina=@fechaTermina " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new { id, fechaTermina = nuevaFecha }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actualizar Fecha Termina", ex);
			}
		}

		public static async Task Tipo(string id, string nuevoTipo)
		{
			string actualizar = "UPDATE noticias " +
					"SET noticiaTipo=@noticiaTipo " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new { id, noticiaTipo = nuevoTipo }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actualizar Tipo", ex);
			}
		}

		public static async Task ContenidoEn(string id, string nuevoContenido)
		{
			string actualizar = "UPDATE noticias " +
					"SET contenidoEn=@contenidoEn " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new { id, contenidoEn = nuevoContenido }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actualizar ContenidoEn", ex);
			}
		}

		public static async Task ContenidoEs(string id, string nuevoContenido)
		{
			string actualizar = "UPDATE noticias " +
					"SET contenidoEs=@contenidoEs " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new { id, contenidoEs = nuevoContenido }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actualizar ContenidoEs", ex);
			}
		}

		public static async Task BundleId(string id, string nuevoValor)
		{
			string actualizar = "UPDATE noticias " +
					"SET bundleId=@bundleId " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new { id, bundleId = nuevoValor }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actualizar BundleId", ex);
			}
		}

		public static async Task GratisIds(string id, string nuevoValor)
		{
			string actualizar = "UPDATE noticias " +
					"SET gratisIds=@gratisIds " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new { id, gratisIds = nuevoValor }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actualizar GratisIds", ex);
			}
		}

		public static async Task SuscripcionesIds(string id, string nuevoValor)
		{
			string actualizar = "UPDATE noticias " +
					"SET suscripcionesIds=@suscripcionesIds " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new { id, suscripcionesIds = nuevoValor }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actualizar SuscripcionesIds", ex);
			}
		}

		public static async Task Steam(Noticia noticia)
		{
			string actualizar = "UPDATE noticias " +
					"SET steamEn=@steamEn, steamEs=@steamEs " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(actualizar, new {
						id = noticia.Id,
						steamEn = JsonSerializer.Serialize(noticia.SteamEn),
						steamEs = JsonSerializer.Serialize(noticia.SteamEs)
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Noticias Actualizar Steam", ex);
			}
		}
	}
}
