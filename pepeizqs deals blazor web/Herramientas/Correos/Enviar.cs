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

				MimeMessage mimeMensaje = new MimeMessage();
				mimeMensaje.From.Add(new MailboxAddress("pepe's deals", correoDesde));
				mimeMensaje.To.Add(MailboxAddress.Parse(correoHacia));
				mimeMensaje.Subject = titulo;
				mimeMensaje.Body = new TextPart("html") { Text = html };
				mimeMensaje.Headers.Add("List-Unsubscribe", "<https://pepe.deals/account/notifications-email/>");
				mimeMensaje.Headers.Add("List-Unsubscribe-Post", "List-Unsubscribe=One-Click");

				using SmtpClient smtpMailKit = new SmtpClient();

				if (correoDesde.ToLower().Contains("gmail.com") == true)
				{
					try
					{
						await smtpMailKit.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
						await smtpMailKit.AuthenticateAsync(correoDesde, contraseña);
						await smtpMailKit.SendAsync(mimeMensaje);
						await smtpMailKit.DisconnectAsync(true);
						return true;
					}
					catch (Exception ex)
					{
						DateTime nuevaFecha = DateTime.Now;
						nuevaFecha = nuevaFecha.AddMinutes(10);

						await BaseDatos.Admin.Actualizar.TareaUso("correosEnviar", nuevaFecha);

						BaseDatos.Errores.Insertar.Mensaje("Correo Enviar", ex, false);
						return false;
					}
				}
				else
				{
					try
					{
						await smtpMailKit.ConnectAsync(host, 8889, SecureSocketOptions.None);
						await smtpMailKit.AuthenticateAsync(correoDesde, contraseña);
						await smtpMailKit.SendAsync(mimeMensaje);
						await smtpMailKit.DisconnectAsync(true);
						return true;
					}
					catch (Exception ex)
					{
						DateTime nuevaFecha = DateTime.Now;
						nuevaFecha = nuevaFecha.AddMinutes(10);

						await BaseDatos.Admin.Actualizar.TareaUso("correosEnviar", nuevaFecha);

						BaseDatos.Errores.Insertar.Mensaje("Correo Enviar " + correoDesde + " - " + correoHacia, ex, false);

						return false;
					}
				}
			}

			return false;
		}
	}
}
