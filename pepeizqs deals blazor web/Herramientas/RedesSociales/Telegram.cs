#nullable disable

using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Herramientas.RedesSociales
{
	public static class Telegram
	{
		public static async Task<bool> Postear(IConfiguration configuracion, Noticias.Noticia noticia, string dominio, string imagenPostimage)
		{
			string token = configuracion.GetValue<string>("Telegram:Token");
			string chatId = configuracion.GetValue<string>("Telegram:ChatID");

			if (string.IsNullOrEmpty(token) == false)
			{
				TelegramBotClient cliente = new TelegramBotClient(token);

				string enlace = string.Empty;
				
				if (noticia.Id == 0)
				{
					enlace = "/link/news/" + noticia.IdMaestra.ToString() + "/";
				}
				else
				{
					enlace = "/link/news/" + noticia.Id.ToString() + "/";
				}

				if (string.IsNullOrEmpty(enlace) == false)
				{
					if (enlace.Contains("https://" + dominio) == false)
					{
						enlace = "https://" + dominio + enlace;
					}
				}

				bool error = false;
				
				try
				{
					await cliente.SendPhoto(chatId, InputFile.FromUri(WebUtility.HtmlDecode(imagenPostimage)), noticia.TituloEn + " " + enlace);

					return true;
				}
				catch
				{
					error = true;
				}

				if (error == true)
				{
					try
					{
						await cliente.SendMessage(chatId, noticia.TituloEn + Environment.NewLine + Environment.NewLine + enlace);

						return true;
					}
					catch
					{
						return false;
					}
				}
			}

			return false;
		}
	}
}
