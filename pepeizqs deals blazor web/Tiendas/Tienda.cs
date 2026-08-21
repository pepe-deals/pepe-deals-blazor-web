#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tiendas2
{
	public class Tienda
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string Id;
		public string Nombre;
		public string ImagenLogo;
		public string ImagenIcono;
		public string Imagen300x80;
		public string Color;
		public bool AdminUso;
		public bool UsuarioUso = true;
		public List<TiendaRegion> Regiones;
		public TiendaTipo Tipo;
	}

	public enum TiendaRegion
	{
		Europa,
		EstadosUnidos
	}

	public enum TiendaTipo
	{
		Oficial,
		NoOficial,
		Marketplace
	}
}
