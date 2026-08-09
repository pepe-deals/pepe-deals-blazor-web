#nullable disable

using Bundles2;
using pepeizqs_deals_web.Data;
using System.Text.Json;
using Tiendas2;

namespace Herramientas.Correos
{
	public class CorreoDeseadoBundleJson
	{
		public string BundleNombre { get; set; }
		public string BundleImagen { get; set; }
		public string BundleEnlace { get; set; }
		public string TiendaNombre { get; set; }
		public string NombreJuego { get; set; }
		public string ImagenJuego { get; set; }
		public string Idioma { get; set; }
	}

	public static class DeseadoBundle
	{
		public static async Task Nuevo(string usuarioId, Bundle bundle, BundleJuego juego, string correoHacia)
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
<body style=""margin:0; padding:0; background-color:#222b44;"">

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#222b44;"">
<tr>
<td align=""center"">

<table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""font-family:Arial, Helvetica, sans-serif; font-size:16px; color:#f5f5f5;"">
<tr>
<td style=""padding:40px;"">

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#293751;"">
<tr>
<td width=""200"" style=""padding:20px;"">
<img src=""{{imagenJuego}}"" width=""200"" style=""display:block; max-width:200px; border:0;"" />
</td>

<td style=""padding:20px; line-height:25px;"">
{{mensajeAviso}}
</td>
</tr>
</table>

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-top:40px;"">
<tr>
<td>
<a href=""{{enlace}}"" target=""_blank"">
<img src=""{{imagenBundle}}"" width=""100%"" style=""display:block; border:0; max-width:100%;"" />
</a>
</td>
</tr>
</table>

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-top:20px;"">
<tr>
<td align=""center"" style=""background-color:#293751;"">
<a href=""{{enlace}}"" target=""_blank"" style=""display:block; padding:15px; color:#95c0fe; text-decoration:none; font-size:17px;"">
{{mensajeAbrir}}
</a>
</td>
</tr>
</table>

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-top:40px; font-size:14px;"">
<tr>
<td>
&copy; {{año}} • 
<a href=""https://pepeizqapps.com/"" target=""_blank"" style=""color:#95c0fe;"">pepeizq's apps</a> • 
<a href=""https://pepe.deals/"" target=""_blank"" style=""color:#95c0fe;"">pepe's deals</a>
</td>
</tr>

<tr>
<td style=""padding-top:20px;"">
{{mensaje}} 
<a href=""https://pepe.deals/contact"" target=""_blank"" style=""color:#95c0fe;"">/contact/</a>
</td>
</tr>
</table>

</td>
</tr>
</table>

</td>
</tr>
</table>

</body>
</html>";

			string enlace = Herramientas.EnlaceAcortador.Generar(regionUsar, bundle.Enlace, bundle.BundleTipo, false, false);
			html = html.Replace("{{enlace}}", enlace);
			html = html.Replace("{{imagenJuego}}", juego.Imagen);

			string mensajeAviso = null;
			if (bundle.Nombre.ToLower().Contains("bundle") == true)
			{
				mensajeAviso = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String1", "Mails"), juego.Nombre, bundle.Tienda, bundle.Nombre);
			}
			else
			{
				mensajeAviso = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String2", "Mails"), juego.Nombre, bundle.Tienda, bundle.Nombre);
			}

			html = html.Replace("{{mensajeAviso}}", mensajeAviso);
			html = html.Replace("{{imagenBundle}}", bundle.ImagenNoticia);
			html = html.Replace("{{mensajeAbrir}}", Herramientas.Idiomas.BuscarTexto(idioma, "String3", "Mails"));

			html = html.Replace("{{año}}", DateTime.Now.Year.ToString());
			html = html.Replace("{{mensaje}}", Herramientas.Idiomas.BuscarTexto(idioma, "Message", "Mails"));

			CorreoDeseadoBundleJson json = new CorreoDeseadoBundleJson();
			json.BundleNombre = bundle.Nombre;
			json.BundleEnlace = enlace;
			json.BundleImagen = bundle.ImagenNoticia;
			json.TiendaNombre = bundle.Tienda;
			json.NombreJuego = juego.Nombre;
			json.ImagenJuego = juego.Imagen;
			json.Idioma = idioma;

			await global::BaseDatos.CorreosEnviar.Insertar.Ejecutar(html, mensajeAviso, "mail@pepe.deals", correoHacia, DateTime.Now, global::BaseDatos.CorreosEnviar.CorreoPendienteTipo.DeseadoBundle, JsonSerializer.Serialize(json));
		}

		public static async Task Nuevos(List<CorreoDeseadoBundleJson> jsons, string correoHacia, DateTime horaOriginal)
		{
			if (jsons?.Count > 0)
			{
				string idioma = jsons[0].Idioma;

				if (string.IsNullOrEmpty(idioma) == true)
				{
					idioma = "en";
				}

				string titulo = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String4", "Mails"), jsons.Count, jsons[0].BundleNombre);

				string html = @"<!DOCTYPE html>
<html>
<head>
	<meta charset=""utf-8"" />
	<title></title>
</head>
<body style=""margin:0; padding:0; background-color:#222b44;"">

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#222b44;"">
<tr>
<td align=""center"">

<table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""font-family:Arial, Helvetica, sans-serif; font-size:16px; color:#f5f5f5;"">
<tr>
<td style=""padding:40px;"">

<table width=""100%"" cellpadding=""0"" cellspacing=""0"">
<tr>
<td style=""font-size:18px; line-height:25px;"">
{{descripcion}}
</td>
</tr>
</table>";

				foreach (var json in jsons)
				{
					string htmlJson = @"<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-top:20px; background-color:#293751;"">
<tr>
<td width=""200"" style=""padding:20px;"">
<img src=""{{imagenJuego}}"" width=""200"" style=""display:block; max-width:200px; border:0;"" />
</td>

<td style=""padding:20px;"">
{{nombreJuego}}
</td>
</tr>
</table>";

					htmlJson = htmlJson.Replace("{{imagenJuego}}", json.ImagenJuego);
					htmlJson = htmlJson.Replace("{{nombreJuego}}", json.NombreJuego);

					html = html + htmlJson;
				}

				html = html + @"<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-top:30px;"">
<tr>
<td>
<a href=""{{enlaceBundle}}"" target=""_blank"">
<img src=""{{imagenBundle}}"" width=""100%"" style=""display:block; border:0; max-width:100%;"" />
</a>
</td>
</tr>
</table>

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-top:20px;"">
<tr>
<td align=""center"" style=""background-color:#293751;"">
<a href=""{{enlaceBundle}}"" target=""_blank"" style=""display:block; padding:15px; color:#95c0fe; text-decoration:none; font-size:17px;"">
{{mensajeAbrir}}
</a>
</td>
</tr>
</table>

<table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-top:40px; font-size:14px;"">
<tr>
<td>
&copy; {{año}} • 
<a href=""https://pepeizqapps.com/"" target=""_blank"" style=""color:#95c0fe;"">pepeizq's apps</a> • 
<a href=""https://pepe.deals/"" target=""_blank"" style=""color:#95c0fe;"">pepe's deals</a>
</td>
</tr>

<tr>
<td style=""padding-top:20px;"">
{{mensaje}} 
<a href=""https://pepe.deals/contact"" target=""_blank"" style=""color:#95c0fe;"">/contact/</a>
</td>
</tr>
</table>

</td>
</tr>
</table>

</td>
</tr>
</table>

</body>
</html>";


				string descripcion = string.Format(Herramientas.Idiomas.BuscarTexto(idioma, "String4", "Mails"), jsons[0].TiendaNombre, jsons.Count);

				html = html.Replace("{{descripcion}}", descripcion);
				html = html.Replace("{{enlaceBundle}}", jsons[0].BundleEnlace);
				html = html.Replace("{{imagenBundle}}", jsons[0].BundleImagen);
				html = html.Replace("{{mensajeAbrir}}", Herramientas.Idiomas.BuscarTexto(idioma, "String3", "Mails"));

				html = html.Replace("{{año}}", DateTime.Now.Year.ToString());
				html = html.Replace("{{mensaje}}", Herramientas.Idiomas.BuscarTexto(idioma, "Message", "Mails"));

				await global::BaseDatos.CorreosEnviar.Insertar.Ejecutar(html, titulo, "mail@pepe.deals", correoHacia, horaOriginal, global::BaseDatos.CorreosEnviar.CorreoPendienteTipo.DeseadosBundle, JsonSerializer.Serialize(jsons));
			}
		}
	}
}
