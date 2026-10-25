#nullable disable

using Dapper;
using Tiendas2;

namespace BaseDatos.Portada
{
	public static class Limpiar
	{
		public static async Task<bool> Total(TiendaTipo tipo, TiendaRegion region)
		{
			string tabla = string.Empty;
			string precioMinimosHistoricos = string.Empty;

			if (tipo == TiendaTipo.Oficial && region == TiendaRegion.Europa)
			{
				tabla = "seccionMinimos";
				precioMinimosHistoricos = "sm.precioMinimosHistoricos";
			}
			else if (tipo == TiendaTipo.Oficial && region == TiendaRegion.EstadosUnidos)
			{
				tabla = "seccionMinimosUS";
				precioMinimosHistoricos = "sm.precioMinimosHistoricosUS";
			}
			else if (tipo == TiendaTipo.NoOficial && region == TiendaRegion.Europa)
			{
				tabla = "seccionMinimosNoOficialesEU";
				precioMinimosHistoricos = "sm.preciosHistoricosNoOficialesEU";
			}
			else if (tipo == TiendaTipo.NoOficial && region == TiendaRegion.EstadosUnidos)
			{
				tabla = "seccionMinimosNoOficialesUS";
				precioMinimosHistoricos = "sm.preciosHistoricosNoOficialesUS";
			}
			else if (tipo == TiendaTipo.Marketplace && region == TiendaRegion.Europa)
			{
				tabla = "seccionMinimosMarketplacesEU";
				precioMinimosHistoricos = "sm.preciosHistoricosMarketplacesEU";
			}
			else if (tipo == TiendaTipo.Marketplace && region == TiendaRegion.EstadosUnidos)
			{
				tabla = "seccionMinimosMarketplacesUS";
				precioMinimosHistoricos = "sm.preciosHistoricosMarketplacesUS";
			}

			if (string.IsNullOrEmpty(tabla) == true || string.IsNullOrEmpty(precioMinimosHistoricos) == true)
			{
				return false;
			}

			string limpiar = $@"WHILE 1 = 1
				BEGIN
					DELETE TOP (500) sm
					FROM {tabla} sm
					CROSS APPLY OPENJSON({precioMinimosHistoricos}, '$[0]')
					WITH (
						FechaActualizacion DATETIME2 '$.FechaActualizacion',
						FechaTermina DATETIME2 '$.FechaTermina',
						Tienda NVARCHAR(50) '$.Tienda'
					) AS pmh
					WHERE
						-- Caducada por falta de actualizacion
						NOT (
							(pmh.Tienda IN ('steam', 'steambundles') AND pmh.FechaActualizacion >= DATEADD(hour, -24, GETDATE())) OR
							(pmh.Tienda IN ('humblestore', 'humblechoice') AND pmh.FechaActualizacion >= DATEADD(hour, -25, GETDATE())) OR
							(pmh.Tienda = 'epicgamesstore' AND pmh.FechaActualizacion >= DATEADD(hour, -48, GETDATE())) OR
							(pmh.FechaActualizacion >= DATEADD(hour, -12, GETDATE()))
						)
						-- Oferta ya terminada (mismo criterio que BuscarMinimos)
						OR (
							pmh.FechaTermina IS NOT NULL
							AND pmh.FechaTermina > '2020-01-01'
							AND pmh.FechaTermina <= GETDATE()
						);

					IF @@ROWCOUNT = 0 BREAK;
				END";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(limpiar, transaction: sentencia, commandTimeout: 300);
				});

				return true;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Portada Limpiar", ex, false);
			}

			return false;
		}
	}
}