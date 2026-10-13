#nullable disable

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Text;
using Image = SixLabors.ImageSharp.Image;

namespace Herramientas.Ficheros
{
	public static class Imagenes
	{
		public static string ServidorExterno(string enlace, int ancho = 0, int alto = 0)
		{
			if (string.IsNullOrEmpty(enlace) == false)
			{
				if (enlace.Contains("https://pepe.deals") == true)
				{
					enlace = enlace.Replace("https://pepe.deals", null);
				}

				//bool añadirServidor = true;

				//if (enlace.Contains("https://hb.imgix.net/") == true)
				//{
				//	añadirServidor = false;
				//}
				//else if (enlace.Contains("https://i.imgur.com/") == true)
				//{
				//	añadirServidor = false;
				//}
    //            else if (enlace.Contains("https://media.greenmangamingbundles.com/") == true)
    //            {
    //                añadirServidor = false;
    //            }

    //            if (añadirServidor == true)
				//{
				//	//enlace = "https://wsrv.nl/?n=-1&output=webp&url=" + enlace;

				//	//if (ancho > 0 && alto > 0)
				//	//{
				//	//	enlace = enlace + "&w=" + ancho + "&h=" + alto + "&dpr=2";
				//	//}
				//}
			}

			return enlace;
		}
	}

	public static class Constructor
	{
		public enum LayoutModo
		{
			Vertical,  
			Horizontal 
		}

		public enum ImagenModo
		{
			Juego,
			Bundle,
			Web
		}

		public static async Task<CombinarImagenResultado> Imagen(List<string> imagenesEnlaces, string encabezadoEnlace = null, ImagenModo modoImagen = ImagenModo.Juego)
		{
			LayoutModo modo = LayoutModo.Vertical;

			if (imagenesEnlaces?.Count <= 3)
			{
				modo = LayoutModo.Horizontal;
			}

			if (modoImagen == ImagenModo.Bundle || modoImagen == ImagenModo.Web)
			{
				modo = LayoutModo.Vertical;
			}

			try
			{
				Image encabezadoImagen = null;
				int encabezadoAncho = 0;
				int encabezadoAltura = 0;
				int encabezadoPadding = 40;
				int encabezadoAlturaMaxima = 150;
				int encabezadoAnchoMaximo = 250;
				int separadorAltura = 2;
				int separadorAncho = 2;

				if (modoImagen == ImagenModo.Bundle)
				{
					encabezadoAlturaMaxima = 75;
					encabezadoAnchoMaximo = 150;
					separadorAltura = 0; 
					separadorAncho = 0;
				}
				else if (modoImagen == ImagenModo.Web)
				{
					encabezadoAlturaMaxima = 120;
					encabezadoAnchoMaximo = 120;
					separadorAltura = 0;
					separadorAncho = 0;
				}

				if (string.IsNullOrEmpty(encabezadoEnlace) == false)
				{
					try
					{
						encabezadoImagen = await CargarImagen(encabezadoEnlace);

						if (modo == LayoutModo.Vertical)
						{
							int finalEncabezadoAltura = Math.Min(encabezadoImagen.Height, encabezadoAlturaMaxima);
							int paddingExtra = modoImagen == ImagenModo.Bundle || modoImagen == ImagenModo.Web ? (int)(encabezadoPadding * 1.25) : encabezadoPadding * 2;
							encabezadoAltura = finalEncabezadoAltura + paddingExtra + separadorAltura;
						}
						else
						{
							int finalEncabezadoAncho = Math.Min(encabezadoImagen.Width, encabezadoAnchoMaximo);
							encabezadoAncho = finalEncabezadoAncho + (encabezadoPadding * 2) + separadorAncho;
						}
					}
					catch (Exception ex)
					{
						global::BaseDatos.Errores.Insertar.Mensaje("Error descarga imagen de encabezado", ex.Message);

						return new CombinarImagenResultado
						{
							Exito = false,
							ErrorMensaje = $"Error al descargar imagen de encabezado: {ex.Message}",
							RequestMalo = true
						};
					}
				}

				List<Image> imagenes = new List<Image>();
				HttpClient httpClient = new HttpClient();

				foreach (string imagenEnlace in imagenesEnlaces)
				{
					try
					{
						imagenes.Add(await CargarImagen(imagenEnlace));
					}
					catch (Exception ex)
					{
						global::BaseDatos.Errores.Insertar.Mensaje("Error descarga imagen", ex.Message);

						return new CombinarImagenResultado
						{
							Exito = false,
							ErrorMensaje = $"Error al descargar imagen de {imagenEnlace}: {ex.Message}",
							RequestMalo = true
						};
					}
				}

				if (imagenes.Count == 0)
				{
					return new CombinarImagenResultado
					{
						Exito = false,
						ErrorMensaje = "No se pudieron descargar las imágenes",
						RequestMalo = true
					};
				}

				int imagenAncho = 300;
				int imagenAltura = 440;
				int padding = 30;
				int columnasPorFila = 4;

				if (modoImagen == ImagenModo.Bundle)
				{
					imagenAncho = 720;
					imagenAltura = 405;
					columnasPorFila = 1;
				}
				else if (modoImagen == ImagenModo.Web)
				{
					imagenAncho = 800;
					imagenAltura = 400;
					columnasPorFila = 1;
				}

				int actualesColumnas = modo == LayoutModo.Horizontal ? Math.Min(imagenes.Count, columnasPorFila) : columnasPorFila;
				int filas = (int)Math.Ceiling((double)imagenes.Count / actualesColumnas);
				int gridAncho = (imagenAncho * actualesColumnas) + (padding * (actualesColumnas + 1));
				int gridAltura = (imagenAltura * filas) + (padding * (filas + 1));

				int totalAncho = modo == LayoutModo.Vertical ? gridAncho : gridAncho + encabezadoAncho;
				int totalAltura = modo == LayoutModo.Vertical ? gridAltura + encabezadoAltura : gridAltura;

				var imagenCombinada = new Image<Rgba32>(totalAncho, totalAltura);

				var empiezaColorRgba = new Rgba32(0, 32, 51, 255); // #002033
				//var terminarColorRgba = new Rgba32(0, 44, 71, 255);   // #002c47
				var terminarColorRgba = new Rgba32(0, 32, 51, 255);

				double anguloRadianes = 66 * Math.PI / 180;
				double cosAngulo = Math.Cos(anguloRadianes);
				double sinAngulo = Math.Sin(anguloRadianes);
				double distanciaMaxima = Math.Sqrt(totalAncho * totalAncho + totalAltura * totalAltura);

				imagenCombinada.Mutate(ctx =>
				{
					for (int y = 0; y < totalAltura; y++)
					{
						for (int x = 0; x < totalAncho; x++)
						{
							double distancia = (x * cosAngulo) + (y * sinAngulo);
							float ratio = (float)Math.Clamp(distancia / distanciaMaxima, 0, 1);

							var colorFondo = new Rgba32(
								(byte)((1 - ratio) * empiezaColorRgba.R + ratio * terminarColorRgba.R),
								(byte)((1 - ratio) * empiezaColorRgba.G + ratio * terminarColorRgba.G),
								(byte)((1 - ratio) * empiezaColorRgba.B + ratio * terminarColorRgba.B),
								255
							);

							imagenCombinada[x, y] = colorFondo;
						}
					}
				});

				if (encabezadoImagen != null)
				{
					if (modo == LayoutModo.Vertical)
					{
						ProcesarEncabezadoVertical(ref imagenCombinada, encabezadoImagen, totalAncho, encabezadoPadding, encabezadoAlturaMaxima, separadorAltura, modoImagen);
					}
					else
					{
						ProcesarEncabezadoHorizontal(ref imagenCombinada, encabezadoImagen, gridAltura, encabezadoPadding, encabezadoAnchoMaximo, separadorAncho);
					}

					encabezadoImagen.Dispose();
				}

				int gridArranqueX = modo == LayoutModo.Horizontal ? encabezadoAncho : 0;
				int gridArranqueY = modo == LayoutModo.Vertical ? encabezadoAltura : 0;

				int xPosicion = gridArranqueX + padding;
				int yPosicion = gridArranqueY + padding;

				for (int i = 0; i < imagenes.Count; i++)
				{
					var img = imagenes[i];
					var imagenRedimensionada = img.Clone(x => x.Resize(imagenAncho, imagenAltura));
					imagenCombinada.Mutate(ctx => ctx.DrawImage(imagenRedimensionada, new SixLabors.ImageSharp.Point(xPosicion, yPosicion), 1));

					xPosicion += imagenAncho + padding;
					imagenRedimensionada.Dispose();

					if ((i + 1) % actualesColumnas == 0)
					{
						xPosicion = gridArranqueX + padding;
						yPosicion += imagenAltura + padding;
					}
				}

				string nombreFichero = $"{Guid.NewGuid()}.jpeg";
				string rutaDirectorio = Path.Combine(AppContext.BaseDirectory, "imagenes", "noticias");

				if (Directory.Exists(rutaDirectorio) == false)
				{
					Directory.CreateDirectory(rutaDirectorio);
				}

				string rutaFichero = Path.Combine(rutaDirectorio, nombreFichero);

				try
				{
					await imagenCombinada.SaveAsJpegAsync(rutaFichero);

					foreach (var img in imagenes)
					{
						img.Dispose();
					}
						
					imagenCombinada.Dispose();

					return new CombinarImagenResultado
					{
						Exito = true,
						DirectorioFichero = $"/imagenes/noticias/{nombreFichero}"
					};
				}
				catch (Exception ex)
				{
					return new CombinarImagenResultado
					{
						Exito = false,
						ErrorMensaje = $"Error al guardar la imagen: {ex.Message}",
						RequestMalo = false
					};
				}
			}
			catch (Exception ex)
			{
				global::BaseDatos.Errores.Insertar.Mensaje("Error combinar imagenes", ex.Message);

				return new CombinarImagenResultado
				{
					Exito = false,
					ErrorMensaje = $"Error al combinar imágenes: {ex.Message}",
					RequestMalo = false
				};
			}
		}

		private static readonly HttpClient clienteImagenes = CrearCliente();

		private static HttpClient CrearCliente()
		{
			var cliente = new HttpClient(new HttpClientHandler
			{
				AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.Brotli
			});
			cliente.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:157.0) Gecko/20100101 Firefox/157.0");
			cliente.DefaultRequestHeaders.Accept.ParseAdd("image/webp,image/png,image/jpeg,image/gif;q=0.9");
			return cliente;
		}

		public static class PeticionInterna
		{
			public static string Clave { get; set; }

			public static bool Es(HttpContext contexto)
			{
				string recibida = contexto.Request.Headers["X-Interno"].ToString();

				if (string.IsNullOrEmpty(Clave) == true || string.IsNullOrEmpty(recibida) == true)
				{
					return false;
				}

				return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(recibida), Encoding.UTF8.GetBytes(Clave));
			}
		}

		public static async Task<Image> CargarImagen(string enlace)
		{
			string relativa = enlace;
		
			if (relativa.StartsWith("https://pepe.deals") == true)
			{
				relativa = relativa.Substring("https://pepe.deals".Length);
			}

			if (relativa.StartsWith("/imagenes/") == true && relativa.Contains("..") == false)
			{
				string rutaRelativa = relativa.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);

				foreach (string baseRuta in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
				{
					string ruta = Path.Combine(baseRuta, rutaRelativa);

					if (File.Exists(ruta) == true)
					{
						return await Image.LoadAsync(ruta);
					}
				}
			}

			if (Uri.TryCreate(enlace, UriKind.Absolute, out Uri absoluta) == false)
			{
				absoluta = new Uri(new Uri("https://pepe.deals"), enlace);
			}

			using var peticion = new HttpRequestMessage(HttpMethod.Get, absoluta);

			if (absoluta.Host == "pepe.deals")
			{
				peticion.Headers.Add("X-Interno", PeticionInterna.Clave);
			}

			using var respuesta = await clienteImagenes.SendAsync(peticion);
			respuesta.EnsureSuccessStatusCode();
			using var stream = await respuesta.Content.ReadAsStreamAsync();
			return await Image.LoadAsync(stream);
		}

		private static void ProcesarEncabezadoVertical(ref Image<Rgba32> imagenCombinada, Image encabezadoImagen, int totalAncho, int padding, int alturaMaxima, int separadorAltura, ImagenModo modoImagen)
		{
			int encabezadoMaximoAncho = totalAncho - (padding * 2);
			double ratio = (double)encabezadoImagen.Width / encabezadoImagen.Height;

			int nuevoEncabezadoAncho = encabezadoMaximoAncho;
			int nuevoEncabezadoAltura = (int)(encabezadoMaximoAncho / ratio);

			if (nuevoEncabezadoAltura > alturaMaxima)
			{
				nuevoEncabezadoAltura = alturaMaxima;
				nuevoEncabezadoAncho = (int)(alturaMaxima * ratio);
			}

			var encabezadoRedimensionado = encabezadoImagen.Clone(x => x.Resize(nuevoEncabezadoAncho, nuevoEncabezadoAltura));
			int offsetX = (totalAncho - nuevoEncabezadoAncho) / 2;
			int offsetY = padding;

			imagenCombinada.Mutate(ctx => ctx.DrawImage(encabezadoRedimensionado, new SixLabors.ImageSharp.Point(offsetX, offsetY), 1));

			if (modoImagen == ImagenModo.Juego)
			{
				//int separadorY = offsetY + nuevoEncabezadoAltura + padding;
				//int separadorMargin = 30;
				//for (int x = separadorMargin; x < totalAncho - separadorMargin; x++)
				//{
				//	imagenCombinada[x, separadorY] = new Rgba32(100, 100, 100, 255);
				//}
			}

			encabezadoRedimensionado.Dispose();
		}

		private static void ProcesarEncabezadoHorizontal(ref Image<Rgba32> imagenCombinada, Image encabezadoImagen, int gridAltura, int padding, int anchoMaximo, int separadorAncho)
		{
			int encabezadoMaximaAltura = gridAltura - (padding * 2);
			double ratio = (double)encabezadoImagen.Width / encabezadoImagen.Height;

			int nuevoEncabezadoAncho = (int)(encabezadoMaximaAltura * ratio);
			int nuevoEncabezadoAltura = encabezadoMaximaAltura;

			if (nuevoEncabezadoAncho > anchoMaximo)
			{
				nuevoEncabezadoAncho = anchoMaximo;
				nuevoEncabezadoAltura = (int)(anchoMaximo / ratio);
			}

			var encabezadoRedimensionado = encabezadoImagen.Clone(x => x.Resize(nuevoEncabezadoAncho, nuevoEncabezadoAltura));
			int offsetX = padding;
			int offsetY = (gridAltura - nuevoEncabezadoAltura) / 2;

			imagenCombinada.Mutate(ctx => ctx.DrawImage(encabezadoRedimensionado, new SixLabors.ImageSharp.Point(offsetX, offsetY), 1));

			encabezadoRedimensionado.Dispose();
		}
	}

	public class CombinarImagenResultado
	{
		public bool Exito { get; set; }
		public string ErrorMensaje { get; set; } = string.Empty;
		public bool RequestMalo { get; set; }
		public string DirectorioFichero { get; set; }
	}
}
