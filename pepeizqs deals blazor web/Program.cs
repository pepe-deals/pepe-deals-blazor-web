using ApexCharts;
using AspNet.Security.OpenId.Steam;
using Herramientas;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using pepeizqs_deals_blazor_web.Componentes;
using pepeizqs_deals_web.Data;
using System.IO.Compression;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

ClasesDapper.Registrar();

var builder = WebApplication.CreateBuilder(args);

#region Compresion (Primero)

builder.Services.AddResponseCompression(opciones =>
{
	opciones.Providers.Add<BrotliCompressionProvider>();
	opciones.Providers.Add<GzipCompressionProvider>();
	opciones.EnableForHttps = true;
	opciones.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
				new[] { "application/octet-stream", "application/rss+xml", "text/html", "text/css", "image/png", "image/x-icon", "text/javascript", "application/javascript" });
});
builder.Services.AddRequestDecompression();

builder.Services.Configure<GzipCompressionProviderOptions>(opciones =>
{
	opciones.Level = CompressionLevel.SmallestSize;
});

builder.Services.Configure<BrotliCompressionProviderOptions>(opciones =>
{
	opciones.Level = CompressionLevel.Optimal;
});

#endregion

#region Optimizador

builder.Services.AddWebOptimizer(
	acciones => 
	{
		acciones.MinifyCssFiles();

		var jsSettings = new WebOptimizer.Processors.JsSettings
		{
			GenerateSourceMap = false
		};

		acciones.AddJavaScriptBundle("/superjs.js", jsSettings,
			"lib/jquery/dist/jquery.min.js",
			"lib/bootstrap/dist/js/bootstrap.bundle.min.js"
		);

		acciones.MinifyJsFiles("inicio.js", "push-notifications.js");
	});

#endregion

builder.Services.AddRazorComponents(opciones =>
{
	opciones.DetailedErrors = true;
}).AddInteractiveServerComponents(opciones =>
{
	opciones.DetailedErrors = true;
	opciones.DisconnectedCircuitMaxRetained = 120;
	opciones.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(2);
	opciones.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(60);
}).AddHubOptions(opciones =>
{
	opciones.EnableDetailedErrors = true;
	opciones.MaximumReceiveMessageSize = null;
	opciones.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
	opciones.HandshakeTimeout = TimeSpan.FromSeconds(30);
	opciones.KeepAliveInterval = TimeSpan.FromSeconds(20);
});

builder.Services.AddSignalR(opciones =>
{
	opciones.MaximumReceiveMessageSize = 10 * 1024 * 1024;
	opciones.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
	opciones.HandshakeTimeout = TimeSpan.FromSeconds(30);
	opciones.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.IdleTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddAuthentication(opciones =>
{
	opciones.DefaultScheme = IdentityConstants.ApplicationScheme;
	opciones.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddSteam(opciones =>
{
	opciones.ApplicationKey = builder.Configuration["SteamAPI:Key"];
	opciones.CallbackPath = "/signin-steam";
	opciones.CorrelationCookie.SameSite = SameSiteMode.Lax;
	opciones.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
	opciones.CorrelationCookie.HttpOnly = true;
	opciones.Events = new AspNet.Security.OpenId.OpenIdAuthenticationEvents
	{
		OnRemoteFailure = context =>
		{
			context.Response.Redirect("/account/login?error=steam");
			context.HandleResponse();
			return Task.CompletedTask;
		}
	};

	var handler = new HttpClientHandler();
	opciones.Backchannel = new HttpClient(handler)
	{
		Timeout = TimeSpan.FromSeconds(30)
	};
	opciones.Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
}).AddIdentityCookies();

var conexionTexto = builder.Configuration.GetConnectionString("pepeizqs_deals_webContextConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
Herramientas.BaseDatos.cadenaConexion = conexionTexto;

APIs.Steam.Cuenta.ApiKey = builder.Configuration.GetValue<string>("SteamAPI:Key");
APIs.Fanatical.Tienda.ApiKey = builder.Configuration.GetValue<string>("FanaticalAPI:Key");

Impact.AccountSid = builder.Configuration.GetValue<string>("ImpactLoaded:AccountSid");
Impact.AuthToken = builder.Configuration.GetValue<string>("ImpactLoaded:AuthToken");

builder.Services.AddDbContextPool<pepeizqs_deals_webContext>(opciones => {
	opciones.UseSqlServer(conexionTexto, opciones2 =>
	{
		opciones2.CommandTimeout(30);
		opciones2.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), new List<int>() { 18, 19 });
	});
	//opciones.EnableSensitiveDataLogging();
	//opciones.EnableDetailedErrors();
});

builder.Services.ConfigureApplicationCookie(opciones =>
{
	opciones.AccessDeniedPath = "/";
	opciones.Cookie.Name = "cookiePepeizq";
	opciones.ExpireTimeSpan = TimeSpan.FromDays(30);
	opciones.LoginPath = "/account/login";
	opciones.LogoutPath = "/account/logout";
	opciones.SlidingExpiration = true;
	opciones.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddDataProtection().PersistKeysToDbContext<pepeizqs_deals_webContext>().SetDefaultKeyLifetime(TimeSpan.FromDays(30));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<Usuario>(opciones =>
{
	opciones.SignIn.RequireConfirmedAccount = false;
	opciones.Lockout.MaxFailedAccessAttempts = 15;
	opciones.Lockout.AllowedForNewUsers = true;
	opciones.User.RequireUniqueEmail = true;
})
	.AddEntityFrameworkStores<pepeizqs_deals_webContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<Usuario>, Herramientas.Correos.IdentityNoOpEmailSender>();

#region Servicios

builder.Services.AddScoped<Servicios.TiendaNoOficial>();
builder.Services.AddScoped<Servicios.Marketplace>();
builder.Services.AddScoped<Servicios.Moneda>();
builder.Services.AddScoped<Servicios.Opciones>();

#endregion

#region Tareas

builder.Services.Configure<HostOptions>(opciones =>
{
	opciones.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

builder.Services.AddSingleton<Tareas.VigiladorRAM>();
builder.Services.AddSingleton<Tareas.Comprobador>();
builder.Services.AddSingleton<Tareas.Minimos.OficialesEuropa>();
builder.Services.AddSingleton<Tareas.LimpiezaLog>();
//builder.Services.AddSingleton<Tareas.LimpiezaCircuits>();
builder.Services.AddSingleton<Tareas.Mantenimiento.Diario>();
//builder.Services.AddSingleton<Tareas.Mantenimiento.Encoger>();
//builder.Services.AddSingleton<Tareas.Pings>();
builder.Services.AddSingleton<Tareas.CorreosEnviar>();
builder.Services.AddSingleton<Tareas.Patreon>();
builder.Services.AddSingleton<Tareas.JuegosActualizar>();
builder.Services.AddSingleton<Tareas.Duplicados>();
builder.Services.AddSingleton<Tareas.UsuariosActualizar>();
builder.Services.AddSingleton<Tareas.RedesSociales>();
builder.Services.AddSingleton<Tareas.IndexNow>();
builder.Services.AddSingleton<Tareas.SteamBundles>();
builder.Services.AddSingleton<Tareas.SteamDLCs>();
builder.Services.AddSingleton<Tareas.Minimos.OficialesEstadosUnidos>();
builder.Services.AddSingleton<Tareas.Minimos.NoOficialesEuropa>();
builder.Services.AddSingleton<Tareas.Minimos.NoOficialesEstadosUnidos>();

builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.VigiladorRAM>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Comprobador>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Minimos.OficialesEuropa>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.LimpiezaLog>());
//builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.LimpiezaCircuits>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Mantenimiento.Diario>());
//builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Mantenimiento.Encoger>());
//builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Pings>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.CorreosEnviar>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Patreon>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.JuegosActualizar>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Duplicados>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.UsuariosActualizar>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.RedesSociales>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.IndexNow>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.SteamBundles>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.SteamDLCs>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Minimos.OficialesEstadosUnidos>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Minimos.NoOficialesEuropa>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<Tareas.Minimos.NoOficialesEstadosUnidos>());

#endregion

#region Decompilador

builder.Services.AddHttpClient<IDecompiladores, Decompiladores2>()
	.ConfigurePrimaryHttpMessageHandler(() =>
		new HttpClientHandler
		{
			AutomaticDecompression = System.Net.DecompressionMethods.GZip,
			MaxConnectionsPerServer = 50
		});

builder.Services.AddSingleton<IDecompiladores, Decompiladores2>();

#endregion

#region CORS necesario para extension navegador

builder.Services.AddCors(policy => {
	policy.AddPolicy("Extension", builder =>
		builder.WithOrigins("https://*:5001/").SetIsOriginAllowedToAllowWildcardSubdomains().AllowAnyOrigin()
	);
});

#endregion

#region Redireccionador

builder.Services.AddControllersWithViews().AddMvcOptions(opciones =>
	opciones.Filters.Add(
		new ResponseCacheAttribute
		{
			NoStore = true,
			Location = ResponseCacheLocation.None
		}));

#endregion

#region Linea Grafico

builder.Services.AddApexCharts(e =>
{
	e.GlobalOptions = new ApexChartBaseOptions
	{
		Debug = false,
		Theme = new ApexCharts.Theme
		{
			Palette = PaletteType.Palette2,
			Mode = Mode.Dark
		}
	};
});

#endregion

#region Mejora velocidad carga

builder.Services.AddHsts(opciones =>
{
	opciones.Preload = true;
	opciones.IncludeSubDomains = true;
	opciones.MaxAge = TimeSpan.FromDays(730);
});

#endregion

#region Tiempo Token Enlaces Correos 

builder.Services.Configure<DataProtectionTokenProviderOptions>(opciones => opciones.TokenLifespan = TimeSpan.FromHours(3));

#endregion

#region Acceder Usuario en Codigo y RSS

builder.Services.AddControllers(opciones =>
{
	opciones.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
}).AddJsonOptions(opciones2 =>
{
	opciones2.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
	opciones2.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

});
builder.Services.AddHttpContextAccessor();

#endregion

builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

#region Notificaciones Push

builder.Services.AddScoped<NotificacionesPush>();

#endregion

var app = builder.Build();

#region Control inicial de peticiones

app.Use(async (contexto, siguiente) =>
{
	string? ruta = contexto.Request.Path.Value?.ToLowerInvariant() ?? "";

	HashSet<string> extensiones = new()
	{
		"/.svg", "/.png", "/.jpg", "/.webp", "/.gif", "/ads.txt", ".php", "/en/", "/es/", "/game", "/.env"
	};

	if (extensiones.Any(ext => ruta.EndsWith(ext)) == true || ruta.Contains("./") == true || ruta.Contains("/wp-admin/") == true)
	{
		contexto.Response.StatusCode = StatusCodes.Status301MovedPermanently;
		contexto.Response.Headers.Location = "/";
		return;
	}

	// Antiguo formato de noticias
	string[] segmentos = ruta.Split('/', StringSplitOptions.RemoveEmptyEntries);
	if (segmentos.Length > 0 && int.TryParse(segmentos[0], out _))
	{
		contexto.Response.StatusCode = StatusCodes.Status301MovedPermanently;
		contexto.Response.Headers.Location = "/";
		return;
	}

	// Links muertos para Google Search Console (13/01/2026)
	if (ruta.StartsWith("/link/", StringComparison.OrdinalIgnoreCase) && ruta.Contains("/news/") == false)
	{
		contexto.Response.StatusCode = StatusCodes.Status301MovedPermanently;
		contexto.Response.Headers.Location = "/";
		return;
	}

	// Redireccionar HTTP a HTTPS
	#nullable disable

	string piscinaApp = builder.Configuration.GetValue<string>("PoolWeb:Contenido");
	string piscinaUsada = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

	if (piscinaApp == piscinaUsada && contexto.Request.IsHttps == false)
	{
		string rutaFinal = $"https://{contexto.Request.Host}{contexto.Request.Path}{contexto.Request.QueryString}";
		contexto.Response.StatusCode = StatusCodes.Status301MovedPermanently;
		contexto.Response.Headers.Location = rutaFinal;
		return;
	}

	// Redireccionar www a no-www
	string host = contexto.Request.Host.Host;

	if (host.StartsWith("www."))
	{
		string nuevoHost = host.Substring(4);
		string scheme = contexto.Request.Scheme;
		PathString rutaBase = contexto.Request.PathBase;
		PathString rutaSlug = contexto.Request.Path;
		QueryString queryTexto = contexto.Request.QueryString;

		string nuevoEnlace = $"{scheme}://{nuevoHost}{rutaBase}{rutaSlug}{queryTexto}";

		contexto.Response.StatusCode = StatusCodes.Status301MovedPermanently;
		contexto.Response.Headers.Location = nuevoEnlace;
		return;
	}

	try
	{
		await siguiente();

		// Evitar peticiones a recursos que no existen despues de enviar una peticion
		if (contexto.Response.StatusCode == 404 && contexto.Response.HasStarted == false)
		{
			contexto.Response.StatusCode = StatusCodes.Status301MovedPermanently;
			contexto.Response.Headers.Location = "/";
		}
	}
	catch (Exception ex)
	{
		if (contexto.Response.HasStarted == false)
		{
			contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
		}

		BaseDatos.Errores.Insertar.Mensaje($"Error 500: {contexto.Request.Path}", ex, false);
	}
});

#endregion

#region Guardo Service Provider para notificaciones push

ServiciosGlobales.ServiceProvider = app.Services;

#endregion

#region Compresion

app.UseRequestDecompression();
app.UseResponseCompression();

#endregion

#region Optimizador

app.UseWebOptimizer();

#endregion

#region Cache 

app.UseResponseCaching();

app.UseStaticFiles(new StaticFileOptions
{
	OnPrepareResponse = contexto =>
	{
		string ruta = contexto.File.Name.ToLowerInvariant();

		if (ruta.EndsWith(".js") || ruta.EndsWith(".css"))
		{
			contexto.Context.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
		}
		else if (ruta.EndsWith(".woff2") || ruta.EndsWith(".woff") || ruta.EndsWith(".ttf"))
		{
			contexto.Context.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
		}
		else if (ruta.EndsWith(".png") || ruta.EndsWith(".jpg") || ruta.EndsWith(".webp"))
		{
			contexto.Context.Response.Headers.CacheControl = "public, max-age=2592000";
		}
		else
		{
			contexto.Context.Response.Headers.CacheControl = "public, max-age=86400";
		}
	}
});

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
	//app.UseDeveloperExceptionPage();

	app.UseExceptionHandler(errorApp =>
	{
		errorApp.Run(async contexto =>
		{
			IExceptionHandlerPathFeature error1 = contexto.Features.Get<IExceptionHandlerPathFeature>();
			Exception error2 = error1?.Error;

			if (error2?.Message != null && error2.Message.Contains("anti-forgery"))
			{
				contexto.Response.StatusCode = StatusCodes.Status301MovedPermanently;
				contexto.Response.Headers.Location = "/";
				return;
			}

			contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
			contexto.Response.ContentType = "text/plain";

			await contexto.Response.WriteAsync("Unexpected error, I've already put the slaves to work on the fix.");

			StringBuilder detallesError = new StringBuilder();
			detallesError.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
			detallesError.AppendLine($"Ruta: {contexto.Request.Path}");
			detallesError.AppendLine($"Método: {contexto.Request.Method}");
			detallesError.AppendLine($"Query: {contexto.Request.QueryString}");
			detallesError.AppendLine($"Excepción: {error2?.GetType().Name}");
			detallesError.AppendLine($"Mensaje: {error2?.Message}");
			detallesError.AppendLine($"StackTrace: {error2?.StackTrace}");

			if (error2?.InnerException != null)
			{
				detallesError.AppendLine($"--- InnerException ---");
				detallesError.AppendLine($"Tipo: {error2.InnerException.GetType().Name}");
				detallesError.AppendLine($"Mensaje: {error2.InnerException.Message}");
				detallesError.AppendLine($"StackTrace: {error2.InnerException.StackTrace}");
			}

			BaseDatos.Errores.Insertar.Mensaje($"Error 500 {contexto.Request.Path}", detallesError.ToString());
		});
	});

	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseSession(); //Para login de Steam
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapStaticAssets();

#region Carpeta Imagenes

string imagenesRuta = Path.Combine(builder.Environment.ContentRootPath, "imagenes");

if (Directory.Exists(imagenesRuta) == true)
{
	app.UseStaticFiles(new StaticFileOptions
	{
		FileProvider = new PhysicalFileProvider(imagenesRuta),
		RequestPath = "/imagenes"
	});
}

#endregion

app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode(opciones =>
{
	opciones.DisableWebSocketCompression = false;
});

#region CORS necesario para extension navegador web

app.UseCors("Extension");

#endregion

#region Redireccionador

app.MapControllers();

#endregion

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

#region Extension

app.MapGet("extension/steam4/{id}/{region}/{noOficial}/{marketplace}/{clave}/", async (int id, string region, string clave) =>
{
#nullable disable

	string claveExtension = builder.Configuration.GetValue<string>("Extension:Clave");

	if (clave == claveExtension)
	{
		BaseDatos.Extension.Extension juego = await BaseDatos.Extension.Buscar.Steam3(region, id.ToString());

		if (juego?.Id > 0)
		{
			return Results.Json(juego);
		}
	}

	return Results.NotFound();
});

app.MapGet("extension/steam3/{id}/{region}/{clave}/", async (int id, string region, string clave) =>
{
	#nullable disable

	string claveExtension = builder.Configuration.GetValue<string>("Extension:Clave");

	if (clave == claveExtension)
	{
		BaseDatos.Extension.Extension juego = await BaseDatos.Extension.Buscar.Steam3(region, id.ToString());

		if (juego?.Id > 0)
		{
			return Results.Json(juego);
		}
	}

	return Results.NotFound();
});

app.MapGet("extension/gog3/{slug}/{region}/{clave}/", async (string slug, string region, string clave) =>
{
	#nullable disable

	string claveExtension = builder.Configuration.GetValue<string>("Extension:Clave");

	if (clave == claveExtension)
	{
		BaseDatos.Extension.Extension juego = await BaseDatos.Extension.Buscar.Gog3(region, slug);

		if (juego?.Id > 0)
		{
			return Results.Json(juego);
		}
	}

	return Results.NotFound();
});

app.MapGet("extension/epic3/{slug}/{region}/{clave}/", async (string slug, string region, string clave) =>
{
	#nullable disable

	string claveExtension = builder.Configuration.GetValue<string>("Extension:Clave");

	if (clave == claveExtension)
	{
		BaseDatos.Extension.Extension juego = await BaseDatos.Extension.Buscar.EpicGames3(region, slug);

		if (juego?.Id > 0)
		{
			return Results.Json(juego);
		}
	}

	return Results.NotFound();
});

#endregion

#region Sitemaps

app.MapGet("/sitemap.xml", async (HttpContext http) =>
{
	await Herramientas.Sitemaps.Maestro(http);
});

app.MapGet("/sitemap-main.xml", async (HttpContext http) =>
{
	await Herramientas.Sitemaps.Principal(http);
});

app.MapGet("/sitemap-games-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.Juegos(http, i);
});

app.MapGet("/sitemap-bundles-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.Bundles(http, i);
});

app.MapGet("/sitemap-free-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.Gratis(http, i);
});

app.MapGet("/sitemap-subscriptions-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.Suscripciones(http, i);
});

app.MapGet("/sitemap-lastnews-en.xml", async (HttpContext http) =>
{
	await Herramientas.Sitemaps.NoticiasUltimasIngles(http);
});

app.MapGet("/sitemap-lastnews-es.xml", async (HttpContext http) =>
{
	await Herramientas.Sitemaps.NoticiasUltimasEspañol(http);
});

app.MapGet("/sitemap-news-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.NoticiasIngles(http, i);
});

app.MapGet("/sitemap-news-en-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.NoticiasIngles(http, i);
});

app.MapGet("/sitemap-curators-{i:int}.xml", async (HttpContext http, int i) =>
{
	await Herramientas.Sitemaps.Curators(http, i);
});

#endregion

#region Robots.txt

app.MapGet("/robots.txt", async (HttpContext http) =>
{
	string dominio = http.Request.Host.Value;

	string texto = @$"Sitemap: https://{dominio}/sitemap.xml

User-agent: *
Disallow: /account/
Disallow: /link/
Disallow: /publisher/
Disallow: /_framework/
Disallow: /_blazor/

User-agent: AhrefsBot
Disallow: /

User-agent: SemrushBot
Disallow: /

User-agent: SERankingBacklinksBot
Disallow: /

User-agent: meta-externalagent
Disallow: /

User-agent: Amazonbot
Disallow: /
";

	return Results.Text(texto, "text/plain; charset=utf-8");
});

#endregion

#region Manifest

app.MapGet("/manifest.json", async (IConfiguration configuracion) =>
{
	string idioma = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

	var manifiesto = new
	{
		name = "pepe's deals",
		short_name = "pepe's deals",
		description = Idiomas.BuscarTexto(idioma, "String1", "Manifest"),
		lang = idioma,
		dir = "ltr",
		start_url = "/",
		scope = "/",
		theme_color = "#002033",
		background_color = "#002033",
		display = "standalone",
		display_override = new[] { "window-controls-overlay", "standalone" },
		categories = new[] { "utilities" },
		id = "pepesdeals",
		orientation = "any",
		icons = new[]
		{
			new { src = "/favicons/favicon-192x192.png", sizes = "192x192", type = "image/png" },
			new { src = "/favicons/favicon-512x512.png", sizes = "512x512", type = "image/png" },
			new { src = "/favicons/android-icon-192x192.png", sizes = "192x192", type = "image/png" },
			new { src = "/favicons/favicon-192x192.webp", sizes = "192x192", type = "image/webp" },
			new { src = "/favicons/favicon-512x512.webp", sizes = "512x512", type = "image/webp" },
			new { src = "/favicons/android-icon-192x192.webp", sizes = "192x192", type = "image/webp" }
		},
		screenshots = new[]
		{
			new {
				src = "/imagenes/screenshots/pepe.deals_.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String2", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_phone.webp",
				sizes = "750x1334",
				type = "image/webp",
				form_factor = "narrow",
				label = Idiomas.BuscarTexto(idioma, "String2", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_bundle_humblebundle.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String3", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_bundles.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String4", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_game.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String5", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_historicallows.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String6", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_search.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String7", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_steamdeck.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String8", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_subscriptions.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String9", "Manifest")
			},
			new {
				src = "/imagenes/screenshots/pepe.deals_subscriptions_humblechoice.webp",
				sizes = "1280x720",
				type = "image/webp",
				form_factor = "wide",
				label = Idiomas.BuscarTexto(idioma, "String10", "Manifest")
			}
		},
		prefer_related_applications = false,
		related_applications = new object[]
		{
			new Dictionary<string, string>
			{
				["platform"] = "chrome_web_store",
				["url"] = configuracion.GetValue<string>("Extension:Chrome"),
				["id"] = configuracion.GetValue<string>("Extension:ChromeId")
			},
			new Dictionary<string, string>
			{
				["platform"] = "firefox",
				["url"] = configuracion.GetValue<string>("Extension:Firefox"),
				["id"] = configuracion.GetValue<string>("Extension:FirefoxId")
			},
			new Dictionary<string, string>
			{
				["platform"] = "edge",
				["url"] = configuracion.GetValue<string>("Extension:Edge")
			}
		}
	};

	var json = JsonSerializer.Serialize(manifiesto, new JsonSerializerOptions
	{
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	});

	return Results.Content(json, "application/manifest+json");
});

#endregion

#region Login Steam

app.MapGet("/login-steam", async (HttpContext contexto, string returnUrl) =>
{
	await contexto.ChallengeAsync(SteamAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties
	{
		RedirectUri = $"/login-steam/callback?returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}"
	});
});

app.MapGet("/login-steam/callback", async (HttpContext contexto, UserManager<Usuario> usuarioManager, SignInManager<Usuario> signInManager, string returnUrl) =>
{
	var resultado = await contexto.AuthenticateAsync(IdentityConstants.ExternalScheme);

	if (resultado.Succeeded == false)
	{
		return Results.Redirect("/account/login?steam1=true");
	}

	var steamId64 = resultado.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value.Split('/').Last();

	if (steamId64 == null)
	{
		return Results.Redirect("/account/login?steam2=true");
	}

	if (contexto.User.Identity?.IsAuthenticated == true)
	{
		Usuario usuarioActual = await usuarioManager.GetUserAsync(contexto.User);

		if (usuarioActual != null)
		{
			usuarioActual.SteamId = steamId64;
			var resultado2 = await usuarioManager.UpdateAsync(usuarioActual);

			if (resultado2.Succeeded == true)
			{
				return Results.Redirect("/account/sync/steam/?success=true");
			}
			else
			{
				return Results.Redirect("/account/sync/steam/?success=false");
			}
		}
	}

	Usuario usuario = await usuarioManager.Users.FirstOrDefaultAsync(u => u.SteamId == steamId64);

	if (usuario != null)
	{
		await signInManager.SignInAsync(usuario, isPersistent: true);
		return Results.Redirect(returnUrl ?? "/");
	}

	contexto.Session.SetString("PendingSteamId", steamId64);
	return Results.Redirect($"/account/register");
});

#endregion

app.MapHealthChecks("/vida");

Herramientas.ImagenesOptimizador.GenerarImagenesResponsive(builder.Environment.WebRootPath);

app.Run();
