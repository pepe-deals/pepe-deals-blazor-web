#nullable disable

using APIs.Steam;
using Dapper;
using Juegos;
using System.Text.Json;

namespace BaseDatos.Reseñas
{
	public static class Buscar
	{
		public static async Task<JuegoReseñasAmpliado> Cargar(int id, string idioma)
		{
			try
			{
				var fila = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync($"SELECT contenido{idioma} as Contenido, positivos{idioma} as CantidadPositivos, negativos{idioma} as CantidadNegativos FROM juegosAnalisis WHERE id=@id", new { id });
				});

				JuegoReseñasAmpliado reseñas = new JuegoReseñasAmpliado();

				if (fila != null)
				{
					if (!string.IsNullOrEmpty(fila.Contenido))
					{
						reseñas.Contenido = JsonSerializer.Deserialize<List<SteamReseñasApiReseña>>(fila.Contenido);
					}

					reseñas.CantidadPositivos = fila.CantidadPositivos ?? 0;
					reseñas.CantidadNegativos = fila.CantidadNegativos ?? 0;
				}

				return reseñas;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Reseñas Cargar", ex);
			}

			return null;
		}

		public static async Task<bool> DebeModificarse(int id, string idioma)
		{
			try
			{
				DateTime? fechaRegistrada = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.ExecuteScalarAsync<DateTime?>($"SELECT fecha{idioma} FROM juegosAnalisis WHERE id=@id", new { id });
				});

				if (fechaRegistrada.HasValue == false)
				{
					return true;
				}

				return fechaRegistrada.Value.AddDays(7) < DateTime.Now;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Reseñas Debe Modificarse", ex);
			}

			return false;
		}
	}
}
