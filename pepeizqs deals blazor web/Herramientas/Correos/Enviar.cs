#nullable disable

using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Herramientas.Correos
{
	public static class Enviar
	{
		public static async Task<bool> Ejecutar(IConfiguration configuracion, string html, string titulo, string correoDesde, string correoHacia)
		{
			if (string.IsNullOrEmpty(html) == false &&
				string.IsNullOrEmpty(titulo) == false &&
				string.IsNullOrEmpty(correoDesde) == false &&
				string.IsNullOrEmpty(correoHacia) == false)
			{
				await BaseDatos.CorreosEnviar.Insertar.Registro(html, titulo, correoHacia);

				string host = configuracion.GetValue<string>("WebmasterDeals:CorreoBot");
				string contraseña = configuracion.GetValue<string>("WebmasterDeals:CorreoBotContraseña");

				MimeMessage mimeMessage = new MimeMessage();
				mimeMessage.From.Add(new MailboxAddress("pepe's deals", correoDesde));
				mimeMessage.To.Add(MailboxAddress.Parse(correoHacia));
				mimeMessage.Subject = titulo;
				mimeMessage.Body = new TextPart("html") { Text = html };

				using SmtpClient smtpMailKit = new SmtpClient();

				if (correoDesde.ToLower().Contains("gmail.com") == true)
				{
					try
					{
						await smtpMailKit.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
						await smtpMailKit.AuthenticateAsync(correoDesde, contraseña);
						await smtpMailKit.SendAsync(mimeMessage);
						await smtpMailKit.DisconnectAsync(true);
						return true;
					}
					catch (Exception ex)
					{
						DateTime nuevaFecha = DateTime.Now;
						nuevaFecha = nuevaFecha.AddMinutes(10);

						await global::BaseDatos.Admin.Actualizar.TareaUso("correosEnviar", nuevaFecha);

						global::BaseDatos.Errores.Insertar.Mensaje("Correo Enviar", ex, false);
						return false;
					}
				}
				else
				{
					try
					{
						await smtpMailKit.ConnectAsync(host, 8889, SecureSocketOptions.None);
						await smtpMailKit.AuthenticateAsync(correoDesde, contraseña);
						await smtpMailKit.SendAsync(mimeMessage);
						await smtpMailKit.DisconnectAsync(true);
						return true;
					}
					catch (Exception ex)
					{
						DateTime nuevaFecha = DateTime.Now;
						nuevaFecha = nuevaFecha.AddMinutes(10);

						await global::BaseDatos.Admin.Actualizar.TareaUso("correosEnviar", nuevaFecha);

						global::BaseDatos.Errores.Insertar.Mensaje("Correo Enviar " + correoDesde + " - " + correoHacia, ex, false);

						return false;
					}
				}
			}

			return false;
		}
	}
}
