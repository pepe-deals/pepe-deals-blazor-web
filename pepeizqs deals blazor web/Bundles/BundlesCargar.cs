#nullable disable

namespace Bundles2
{
	public enum BundleTipo
	{
		Desconocido,
		HumbleBundle,
		Fanatical,
		JingleJam,
		IndieGala,
		Itchio,
        GreenManGaming,
		Digiphile,
		PlanetPlay,
		XsollaMall
    }

	public class BundlesCargar
	{
		public static List<Bundle> GenerarListado()
		{
			List<Bundle> bundles = new List<Bundle>
			{
				APIs.Desconocido.Bundle.Generar(),
				APIs.Humble.Bundle.Generar(),
				APIs.Fanatical.Bundle.Generar(),
				APIs.JingleJam.Bundle.Generar(),
				APIs.IndieGala.Bundle.Generar(),
				APIs.itchio.Bundle.Generar(),
				APIs.GreenManGaming.Bundle.Generar(),
				APIs.Digiphile.Bundle.Generar(),
				APIs.PlanetPlay.Bundle.Generar(),
				APIs.Xsolla.Bundle.Generar()
            };

			return bundles;
		}

		public static async Task<Bundle> SeleccionarBundle(string enlace)
		{
			foreach (var bundle in GenerarListado())
			{
				if (string.IsNullOrEmpty(enlace) == false && string.IsNullOrEmpty(bundle.EnlaceBase) == false)
				{
					if (enlace.Contains(bundle.EnlaceBase) == true)
					{
						if (bundle.BundleTipo == BundleTipo.HumbleBundle)
						{
							Bundle nuevoBundle = bundle;
							nuevoBundle.Enlace = LimpiarEnlace(enlace);
							nuevoBundle = await APIs.Humble.Bundle.ExtraerDatos(nuevoBundle);

							return nuevoBundle;
						}
						else if (bundle.BundleTipo == BundleTipo.Fanatical)
						{
							Bundle nuevoBundle = bundle;
							nuevoBundle.Enlace = LimpiarEnlace(enlace);
							nuevoBundle = await APIs.Fanatical.Bundle.ExtraerDatos(nuevoBundle);

							return nuevoBundle;
						}
						else if (bundle.BundleTipo == BundleTipo.GreenManGaming)
						{
							Bundle nuevoBundle = bundle;
							nuevoBundle.Enlace = LimpiarEnlace(enlace);
							nuevoBundle = await APIs.GreenManGaming.Bundle.ExtraerDatos(nuevoBundle);
							return nuevoBundle;
						}
						else
						{
							Bundle nuevoBundle = bundle;
							nuevoBundle.Enlace = LimpiarEnlace(enlace);

							return nuevoBundle;
						}
					}
				}			
			}

			return null;
		}

		private static string LimpiarEnlace(string enlace)
		{
			if (enlace.Contains("?") == true)
			{
				int int1 = enlace.IndexOf("?");
				enlace = enlace.Remove(int1, enlace.Length - int1);
			}

			enlace = enlace.Replace("/en/", "/");
            enlace = enlace.Replace("/es-es/", "/");

            return enlace;
		}

		public static Bundle DevolverBundle(BundleTipo bundleTipo)
		{
			foreach (var bundle in GenerarListado())
			{
				if (bundle.BundleTipo == bundleTipo)
				{
					return bundle;
				}
			}

			return null;
		}

		public static Bundle DevolverBundle(string bundleTipo)
		{
			foreach (var bundle in GenerarListado())
			{
				if (bundle.BundleTipo == (BundleTipo)int.Parse(bundleTipo))
				{
					return bundle;
				}
			}

			return null;
		}

		public static Bundle DevolverBundle(int posicion)
		{
			BundleTipo tipo = CargarBundles()[posicion];

			foreach (var bundle in GenerarListado())
			{
				if (bundle.BundleTipo == tipo)
				{
					return bundle;
				}
			}

			return null;
		}

		public static List<BundleTipo> CargarBundles()
		{
			List<BundleTipo> bundles = Enum.GetValues(typeof(BundleTipo)).Cast<BundleTipo>().ToList();

			return bundles;
		}
	}
}
