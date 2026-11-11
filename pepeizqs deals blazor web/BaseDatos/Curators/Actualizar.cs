#nullable disable

using Dapper;
using System.Text.Json;

namespace BaseDatos.Curators
{
	public static class Actualizar
	{
		public static async void Ejecutar(Curator curator)
		{
			string añadirImagenFondo = null;

			if (string.IsNullOrEmpty(curator.ImagenFondo) == false)
			{
				añadirImagenFondo = ", imagenFondo=@imagenFondo";
			}

			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync($@"UPDATE curators SET nombre=@nombre, imagen=@imagen, descripcion=@descripcion, slug=@slug, steamIds=@steamIds, web=@web, fecha=@fecha {añadirImagenFondo} WHERE idSteam=@idSteam", new
					{
						idSteam = curator.IdSteam,
						nombre = curator.Nombre,
						imagen = curator.Imagen,
						descripcion = curator.Descripcion,
						slug = curator.Slug,
						steamIds = JsonSerializer.Serialize(curator.SteamIds),
						web = JsonSerializer.Serialize(curator.Web),
						fecha = DateTime.Now,
						imagenFondo = curator.ImagenFondo
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Curator Actualizar", ex);
			}
		}

		public static async void ImagenFondo(string imagenFondo, int id)
		{
			try
			{
				await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("UPDATE curators SET imagenFondo=@imagenFondo WHERE idSteam=@idSteam", new
					{
						imagenFondo,
						idSteam = id
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Curator Actualizar Imagen Fondo", ex);
			}
		}
	}
}
