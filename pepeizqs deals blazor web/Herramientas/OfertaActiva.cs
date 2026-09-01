#nullable disable

using Juegos;

namespace Herramientas
{
	public static class OfertaActiva
	{
		public static bool Verificar(JuegoPrecio precio)
		{
			Tiendas2.Tienda tienda = Tiendas2.TiendasCargar.DevolverTienda(precio.Tienda);

			if (tienda != null && tienda.Tipo == Tiendas2.TiendaTipo.Oficial && precio.FechaTermina > DateTime.MinValue)
			{
				if (precio.FechaTermina > DateTime.Now)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			TimeSpan actualizado = DateTime.Now.Subtract(precio.FechaActualizacion);

			if (precio.Tienda == APIs.Steam.Tienda.Generar().Id || precio.Tienda == APIs.Steam.Tienda.GenerarBundles().Id)
			{
				if (actualizado.TotalHours < 24)
				{
					return true;
				}
			}
			else if (precio.Tienda == APIs.Humble.Tienda.Generar().Id || precio.Tienda == APIs.Humble.Tienda.GenerarChoice().Id)
			{
				if (actualizado.TotalHours < 25)
				{
					return true;
				}
			}
			else if (precio.Tienda == APIs.EpicGames.Tienda.Generar().Id)
			{
				if (actualizado.TotalHours < 48)
				{
					return true;
				}
			}
			else
			{
				if (actualizado.TotalHours < 11)
				{
					return true;
				}
			}

			return false;
		}
	}
}
