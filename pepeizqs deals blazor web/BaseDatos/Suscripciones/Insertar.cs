#nullable disable

using Dapper;
using Juegos;

namespace BaseDatos.Suscripciones
{
	public static class Insertar
	{
		public static async Task Ejecutar(int juegoId, JuegoSuscripcion actual)
		{
			string sqlInsertar = @"INSERT INTO suscripciones (suscripcion, juegoId, nombre, imagen, drm, enlace, fechaEmpieza, fechaTermina, imagenNoticia) VALUES (@Tipo, @JuegoId, @Nombre, @Imagen, @DRM, @Enlace, @FechaEmpieza, @FechaTermina, @ImagenNoticia); ";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlInsertar, new
					{
						Tipo = actual.Tipo,
						JuegoId = actual.JuegoId,
						Nombre = actual.Nombre,
						Imagen = actual.Imagen,
						DRM = actual.DRM,
						Enlace = actual.Enlace,
						FechaEmpieza = actual.FechaEmpieza,
						FechaTermina = actual.FechaTermina,
						ImagenNoticia = actual.ImagenNoticia
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Insertar", ex);
			}
		}

		public static async Task Temporal(string nombreTabla, string enlace, string nombreJuego = "vacio", string imagen = "vacio")
		{
			string sqlBuscar = $"SELECT enlace FROM temporal{nombreTabla} WHERE enlace = @enlace";

			try
			{
				var resultado = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.QuerySingleOrDefaultAsync(sqlBuscar, new { enlace });
				});

				if (resultado == null)
				{
					string sqlInsertar = $"INSERT INTO temporal{nombreTabla} (enlace, nombre, imagen) " +
									 "VALUES (@enlace, @nombre, @imagen)";

					await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(sqlInsertar, new
						{
							enlace,
							nombre = nombreJuego,
							imagen
						}, transaction: sentencia);
					});
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Insertar Temporal " + nombreTabla, ex);
			}
		}

		public static async Task Steam(int idPaquete, int idJuego)
		{
			string sql = @"
BEGIN TRAN;

UPDATE tiendasteamsuscripciones
SET fecha = GETDATE()
WHERE idPaquete = @IdPaquete
  AND idJuego   = @IdJuego;

IF @@ROWCOUNT = 0
BEGIN
    INSERT INTO tiendasteamsuscripciones (idPaquete, idJuego, fecha)
    VALUES (@IdPaquete, @IdJuego, GETDATE());
END

COMMIT TRAN;
";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sql, new
					{
						idPaquete,
						idJuego
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripciones Insertar Steam", ex);
			}
		}
	}
}
