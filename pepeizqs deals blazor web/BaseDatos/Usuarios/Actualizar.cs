#nullable disable

using Dapper;
using Juegos;
using pepeizqs_deals_web.Data;

namespace BaseDatos.Usuarios
{
    public static class Actualizar
    {
		public static async Task PatreonComprobacion(string correoBuscar, DateTime fechaActualizar, int contribucion)
        {
			string busqueda = "SELECT Id FROM AspNetUsers WHERE Email = @Email OR PatreonMail = @PatreonMail";
			string id = string.Empty;

			try
			{
				id = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new
					{
						Email = correoBuscar,
						PatreonMail = correoBuscar
					});
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Patreon Comprobacion 1", ex, false);
			}

			if (string.IsNullOrEmpty(id) == false)
			{
				string actualizar = @"UPDATE AspNetUsers 
                                 SET PatreonLastCheck = @PatreonLastCheck, 
                                     PatreonContribution = @PatreonContribution 
                                 WHERE Id = @Id";

				try
				{
					await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(actualizar, new
						{
							Id = id,
							PatreonLastCheck = fechaActualizar,
							PatreonContribution = contribucion
						}, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Patreon Comprobacion 2", ex, false);
				}
			}
		}

		public static async Task<bool> PatreonCorreo(string usuarioId, string correoNuevo)
		{
			string busqueda = "SELECT Id FROM AspNetUsers WHERE Email = @Email OR PatreonMail = @PatreonMail";
			string id = string.Empty;

			try
			{
				id = await Herramientas.BaseDatos.Select(async conexion =>
				{
					return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new
					{
						Email = correoNuevo,
						PatreonMail = correoNuevo
					});
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Patreon Correo 1", ex, false);
			}

			if (string.IsNullOrEmpty(id))
			{
				string actualizar = "UPDATE AspNetUsers SET PatreonMail = @PatreonMail WHERE Id = @Id";

				try
				{
					await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(actualizar, new
						{
							Id = usuarioId,
							PatreonMail = correoNuevo
						}, transaction: sentencia);
					});

					return false; 
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Usuario Patreon Correo 2", ex, false);
				}
			}

			return true;
		}

		public static async Task Opcion(string variable, bool valor, string usuarioId)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("UPDATE AspNetUsers SET " + variable + "='" + valor + "' WHERE Id='" + usuarioId + "'", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Bool", ex, false);
			}
		}

		public static async Task Opcion(string variable, string valor, string usuarioId)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync($"UPDATE AspNetUsers SET {variable}=@valor WHERE Id=@usuarioId", new { valor, usuarioId }, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje($"Usuario Actualiza Datos String: {variable} - {valor}", ex, false);
			}
		}

		public static async Task Opcion(string variable, int valor, string usuarioId)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("UPDATE AspNetUsers SET " + variable + "='" + valor + "' WHERE Id='" + usuarioId + "'", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Int", ex, false);
			}
		}

		public static async Task Opcion(string variable, decimal valor, string usuarioId)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("UPDATE AspNetUsers SET " + variable + "='" + valor + "' WHERE Id='" + usuarioId + "'", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Decimal", ex, false);
			}
		}

		public static async Task Opcion(string variable, DateTime valor, string usuarioId)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync("UPDATE AspNetUsers SET " + variable + "='" + valor + "' WHERE Id='" + usuarioId + "'", transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Datetime", ex, false);
			}
		}

		public static async Task Steam(Usuario usuario)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(@"UPDATE AspNetUsers 
														SET SteamGames = @SteamGames, 
															SteamGamesAllow = @SteamGamesAllow, 
															SteamWishlist = @SteamWishlist, 
															SteamWishlistAllow = @SteamWishlistAllow,
															Avatar = @Avatar, 
															Nickname = @Nickname,
															SteamAccountLastCheck = @SteamAccountLastCheck,
															OfficialGroup = @OfficialGroup,
															OfficialGroup2 = @OfficialGroup2
														WHERE Id = @Id", new
					{
						Id = usuario.Id,
						SteamGames = usuario.SteamGames,
						SteamGamesAllow = usuario.SteamGamesAllow,
						SteamWishlist = usuario.SteamWishlist,
						SteamWishlistAllow = usuario.SteamWishlistAllow,
						Avatar = usuario.Avatar,
						Nickname = usuario.Nickname,
						SteamAccountLastCheck = usuario.SteamAccountLastCheck,
						OfficialGroup = usuario.OfficialGroup,
						OfficialGroup2 = usuario.OfficialGroup2
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Steam", ex, false);
			}
		}

		public static async Task GOG(Usuario usuario)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(@"UPDATE AspNetUsers 
														SET GogGames = @GogGames, 
															GogGamesAllow = @GogGamesAllow,
															GogWishlist = @GogWishlist, 
															GogWishlistAllow = @GogWishlistAllow,
															GogAccountLastCheck = @GogAccountLastCheck
														WHERE Id = @Id", new
					{
						Id = usuario.Id,
						GogGames = usuario.GogGames,
						GogGamesAllow = usuario.GogGamesAllow,
						GogWishlist = usuario.GogWishlist,
						GogWishlistAllow = usuario.GogWishlistAllow,
						GogAccountLastCheck = usuario.GogAccountLastCheck
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos GOG", ex, false);
			}
		}

		public static async Task Amazon(Usuario usuario)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(@"UPDATE AspNetUsers 
														SET AmazonGames = @AmazonGames, 
															AmazonLastImport = @AmazonLastImport
														WHERE Id = @Id", new
					{
						Id = usuario.Id,
						AmazonGames = usuario.AmazonGames,
						AmazonLastImport = usuario.AmazonLastImport
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Amazon", ex, false);
			}
		}

		public static async Task Epic(Usuario usuario)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(@"UPDATE AspNetUsers 
														SET EpicGames = @EpicGames, 
															EpicGamesLastImport = @EpicGamesLastImport
														WHERE Id = @Id", new
					{
						Id = usuario.Id,
						EpicGames = usuario.EpicGames,
						EpicGamesLastImport = usuario.EpicGamesLastImport
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Epic", ex, false);
			}
		}

		public static async Task EA(Usuario usuario)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(@"UPDATE AspNetUsers 
														SET EaGames = @EaGames, 
															EaLastImport = @EaLastImport
														WHERE Id = @Id", new
					{
						Id = usuario.Id,
						EaGames = usuario.EaGames,
						EaLastImport = usuario.EaLastImport
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos EA", ex, false);
			}
		}

		public static async Task Ubisoft(Usuario usuario)
		{
			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(@"UPDATE AspNetUsers 
														SET UbisoftGames = @UbisoftGames, 
															UbisoftLastImport = @UbisoftLastImport
														WHERE Id = @Id", new
					{
						Id = usuario.Id,
						UbisoftGames = usuario.UbisoftGames,
						UbisoftLastImport = usuario.UbisoftLastImport
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Usuario Actualiza Datos Ubisoft", ex, false);
			}
		}
	}

    public class Clave
    {
        public string Nombre { get; set; }
		public string JuegoId { get; set; }
		public string Codigo { get; set; }
	}

	public class DeseadosDatos
	{
		public int Cantidad { get; set; }
		public DateTime? UltimaVisita { get; set; }
		public List<DeseadosDatosJuego> Juegos { get; set; }
	}

	public class DeseadosDatosJuego
	{
		public int Id { get; set; }
		public string Enlace { get; set; }
		public string Descuento { get; set; }
		public string Precio { get; set; }
		public DateTime Fecha { get; set; }
		public string Tienda { get; set; }
		public JuegoDRM DRM { get; set; }
		public string Nombre2 { get; set; }
		public string Imagen2 { get; set; }
	}
}
