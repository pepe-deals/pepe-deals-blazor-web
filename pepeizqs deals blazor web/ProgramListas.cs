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
		"111.119.",
		"112.65.",
		"113.4.",
		"113.44.",
		"116.204.",
		"117.34.",
		"119.8.",
		"119.12.",
		"119.13.",
		"121.37.",
		"124.156.",
		"140.206.",
		"149.232.",
		"150.40.",
		"150.109.",
		"159.138.",
		"161.120.",
		"170.",
		"180.153.",
		"188.34.",
		"190.92.",
		"192.42.",
		"2.31.",
		"20.83.",
		"213.209.",
		"221.8.",
		"23.21.",
		"3.210.",
		"3.230.",
		"3.69.",
		"3.86.",
		"32.193.",
		"43.",
		"44.",
		"45.",
		"46.250.",
		"49.232.",
		"52.54.",
		"54.",
		"61.84.",
		"79.106.",
		"88.249.",
		"93.123."
	};
}