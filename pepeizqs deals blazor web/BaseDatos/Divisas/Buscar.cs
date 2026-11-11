#nullable disable

using Dapper;
using Herramientas;
using pepeizqs_deals_web.Data;

namespace BaseDatos.Divisas
{
	public static class Buscar
	{
		public static Divisa Una(string id)
		{
			if (id.ToLower() == "eur")
			{
				return new Divisa
				{
					Id = "EUR",
					Cantidad = 1,
					Fecha = DateTime.Now
				};
			}

			if (string.IsNullOrEmpty(id) == false)
			{
				string sqlBuscar = "SELECT * FROM divisas WHERE id=@id";

				try
				{
					return Herramientas.Conexiones.Pepedeals.Select(conexion =>
					{
						return conexion.QueryFirstOrDefault<Divisa>(sqlBuscar, new { id });
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Divisa Buscar", ex, false);
				}
			}

			return null;
		}

		public static async Task<List<Divisa>> Todas()
		{
			string sqlBuscar = "SELECT * FROM divisas";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Divisa>(sqlBuscar)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Divisa Buscar Todas", ex, false);
			}

			return null;
		}
	}
}
