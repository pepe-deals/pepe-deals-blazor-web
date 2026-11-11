#nullable disable

using Dapper;

namespace BaseDatos.CorreosEnviar
{
	public static class Buscar
	{
		public static async Task<List<CorreoPendienteEnviar>> PendientesEnviar()
		{
			try
			{
				string busqueda = "SELECT * FROM correosEnviar";

				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<CorreoPendienteEnviar>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Correos Enviar Buscar", ex);
			}

			return new List<CorreoPendienteEnviar>();
		}

		public static async Task<int> Cantidad()
		{
			try
			{
				string busqueda = "SELECT COUNT(*) FROM correosEnviar";

				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<int>(busqueda);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Correos Enviar Cantidad", ex);
			}

			return 0;
		}
	}

	public class CorreoPendienteEnviar
	{
		public int Id { get; set; }
		public string Html { get; set; }
		public string Titulo { get; set; }
		public string CorreoDesde { get; set; }
		public string CorreoHacia { get; set; }
		public CorreoPendienteTipo Tipo { get; set; }
		public string Json { get; set; }
		public DateTime Fecha { get; set; }
	}

	public enum CorreoPendienteTipo
	{
		Desconocido,
		Minimo,
		Minimos,
		Noticia,
		ContraseñaReseteada,
		ContraseñaOlvidada,
		ContraseñaCambio,
		CorreoCambio,
		CorreoConfirmacion,
		DeseadoBundle,
		DeseadosBundle
	}
}
