#nullable disable

using Dapper;

namespace BaseDatos.Cupones
{
	public static class Insertar
	{
		public static async void Ejecutar(string codigo, int? porcentaje, decimal? precioMinimo, decimal? precioRebaja, string tienda, DateTime fechaEmpieza, DateTime fechaAcaba)
		{
			string colPorcentaje = (porcentaje.HasValue && porcentaje > 0) ? ", porcentaje" : "";
			string valPorcentaje = (porcentaje.HasValue && porcentaje > 0) ? ", @porcentaje" : "";

			string colMinimo = (precioMinimo.HasValue && precioMinimo > 0) ? ", precioMinimo" : "";
			string valMinimo = (precioMinimo.HasValue && precioMinimo > 0) ? ", @precioMinimo" : "";

			string colRebaja = (precioRebaja.HasValue && precioRebaja > 0) ? ", precioRebaja" : "";
			string valRebaja = (precioRebaja.HasValue && precioRebaja > 0) ? ", @precioRebaja" : "";

			string sql = @"
                INSERT INTO cupones
                (codigo, tienda, fechaEmpieza, fechaTermina"
				+ colPorcentaje + colMinimo + colRebaja + @")
                VALUES
                (@codigo, @tienda, @fechaEmpieza, @fechaTermina"
				+ valPorcentaje + valMinimo + valRebaja + @")
            ";

			try
			{
				var parametros = new
				{
					codigo,
					tienda,
					fechaEmpieza,
					fechaTermina = fechaAcaba,
					porcentaje = (porcentaje.HasValue && porcentaje > 0) ? porcentaje : null,
					precioMinimo = (precioMinimo.HasValue && precioMinimo > 0) ? precioMinimo : null,
					precioRebaja = (precioRebaja.HasValue && precioRebaja > 0) ? precioRebaja : null
				};

				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sql, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Cupones Insertar", ex);
			}
		}
	}
}
