#nullable disable

using APIs.Steam;
using Dapper;
using System.Text.Json;

namespace BaseDatos.Curators
{
	public static class Insertar
	{
		public static async void Ejecutar(SteamCuratorAPI api)
		{
			if (api != null)
			{
				if (string.IsNullOrEmpty(api.Nombre) == false || string.IsNullOrEmpty(api.Slug) == false)
				{
					string nombre = string.IsNullOrEmpty(api.Nombre) ? api.Slug : api.Nombre; 
					string slug = string.IsNullOrEmpty(api.Slug) ? Herramientas.EnlaceAdaptador.Nombre(api.Nombre) : api.Slug; 
					
					try {
						await Herramientas.Conexiones.Pepedeals.RestoOperaciones(async (conexion, sentencia) =>
						{
							return await conexion.ExecuteAsync(@"INSERT INTO curators (idSteam, nombre, imagen, descripcion, slug, steamIds, web, fecha) VALUES (@idSteam, @nombre, @imagen, @descripcion, @slug, @steamIds, @web, @fecha)", new
							{
								idSteam = api.Id,
								nombre,
								imagen = api.Imagen,
								descripcion = api.Descripcion,
								slug,
								steamIds = JsonSerializer.Serialize(api.SteamIds),
								web = JsonSerializer.Serialize(api.Web),
								fecha = DateTime.Now
							}, transaction: sentencia);
						});
					} 
					catch (Exception ex) 
					{ 
						BaseDatos.Errores.Insertar.Mensaje("Curator Insertar", ex); 
					}
				}
			}
		}
	}
}
