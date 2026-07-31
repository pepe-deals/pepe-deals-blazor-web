#nullable disable

using Bundles2;
using Dapper;
using Juegos;
using Noticias;
using System.Data;
using System.Data.SqlTypes;
using System.Text;
using System.Text.Json;

public class JuegoHandler<T> : SqlMapper.TypeHandler<T>
{
	public override T Parse(object valor)
	{
		if (valor == null || valor == DBNull.Value)
		{
			return default;
		}

		string texto;

		switch (valor)
		{
			case SqlString ss:
				texto = ss.Value;
				break;
			case SqlChars sc:
				texto = new string(sc.Value);
				break;
			case SqlBytes sb:
				texto = Encoding.UTF8.GetString(sb.Value);
				break;
			default:
				texto = valor.ToString();
				break;
		}

		if (string.IsNullOrWhiteSpace(texto) || texto == "null")
		{
			return default;
		}

		try
		{
			return JsonSerializer.Deserialize<T>(texto);
		}
		catch (Exception ex) 
		{
			BaseDatos.Errores.Insertar.Mensaje($"[JuegoHandler] JSON inválido para tipo {typeof(T).Name}: {texto}", ex.Message);

			if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
			{
				return (T)Activator.CreateInstance(typeof(T));
			}
				
			return default;
		}
	}

	public override void SetValue(IDbDataParameter parametro, T valor)
	{
		if (valor == null)
		{
			parametro.Value = DBNull.Value;
		}
		else
		{
			string json = JsonSerializer.Serialize(valor);
			parametro.Value = json;
		}
	}
}

public class BundleHandler<T> : SqlMapper.TypeHandler<List<T>>
{
	public override List<T> Parse(object valor)
	{
		if (valor == null || valor == DBNull.Value)
		{
			return default;
		}

		string texto = valor.ToString();
		if (string.IsNullOrWhiteSpace(texto) || texto == "null")
		{
			return default;
		}

		return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(texto);
	}

	public override void SetValue(IDbDataParameter parametro, List<T> valor)
	{
		parametro.Value = valor == null ? (object)DBNull.Value : Newtonsoft.Json.JsonConvert.SerializeObject(valor);
	}
}

public class JuegoDRMHandler : SqlMapper.TypeHandler<JuegoDRM>
{
	public override JuegoDRM Parse(object valor)
	{
		return JuegoDRM2.DevolverDRM((int)valor);
	}

	public override void SetValue(IDbDataParameter parametro, JuegoDRM valor)
	{
		parametro.Value = (int)valor;
	}
}

public static class ClasesDapper
{
	public static void Registrar()
	{
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoImagenes>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoPrecio>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoAnalisis>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoCaracteristicas>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoMedia>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoMediaVideo>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoBundle>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoBundlesActuales>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoBundlesPasados>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoGratis>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoGratisActuales>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoGratisPasados>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoSuscripcion>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoSuscripcionActuales>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoSuscripcionPasados>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<string>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoDeckToken>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoHistorico>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoGalaxyGOG>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoCantidadJugadoresSteam>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoIdioma>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoEpicGames>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoXbox>());
		SqlMapper.AddTypeHandler(new JuegoHandler<NoticiaSteam>());
		SqlMapper.AddTypeHandler(new BundleHandler<BundleJuego>());
		SqlMapper.AddTypeHandler(new BundleHandler<BundleTier>());
		SqlMapper.AddTypeHandler(new JuegoDRMHandler());
	}
}

public static class DapperSentencias
{
	public static string ConstruirSqlDebug(string sql, object parametros)
	{
		var valores = new Dictionary<string, object>();

		if (parametros is DynamicParameters dinamicos)
		{
			foreach (var nombre in dinamicos.ParameterNames)
			{
				valores[nombre] = dinamicos.Get<object>(nombre);
			}
		}
		else if (parametros != null)
		{
			foreach (var propiedad in parametros.GetType().GetProperties())
			{
				valores[propiedad.Name] = propiedad.GetValue(parametros);
			}
		}

		string sqlConValores = sql;
		foreach (var kvp in valores)
		{
			string valorTexto = kvp.Value switch
			{
				null => "NULL",
				string s => $"'{s.Replace("'", "''")}'",
				DateTime d => $"'{d:yyyy-MM-dd HH:mm:ss}'",
				bool b => b ? "1" : "0",
				_ => Convert.ToString(kvp.Value, System.Globalization.CultureInfo.InvariantCulture)
			};

			sqlConValores = System.Text.RegularExpressions.Regex.Replace(
				sqlConValores,
				$@"@{kvp.Key}\b",
				valorTexto.Replace("$", "$$"));
		}

		return sqlConValores;
	}
}