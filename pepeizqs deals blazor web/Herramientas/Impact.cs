#nullable disable

using Microsoft.Identity.Client;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

#nullable disable

using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;

namespace Herramientas
{
	[XmlRoot("ImpactRadiusResponse")]
	public class ImpactRadiusResponse
	{
		[XmlElement("Items")]
		public ImpactItemsPage Items { get; set; }
	}

	public class ImpactItemsPage
	{
		[XmlAttribute("page")]
		public int Pagina { get; set; }

		[XmlAttribute("numpages")]
		public int TotalPaginas { get; set; }

		[XmlAttribute("total")]
		public int TotalItems { get; set; }

		[XmlAttribute("nextpageuri")]
		public string SiguientePaginaUri { get; set; }

		[XmlElement("Item")]
		public List<ImpactCatalogItem> Item { get; set; }
	}

	public class ImpactCatalogItem
	{
		[XmlElement("Id")]
		public string Id { get; set; }

		[XmlElement("CatalogItemId")]
		public string CatalogItemId { get; set; }

		[XmlElement("CampaignName")]
		public string Tienda { get; set; }

		[XmlElement("Name")]
		public string Nombre { get; set; }

		[XmlElement("Description")]
		public string Descripcion { get; set; }

		[XmlElement("Url")]
		public string Url { get; set; }

		[XmlElement("ImageUrl")]
		public string ImagenUrl { get; set; }

		[XmlElement("CurrentPrice")]
		public string PrecioActualTexto { get; set; }

		[XmlElement("OriginalPrice")]
		public string PrecioOriginalTexto { get; set; }

		[XmlElement("DiscountPercentage")]
		public string DescuentoTexto { get; set; }

		[XmlElement("Currency")]
		public string Moneda { get; set; }

		[XmlElement("StockAvailability")]
		public string Disponibilidad { get; set; }

		[XmlElement("Gtin")]
		public string Gtin { get; set; }

		[XmlElement("Category")]
		public string Categoria { get; set; }

		[XmlElement("Text3")]
		public string Plataforma { get; set; }

		[XmlArray("Labels")]
		[XmlArrayItem("Label")]
		public List<string> Labels { get; set; }

		[XmlIgnore]
		public decimal? PrecioActual =>
			decimal.TryParse(PrecioActualTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor) ? valor : null;

		[XmlIgnore]
		public decimal? PrecioOriginal =>
			decimal.TryParse(PrecioOriginalTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor) ? valor : null;

		[XmlIgnore]
		public int? Descuento =>
			int.TryParse(DescuentoTexto, out int valor) ? valor : null;
	}

	public static class Impact
	{
		public static string AccountSid { get; set; }
		public static string AuthToken { get; set; }

		public static async Task<List<ImpactCatalogItem>> ObtenerCatalogo(string catalogId)
		{
			List<ImpactCatalogItem> todos = new List<ImpactCatalogItem>();

			string siguienteUri = $"/Mediapartners/{AccountSid}/Catalogs/{catalogId}/Items?PageSize=100";

			using var cliente = new HttpClient();

			string credenciales = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{AccountSid}:{AuthToken}"));
			cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credenciales);

			var serializador = new XmlSerializer(typeof(ImpactRadiusResponse));

			while (string.IsNullOrEmpty(siguienteUri) == false)
			{
				var respuesta = await cliente.GetAsync($"https://api.impact.com{siguienteUri}");
				respuesta.EnsureSuccessStatusCode();

				using var stream = await respuesta.Content.ReadAsStreamAsync();
				var contenido = (ImpactRadiusResponse)serializador.Deserialize(stream);

				if (contenido?.Items?.Item != null)
				{
					todos.AddRange(contenido.Items.Item);
				}

				siguienteUri = contenido?.Items?.SiguientePaginaUri;
			}

			return todos;
		}
	}
}
