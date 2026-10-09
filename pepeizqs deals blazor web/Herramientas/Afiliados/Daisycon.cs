#nullable disable

using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace Herramientas.Afiliados
{
	public class DaisyconRespuesta
	{
		[JsonPropertyName("datafeed")]
		public DaisyconDatafeed Datafeed { get; set; }
	}

	public class DaisyconDatafeed
	{
		[JsonPropertyName("info")]
		public DaisyconFeedInfo Info { get; set; }

		[JsonPropertyName("programs")]
		public List<DaisyconPrograma> Programas { get; set; }
	}

	public class DaisyconFeedInfo
	{
		[JsonPropertyName("category")]
		public string Categoria { get; set; }

		[JsonPropertyName("sub_category")]
		public string Subcategoria { get; set; }

		[JsonPropertyName("product_count")]
		public int TotalProductos { get; set; }

		[JsonPropertyName("last_modified")]
		public string UltimaModificacion { get; set; }

		[JsonPropertyName("date_created")]
		public string FechaCreacion { get; set; }
	}

	public class DaisyconPrograma
	{
		[JsonPropertyName("program_info")]
		public DaisyconProgramaInfo Info { get; set; }

		[JsonPropertyName("products")]
		public List<DaisyconProducto> Productos { get; set; }
	}

	public class DaisyconProgramaInfo
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("name")]
		public string Nombre { get; set; }

		[JsonPropertyName("currency")]
		public string Moneda { get; set; }

		[JsonPropertyName("product_count")]
		public int TotalProductos { get; set; }
	}

	public class DaisyconProducto
	{
		[JsonPropertyName("update_info")]
		public DaisyconActualizacion Actualizacion { get; set; }

		[JsonPropertyName("product_info")]
		public DaisyconProductoInfo Info { get; set; }

		// Se rellenan al cargar el feed, para no perder de que programa viene cada producto

		[JsonIgnore]
		public int ProgramaId { get; set; }

		[JsonIgnore]
		public string Tienda { get; set; }
	}

	public class DaisyconActualizacion
	{
		[JsonPropertyName("daisycon_unique_id")]
		public string Id { get; set; }

		[JsonPropertyName("data_hash")]
		public string Hash { get; set; }

		[JsonPropertyName("status")]
		public string Estado { get; set; }

		[JsonPropertyName("insert_date")]
		public string FechaInsercion { get; set; }

		[JsonPropertyName("update_date")]
		public string FechaActualizacion { get; set; }

		[JsonPropertyName("delete_date")]
		public string FechaEliminacion { get; set; }
	}

	public class DaisyconProductoInfo
	{
		[JsonPropertyName("title")]
		public string Nombre { get; set; }

		[JsonPropertyName("description")]
		public string Descripcion { get; set; }

		[JsonPropertyName("description_short")]
		public string DescripcionCorta { get; set; }

		[JsonPropertyName("link")]
		public string Url { get; set; }

		[JsonPropertyName("sku")]
		public string Sku { get; set; }

		[JsonPropertyName("ean")]
		public string Ean { get; set; }

		[JsonPropertyName("price")]
		public string PrecioActualTexto { get; set; }

		[JsonPropertyName("price_old")]
		public string PrecioOriginalTexto { get; set; }

		[JsonPropertyName("discount")]
		public string EnDescuentoTexto { get; set; }

		[JsonPropertyName("discount_amount")]
		public string DescuentoCantidadTexto { get; set; }

		[JsonPropertyName("discount_percentage")]
		public string DescuentoTexto { get; set; }

		[JsonPropertyName("currency")]
		public string Moneda { get; set; }

		[JsonPropertyName("in_stock")]
		public string EnStockTexto { get; set; }

		[JsonPropertyName("category")]
		public string Categoria { get; set; }

		[JsonPropertyName("category_path")]
		public string CategoriaRuta { get; set; }

		[JsonPropertyName("google_category_path")]
		public List<string> GoogleCategoriaRuta { get; set; }

		[JsonPropertyName("images")]
		public List<DaisyconImagen> Imagenes { get; set; }

		[JsonPropertyName("gaming_platform")]
		public string Plataforma { get; set; }

		[JsonPropertyName("activation_platform")]
		public string PlataformaActivacion { get; set; }

		[JsonPropertyName("region_lock")]
		public string Region { get; set; }

		[JsonPropertyName("release_date")]
		public string FechaLanzamiento { get; set; }

		[JsonPropertyName("steam_id")]
		public string SteamId { get; set; }

		[JsonPropertyName("genres")]
		public string Generos { get; set; }

		[JsonPropertyName("publishers")]
		public string Editores { get; set; }

		[JsonPropertyName("product_type")]
		public string TipoProducto { get; set; }

		// Cualquier campo que no este mapeado arriba (otros programas/verticales) cae aqui

		[JsonExtensionData]
		public Dictionary<string, JsonElement> Extra { get; set; }

		[JsonIgnore]
		public decimal? PrecioActual => LeerDecimal(PrecioActualTexto);

		[JsonIgnore]
		public decimal? PrecioOriginal => LeerDecimal(PrecioOriginalTexto);

		[JsonIgnore]
		public decimal? DescuentoCantidad => LeerDecimal(DescuentoCantidadTexto);

		[JsonIgnore]
		public int? Descuento =>
			int.TryParse(DescuentoTexto, out int valor) ? valor : null;

		[JsonIgnore]
		public bool EnDescuento =>
			string.Equals(EnDescuentoTexto, "true", StringComparison.OrdinalIgnoreCase);

		[JsonIgnore]
		public bool EnStock =>
			string.Equals(EnStockTexto, "true", StringComparison.OrdinalIgnoreCase);

		[JsonIgnore]
		public string ImagenUrl
		{
			get
			{
				if (Imagenes == null || Imagenes.Count == 0)
				{
					return null;
				}

				DaisyconImagen imagen = Imagenes.FirstOrDefault(i => i.Tag == "default") ?? Imagenes[0];
				return imagen.Url;
			}
		}

		private static decimal? LeerDecimal(string texto)
		{
			return decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor) ? valor : null;
		}
	}

	public class DaisyconImagen
	{
		[JsonPropertyName("size")]
		public string Tamano { get; set; }

		[JsonPropertyName("tag")]
		public string Tag { get; set; }

		[JsonPropertyName("type")]
		public string Tipo { get; set; }

		[JsonPropertyName("location")]
		public string Url { get; set; }
	}

	public static class Daisycon
	{
		public static string GamesporiumEuropa = string.Empty;
		public static string DriffleEuropa = string.Empty;

		private static readonly HttpClient cliente = new HttpClient(new HttpClientHandler
		{
			AutomaticDecompression = DecompressionMethods.All
		})
		{
			Timeout = TimeSpan.FromMinutes(15)
		};

		private static readonly JsonSerializerOptions opciones = new JsonSerializerOptions
		{
			NumberHandling = JsonNumberHandling.AllowReadingFromString,
			UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
		};

		public static async Task<DaisyconDatafeed> ObtenerFeed(string urlFeed)
		{
			using var respuesta = await cliente.GetAsync(urlFeed, HttpCompletionOption.ResponseHeadersRead);
			respuesta.EnsureSuccessStatusCode();

			using var stream = await respuesta.Content.ReadAsStreamAsync();
			var contenido = await JsonSerializer.DeserializeAsync<DaisyconRespuesta>(stream, opciones);

			return contenido?.Datafeed;
		}

		public static string LimpiarEnlace(string enlace, string dominio)
		{
			var consulta = HttpUtility.ParseQueryString(new Uri(enlace).Query);
			string destino = consulta["dl"];

			if (string.IsNullOrEmpty(destino))
			{
				return null;
			}

			int posicion = destino.IndexOf('?');

			if (posicion >= 0)
			{
				destino = destino.Substring(0, posicion);
			}

			return $"{dominio.TrimEnd('/')}/{destino.TrimStart('/')}";
		}

		public static string CrearEnlaceAfiliado(string enlaceLimpio)
		{
			string si = string.Empty;
			string li = string.Empty;
			const string wi = "425594";

			if (enlaceLimpio.Contains("yuplay.com") == true)
			{
				si = "19969";
				li = "1857632";
			}
			else if (enlaceLimpio.Contains("gamesporium.com") == true)
			{
				si = "21412";
				li = "1924736";
			}
			else if (enlaceLimpio.Contains("driffle.com") == true)
			{
				si = "19866";
				li = "1850067";
			}

			if (string.IsNullOrEmpty(si) == false && string.IsNullOrEmpty(li) == false)
			{
				var uri = new Uri(enlaceLimpio);
				string destino = uri.PathAndQuery.TrimStart('/');

				return $"https://glp8.net/c/?si={si}&li={li}&wi={wi}&dl={Uri.EscapeDataString(destino)}&ws=";
			}

			return enlaceLimpio;
		}
	}
}