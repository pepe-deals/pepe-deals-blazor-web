#nullable disable

using System.Xml.Linq;

namespace Herramientas.Ficheros
{

	public static class PostImage
	{
		public static async Task<string> SubirImagen(string imagen)
		{
			try
			{
				var httpCliente = new HttpClient();
				httpCliente.Timeout = TimeSpan.FromSeconds(15);

				using var peticion = new HttpRequestMessage(HttpMethod.Get, imagen);
				peticion.Headers.Add("X-Interno", Herramientas.Ficheros.Constructor.PeticionInterna.Clave);

				using var imagenRespuesta = await httpCliente.SendAsync(peticion);
				imagenRespuesta.EnsureSuccessStatusCode();

				var imagenBytes = await imagenRespuesta.Content.ReadAsByteArrayAsync();

				var imagenBase64 = Convert.ToBase64String(imagenBytes);

				var contenido = new FormUrlEncodedContent(new[]
				{
						new KeyValuePair<string, string>("key",     "8ca0b57a6bb9c4c33cd9e7ab8e6a7f05"),
						new KeyValuePair<string, string>("o",       "2b819584285c102318568238c7d4a4c7"),
						new KeyValuePair<string, string>("m",       "fb733cccce28e7db3ff9f17d7ccff3d1"),
						new KeyValuePair<string, string>("version", "1.0.1"),
						new KeyValuePair<string, string>("name",    "image"),
						new KeyValuePair<string, string>("type",    "jpg"),
						new KeyValuePair<string, string>("image",   imagenBase64)
					});

				var respuesta = await httpCliente.PostAsync("http://api.postimage.org/1/upload", contenido);
				var xmlTexto = await respuesta.Content.ReadAsStringAsync();

				var xml = XDocument.Parse(xmlTexto);
				var hotlink = xml.Descendants("hotlink").FirstOrDefault()?.Value;

				return hotlink;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Postimage Subir Imagen", ex);
			}

			return null;
		}
	}
}
