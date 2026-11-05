#nullable disable

using Bundles2;
using Dapper;

namespace BaseDatos.Bundles
{
	public static class Buscar
	{
		public static async Task<List<Bundle>> Actuales(int ordenamiento = 0, BundleTipo tipo = BundleTipo.Desconocido)
		{
			string busqueda = "SELECT * FROM bundles WHERE (GETDATE() BETWEEN fechaEmpieza AND fechaTermina)";

			if (tipo != BundleTipo.Desconocido)
			{
				busqueda = busqueda + " AND (bundleTipo=" + (int)tipo + ")";
			}

			if (ordenamiento == 0)
			{
				busqueda = busqueda + " ORDER BY DATEPART(YEAR,fechaTermina), DATEPART(MONTH,fechaTermina), DATEPART(DAY,fechaTermina)";
			}
			else if (ordenamiento == 1)
			{
				busqueda = busqueda + " ORDER BY DATEPART(YEAR,fechaEmpieza) DESC, DATEPART(MONTH,fechaEmpieza) DESC, DATEPART(DAY,fechaEmpieza) DESC";
			}
			else if (ordenamiento == 2)
			{
				busqueda = busqueda + " ORDER BY nombre";
			}
			else if (ordenamiento == 3)
			{
				busqueda = busqueda + " ORDER BY nombre DESC";
			}

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Bundle>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Actuales", ex);
			}

			return null;
		}

		public static async Task<List<Bundle>> Año(string año)
		{
			string busqueda = "SELECT * FROM bundles WHERE YEAR(fechaEmpieza) = " + año + " AND GETDATE() > fechaTermina ORDER BY nombre DESC";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Bundle>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Año", ex);
			}

			return new List<Bundle>();
		}

		public static async Task<Bundle> UnBundle(int bundleId)
		{
			string busqueda = "SELECT * FROM bundles WHERE id=@id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Bundle>(busqueda, new { id = bundleId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundle Uno", ex);
			}

			return null;
		}

		public static async Task<List<Bundle>> VariosBundles(int[] ids)
		{
			string busqueda = "SELECT * FROM Bundles WHERE Id IN @Ids";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Bundle>(busqueda, new { Ids = ids })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundle Varios 1", ex);
			}

			return null;
		}

		public static async Task<List<Bundle>> VariosBundles(List<int> ids)
		{
			if (ids == null || ids.Count == 0)
			{
				return null;
			}

			string busqueda = "SELECT * FROM Bundles WHERE Id IN @Ids";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Bundle>(busqueda, new { Ids = ids })).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundle Varios 2", ex);
			}

			return null;
		}

		public static async Task<List<Bundle>> Ultimos(int cantidad)
        {
			string busqueda = "SELECT TOP " + cantidad.ToString() + " * FROM bundles ORDER BY id DESC";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Bundle>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Ultimos", ex);
			}

			return null;
		}

		public static async Task<List<Bundle>> Aleatorios()
		{
			string busqueda = @"SELECT TOP 50 id, nombre FROM bundles ORDER BY NEWID()";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Bundle>(busqueda)).ToList();
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Aleatorios", ex);
			}

			return null;
		}
	}
}
