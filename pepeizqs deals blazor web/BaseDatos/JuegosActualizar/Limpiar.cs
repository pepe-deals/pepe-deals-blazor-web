#nullable disable

using Dapper;

namespace BaseDatos.JuegosActualizar
{
	public static class Limpiar
	{
		public static async Task Una(JuegoActualizar ficha)
		{
			if (ficha != null)
			{
				string sqlEliminar = "DELETE FROM fichasActualizar WHERE idJuego=@idJuego AND idPlataforma=@idPlataforma AND metodo=@metodo";

				try
				{
					await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(sqlEliminar, new
						{
							idJuego = ficha.IdJuego,
							idPlataforma = ficha.IdPlataforma,
							metodo = ficha.Metodo
						}, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Limpiar Fichas Actualizar", ex);
				}
			}
		}
	}
}
