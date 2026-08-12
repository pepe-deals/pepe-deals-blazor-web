#nullable disable

namespace Tiendas2
{
	public static class TiendasCargar
	{
		public static List<Tienda> GenerarListado()
		{
			List<Tienda> tiendas = new List<Tienda>
			{
				APIs.Steam.Tienda.Generar(),
				APIs.Steam.Tienda.GenerarBundles(),
				APIs.GamersGate.Tienda.Generar(),
				APIs.Humble.Tienda.Generar(),
				APIs.Humble.Tienda.GenerarChoice(),
				APIs.Gamesplanet.Tienda.GenerarUk(),
				APIs.Gamesplanet.Tienda.GenerarFr(),
				APIs.Gamesplanet.Tienda.GenerarDe(),
				APIs.Gamesplanet.Tienda.GenerarUs(),
				APIs.Fanatical.Tienda.Generar(),
				APIs.GreenManGaming.Tienda.Generar(),
				APIs.GreenManGaming.Tienda.GenerarGold(),
				APIs.GOG.Tienda.Generar(),
                APIs.IndieGala.Tienda.Generar(),
                APIs.WinGameStore.Tienda.Generar(),
				APIs.DLGamer.Tienda.Generar(),
				APIs.Battlenet.Tienda.Generar(),
				APIs.JoyBuggy.Tienda.Generar(),
				APIs.Voidu.Tienda.Generar(),
				APIs.EpicGames.Tienda.Generar(),
				APIs.GameBillet.Tienda.Generar(),
				APIs.Ubisoft.Tienda.Generar(),
				APIs.Playsum.Tienda.Generar(),
				APIs.PlanetPlay.Tienda.Generar(),
				APIs.EA.Tienda.Generar(),
				APIs.Nexus.Tienda.Generar(),
				APIs._2Game.Tienda.Generar(),
				APIs.Muvegames.Tienda.Generar(),
				APIs.Stove.Tienda.Generar(),
				APIs.Gamesporium.Tienda.Generar(),
				APIs.PlayerLand.Tienda.Generar(),
				APIs.Loaded.Tienda.Generar(),
				APIs.Yuplay.Tienda.Generar()
			};

			return tiendas;
		}

		public static async Task TareasGestionador(TiendaRegion region = TiendaRegion.Europa, string id = null)
		{
			if (id == APIs.Steam.Tienda.Generar().Id)
			{
				await APIs.Steam.Tienda.BuscarOfertas2(region);
			}
			else if (id == APIs.GamersGate.Tienda.Generar().Id)
			{
				await APIs.GamersGate.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.Humble.Tienda.Generar().Id)
			{
				await APIs.Humble.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.Gamesplanet.Tienda.GenerarUk().Id)
			{
				await APIs.Gamesplanet.Tienda.BuscarOfertasUk(region);
			}
			else if (id == APIs.Gamesplanet.Tienda.GenerarFr().Id)
			{
				await APIs.Gamesplanet.Tienda.BuscarOfertasFr(region);
			}
			else if (id == APIs.Gamesplanet.Tienda.GenerarDe().Id)
			{
				await APIs.Gamesplanet.Tienda.BuscarOfertasDe(region);
			}
			else if (id == APIs.Gamesplanet.Tienda.GenerarUs().Id)
			{
				await APIs.Gamesplanet.Tienda.BuscarOfertasUs(region);
			}
			else if (id == APIs.Fanatical.Tienda.Generar().Id)
			{
				await APIs.Fanatical.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.GreenManGaming.Tienda.Generar().Id)
			{
				await APIs.GreenManGaming.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.GreenManGaming.Tienda.GenerarGold().Id)
			{
				await APIs.GreenManGaming.Tienda.BuscarOfertasGold(region);
			}
			else if (id == APIs.GOG.Tienda.Generar().Id)
			{
				await APIs.GOG.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.IndieGala.Tienda.Generar().Id)
			{
				await APIs.IndieGala.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.WinGameStore.Tienda.Generar().Id)
			{
				await APIs.WinGameStore.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.EA.Tienda.Generar().Id)
			{
				await APIs.EA.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.DLGamer.Tienda.Generar().Id)
			{
				await APIs.DLGamer.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.Battlenet.Tienda.Generar().Id)
			{
				await APIs.Battlenet.Tienda.BuscarOfertas(region);
			}
            else if (id == APIs.JoyBuggy.Tienda.Generar().Id)
            {
                await APIs.JoyBuggy.Tienda.BuscarOfertas(region);
            }
			else if (id == APIs.Voidu.Tienda.Generar().Id)
			{
				await APIs.Voidu.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.EpicGames.Tienda.Generar().Id)
			{
				await APIs.EpicGames.Tienda.BuscarOfertas(region);
			}
            else if (id == APIs._2Game.Tienda.Generar().Id)
            {
                await APIs._2Game.Tienda.BuscarOfertas(region);
            }
			else if (id == APIs.GameBillet.Tienda.Generar().Id)
			{
				await APIs.GameBillet.Tienda.BuscarOfertas(region);
			}
            else if (id == APIs.Ubisoft.Tienda.Generar().Id)
            {
                await APIs.Ubisoft.Tienda.BuscarOfertas(region);
            }
			else if (id == APIs.Playsum.Tienda.Generar().Id)
			{
				await APIs.Playsum.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.PlanetPlay.Tienda.Generar().Id)
			{
				await APIs.PlanetPlay.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.Nexus.Tienda.Generar().Id)
			{
				await APIs.Nexus.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.Muvegames.Tienda.Generar().Id)
			{
				await APIs.Muvegames.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.Stove.Tienda.Generar().Id)
			{
				await APIs.Stove.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.Loaded.Tienda.Generar().Id)
			{
				await APIs.Loaded.Tienda.BuscarOfertas(region);
			}
			else if (id == APIs.Yuplay.Tienda.Generar().Id)
			{
				await APIs.Yuplay.Tienda.BuscarOfertas(region);
			}
		}

		public static Tienda DevolverTienda(string tiendaTexto)
		{
			foreach (var tienda in GenerarListado())
			{
				if (tienda.Id.ToString() == tiendaTexto)
				{
					return tienda;
				}
            }

			return null;
        }
    }
}
