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
					"ann taylor",
					"anthemion",
					"antiplagiarism.net",
					"antstream",
					"antstream arcade",
					"anytech365",
					"anyviewer",
					"aomei",
					"apple",
					"aquarium us",
					"archicad",
					"arena breakout",
					"arena of valor",
					"ashampoo",
					"asmjs",
					"asos",
					"assassin's creed valhalla",
					"astropay",
					"athleta",
					"atlantis",
					"att",
					"audio-download",
					"auntie anne's",
					"aurora hdr",
					"auslogics",
					"autodesk",
					"autozone",
					"avast",
					"avg",
					"avg antivirus",
					"avs",
					"axure",
					"azte.co",
					"babin’s seafood house",
					"baby depot",
					"baby gap",
					"backup4all",
					"bahama breeze",
					"baja fresh",
					"bakers square",
					"banana republic",
					"bandcam",
					"barnes & noble",
					"baskin robbins",
					"bass pro shop",
					"bath & body works",
					"be the king",
					"belk",
					"benihana",
					"bertucci's",
					"best buy",
					"bethesda",
					"bigasoft",
					"bill's bar & burger",
					"binance",
					"birch lane",
					"bitdefender",
					"bitnovo",
					"bitsa",
					"black desert online",
					"blacknut",
					"blaze pizza",
					"blood strike",
					"bloomin brands",
					"bloomingdale's",
					"blue fin seafood sushi",
					"blueprotocol:starresonance",
					"bob evans restaurants",
					"borderlands 3",
					"boscov's",
					"bouchee patisserie",
					"boz",
					"brawlhalla",
					"brenner's steakhouse",
					"brick house tavern & tap",
					"bristol seafood grill",
					"bropacks",
					"bubba gump",
					"bucketlistgift",
					"buffalo wild wings",
					"build a bear",
					"burger king",
					"burlington",
					"cabela's",
					"cadillac bar",
					"cadillac ranch",
					"california pizza kitchen",
					"call of duty",
					"call of duty beta",
					"callaway golf",
					"camerabag",
					"candy crush",
					"canva",
					"canvaspop",
					"carnival cruise lines",
					"carter's",
					"carvel",
					"cashtocode",
					"cbsi paramount",
					"ccleaner",
					"cdc foundation",
					"celebrity cruises",
					"centerparcs",
					"chamet",
					"chaossoftware",
					"charity choice",
					"charity on top",
					"charles",
					"chart house restaurant",
					"chatgpt",
					"cherry audio",
					"cherry credits",
					"cheryl’s",
					"chevron",
					"chevron and texaco",
					"chewy",
					"chico's",
					"chili's",
					"chipotle",
					"chromix",
					"chuck e. cheese's",
					"cinemark",
					"cinnabon",
					"claim jumper",
					"clash of clans",
					"clash royale",
					"clean water fund",
					"cleanmymac",
					"cleanmyphone",
					"clickteam",
					"clinkerdagger",
					"clubs ludo star",
					"cod website",
					"coffee bean & tea leaf",
					"cold stone creamery",
					"color",
					"columbia sportwear",
					"computer repair simulator",
					"conoco",
					"cooking fever",
					"corel",
					"coreldraw",
					"cost plus",
					"costco",
					"cracker barrel",
					"craft f&b",
					"crate & barrel",
					"crate & kids",
					"cricket",
					"crossover",
					"crunchyroll",
					"crutchfield",
					"crypto voucher",
					"csgiftcards",
					"cutters crabhouse",
					"cyberghost",
					"cyberlink",
					"cyberport",
					"dana cash indonesia",
					"darden restaurants",
					"dave & buster's",
					"deezer premium",
					"defiance",
					"del frisco's",
					"delta",
					"delta force",
					"desigual",
					"destiny 2",
					"destiny: rising",
					"desura",
					"developer website",
					"devon",
					"dick's sporting goods",
					"digital solution visa",
					"discord",
					"discord trial",
					"disney",
					"domino's pizza",
					"doomsday: last survivors",
					"doordash",
					"dos caminos",
					"dota2",
					"download",
					"download_drm",
					"dr.web",
					"driver",
					"drivethrurpg",
					"dungeon defenders",
					"dunkin’",
					"duolingo",
					"ea gift card",
					"ebay",
					"editplus",
					"eggy party",
					"ej technologies",
					"elearngift",
					"emeditor",
					"endnote",
					"eneba",
					"equal justice initiative",
					"escape from tarkov",
					"eset",
					"etheria: restart",
					"etsy",
					"eve echoes",
					"eve online",
					"eventim",
					"evixcard",
					"exitlag",
					"expedia group",
					"express",
					"expressvpn",
					"f-secure internet",
					"facy",
					"fairmont hotels & resorts",
					"famous dave’s",
					"famous footwear",
					"fanatics",
					"fandango",
					"farlight 84",
					"faststone",
					"filecenter",
					"filoz.net",
					"final fantasy xiv",
					"fish tales",
					"fisherman's wharf",
					"five guys",
					"flexepin",
					"flying dutchman",
					"flystaygift",
					"fm redeem",
					"focus",
					"focus magic",
					"foldersizes",
					"fonetool",
					"foods co",
					"foot locker",
					"football manager",
					"forever 21",
					"forspoken",
					"fortnite",
					"fortnite gift card",
					"four seasons",
					"foxit",
					"free fire",
					"freeup mobile",
					"frickers",
					"fruit bouquets",
					"fxhome limited",
					"g data",
					"gaijin",
					"galveston island",
					"gamemaker",
					"gamestop",
					"gander outdoors",
					"gandy dancer",
					"gap",
					"garena",
					"gate.io",
					"gatorade",
					"gcodes",
					"gekas",
					"gemini",
					"genie",
					"genshin impact",
					"gerbview",
					"giant eagle",
					"gift me crypto",
					"gift of college",
					"gigsky",
					"gilisoft",
					"girls who code",
					"glaad",
					"glary utilities",
					"global experiences",
					"glyph",
					"gnc",
					"go play golf by fairway rewards",
					"go rio",
					"gocash game",
					"goddess of victory: nikke",
					"golden nugget casino",
					"goldwave",
					"goodcents",
					"google play",
					"google_workspace",
					"gopay wallet indonesia",
					"gopuff",
					"gosmart",
					"grand concourse",
					"great american days",
					"green gift",
					"grotto",
					"groupon",
					"grown brilliance",
					"grubhub",
					"guess",
					"guitar center",
					"h&m",
					"h1z1",
					"h2o",
					"halo infinite",
					"hard rock cafe",
					"harry & david",
					"harry potter: magic awakened",
					"hay day",
					"hdr",
					"hello fresh",
					"hello yo",
					"henry's tavern",
					"hetman",
					"hilton galveston island",
					"hitfilm",
					"hma",
					"home chef",
					"honkai: star rail",
					"honor of kings",
					"hope for ukraine",
					"horatio's",
					"hornet",
					"hotels.com",
					"hotelsgift",
					"hulu",
					"huntting inn",
					"hy-plugins",
					"icofx",
					"identity v",
					"idw",
					"ifly",
					"ikea",
					"imo",
					"imvu",
					"indieflix",
					"inspire travel",
					"instacart",
					"internet download manager",
					"iolo",
					"itch.io",
					"itop",
					"j gilberts",
					"jake's famous crawfish",
					"jake's grill",
					"jamba juice",
					"jawaker",
					"jcpenney",
					"jersey mike's",
					"jetoncash",
					"jiffy lube",
					"joe's crab shack",
					"jossandmain.com",
					"jottacloud",
					"jtv.com",
					"kaizen",
					"kammelna",
					"kaspersky",
					"kemah boardwalk",
					"kfc",
					"kincaid's fish chop & steakhouse",
					"king ranch texas kitchen",
					"kingsisle",
					"kirkland's",
					"klevgrand",
					"knives out",
					"knix",
					"kobo",
					"kofax",
					"kohls",
					"krafton",
					"krispy kreme",
					"kroger",
					"kumu",
					"kura sushi",
					"l.l. bean",
					"la griglia",
					"land's end",
					"landry's",
					"laplink",
					"lawl",
					"league of legends",
					"legal sea foods",
					"likee",
					"linkedin",
					"liveme",
					"lord of the rings online",
					"love and deepspace",
					"lowe's",
					"ludo club",
					"ludo king",
					"lululemon",
					"lyca mobile",
					"lyft",
					"macrorit",
					"macx",
					"macy's",
					"maggiano's",
					"maggie bluffs",
					"magic chess: go go",
					"magix",
					"mai tai bar",
					"main event",
					"mandarin oriental hotel group",
					"mangatoon",
					"mark & graham",
					"marriott bonvoy",
					"marriott hotels",
					"marvel rivals",
					"mastercard ereward",
					"mastro's",
					"maurices",
					"maya ewallet",
					"mcafee",
					"mcalister's deli",
					"mccormick & schmick’s restaurant",
					"mdirect mobile legends",
					"mecha break",
					"mediamon",
					"merge kingdom warlords",
					"meta horizon store",
					"meta quest",
					"metro market",
					"metropcs retail",
					"michaels",
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
					"mitchell's steakhouse",
					"mitchell’s fish market restaurant",
					"mix it up",
					"mixcraft",
					"mlb shop",
					"mobile legends",
					"moe's southwest grill",
					"mog station",
					"mogstation",
					"moneydance",
					"moonlock",
					"morton’s restaurant",
					"movavi",
					"mpsoftware",
					"ms store",
					"muchbetter",
					"mycard",
					"myprepaidcentervisa",
					"mytrainz",
					"mytrainz.com",
					"national park foundation",
					"nautica",
					"naver pay korea",
					"nba store",
					"ncsoft",
					"ncss",
					"nero",
					"nespresso",
					"net10 usa parent",
					"netdragon",
					"netease",
					"netflix",
					"new state mobile",
					"nexon",
					"nfl",
					"nhl shop",
					"nike",
					"ninety nine restaurant & pub",
					"nintendo",
					"nintendo pokemon",
					"nintendo switch",
					"nitro",
					"nitro pdf",
					"no",
					"nomad factory",
					"none",
					"noping",
					"nordstrom",
					"nordstrom rack",
					"nordvpn",
					"norton",
					"novyro",
					"nuance",
					"null",
					"nutaku.com",
					"nv_bundle",
					"nvidia",
					"o&o software",
					"o'charley's",
					"oculus",
					"offgamers",
					"old navy",
					"olive garden",
					"omaha steaks",
					"once human",
					"other",
					"other drm",
					"other-",
					"oura ring",
					"overstock.com",
					"ovo cash indonesia",
					"p.c. richard and son",
					"paladins",
					"palisade",
					"panda dome",
					"panda express",
					"papa john's",
					"parallels",
					"parchis club",
					"paul martin's american grill",
					"paypal instant top up",
					"paysafecard player pass",
					"peet's coffee",
					"peohe's",
					"perigold.com",
					"perplexity",
					"petco",
					"petsmart",
					"phonepe",
					"photomatix pro",
					"pinkberry",
					"pinnacle studio",
					"platform &gt; linux",
					"platform &gt; mac",
					"platform &gt; windows",
					"poe website",
					"pokemon_tcg",
					"poppo",
					"pottery barn",
					"pottery barn teen",
					"prepaid_mastercard",
					"primark",
					"princess cruises",
					"profantasy",
					"project softwares",
					"ps5",
					"psn",
					"pubg",
					"pubg mobile",
					"publix",
					"puckett's grocery",
					"punishing: gray raven",
					"puzzles & survival",
					"quickbooks",
					"quickgifts",
					"quillbot",
					"quizno's",
					"r2 games",
					"raffles hotels & resorts",
					"ragnarok origin",
					"ragnarokcrush",
					"rainbow six siege mobile",
					"rainforest cafe",
					"random",
					"random xbox",
					"ratioclothing",
					"ravendawn online",
					"razer",
					"razer gold",
					"razer_inc",
					"regal",
					"rei",
					"rentacargift",
					"restaurant.com",
					"revo uninstaller",
					"rewarble",
					"rewarble paypal",
					"rewarble super gift",
					"rewup",
					"rewup crypto gift card",
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
					"river crab/bluewater inn",
					"roblox",
					"roblox official website",
					"rogue company",
					"royal caribbean cruises",
					"rubio’s coastal grill",
					"ruby tuesday",
					"runescape",
					"rusty pelican",
					"ruth's chris steak house",
					"safelink wireless",
					"saks fifth avenue",
					"saks off 5th",
					"saltgrass steak house restaurant",
					"sam's club",
					"san luis resort",
					"sawajackaroo",
					"schlotzsky's",
					"sephora",
					"shari's berries",
					"sheetz",
					"shell",
					"shipt",
					"shoney's",
					"shutterfly",
					"silver and blood",
					"simms steakhouse",
					"simon & seafort's saloon & grill",
					"simplemobile",
					"simply chocolate",
					"sims 3 website",
					"sinpin",
					"siriusxm",
					"skates on the bay",
					"sky: children of the light",
					"sling",
					"smart",
					"smite",
					"softmaker",
					"soho101",
					"solidworks",
					"soma",
					"sonic app",
					"southwest airlines",
					"spa week media",
					"spafinder",
					"special olympics",
					"splitgate",
					"sportsmans warehouse",
					"spotify",
					"sql",
					"stanley & seafort's",
					"staples",
					"star trek online",
					"star wars",
					"star wars: the old republic",
					"starbucks",
					"starmaker",
					"steak 'n shake",
					"steam gift",
					"steam gift card",
					"sticky password",
					"stitch fix",
					"stock yards",
					"strip house",
					"stronghold kingdoms",
					"stubhub",
					"stumble guys",
					"subway",
					"sugo",
					"surfshark",
					"surfshark promo",
					"swarovski",
					"sweetfrog",
					"swissôtel hotels & resorts",
					"switch 2",
					"swype",
					"synty store",
					"t-mobile",
					"t-rex cafe",
					"tableau software",
					"taco bell",
					"tango",
					"target",
					"techland gg",
					"telltale games",
					"texas roadhouse",
					"the boathouse restaurant",
					"the chant",
					"the cheesecake factory",
					"the children's place",
					"the container store",
					"the home depot",
					"the nature conservancy",
					"the oceanaire restaurant",
					"the palm",
					"the popcorn factory",
					"the post oak hotel uptown houston",
					"the secret world",
					"the vitamin shoppe",
					"tibia",
					"tidal",
					"tijuana flats",
					"tinder",
					"tommy bahama",
					"tommy john",
					"top eleven",
					"top password",
					"topgolf",
					"topware protection (psi-key.com)",
					"total wine",
					"total wireless",
					"toursgift",
					"tower of fantasy",
					"tower of the americas",
					"tq game card",
					"tracfone",
					"trevi italian restaurant",
					"tripgift",
					"true classic",
					"truemoney ewallet thailand",
					"turo",
					"twitch",
					"uber",
					"uber eats",
					"ulta beauty",
					"ultra mobile",
					"undawn",
					"under armour",
					"unicef",
					"unipin",
					"unknown",
					"uno's pizzeria & grill",
					"uquid",
					"vanilla visa",
					"vegas creative software",
					"venmo",
					"verizon",
					"veromoda",
					"vic & anthony’s restaurant",
					"victoria's secret",
					"video-download",
					"videoproc",
					"village inn-us",
					"vintage story",
					"virgin experience",
					"virtual visa",
					"virtual-reward-visa",
					"voicemod",
					"von diamonds",
					"voucher",
					"vpn",
					"vudu",
					"walgreens",
					"walmart",
					"warframe",
					"wargame 1942",
					"wargaming",
					"watch dogs: legion",
					"watchdog",
					"watcher of realms",
					"wayfair",
					"webroot",
					"website",
					"webzen",
					"west elm",
					"wetgo",
					"where winds meet",
					"white house black market-us",
					"whitebit",
					"whiteout survival",
					"wild terra",
					"williams-sonoma",
					"willie g's",
					"windows",
					"windows store",
					"winxvideo",
					"winzip",
					"wolf game",
					"wolferman’s",
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
					"xsolla",
					"xsplit",
					"xyplorer",
					"yak & yeti restaurant",
					"yalla ludo",
					"yankee candle",
					"yard house",
					"yes",
					"youcam",
					"your gift choice",
					"youtube",
					"zenimax",
					"zenless zone zero",
					"zenva",
					"zenva academy",
					"zepeto",
					"zoolz"
				];

				string drmComparar = System.Text.RegularExpressions.Regex.Replace(drmTexto, @"[\s\u200B]+", " ").Trim();

				if (ignorados.Contains(drmComparar) == true)
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
