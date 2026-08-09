#nullable disable

using Juegos;
using pepeizqs_deals_web.Data;
using System.Globalization;
using System.Text.Json;
using Tiendas2;

namespace Herramientas.Correos
{
	public class CorreoMinimoJson
	{
		public string Precio { get; set; }
		public string TiendaNombre { get; set; }
		public string TiendaIcono { get; set; }
		public string Nombre { get; set; }
		public string Imagen { get; set; }
		public string Enlace { get; set; }
		public string Descuento { get; set; }
		public string Idioma { get; set; }
		public int JuegoId { get; set; }
		public int JuegoDRM { get; set; }
	}

	public static class DeseadoMinimo
	{
		public static async Task Nuevo(string usuarioId, int idJuego, JuegoPrecio precio, string correoHacia)
		{
			Juego juego = await global::BaseDatos.Juegos.Buscar.UnJuego(idJuego);
			await Nuevo(usuarioId, juego, precio, correoHacia);
		}

		public static async Task Nuevo(string usuarioId, Juego juego, JuegoPrecio precio, string correoHacia)
		{
			Usuario opciones = await global::BaseDatos.Usuarios.Buscar.OpcionesDeseadoCorreo(usuarioId);
			string idioma = opciones.LanguageOverride;

			TiendaRegion regionUsar = (TiendaRegion)opciones.Currency;

			if (string.IsNullOrEmpty(idioma) == true)
			{
				idioma = "en";
			}

			string html = @"<!DOCTYPE html>
							<html>
							<head>
								<meta charset=""utf-8"" />
								<title></title>
							</head>
							<body>
								<div style=""min-width: 0; word-wrap: break-word; background-color: #222b44; background-clip: border-box; border: 0px; padding: 40px; font-family: Roboto, Helevtica, Arial, sans-serif, serif, EmojiFont; font-size: 16px; color: #f5f5f5;"">
									<div style=""line-height: 24px;"">
										{{descripcion}}
									</div>

									<div style=""margin-top: 40px;"">
										<a href=""{{enlace}}"" style=""color: #f5f5f5; background-color: #293751; display: inline-block; user-select: none; width: 100%; text-align: left; font-size: 16px; border: 0px; text-decoration: none; box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.28),0px 0px 2px 0px rgba(0, 0, 0, 0.24);"">
											<div style=""display: flex; align-content: center; align-items: center; justify-content: center; font-size: 18px;"">
												<img src=""{{imagen}}"" style=""max-width: 100%; max-height: 100%;"" />
											</div>
											<div style=""display: flex; align-content: center; align-items: center; justify-content: center; font-size: 18px; padding: 15px;"">
												<img src=""{{imagenTienda}}"" style=""width: 120px; margin-right: 10px;"" />
												<div style=""padding: 10px; background-color: darkgreen; box-shadow: rgba(0, 0, 0, 0.1) 0px 2px 3px 0px, rgba(0, 0, 0, 0.1) 0px 0px 2px 0px;"">
													{{descuento}}
												</div>
												<div style=""padding: 5px 10px;"">
													{{precio}}
												</div>
											</div>
										</a>
									</div>

									<div style=""margin-top: 20px;"">
										<a href=""{{enlace}}"" style=""color: #95c0fe; background-color: #293751; display: inline-block; user-select: none; width: 100%; text-align: center; border: 0px; box-shadow: 0px 4px 8px 0px rgba(0, 0, 0, 0.28),0px 0px 2px 0px rgba(0, 0, 0, 0.24);"">
											<div style=""display: flex; align-content: center; align-items: center; justify-content: center; font-size: 17px; margin: 15px;"">
												{{mensajeAbrir}}
											</div>
										</a>
									</div>

									<div style=""margin-top: 40px;"">
										<div>
											&copy; {{año}} • <a href=""https://pepeizqapps.com/"" style=""color: #95c0fe; user-select: none; width: 100%; text-align: left; font-size: 16px;"" target=""_blank"">pepeizq's apps</a> • <a href=""https://pepe.deals/"" style=""color: #95c0fe; user-select: none; width: 100%; text-align: left; font-size: 16px;"" target=""_blank"">pepe's deals</a>
										</div>
										<div style=""margin-top: 20px; font-size: 14px;"">
											{{mensaje}} <a href=""https://pepe.deals/contact"" style=""color: #95c0fe; user-select: none; width: 100%; text-align: left; font-size: 16px;"" target=""_blank"">/contact/</a>
										</div>
									</div>
								</div>
							</body>
							</html>";

			string precio2 = string.Empty;

			if (regionUsar == TiendaRegion.Europa)
			{
				precio2 = Herramientas.Precios.Euro(precio.Precio);

				if (precio.PrecioCambiado > 0)
				{
					precio2 = Herramientas.Precios.Euro(precio.PrecioCambiado);
				}
			}

			if (regionUsar == TiendaRegion.EstadosUnidos)
			{
				precio2 = Herramientas.Precios.Dolar(precio.Precio);

				if (precio.PrecioCambiado > 0)
				{
					precio2 = Herramientas.Precios.Dolar(precio.PrecioCambiado);
				}
			}

			string tiendaFinal = string.Empty;
			string imagenTienda = string.Empty;
			List<Tiendas2.Tienda> tiendas = Tiendas2.TiendasCargar.GenerarListado();

			foreach (var tienda in tiendas)
			{
				if (tienda.Id == precio.Tienda)
				{
					tiendaFinal = tienda.Nombre;
					imagenTienda = "https://pepe.deals/" + tienda.Imagen300x80;
				}
			}

			string titulo = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "Lows1", "Mails"), juego.Nombre, precio2, tiendaFinal);
			string descripcion = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "Lows2", "Mails"), juego.Nombre, tiendaFinal);
			string imagen = juego.Imagenes.Header_460x215;
			string descuento = precio.Descuento.ToString() + "%";
			string enlace = Herramientas.EnlaceAcortador.Generar(regionUsar, precio.Enlace, precio.Tienda, false, false);

			string mensajeAbrir = string.Empty;

			if (juego.Tipo == JuegoTipo.Game)
			{
				mensajeAbrir = Herramientas.Idiomas.BuscarTexto(idioma, "Lows3", "Mails");
			}
			else if (juego.Tipo == JuegoTipo.DLC)
			{
				mensajeAbrir = Herramientas.Idiomas.BuscarTexto(idioma, "Lows4", "Mails");
			}
			else
			{
				mensajeAbrir = Herramientas.Idiomas.BuscarTexto(idioma, "Lows5", "Mails");
			}

			html = html.Replace("{{descripcion}}", descripcion);
			html = html.Replace("{{imagen}}", imagen);
			html = html.Replace("{{descuento}}", descuento);
			html = html.Replace("{{precio}}", precio2);
			html = html.Replace("{{enlace}}", enlace);
			html = html.Replace("{{imagenTienda}}", imagenTienda);
			html = html.Replace("{{mensajeAbrir}}", mensajeAbrir);
			html = html.Replace("{{año}}", DateTime.Now.Year.ToString());
			html = html.Replace("{{mensaje}}", Herramientas.Idiomas.BuscarTexto(idioma, "Message", "Mails"));

			CorreoMinimoJson json = new CorreoMinimoJson();
			json.Precio = precio2;
			json.TiendaNombre = tiendaFinal;
			json.TiendaIcono = imagenTienda;
			json.Nombre = juego.Nombre;
			json.Imagen = imagen;
			json.Enlace = enlace;
			json.Descuento = descuento;
			json.Idioma = idioma;
			json.JuegoId = juego.Id;
			json.JuegoDRM = (int)juego.Tipo;

			await global::BaseDatos.CorreosEnviar.Insertar.Ejecutar(html, titulo, "mail@pepe.deals", correoHacia, DateTime.Now, global::BaseDatos.CorreosEnviar.CorreoPendienteTipo.Minimo, JsonSerializer.Serialize(json));
		}

		public static async Task Nuevos(List<CorreoMinimoJson> jsons, string correoHacia, DateTime horaOriginal)
		{
			decimal ParsePrecio(string precio)
			{
				if (string.IsNullOrWhiteSpace(precio))
				{
					return decimal.MaxValue;
				}

				string limpio = precio.Replace("€", null).Trim();

				if (decimal.TryParse(limpio, NumberStyles.Any, new CultureInfo("es-ES"), out var resultado))
				{
					return resultado;
				}

				return decimal.MaxValue;
			}

			if (jsons?.Count > 0)
			{
				string idioma = jsons[0].Idioma;

				if (string.IsNullOrEmpty(idioma) == true)
				{
					idioma = "en";
				}

				jsons = jsons.GroupBy(x => new { x.JuegoId, x.JuegoDRM }).Select(g => g.Aggregate((minimo, actual) => ParsePrecio(actual.Precio) < ParsePrecio(minimo.Precio) ? actual : minimo)).OrderBy(x => x.Nombre).ToList();

				string titulo = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "Lows6", "Mails"), jsons.Count);

				string html = @"<!DOCTYPE html>
							<html>
							<head>
								<meta charset=""utf-8"" />
								<title></title>
							</head>
							<body>
								<div style=""min-width: 0; word-wrap: break-word; background-color: #222b44; background-clip: border-box; border: 0px; padding: 40px; font-family: Roboto, Helevtica, Arial, sans-serif, serif, EmojiFont; font-size: 16px; color: #f5f5f5;"">
								";

				foreach (var json in jsons)
				{
					string htmlJson = @"<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom:20px; background-color:#0d1621; box-shadow:0px 4px 8px rgba(0,0,0,0.28);"">
  <tr>
    <td>
      <a href=""{{enlace}}"" target=""_blank"" style=""text-decoration:none; color:#f5f5f5;"">

        <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
          <tr>
            <td align=""center"">
              <img src=""{{imagen}}"" title=""{{nombreJuego}}"" style=""max-width:100%; max-height:300px; display:block;"" />
            </td>
          </tr>
        </table>

        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""padding:15px;"">
          <tr>
            <td align=""center"" style=""padding-right:10px;"">
              <img src=""{{imagenTienda}}"" style=""width:120px; display:block;"" />
            </td>

            <td align=""center"" style=""width: 70px;"">
              <div style=""padding:10px; background-color:darkgreen; color:#fff; font-size:17px;"">
                {{descuento}}
              </div>
            </td>

            <td align=""center"" style=""padding-left:10px; font-size:17px;"">
              {{precio}}
            </td>
          </tr>
        </table>

      </a>
    </td>
  </tr>
</table>";

					htmlJson = htmlJson.Replace("{{nombreJuego}}", json.Nombre);
					htmlJson = htmlJson.Replace("{{enlace}}", json.Enlace);
					htmlJson = htmlJson.Replace("{{imagen}}", json.Imagen);
					htmlJson = htmlJson.Replace("{{imagenTienda}}", json.TiendaIcono);
					htmlJson = htmlJson.Replace("{{descuento}}", json.Descuento);
					htmlJson = htmlJson.Replace("{{precio}}", json.Precio);

					html = html + htmlJson;
				}

				html = html + @"<div style=""margin-top: 40px;"">
									<div>
										&copy; {{año}} • <a href=""https://pepeizqapps.com/"" style=""color: #95c0fe; user-select: none; width: 100%; text-align: left; font-size: 16px;"" target=""_blank"">pepeizq's apps</a> • <a href=""https://pepe.deals/"" style=""color: #95c0fe; user-select: none; width: 100%; text-align: left; font-size: 16px;"" target=""_blank"">pepe's deals</a>
									</div>
									<div style=""margin-top: 20px; font-size: 14px;"">
										{{mensaje}} <a href=""https://pepe.deals/contact"" style=""color: #95c0fe; user-select: none; width: 100%; text-align: left; font-size: 16px;"" target=""_blank"">/contact/</a>
									</div>
								</div>
							</div>
						</body>
						</html>";

				html = html.Replace("{{año}}", DateTime.Now.Year.ToString());
				html = html.Replace("{{mensaje}}", Herramientas.Idiomas.BuscarTexto(idioma, "Message", "Mails"));

				await global::BaseDatos.CorreosEnviar.Insertar.Ejecutar(html, titulo, "mail@pepe.deals", correoHacia, horaOriginal, global::BaseDatos.CorreosEnviar.CorreoPendienteTipo.Minimos, JsonSerializer.Serialize(jsons));
			}
		}
	}
}
