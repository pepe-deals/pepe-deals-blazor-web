#nullable disable

using Dapper;

namespace BaseDatos.Bundles
{
	public static class Actualizar
	{
		public static async void Nombre(string id, string nombre)
		{
			string sqlActualizar = "UPDATE bundles " +
					"SET nombre=@nombre " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new { id, nombre }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Actualizar Nombre", ex);
			}
		}

		public static async void FechaEmpieza(string id, string fechaEmpieza)
		{
			string sqlActualizar = "UPDATE bundles " +
					"SET fechaEmpieza=@fechaEmpieza " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new { id, fechaEmpieza }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Actualizar Fecha Empieza", ex);
			}
		}

		public static async void FechaTermina(string id, string fechaTermina)
		{
			string sqlActualizar = "UPDATE bundles " +
					"SET fechaTermina=@fechaTermina " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new { id, fechaTermina }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Actualizar Fecha Termina", ex);
			}
		}

        public static async void ImagenBundle(string id, string imagen)
        {
            string sqlActualizar = "UPDATE bundles " +
                    "SET imagen=@imagen " +
                    "WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new { id, imagen }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Actualizar Imagen", ex);
			}
		}

        public static async void ImagenNoticia(string id, string imagen)
        {
            string sqlActualizar = "UPDATE bundles " +
                    "SET imagenNoticia=@imagenNoticia " +
                    "WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new { id, imagenNoticia = imagen }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Actualizar Imagen Noticia", ex);
			}
        }

        public static async Task Juegos(string id, string juegos)
		{
			string sqlActualizar = "UPDATE bundles " +
					"SET juegos=@juegos " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new { id, juegos }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Actualizar Juegos", ex);
			}
		}

		public static async Task Tiers(string id, string tiers)
		{
			string sqlActualizar = "UPDATE bundles " +
					"SET tiers=@tiers " +
					"WHERE id=@id";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new { id, tiers }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Actualizar Tiers", ex);
			}
		}
	}
}
