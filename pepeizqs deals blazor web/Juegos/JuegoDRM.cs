#nullable disable

namespace Juegos
{
	//No cambiar orden
	public enum JuegoDRM
	{
		Steam,
		DRMFree,
		Ubisoft,
		EA,
		Rockstar,
		Microsoft,
		Epic,
		NoEspecificado,
		GOG,
		Amazon,
		ElderScrolls,
		BattleNet,
		Giants,
		PearlAbyss,
		SquareEnix,
		AmazonLuna,
		PlayStation,
		Stove,
		Stadia,
		IndiePass,
		ArenaNet
	}

	public static class JuegoDRM2
	{
		public static List<DRM> GenerarListado()
		{
			List<DRM> drms = new List<DRM>();

			//----------------------------

			DRM steam = new DRM
			{
				Id = JuegoDRM.Steam,
				Nombre = "Steam",
				Imagen = "/imagenes/drm/steam.webp",
				Acepciones = new List<string> { "steam", "steamworks", "steam works", "steam works (linux)" }
			};

			drms.Add(steam);

			//----------------------------

			DRM gog = new DRM
			{
				Id = JuegoDRM.GOG,
				Nombre = "GOG",
				Imagen = "/imagenes/drm/gog.webp",
				Acepciones = new List<string> { "gog", "gog galaxy", "gog.com" }
			};

			drms.Add(gog);

			//----------------------------

			DRM drmfree = new DRM
			{
				Id = JuegoDRM.DRMFree,
				Nombre = "DRM Free",
				Acepciones = new List<string> { "drm free", "drm_free" }
			};

			drms.Add(drmfree);

			//----------------------------

			DRM ubisoft = new DRM
			{
				Id = JuegoDRM.Ubisoft,
				Nombre = "Ubisoft Connect",
				Imagen = "/imagenes/drm/ubisoft.webp",
				Acepciones = new List<string> { "ubisoft connect", "ubisoft", "uplay" }
			};

			drms.Add(ubisoft);

			//----------------------------

			DRM ea = new DRM
			{
				Id = JuegoDRM.EA,
				Nombre = "EA App",
				Imagen = "/imagenes/drm/ea.webp",
				Acepciones = new List<string> { "ea app", "electronic arts", "origin", "ea_app" }
			};

			drms.Add(ea);

			//----------------------------

			DRM rockstar = new DRM
			{
				Id = JuegoDRM.Rockstar,
				Nombre = "Rockstar Games Launcher",
				Imagen = "/imagenes/drm/rockstar.webp",
				Acepciones = new List<string> { "rockstar social club", "rockstar", "rockstar launcher", "rockstarsocialclub", "rockstarsocial", "rockstar games" }
			};

			drms.Add(rockstar);

			//----------------------------

			DRM microsoft = new DRM
			{
				Id = JuegoDRM.Microsoft,
				Nombre = "Microsoft Store",
				Imagen = "/imagenes/drm/microsoft.webp",
				Acepciones = new List<string> { "microsoft store", "microsoft", "xbox", "msstore" }
			};

			drms.Add(microsoft);

			//----------------------------

			DRM amazon = new DRM
			{
				Id = JuegoDRM.Amazon,
				Nombre = "Amazon Games",
				Imagen = "/imagenes/drm/amazongames.webp",
				Acepciones = new List<string> { "amazon games", "amazon" }
			};

			drms.Add(amazon);

			//----------------------------

			DRM epic = new DRM
			{
				Id = JuegoDRM.Epic,
				Nombre = "Epic Games Store",
				Imagen = "/imagenes/drm/epic.webp",
				Acepciones = new List<string> { "epic game store", "epic games store", "epic games", "epic", "epic_keyless", "epic keyless", "epicgames", "epic games launcher" }
			};

			drms.Add(epic);

			//----------------------------

			DRM elderscrolls = new DRM
			{
				Id = JuegoDRM.ElderScrolls,
				Nombre = "Elder Scrolls Online",
				Imagen = "/imagenes/drm/elderscrolls.webp",
				Acepciones = new List<string> { "esonline", "the elder scrolls online" }
			};

			drms.Add(elderscrolls);

            //----------------------------

            DRM battlenet = new DRM
            {
                Id = JuegoDRM.BattleNet,
                Nombre = "Battle.net",
                Imagen = "/imagenes/drm/battlenet.webp",
                Acepciones = new List<string> { "battlenet", "battle.net" }
            };

            drms.Add(battlenet);

            //----------------------------

            DRM giants = new DRM
            {
                Id = JuegoDRM.Giants,
                Nombre = "Giants",
                Imagen = "/imagenes/drm/giants.webp",
                Acepciones = new List<string> { "giants", "giants software" }
            };

            drms.Add(giants);

            //----------------------------

            DRM pearl = new DRM
            {
                Id = JuegoDRM.PearlAbyss,
                Nombre = "Pearl Abyss",
                Imagen = "/imagenes/drm/pearlabyss.webp",
                Acepciones = new List<string> { "pearl abyss", "pearl abyss launcher" }
            };

            drms.Add(pearl);

			//----------------------------

			DRM square = new DRM
			{
				Id = JuegoDRM.SquareEnix,
				Nombre = "Square Enix",
				Imagen = "/imagenes/drm/squareenix.webp",
				Acepciones = new List<string> { "square-enix" }
			};

			drms.Add(square);

			//----------------------------

			DRM luna = new DRM
			{
				Id = JuegoDRM.AmazonLuna,
				Nombre = "Amazon Luna",
				Imagen = "/imagenes/drm/amazonluna.webp",
				Acepciones = new List<string> { "amazonluna" }
			};

			drms.Add(luna);

			//----------------------------

			DRM ps = new DRM
			{
				Id = JuegoDRM.PlayStation,
				Nombre = "PlayStation",
				Imagen = "/imagenes/drm/playstation.webp",
				Acepciones = new List<string> { "playstation", "playstation network" }
			};

			drms.Add(ps);

			//----------------------------

			DRM stove = new DRM
			{
				Id = JuegoDRM.Stove,
				Nombre = "Stove",
				Imagen = "/imagenes/drm/stove.webp",
				Acepciones = new List<string> { "stove" }
			};

			drms.Add(stove);

			//----------------------------

			DRM stadia = new DRM
			{
				Id = JuegoDRM.Stadia,
				Nombre = "Stadia",
				Imagen = "/imagenes/drm/stadia.webp",
				Acepciones = new List<string> { "stadia" }
			};

			drms.Add(stadia);

			//----------------------------

			DRM indiepass = new DRM
			{
				Id = JuegoDRM.IndiePass,
				Nombre = "Indie Pass",
				Imagen = "/imagenes/drm/indiepass.webp",
				Acepciones = new List<string> { "indie pass" }
			};

			drms.Add(indiepass);

			//----------------------------

			DRM arenanet = new DRM
			{
				Id = JuegoDRM.ArenaNet,
				Nombre = "Arena Net",
				Imagen = "/imagenes/drm/arenanet.webp",
				Acepciones = new List<string> { "arenanet", "arena net", "guild wars" }
			};

			drms.Add(arenanet);

			//----------------------------

			return drms;
		}

		public static string DevolverDRM(JuegoDRM drmBuscar)
		{
			string texto = string.Empty;

			List<DRM> drms = GenerarListado();

			foreach (DRM drm in drms) 
			{ 
				if (drm.Id == drmBuscar)
				{
					texto = drm.Nombre;
					break;
				}
			}

			if (drmBuscar == JuegoDRM.NoEspecificado)
			{
				texto = "Not specified";
			}

			return texto;
		}

		public static JuegoDRM DevolverDRM(string drmBuscar)
		{
			List<DRM> drms = GenerarListado();

			foreach (DRM drm in drms)
			{
				if (drm.Id.ToString() == drmBuscar)
				{
					return drm.Id;
				}
			}

			return JuegoDRM.NoEspecificado;
		}

		public static JuegoDRM DevolverDRM(int posicion)
		{
			return CargarDRMs()[posicion];
		}

		public static JuegoDRM Traducir(string drmTexto, string tienda = null)
		{
			JuegoDRM drmFinal = JuegoDRM.NoEspecificado;

			if (string.IsNullOrEmpty(drmTexto) == false)
			{
				drmTexto = drmTexto.Replace("key", null);
				drmTexto = drmTexto.Replace("DRM &gt;", null);
				drmTexto = drmTexto.Replace("DRM >", null);
				drmTexto = drmTexto.ToLower();
				drmTexto = drmTexto.Trim();

				List<DRM> drms = GenerarListado();

				foreach (DRM drm in drms)
				{
					bool encontroDRM = false;

					foreach (string acepcion in drm.Acepciones)
					{
						if (acepcion.ToLower() == drmTexto)
						{
							drmFinal = drm.Id;

							encontroDRM = true;
							break;
						}
					}	
					
					if (encontroDRM == true)
					{
						break;
					}
				}

				if (tienda == APIs.Humble.Tienda.Generar().Id || tienda == APIs.Humble.Tienda.GenerarChoice().Id)
				{
					if (drmTexto.ToLower() == "download")
					{
						drmFinal = JuegoDRM.DRMFree;
					}
				}
			}

			if (drmFinal == JuegoDRM.NoEspecificado && string.IsNullOrEmpty(drmTexto) == false)
			{
				bool añadirMensaje = true;

				List<string> ignorados = [
					"activation key",
					"all",
					"android",
					"antstream",
					"antstream arcade",
					"apple",
					"asmjs",
					"audio-download",
					"desura",
					"developer website",
					"download",
					"download_drm",
					"drivethrurpg",
					"football manager",
					"gaijin",
					"genshin impact",
					"glyph",
					"idw",
					"kobo",
					"league of legends",
					"mcafee",
					"minecraft",
					"mogstation",
					"mog station",
					"mytrainz",
					"ncsoft",
					"nintendo",
					"nintendo switch",
					"no",
					"none",
					"null",
					"nv_bundle",
					"nvidia",
					"oculus",
					"other-",
					"other drm",
					"platform &gt; linux",
					"platform &gt; mac",
					"platform &gt; windows",
					"ps5",
					"pubg mobile",
					"riot access",
					"roblox",
					"switch 2",
					"topware protection (psi-key.com)",
					"unknown",
					"video-download",
					"voucher",
					"wargaming",
					"website",
					"windows",
					"xbox-anywhere",
					"xbox live",
					"xbox-live-cross-gen",
					"xbox one",
					"xbox-one",
					"xbox one & xbox series x/s",
					"xbox play anywhere",
					"xbox series x/s",
					"xbox store",
					"yes",
					"zenimax",
					"zenva"
				];

				if (ignorados.Contains(drmTexto.ToLower()))
				{
					añadirMensaje = false;
				}

				if (añadirMensaje == true)
				{
					BaseDatos.Errores.Insertar.Mensaje("DRM No Especificado", "DRM: " + drmTexto);
				}			
			}

			return drmFinal;
		}

		public static string SacarImagen(JuegoDRM drmImagen)
		{
			List<DRM> drms = GenerarListado();

			foreach (DRM drm in drms)
			{
				if (drm.Id == drmImagen)
				{
					return drm.Imagen;
				}
			}

			return null;
		}

		public static string SacarImagen(string drmImagen)
		{
			List<DRM> drms = GenerarListado();

			foreach (DRM drm in drms)
			{
				if (drm.Id.ToString() == drmImagen)
				{
					return drm.Imagen;
				}
			}

			return null;
		}

		public static List<JuegoDRM> CargarDRMs()
		{
			List<JuegoDRM> drms = Enum.GetValues(typeof(JuegoDRM)).Cast<JuegoDRM>().ToList();

			return drms;
		}
	}

	public class DRM
	{
		public JuegoDRM Id;
		public string Nombre;
		public string Imagen;
		public List<string> Acepciones;
	}
}
