#nullable disable

using Dapper;
using Juegos;

namespace BaseDatos.Suscripciones
{
	public static class Actualizar
	{
		public static async Task FechaTermina(JuegoSuscripcion suscripcion)
		{
			string sqlActualizar = "UPDATE suscripciones " +
					"SET fechaTermina=@fechaTermina WHERE enlace=@enlace";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						Enlace = suscripcion.Enlace,
						FechaTermina = suscripcion.FechaTermina
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Suscripcion Actualizar FechaTermina", ex);
			}
		}
	}
}
