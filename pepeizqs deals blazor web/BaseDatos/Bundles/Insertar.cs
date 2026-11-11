#nullable disable

using Dapper;
using Newtonsoft.Json;

namespace BaseDatos.Bundles
{
	public static class Insertar
	{
		public static async Task Ejecutar(Bundles2.Bundle bundle) 
		{
			List<string> campos = new List<string>
			{
				"bundleTipo", "nombre", "tienda", 
				"fechaEmpieza", "fechaTermina", "juegos", "tiers", "pick"
			};

			DynamicParameters parametros = new DynamicParameters();
			parametros.Add("@bundleTipo", bundle.BundleTipo);
			parametros.Add("@nombre", bundle.Nombre);
			parametros.Add("@tienda", bundle.Tienda);
			parametros.Add("@fechaEmpieza", bundle.FechaEmpieza);
			parametros.Add("@fechaTermina", bundle.FechaTermina);
			parametros.Add("@juegos", JsonConvert.SerializeObject(bundle.Juegos));
			parametros.Add("@tiers", JsonConvert.SerializeObject(bundle.Tiers));
			parametros.Add("@pick", bundle.Pick.ToString());

			if (string.IsNullOrEmpty(bundle.Enlace) == false && bundle.Enlace.StartsWith("https://"))
			{
				campos.Add("enlace");
				parametros.Add("@enlace", bundle.Enlace);
			}

			if (string.IsNullOrEmpty(bundle.Imagen) == false && bundle.Imagen.StartsWith("https://"))
			{
				campos.Add("imagen");
				parametros.Add("@imagen", bundle.Imagen);
			}

			if (string.IsNullOrEmpty(bundle.ImagenNoticia) == false && bundle.ImagenNoticia.StartsWith("https://"))
			{
				campos.Add("imagenNoticia");
				parametros.Add("@imagenNoticia", bundle.ImagenNoticia);
			}

			string camposStr = string.Join(", ", campos);
			string valoresStr = string.Join(", ", campos.Select(c => "@" + c));
			string sqlInsertar = $"INSERT INTO bundles ({camposStr}) VALUES ({valoresStr})";

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlInsertar, parametros, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Insertar 1", ex);
			}

			try
			{
				int id = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.QuerySingleOrDefaultAsync<int>("SELECT MAX(id) FROM bundles");
				});

				if (id <= 0 || bundle.Juegos == null || bundle.Juegos?.Count == 0)
				{
					return;
				}

				foreach (var juego in bundle.Juegos)
				{
					var juego2 = await Juegos.Buscar.UnJuego(juego.JuegoId);

					if (juego2 == null)
					{
						continue;
					}

					if (juego2.Bundles == null)
					{
						juego2.Bundles = new List<global::Juegos.JuegoBundle>();
					}

					var juegoBundle = new global::Juegos.JuegoBundle
					{
						DRM = juego.DRM,
						JuegoId = int.Parse(juego.JuegoId),
						Tier = juego.Tier,
						BundleId = id,
						FechaEmpieza = bundle.FechaEmpieza,
						FechaTermina = bundle.FechaTermina,
						Imagen = juego.Imagen,
						Nombre = juego.Nombre,
						Enlace = bundle.Enlace,
						Tipo = bundle.BundleTipo
					};

					juego2.Bundles.Add(juegoBundle);

					string sqlActualizarJuego = @"
						UPDATE juegos 
						SET bundles = @bundles
						WHERE id = @id;
					";

					var parametrosJuego = new
					{
						id = juego.JuegoId,
						bundles = JsonConvert.SerializeObject(juego2.Bundles)
					};

					await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(sqlActualizarJuego, parametrosJuego, transaction: sentencia);
					});
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Bundles Insertar 2", ex);
			}
		}
	}
}
