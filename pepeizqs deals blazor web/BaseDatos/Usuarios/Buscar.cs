#nullable disable

using Dapper;
using Juegos;
using pepeizqs_deals_web.Data;
using Tiendas2;

namespace BaseDatos.Usuarios
{
	public static class Buscar
	{
		public static async Task<bool> RolDios(string id)
		{
			if (string.IsNullOrEmpty(id) == true)
			{
				return false;
			}

			string sql = "SELECT Role FROM AspNetUsers WHERE Id = @Id";

			try
			{
				string rol = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(sql, new { Id = id });
				});

				if (string.IsNullOrEmpty(rol) == false)
				{
					if (rol == "God")
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Rol Dios", ex);
			}
			
			return false;
		}

		public static async Task<string> IdiomaSobreescribir(string usuarioId)
		{
			string idioma = "en";

			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return idioma;
			}

			string sql = "SELECT LanguageOverride FROM AspNetUsers WHERE Id = @Id";

			try
			{
				string idiomaBD = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(sql, new { Id = usuarioId });
				});

				if (string.IsNullOrEmpty(idiomaBD) == false)
				{
					idioma = idiomaBD;
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Idioma Sobreescribir", ex);
			}

			return idioma;
		}

		public static async Task<string> IdiomaJuegos(string usuarioId)
		{
			string idioma = "en";

			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return idioma;
			}

			string sql = "SELECT LanguageGames FROM AspNetUsers WHERE Id = @Id";

			try
			{
				string idiomaBD = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(sql, new { Id = usuarioId });
				});

				if (string.IsNullOrEmpty(idiomaBD) == false)
				{
					idioma = idiomaBD;
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Idioma Juegos", ex);
			}

			return idioma;
		}

		public static async Task<Usuario> JuegosTiene(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"
				SELECT 
					GamesExcluded,
					SteamGames, 
					GogGames, 
					AmazonGames, 
					EpicGames, 
					UbisoftGames, 
					EaGames
				FROM AspNetUsers 
				WHERE Id = @Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Juegos Tiene", ex);
			}

			return null;
		}

		public static async Task<DateTime?> FechaPatreon(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT PatreonLastCheck FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<DateTime?>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Fecha Patreon", ex);
			}

			return null;
		}

		public static async Task<Usuario> DeseadosTiene(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{ 
				return null;
			}

			string busqueda = "SELECT SteamWishlist, Wishlist, GogWishlist FROM AspNetUsers WHERE Id = @Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex) 
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Deseados Tiene", ex);
			}
				
			return null;
		}

		public static async Task<Usuario> OpcionesDeseadoCorreo(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT LanguageOverride, Currency FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Deseado Minimo", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesCabecera(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT Avatar, Email, Nickname, Currency, PatreonCoins, NoOfficial, Marketplace FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Cabecera", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesCabeceraDeseados(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT WishlistData FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Cabecera", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesPortada(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT IndexOption1, IndexOption2, IndexOption3, IndexOption4, IndexOption5, IndexOption6, IndexOption7, IndexOption8, IndexOption9, IndexOption10, IndexOption11, IndexDRMs, IndexCategories, ForumIndex, Currency, NoOfficial, Marketplace FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Portada", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesJuego(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT GameOption1, GameOption2, GameOption3, GameOption4, GameOption5, GameOption6, GameOption7, GameOption8, GameOption9, GameOption10, GameOption11, GameOption12, GameOption13, GameOption14, GameOption15, GameOption16, LanguageGames FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Juego", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesDeseados(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT Currency, WishlistSort, WishlistOption3, WishlistOption4, WishlistPosition, WishlistData FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Deseados", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesMinimos(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT Currency, HistoricalLowsOption1, HistoricalLowsOption4, HistoricalLowsOption2, HistoricalLowsOption3, HistoricalLowsDRMs, HistoricalLowsStores, HistoricalLowsTypes, HistoricalLowsTags, HistoricalLowsCategories2, HistoricalLowsSteamDeck, HistoricalLowsSteamOS, HistoricalLowsSort, HistoricalLowsRelease, HistoricalLowsAI FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Minimos", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesCuenta(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT EmailConfirmed, PatreonCoins, SteamAccountLastCheck, GogAccountLastCheck, AmazonLastImport, EpicGamesLastImport, UbisoftLastImport, EaLastImport FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Cuenta", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesIdioma(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT Language, LanguageOverride, LanguageGames FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Cuenta", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesApp(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"SELECT Language, EmailConfirmed, PatreonLastCheck, SteamAccount, SteamAccountLastCheck,
								GogAccount, GogAccountLastCheck, PatreonLastLogin, PatreonCoins, PatreonContribution
								FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones App", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesPatreon(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"SELECT PatreonOption1, PatreonOption2, PatreonMail, PatreonCoins, Keys
								FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones App", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesNotificacionesCorreo(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"SELECT NotificationLows, NotificationWishlistBundles, NotificationBundles, NotificationFree,
								NotificationSubscriptions, NotificationOthers, NotificationWeb, NotificationDelisted
								FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Notificaciones Correo", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesNotificacionesPush(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"SELECT Language, LanguageOverride, NotificationPushBundles, NotificationPushFree, 
								NotificationPushSubscriptions, NotificationPushOthers, NotificationPushWeb, NotificationPushLows
								FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Notificaciones Push", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesSteam(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"SELECT SteamGames, SteamWishlist, Avatar, Nickname, SteamAccountLastCheck, OfficialGroup, OfficialGroup2,
								SteamGamesAllow, SteamWishlistAllow, SteamAccount, SteamId
								FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Steam", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesGOG(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"SELECT GogGames, GogWishlist, GogAccountLastCheck, GogGamesAllow, GogWishlistAllow,
								GogAccount, GogId	
								FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones GOG", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesAmazon(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"SELECT AmazonGames, AmazonLastImport
								FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Amazon", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesEpicGames(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"SELECT EpicGames, EpicGamesLastImport
								FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Epic Games", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesEA(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"SELECT EaGames, EaLastImport
								FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Ea", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesUbisoft(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"SELECT UbisoftGames, UbisoftLastImport
								FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Ubisoft", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesGOGGalaxy(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"SELECT GogGames, GogWishlist, GogAccountLastCheck, AmazonGames, AmazonLastImport, EpicGames, EpicGamesLastImport
								FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones GOG Galaxy", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesPlaynite(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = @"SELECT AmazonGames, AmazonLastImport, EpicGames, EpicGamesLastImport, UbisoftGames, UbisoftLastImport, EaGames, EaLastImport
								FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones GOG Galaxy", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesSteamDeck(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT Currency, SteamDeckOption1, SteamDeckOption2, SteamDeckOption3, SteamDeckOption4 FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Steam Deck", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesSteamMachine(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT Currency, SteamMachineOption1, SteamMachineOption2, SteamMachineOption3, SteamMachineOption4 FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Steam Machine", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesSteamFrame(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT Currency, SteamFrameOption1, SteamFrameOption2, SteamFrameOption3, SteamFrameOption4 FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Steam Frame", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesSteamOS(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT Currency, SteamOSOption1, SteamOSOption2, SteamOSOption3, SteamOSOption4 FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones SteamOS", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesGeforceNOW(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT Currency, GeforceNOWOption1, GeforceNOWOption2, GeforceNOWOption3, GeforceNOWOption4 FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Geforce NOW", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesBoosteroid(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT Currency, BoosteroidOption1, BoosteroidOption2, BoosteroidOption3, BoosteroidOption4 FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Boosteroid", ex);
			}

			return null;
		}

		public static async Task<Usuario> OpcionesAmazonLuna(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			string busqueda = "SELECT Currency, AmazonLunaOption1, AmazonLunaOption2, AmazonLunaOption3, AmazonLunaOption4 FROM AspNetUsers WHERE Id=@Id";

			try
			{
				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { Id = usuarioId });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Opciones Amazon Luna", ex);
			}

			return null;
		}

		public static async Task<List<Usuario>> UsuariosNotificacionesCorreo()
		{
			string busqueda = "SELECT Id, NotificationBundles, NotificationFree, NotificationSubscriptions, NotificationOthers, NotificationWeb, NotificationDelisted, Email, Language FROM AspNetUsers";

			try
			{
				List<Usuario> usuarios = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return (await conexion.QueryAsync<Usuario>(busqueda)).ToList();
				});

				return usuarios
				   .Where(usuario =>
					   usuario.NotificationBundles != null ||
					   usuario.NotificationFree != null ||
					   usuario.NotificationSubscriptions != null ||
					   usuario.NotificationOthers != null ||
					   usuario.NotificationWeb != null ||
					   usuario.NotificationDelisted != null ||
					   string.IsNullOrEmpty(usuario.Email) == false ||
					   string.IsNullOrEmpty(usuario.Language) == false
				   ).ToList();
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Notificaciones Correo", ex);
			}

			return null;
		}

		public static async Task<bool> CorreoYaUsado(string correo)
		{
			if (string.IsNullOrEmpty(correo) == true)
			{
				return false;
			}

			string busqueda = "SELECT Id FROM AspNetUsers WHERE Email=@Email";

			try
			{
				int resultados = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.ExecuteScalarAsync<int>(busqueda, new { Email = correo });
				});

				return resultados > 0;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Correo Ya Usado", ex);
			}

			return false;
		}

		public static async Task<bool> UsuarioTieneJuego(string usuarioId, int juegoId, JuegoDRM drm)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return false;
			}

			string busqueda = string.Empty;

			if (drm == JuegoDRM.Steam)
			{
				busqueda = "DECLARE @idSteam nvarchar(256);\r\n\r\nSET @idSteam = CONCAT('\"Id\":', (SELECT idSteam FROM juegos WHERE id=@juegoId), ',');\r\n\r\nSELECT id FROM AspNetUsers WHERE CHARINDEX(@idSteam, SteamGames) > 0 AND id=@id";
			}

			if (drm == JuegoDRM.GOG)
			{
				busqueda = "DECLARE @idGog nvarchar(256);\r\n\r\nSET @idGog = CONCAT('\"Id\":', (SELECT idGog FROM juegos WHERE id=@juegoId), ',');\r\n\r\nSELECT id FROM AspNetUsers WHERE CHARINDEX(@idGog, GogGames) > 0 AND id=@id";
			}

			if (drm == JuegoDRM.Amazon)
			{
				busqueda = "SELECT id FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(AmazonGames, ',') WHERE VALUE IN ((SELECT idAmazon FROM juegos WHERE id=@juegoId))) AND id=@id";
			}

			if (drm == JuegoDRM.Epic)
			{
				busqueda = "SELECT id FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(EpicGames, ',') WHERE VALUE IN ((SELECT exeEpic FROM juegos WHERE id=@juegoId))) AND id=@id";
			}

			if (drm == JuegoDRM.Ubisoft)
			{
				busqueda = "SELECT id FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(UbisoftGames, ',') WHERE VALUE IN ((SELECT exeUbisoft FROM juegos WHERE id=@juegoId))) AND id=@id";
			}

			if (drm == JuegoDRM.EA)
			{
				busqueda = "SELECT id FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(EaGames, ',') WHERE VALUE IN ((SELECT exeEa FROM juegos WHERE id=@juegoId))) AND id=@id";
			}

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				try
				{
					string resultado = await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { id = usuarioId, juegoId });
					});

					return resultado != null;
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Tiene Juego", ex);
				}
			}

			return false;
		}

		public static async Task<string> UsuarioQuiereCorreos(TiendaRegion region, string usuarioId, string seccion)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return null;
			}

			try
			{
				string busqueda = "SELECT " + seccion + " AS Notificaciones, EmailConfirmed, Email, Currency FROM AspNetUsers WHERE Id=@Id";

				var datos = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync(busqueda, new { id = usuarioId });
				});

				if (datos == null)
				{
					return null;
				}

				bool quiereNotificaciones = datos.Notificaciones ?? false;
				bool correoConfirmado = datos.EmailConfirmed ?? false;
				TiendaRegion usuarioRegion = (TiendaRegion)datos.Currency;

				if (quiereNotificaciones == true && correoConfirmado == true && region == usuarioRegion)
				{
					return datos.Email;
				}
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Quiere Correos", ex);
			}

			return null;
		}

		public static async Task<string> CuentaSteamUsada(string id64Steam)
		{
			try
			{
				string busqueda = "SELECT Id FROM AspNetUsers WHERE SteamId=@SteamId";

				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { SteamId = id64Steam });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Cuenta Steam Usada", ex);
			}

			return null;
		}

		public static async Task<bool> CuentaGogUsada(string idGog, string idUsuario)
		{
			try
			{
				string busqueda = "SELECT Id FROM AspNetUsers WHERE GogId=@GogId AND Id!=@UsuarioId";

				var resultado = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { GogId = idGog, UsuarioId = idUsuario });
				});

				return resultado != null;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Cuenta GOG Usada", ex);
			}

			return false;
		}

		public static async Task<bool> UsuarioNombreRepetido(string nombre)
		{
			if (string.IsNullOrEmpty(nombre) == true)
			{
				return false;
			}

			try
			{
				string busqueda = "SELECT Id FROM AspNetUsers WHERE Nickname=@Nickname";

				var id = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { Nickname = nombre });
				});

				return id != null;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Nombre Repetido", ex);
			}

			return false;
		}

		public static async Task<bool> PerfilYaUsado(string nombre)
		{
			if (string.IsNullOrEmpty(nombre) == true)
			{
				return false;
			}

			try
			{
				string busqueda = "SELECT Id FROM AspNetUsers WHERE ProfileNickname=@ProfileNickname";

				var id = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { ProfileNickname = nombre });
				});

				return id != null;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Perfil Ya Usado", ex);
			}

			return false;
		}

		public static async Task<Usuario> PerfilCargar(string nombre)
		{
			if (string.IsNullOrEmpty(nombre) == true)
			{
				return null;
			}

			try
			{
				string busqueda = "SELECT Id, ProfileNickname2, ProfileAvatar, ProfileSteamAccount, ProfileGogAccount, ProfileLastPlayed, ProfileGames, ProfileWishlist FROM AspNetUsers WHERE ProfileNickname=@ProfileNickname AND ProfileShow='true'";

				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<Usuario>(busqueda, new { ProfileNickname = nombre });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Perfil Cargar", ex);
			}

			return null;
		}

		public static async Task<string> PerfilSteamCuenta(string id)
		{
			if (string.IsNullOrEmpty(id) == true)
			{
				return null;
			}

			try
			{
				string busqueda = "SELECT SteamAccount FROM AspNetUsers WHERE Id=@Id";

				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { Id = id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Perfil Steam Cuenta", ex);
			}

			return null;
		}

		public static async Task<string> PerfilGogCuenta(string id)
		{
			if (string.IsNullOrEmpty(id) == true)
			{
				return null;
			}

			try
			{
				string busqueda = "SELECT GogAccount FROM AspNetUsers WHERE Id=@Id";

				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { Id = id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Perfil GOG Cuenta", ex);
			}

			return null;
		}

		public static async Task<int> CuantosUsuariosTienenJuego(string juegoId, JuegoDRM drm)
		{
			string busqueda = string.Empty;

			if (drm == JuegoDRM.Steam)
			{
				busqueda = "SELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(CONCAT('\"Id\":', (SELECT idSteam FROM juegos WHERE id=@juegoId), ','), SteamGames) > 0";
			}

			if (drm == JuegoDRM.GOG)
			{
				busqueda = "SELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(CONCAT('\"Id\":', (SELECT idGog FROM juegos WHERE id=@juegoId), ','), GogGames) > 0";
			}

			if (drm == JuegoDRM.Amazon)
			{
				busqueda = "SELECT COUNT(*) FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(AmazonGames, ',') WHERE VALUE IN ((SELECT idAmazon FROM juegos WHERE id=@juegoId)))";
			}

			if (drm == JuegoDRM.Epic)
			{
				busqueda = "SELECT COUNT(*) FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(EpicGames, ',') WHERE VALUE IN ((SELECT exeEpic FROM juegos WHERE id=@juegoId)))";
			}

			if (drm == JuegoDRM.Ubisoft)
			{
				busqueda = "SELECT COUNT(*) FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(UbisoftGames, ',') WHERE VALUE IN ((SELECT exeUbisoft FROM juegos WHERE id=@juegoId)))";
			}

			if (drm == JuegoDRM.EA)
			{
				busqueda = "SELECT COUNT(*) FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(EaGames, ',') WHERE VALUE IN ((SELECT exeEa FROM juegos WHERE id=@juegoId)))";
			}

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.ExecuteScalarAsync<int>(busqueda, new { juegoId });
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Cuantos Tienen Juego", ex);
				}
			}

			return 0;
		}

		public static async Task<int> CuantosUsuariosTienenDeseado(string juegoId, JuegoDRM drm)
		{
			string busqueda = string.Empty;

			if (drm == JuegoDRM.Steam)
			{
				busqueda = "DECLARE @idSteam nvarchar(256);\r\n\r\nSET @idSteam = CONCAT('\"IdBaseDatos\":\"',@juegoId,'\",\"DRM\":0}');\r\n\r\nDECLARE @num1 int;\r\nSET @num1 = (SELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(@idSteam, Wishlist) > 0);\r\n\r\nDECLARE @num2 int;\r\nSET @num2 = (SELECT COUNT(*) FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(SteamWishlist, ',') WHERE VALUE IN (SELECT idSteam FROM juegos WHERE id=@juegoId)));\r\n\r\nSELECT @num1 + @num2";
			}

			if (drm == JuegoDRM.GOG)
			{
				busqueda = "DECLARE @idGOG nvarchar(256);\r\n\r\nSET @idGOG = CONCAT('\"IdBaseDatos\":\"',@juegoId,'\",\"DRM\":8}');\r\n\r\nDECLARE @num1 int;\r\nSET @num1 = (SELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(@idGOG, Wishlist) > 0);\r\n\r\nDECLARE @num2 int;\r\nSET @num2 = (SELECT COUNT(*) FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(GogWishlist, ',') WHERE VALUE IN (SELECT idGog FROM juegos WHERE id=@juegoId)));\r\n\r\nSELECT @num1 + @num2";
			}

			if (drm == JuegoDRM.Amazon)
			{
				busqueda = "DECLARE @idAmazon nvarchar(256);\r\n\r\nSET @idAmazon = CONCAT('\"IdBaseDatos\":\"',@juegoId,'\",\"DRM\":9}');\r\n\r\nSELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(@idAmazon, Wishlist) > 0;";
			}

			if (drm == JuegoDRM.Epic)
			{
				busqueda = "DECLARE @idEpic nvarchar(256);\r\n\r\nSET @idEpic = CONCAT('\"IdBaseDatos\":\"',@juegoId,'\",\"DRM\":6}');\r\n\r\nSELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(@idEpic, Wishlist) > 0;";
			}

			if (drm == JuegoDRM.Ubisoft)
			{
				busqueda = "DECLARE @idUbisoft nvarchar(256);\r\n\r\nSET @idUbisoft = CONCAT('\"IdBaseDatos\":\"',@juegoId,'\",\"DRM\":2}');\r\n\r\nSELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(@idUbisoft, Wishlist) > 0;";
			}

			if (drm == JuegoDRM.EA)
			{
				busqueda = "DECLARE @idEA nvarchar(256);\r\n\r\nSET @idEA = CONCAT('\"IdBaseDatos\":\"',@juegoId,'\",\"DRM\":3}');\r\n\r\nSELECT COUNT(*) FROM AspNetUsers WHERE CHARINDEX(@idEA, Wishlist) > 0;";
			}

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.ExecuteScalarAsync<int>(busqueda, new { juegoId });
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Cuantos Desean Juego", ex);
				}
			}

			return 0;
		}

		public static async Task<List<string>> ListaUsuariosTienenDeseado(int juegoId, JuegoDRM drm)
		{
			string busqueda = string.Empty;

			if (drm == JuegoDRM.Steam)
			{
				busqueda = @"DECLARE @idSteam nvarchar(256);
SET @idSteam = CONCAT('""IdBaseDatos"":""',@juegoId,'"",""DRM"":0}');

SELECT id FROM AspNetUsers WHERE CHARINDEX(@idSteam, Wishlist) > 0
UNION
SELECT id FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(SteamWishlist, ',') WHERE VALUE IN (SELECT idSteam FROM juegos WHERE id=@juegoId))";
			}

			if (drm == JuegoDRM.GOG)
			{
				busqueda = @"DECLARE @idGOG nvarchar(256);
SET @idGOG = CONCAT('""IdBaseDatos"":""',@juegoId,'"",""DRM"":8}');

SELECT id FROM AspNetUsers WHERE CHARINDEX(@idGOG, Wishlist) > 0
UNION
SELECT id FROM AspNetUsers WHERE EXISTS(SELECT * FROM STRING_SPLIT(GogWishlist, ',') WHERE VALUE IN (SELECT idGOG FROM juegos WHERE id=@juegoId))";
			}

			if (drm == JuegoDRM.Amazon)
			{
				busqueda = @"DECLARE @idAmazon nvarchar(256);
SET @idAmazon = CONCAT('""IdBaseDatos"":""',@juegoId,'"",""DRM"":9}');

SELECT id FROM AspNetUsers WHERE CHARINDEX(@idAmazon, Wishlist) > 0";
			}

			if (drm == JuegoDRM.Epic)
			{
				busqueda = @"DECLARE @idEpic nvarchar(256);
SET @idEpic = CONCAT('""IdBaseDatos"":""',@juegoId,'"",""DRM"":6}');

SELECT id FROM AspNetUsers WHERE CHARINDEX(@idEpic, Wishlist) > 0";
			}

			if (drm == JuegoDRM.Ubisoft)
			{
				busqueda = @"DECLARE @idUbisoft nvarchar(256);
SET @idUbisoft = CONCAT('""IdBaseDatos"":""',@juegoId,'"",""DRM"":2}');

SELECT id FROM AspNetUsers WHERE CHARINDEX(@idUbisoft, Wishlist) > 0";
			}

			if (drm == JuegoDRM.EA)
			{
				busqueda = @"DECLARE @idEA nvarchar(256);
SET @idEA = CONCAT('""IdBaseDatos"":""',@juegoId,'"",""DRM"":3}');

SELECT id FROM AspNetUsers WHERE CHARINDEX(@idEA, Wishlist) > 0";
			}

			if (string.IsNullOrEmpty(busqueda) == false)
			{
				try
				{
					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return (await conexion.QueryAsync<string>(busqueda, new { juegoId })).ToList();
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Lista Tienen Deseado", ex);
				}
			}

			return new List<string>();
		}

		public static async Task<string> OpcionString(string usuarioId, string valor)
		{
			if (string.IsNullOrEmpty(usuarioId) == true && string.IsNullOrEmpty(valor) == true)
			{
				return null;
			}
				
			if (string.IsNullOrEmpty(usuarioId) == false && string.IsNullOrEmpty(valor) == false)
			{
				try
				{
					string busqueda = $"SELECT {valor} FROM AspNetUsers WHERE id=@Id";

					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { Id = usuarioId });
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Opcion String", ex);
				}
			}

			return null;
		}

		public static async Task<string> OpcionStringRegion(TiendaRegion region, string usuarioId, string valor)
		{
			if (string.IsNullOrEmpty(usuarioId) == true && string.IsNullOrEmpty(valor) == true)
			{
				return null;
			}

			if (string.IsNullOrEmpty(usuarioId) == false && string.IsNullOrEmpty(valor) == false)
			{
				try
				{
					string busqueda = $"SELECT {valor} FROM AspNetUsers WHERE id=@Id AND Currency=@Currency";

					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { Id = usuarioId, Currency = (int)region });
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Opcion String", ex);
				}
			}

			return null;
		}

		public static async Task<int> OpcionInt(string usuarioId, string valor)
		{
			if (string.IsNullOrEmpty(usuarioId) == true && string.IsNullOrEmpty(valor) == true)
			{
				return 0;
			}

			if (string.IsNullOrEmpty(usuarioId) == false && string.IsNullOrEmpty(valor) == false)
			{
				try
				{
					string busqueda = $"SELECT {valor} FROM AspNetUsers WHERE id=@Id";

					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<int>(busqueda, new { Id = usuarioId });
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Opcion Int", ex);
				}
			}

			return 0;
		}

		public static async Task<bool> OpcionBool(string usuarioId, string valor)
		{
			if (string.IsNullOrEmpty(usuarioId) == true && string.IsNullOrEmpty(valor) == true)
			{
				return false;
			}

			if (string.IsNullOrEmpty(usuarioId) == false && string.IsNullOrEmpty(valor) == false)
			{
				try
				{
					string busqueda = $"SELECT {valor} FROM AspNetUsers WHERE id=@Id";

					bool? resultado = await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<bool?>(busqueda, new { Id = usuarioId });
					});

					return resultado ?? false;
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Opcion Bool", ex);
				}
			}

			return false;
		}

		public static async Task<bool> OpcionBoolRegion(TiendaRegion region, string usuarioId, string valor)
		{
			if (string.IsNullOrEmpty(usuarioId) == true && string.IsNullOrEmpty(valor) == true)
			{
				return false;
			}

			if (string.IsNullOrEmpty(usuarioId) == false && string.IsNullOrEmpty(valor) == false)
			{
				try
				{
					string busqueda = $"SELECT {valor} FROM AspNetUsers WHERE id=@Id AND Currency=@Currency";

					return await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<bool?>(busqueda, new { Id = usuarioId, Currency = (int)region });
					}) ?? false;
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Opcion Bool", ex);
				}
			}

			return false;
		}

		public static async Task<int> BundlesOrden(string usuarioId)
		{
			if (string.IsNullOrEmpty(usuarioId) == true)
			{
				return 0; 
			}

			try
			{
				string busqueda = "SELECT BundlesSort FROM AspNetUsers WHERE id=@Id";

				var resultado = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<int?>(busqueda, new { Id = usuarioId });
				});

				return resultado ?? 0;
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Bundles Orden", ex);
			}

			return 0;
		}

		public static async Task<string> JuegosExcluidos(string id)
		{
			if (string.IsNullOrEmpty(id) == true)
			{
				return null;
			}

			try
			{
				string busqueda = "SELECT GamesExcluded FROM AspNetUsers WHERE Id=@Id";

				return await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { Id = id });
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Juegos Excluidos", ex);
			}

			return null;
		}
	}
}
