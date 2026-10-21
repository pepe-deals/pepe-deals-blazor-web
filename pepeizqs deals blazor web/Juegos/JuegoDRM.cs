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
				Acepciones = new List<string> { "rockstar social club", "rockstar", "rockstar launcher", "rockstarsocialclub", "rockstarsocial", "rockstar games", "rockstar games launcher" }
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
					"1-800-petsupplies.com",
					"1800baskets",
					"1800flowers",
					"360 total security",
					"aaseus",
					"abamsoft",
					"ableton live",
					"abrosoft",
					"acdsee",
					"acronis",
					"activation key",
					"actual tools",
					"adguard",
					"adidas",
					"adobe",
					"aerie",
					"aéropostale",
					"affinity",
					"afk journey",
					"agaxtur",
					"age of empires",
					"age of empires mobile",
					"aiarty",
					"airalo",
					"airbnb",
					"airlinegift",
					"airplane chefs",
					"aiseesoft",
					"albertsons",
					"all",
					"allavsoft",
					"allbirds",
					"allmapsoft",
					"allmodern",
					"alosim",
					"alpha academy",
					"amc theatres",
					"american airlines",
					"american cancer society",
					"american eagle",
					"american express",
					"american red cross",
					"amtrak",
					"anarchy online",
					"android",
					"anthemion",
					"antiplagiarism.net",
					"antstream",
					"antstream arcade",
					"anytech365",
					"anyviewer",
					"aomei",
					"apple",
					"archicad",
					"arena breakout",
					"arena of valor",
					"ashampoo",
					"asmjs",
					"assassin's creed valhalla",
					"astropay",
					"atlantis",
					"att",
					"audio-download",
					"aurora hdr",
					"auslogics",
					"autodesk",
					"avast",
					"avg",
					"avg antivirus",
					"avs",
					"axure",
					"azte.co",
					"backup4all",
					"bahama breeze",
					"barnes & noble",
					"be the king",
					"best buy",
					"bethesda",
					"bigasoft",
					"binance",
					"bitdefender",
					"bitnovo",
					"bitsa",
					"black desert online",
					"blacknut",
					"blood strike",
					"blueprotocol:starresonance",
					"borderlands 3",
					"boz",
					"brawlhalla",
					"bropacks",
					"bucketlistgift",
					"buffalo wild wings",
					"build a bear",
					"burger king",
					"burlington",
					"cabela's",
					"call of duty",
					"call of duty beta",
					"camerabag",
					"candy crush",
					"canva",
					"ccleaner",
					"centerparcs",
					"chamet",
					"chaossoftware",
					"charity on top",
					"charles",
					"chatgpt",
					"cherry audio",
					"cherry credits",
					"chevron",
					"chewy",
					"chromix",
					"clash of clans",
					"clash royale",
					"cleanmymac",
					"cleanmyphone",
					"clickteam",
					"clubs ludo star",
					"cod website",
					"coffee bean & tea leaf",
					"color",
					"computer repair simulator",
					"conoco",
					"cooking fever",
					"corel",
					"coreldraw",
					"cost plus",
					"costco",
					"crate & barrel",
					"crate & kids",
					"cricket",
					"crossover",
					"crunchyroll",
					"crypto voucher",
					"csgiftcards",
					"cyberghost",
					"cyberlink",
					"cyberport",
					"dana cash indonesia",
					"deezer premium",
					"defiance",
					"delta force",
					"desigual",
					"destiny 2",
					"destiny: rising",
					"desura",
					"developer website",
					"discord",
					"discord trial",
					"doomsday: last survivors",
					"dota2",
					"download",
					"download_drm",
					"dr.web",
					"driver",
					"drivethrurpg",
					"dungeon defenders",
					"duolingo",
					"ea gift card",
					"editplus",
					"eggy party",
					"ej technologies",
					"emeditor",
					"endnote",
					"eneba",
					"escape from tarkov",
					"eset",
					"etheria: restart",
					"eve echoes",
					"eve online",
					"eventim",
					"evixcard",
					"exitlag",
					"expressvpn",
					"f-secure internet",
					"facy",
					"farlight 84",
					"faststone",
					"filecenter",
					"filoz.net",
					"final fantasy xiv",
					"flexepin",
					"fm redeem",
					"focus",
					"focus magic",
					"foldersizes",
					"fonetool",
					"football manager",
					"forever 21",
					"forspoken",
					"fortnite",
					"fortnite gift card",
					"foxit",
					"free fire",
					"fxhome limited",
					"g data",
					"gaijin",
					"gamemaker",
					"garena",
					"gcodes",
					"gekas",
					"gemini",
					"genie",
					"genshin impact",
					"gerbview",
					"giant eagle",
					"gift me crypto",
					"gigsky",
					"gilisoft",
					"glary utilities",
					"glyph",
					"gocash game",
					"goddess of victory: nikke",
					"goldwave",
					"google play",
					"gopay wallet indonesia",
					"green gift",
					"grubhub",
					"h1z1",
					"halo infinite",
					"harry & david",
					"harry potter: magic awakened",
					"hay day",
					"hdr",
					"hello yo",
					"hetman",
					"hitfilm",
					"hma",
					"honkai: star rail",
					"honor of kings",
					"hornet",
					"hotels.com",
					"hy-plugins",
					"icofx",
					"identity v",
					"idw",
					"imo",
					"imvu",
					"inspire travel",
					"internet download manager",
					"iolo",
					"itch.io",
					"itop",
					"jamba juice",
					"jawaker",
					"jetoncash",
					"jottacloud",
					"kaizen",
					"kammelna",
					"kaspersky",
					"kfc",
					"klevgrand",
					"knives out",
					"kobo",
					"kofax",
					"krafton",
					"kumu",
					"laplink",
					"lawl",
					"league of legends",
					"likee",
					"linkedin",
					"liveme",
					"lord of the rings online",
					"love and deepspace",
					"ludo club",
					"ludo king",
					"lyca mobile",
					"macrorit",
					"macx",
					"magic chess: go go",
					"magix",
					"mangatoon",
					"marriott bonvoy",
					"marriott hotels",
					"marvel rivals",
					"maya ewallet",
					"mcafee",
					"mcalister's deli",
					"mdirect mobile legends",
					"mecha break",
					"mediamon",
					"merge kingdom warlords",
					"meta horizon store",
					"meta quest",
					"mico live",
					"microids",
					"microsoft business software",
					"microsoft office",
					"microsoft windows",
					"midasbuy",
					"midasbuy honor of king",
					"midasbuy undawn",
					"mifinity",
					"minecraft",
					"minitool",
					"mint prepaid",
					"mintroute",
					"mixcraft",
					"mobile legends",
					"mog station",
					"mogstation",
					"moneydance",
					"moonlock",
					"movavi",
					"mpsoftware",
					"ms store",
					"muchbetter",
					"mycard",
					"mytrainz",
					"mytrainz.com",
					"naver pay korea",
					"ncsoft",
					"ncss",
					"nero",
					"netease",
					"netflix",
					"new state mobile",
					"nexon",
					"nhl shop",
					"nintendo",
					"nintendo pokemon",
					"nintendo switch",
					"nitro",
					"nitro pdf",
					"no",
					"nomad factory",
					"none",
					"noping",
					"norton",
					"novyro",
					"nuance",
					"null",
					"nutaku.com",
					"nv_bundle",
					"nvidia",
					"o&o software",
					"oculus",
					"offgamers",
					"omaha steaks",
					"once human",
					"other",
					"other drm",
					"other-",
					"ovo cash indonesia",
					"paladins",
					"panda dome",
					"papa john's",
					"parallels",
					"parchis club",
					"paypal instant top up",
					"perplexity",
					"phonepe",
					"photomatix pro",
					"pinnacle studio",
					"platform &gt; linux",
					"platform &gt; mac",
					"platform &gt; windows",
					"poe website",
					"pokemon_tcg",
					"poppo",
					"prepaid_mastercard",
					"princess cruises",
					"profantasy",
					"project softwares",
					"ps5",
					"psn",
					"pubg",
					"pubg mobile",
					"punishing: gray raven",
					"puzzles & survival",
					"quickbooks",
					"quickgifts",
					"quillbot",
					"r2 games",
					"ragnarokcrush",
					"rainbow six siege mobile",
					"random",
					"random xbox",
					"ravendawn online",
					"razer",
					"razer_inc",
					"rentacargift",
					"revo uninstaller",
					"rewarble",
					"rewarble paypal",
					"rewarble super gift",
					"rewup",
					"rewup google play",
					"rewup mobile legends",
					"rewup netflix",
					"rewup playstation",
					"rewup pubg mobile",
					"rewup razer gold",
					"rewup xbox",
					"rewupgc",
					"riot access",
					"riot games",
					"roblox",
					"roblox official website",
					"rogue company",
					"runescape",
					"sawajackaroo",
					"silver and blood",
					"sims 3 website",
					"sky: children of the light",
					"smart",
					"smite",
					"softmaker",
					"soho101",
					"solidworks",
					"splitgate",
					"spotify",
					"sql",
					"staples",
					"star trek online",
					"star wars",
					"star wars: the old republic",
					"starmaker",
					"steam gift",
					"steam gift card",
					"sticky password",
					"stronghold kingdoms",
					"stumble guys",
					"sugo",
					"surfshark",
					"surfshark promo",
					"sweetfrog",
					"switch 2",
					"swype",
					"synty store",
					"t-mobile",
					"t-rex cafe",
					"tableau software",
					"tango",
					"techland gg",
					"telltale games",
					"the chant",
					"the secret world",
					"tibia",
					"tinder",
					"top eleven",
					"top password",
					"topware protection (psi-key.com)",
					"tower of fantasy",
					"truemoney ewallet thailand",
					"twitch",
					"uber",
					"uber eats",
					"undawn",
					"unipin",
					"unknown",
					"uquid",
					"vegas creative software",
					"venmo",
					"veromoda",
					"video-download",
					"videoproc",
					"vintage story",
					"virtual visa",
					"voicemod",
					"voucher",
					"vpn",
					"walmart",
					"warframe",
					"wargame 1942",
					"wargaming",
					"watch dogs: legion",
					"watchdog",
					"watcher of realms",
					"webroot",
					"website",
					"webzen",
					"where winds meet",
					"whitebit",
					"whiteout survival",
					"wild terra",
					"windows",
					"windows store",
					"winxvideo",
					"winzip",
					"wolf game",
					"wonderfox",
					"wondershare",
					"wtfast",
					"wuthering waves",
					"x-plane",
					"xbox live",
					"xbox one",
					"xbox one & xbox series x/s",
					"xbox play anywhere",
					"xbox series x/s",
					"xbox store",
					"xbox-anywhere",
					"xbox-live-cross-gen",
					"xbox-one",
					"xilisoft",
					"xsplit",
					"xyplorer",
					"yalla ludo",
					"yard house",
					"yes",
					"youcam",
					"youtube",
					"zenimax",
					"zenless zone zero",
					"zenva",
					"zenva academy",
					"zepeto",
					"zoolz"
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
