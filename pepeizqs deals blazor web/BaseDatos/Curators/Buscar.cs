#nullable disable

using APIs.Steam;
using Dapper;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BaseDatos.Curators
{
	public static class Buscar
	{
		public static async Task<Curator> Uno(int idSteam)
		{
			try
			{
				var fila = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync("SELECT * FROM curators WHERE idSteam=@idSteam", new { idSteam });
				});

				if (fila == null)
				{
					return null;
				}

				Curator curator = new Curator
				{
					Id = fila.id,
					IdSteam = fila.idSteam,
					Nombre = fila.nombre,
					Imagen = fila.imagen,
					Descripcion = fila.descripcion,
					Slug = fila.slug,
					SteamIds = JsonSerializer.Deserialize<List<int>>(fila.steamIds),
					Web = JsonSerializer.Deserialize<SteamCuratorAPIWeb>(fila.web),
					DescripcionSEO = fila.descripcionSEO
				};

				curator.ImagenFondo = fila.imagenFondo != null ? (string)fila.imagenFondo : null;

				if (fila.fecha != null)
				{
					curator.Fecha = (DateTime)fila.fecha;
				}

				return curator;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Curator Buscar Uno", ex);
			}

			return null;
		}

		public static async Task<Curator> Uno(string slug)
		{
			try
			{
				var fila = await Herramientas.Conexiones.Pepedeals.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync("SELECT * FROM curators WHERE slug=@slug", new { slug });
				});

				if (fila == null)
				{
					return null;
				}

				Curator curator = new Curator
				{
					Id = fila.id,
					IdSteam = fila.idSteam,
					Nombre = fila.nombre,
					Imagen = fila.imagen,
					Descripcion = fila.descripcion,
					Slug = fila.slug,
					SteamIds = JsonSerializer.Deserialize<List<int>>(fila.steamIds),
					Web = JsonSerializer.Deserialize<SteamCuratorAPIWeb>(fila.web),
					DescripcionSEO = fila.descripcionSEO
				};

				curator.ImagenFondo = fila.imagenFondo != null ? (string)fila.imagenFondo : null;

				if (fila.fecha != null)
				{
					curator.Fecha = (DateTime)fila.fecha;
				}

				return curator;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Curator Buscar Uno", ex);
			}

			return null;
		}
	}

	public class Curator : ComponentBase, IComponent
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public int IdSteam { get; set; }
		public string Nombre { get; set; }
		public string Imagen { get; set; }
		public string Descripcion { get; set; }
		public string Slug { get; set; }
		public List<int> SteamIds { get; set; }
		public SteamCuratorAPIWeb Web { get; set; }
		public string ImagenFondo { get; set; }
		public DateTime? Fecha { get; set; }
		public string DescripcionSEO { get; set; }
	}

	public class CuratorFicha
	{
		public Curator Curator { get; set; }
		public int Posicion { get; set; }
		public List<global::Juegos.Juego> JuegosMostrar { get; set; }
	}
}
