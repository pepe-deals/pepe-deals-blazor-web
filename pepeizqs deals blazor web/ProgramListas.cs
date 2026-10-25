public static class IpsPermitidas
{
	//IPs de Daisycon
	private static readonly HashSet<string> ips = new()
	{
		"80.69.93.119", 
		"80.69.93.124", 
		"85.10.139.157", 
		"90.145.71.210",
		"93.119.12.215", 
		"93.119.12.253", 
		"149.210.191.29"
	};

	public static bool Es(string ip) => ips.Contains(ip);
}

public static class DominiosSospechosos
{
	public static readonly HashSet<string> dominios = new()
	{
		"aocr.org", 
		"binance.com", 
		"heapr.com", 
		"iboogie.com", 
		"iconspedia.com", 
		"onelook.com", 
		"polycola.com"
	};
}

public static class PrefijosSospechosos
{
	public static readonly HashSet<string> prefijos = new()
	{
		"1.92.", 
		"101.43.", 
		"113.4.", 
		"119.8.", 
		"159.138.", 
		"188.34.", 
		"3.210.", 
		"3.69.", 
		"43.", 
		"45.", 
		"79.106."
	};
}