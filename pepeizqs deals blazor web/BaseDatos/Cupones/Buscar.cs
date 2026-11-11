#nullable disable

using Dapper;

namespace BaseDatos.Cupones
{
	public static class Buscar
	{
		public static async Task<Cupon> Activos(string tienda)
		{
			string sql = @"
				SELECT *
				FROM cupones
				WHERE fechaEmpieza < GETDATE() 
				  AND fechaTermina > GETDATE()
				  AND tienda = @tienda
			";

			try
			{
				return await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Cupon>(sql, new { tienda });
				});

			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Cupones Activos", ex, false);
			}

			return null;
		}
	}

	public class Cupon
	{
		public string Codigo { get; set; }
		public DateTime FechaEmpieza { get; set; }
		public DateTime FechaTermina { get; set; }
		public int? Porcentaje { get; set; }
		public decimal? PrecioMinimo { get; set; }
		public string TiendaID { get; set; }
		public decimal? PrecioRebaja { get; set; }
	}
}
