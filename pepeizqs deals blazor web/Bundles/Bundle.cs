#nullable disable

using Herramientas;
using Juegos;

namespace Bundles2
{
	public class Bundle
	{
		public int Id { get; set; }
		public BundleTipo BundleTipo { get; set; }
		public string Nombre { get; set; } //Nombre Bundle
		public string Tienda { get; set; } //Nombre Tienda
		public string Imagen { get; set; } //Imagen Bundle
		public string ImagenTienda { get; set; }
		public string ImagenIcono { get; set; }
		public string ImagenNoticia { get; set; }
		public List<string> ImagenesExtra { get; set; }
		public string Enlace { get; set; }
		public string EnlaceBase { get; set; }
		public DateTime FechaEmpieza { get; set; }
		public DateTime FechaTermina { get; set; }
		public List<BundleJuego> Juegos { get; set; }
		public List<BundleTier> Tiers { get; set; }
		public bool Pick { get; set; }
        public string Twitter { get; set; }
		public string DescripcionSEO { get; set; }
	}

	public class BundleTier
	{
		public int Posicion;
		public string Precio;
		public JuegoMoneda Moneda;
		public int CantidadJuegos;
	}

	public class BundleJuego
	{
		public string JuegoId;
		public string Nombre;
		public string Imagen;
		public BundleTier Tier;
		public JuegoDRM DRM;
		public Juego Juego;
		public List<string> DLCs;
		public bool DesplegableMostrar;
		public string Color;
		public bool Tiene;
		public bool DeseadoImportado;
		public bool DeseadoWeb;
	}

	public enum BundleDiseño
	{
		Cuadricula,
		Lista
	}
}
