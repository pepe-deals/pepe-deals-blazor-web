#nullable disable

using Herramientas.Conexiones;
using Microsoft.Data.SqlClient;

namespace BaseDatos.Foro
{
	public static class Insertar
	{
		public static void Post(ForoPost post, SqlConnection conexion = null)
		{
			if (conexion == null)
			{
				conexion = Herramientas.Conexiones.Pepedeals.Conectar();
			}
			else
			{
				if (conexion.State != System.Data.ConnectionState.Open)
				{
					conexion = Herramientas.Conexiones.Pepedeals.Conectar();
				}
			}
			string insertar = "INSERT INTO foroPost (autorId, categoriaId, titulo, contenido, fecha, respuestaId) " +
				"VALUES (@autorId, @categoriaId, @titulo, @contenido, @fecha, @respuestaId)";
			
			using (SqlCommand comando = new SqlCommand(insertar, conexion))
			{
				comando.Parameters.AddWithValue("@autorId", post.AutorId);
				comando.Parameters.AddWithValue("@categoriaId", post.CategoriaId);
				comando.Parameters.AddWithValue("@titulo", post.Titulo);
				comando.Parameters.AddWithValue("@contenido", post.Contenido);
				comando.Parameters.AddWithValue("@fecha", post.FechaCreacion);
				
				if (post.RespuestaId > 0)
				{
					comando.Parameters.AddWithValue("@respuestaId", post.RespuestaId);
				}
				else
				{
					comando.Parameters.AddWithValue("@respuestaId", DBNull.Value);
				}

				comando.ExecuteNonQuery();
			}
		}
	}
}
