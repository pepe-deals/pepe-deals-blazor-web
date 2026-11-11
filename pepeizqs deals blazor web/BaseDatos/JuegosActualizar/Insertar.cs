#nullable disable

using Dapper;

namespace BaseDatos.JuegosActualizar
{
	public static class Insertar
	{
		public static async Task Ejecutar(int idJuego, int idPlataforma, string metodo)
		{
			if (idJuego > 0 && idPlataforma > 0 && string.IsNullOrEmpty(metodo) == false)
			{
				if (await Buscar.Existe(idJuego, idPlataforma, metodo) == false)
				{
					string sqlAñadir = "INSERT INTO fichasActualizar " +
						 "(idJuego, idPlataforma, metodo) VALUES " +
						 "(@idJuego, @idPlataforma, @metodo) ";

					try
					{
						await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
						{
							return await conexion.ExecuteAsync(sqlAñadir, new
							{
								IdJuego = idJuego,
								IdPlataforma = idPlataforma,
								Metodo = metodo
							}, transaction: sentencia);
						});
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Insertar Fichas Actualizar", ex);
					}
				}
			}
		}
	}
}
