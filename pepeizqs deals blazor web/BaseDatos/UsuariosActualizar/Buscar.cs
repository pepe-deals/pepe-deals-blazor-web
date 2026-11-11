#nullable disable

using Dapper;

namespace BaseDatos.UsuariosActualizar
{
	public static class Buscar
	{
		public static async Task<List<UsuarioActualizar>> Todos()
		{
			string busqueda = "SELECT * FROM usuariosActualizar";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<UsuarioActualizar>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuarios Actualizar Insertar", ex);
			}

			return null;
		}
	}

	public class UsuarioActualizar
	{
		public string IdUsuario { get; set; }
		public string Metodo { get; set; }
	}
}
