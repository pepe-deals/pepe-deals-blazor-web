#nullable disable

using Dapper;

namespace BaseDatos.Pendientes
{
	public static class Actualizar
	{
		public static async Task Tienda(string idTienda, string enlace, string idJuegos)
		{
            if (string.IsNullOrEmpty(idJuegos) == false && idJuegos != "0")
            {
				string sqlActualizar = "UPDATE tienda" + idTienda + " " +
						"SET idJuegos=@idJuegos WHERE enlace=@enlace";

				try
				{
					await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(sqlActualizar, new { idJuegos, enlace }, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Pendientes Actualizar Tienda", ex);
				}
			}
		}

        public static async Task Suscripcion(string tablaInsertar, string tablaBorrar, string enlace, string nombreJuego, string imagen, List<string> idJuegos)
        {
			foreach (var idJuego in idJuegos)
			{
				string descartado = "no";
				var juego = await BaseDatos.Juegos.Buscar.UnJuego(idJuego);

				if (idJuego != "0")
				{
					if (juego != null)
					{
						if (nombreJuego == "vacio")
						{
							nombreJuego = juego.Nombre;
						}
							
						if (imagen == "vacio")
						{
							imagen = juego.Imagenes.Header_460x215;
						}
					}
				}
				else
				{
					descartado = "si";
				}

				if (tablaInsertar.StartsWith("tienda") == true)
				{
					if (descartado == "no")
					{
						descartado = "0";
					}
					else if (descartado == "si")
					{
						descartado = "1";
					}
				}

				string sqlInsertar = $@"
					INSERT INTO {tablaInsertar}
					(enlace, nombre, imagen, idJuegos, descartado)
					VALUES
					(@enlace, @nombre, @imagen, @idJuegos, @descartado)
				";

				try
				{
					await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(sqlInsertar, new
						{
							enlace,
							nombre = nombreJuego,
							imagen,
							idJuegos = idJuego,
							descartado
						}, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Pendientes Actualizar Suscripcion 1", ex);
				}

				string sqlBorrar = $@"
					DELETE FROM {tablaBorrar}
					WHERE enlace = @enlace
				";

				try
				{
					await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(sqlBorrar, new { enlace }, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Pendientes Actualizar Suscripcion 2", ex);
				}
			}
		}

        public static async Task Streaming(string idStreaming, string nombreCodigo, int idJuego)
        {
            if (idJuego > 0)
            {
				if (idStreaming.ToLower() == "amazonluna" || idStreaming.ToLower() == "geforcenow")
				{
					string sqlActualizar = "UPDATE streaming" + idStreaming + " " +
					   "SET idJuego=@idJuego WHERE nombreCodigo=@nombreCodigo";

					try
					{
						await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
						{
							return await conexion.ExecuteAsync(sqlActualizar, new { idJuego, nombreCodigo }, transaction: sentencia);
						});
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Pendientes Actualizar Streaming 1", ex);
					}
				}
				else
				{
					string sqlActualizar2 = "UPDATE streaming" + idStreaming + " " +
						"SET idJuego=@idJuego WHERE id=@id";

					try
					{
						await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
						{
							return await conexion.ExecuteAsync(sqlActualizar2, new { idJuego, id = nombreCodigo }, transaction: sentencia);
						});
					}
					catch (Exception ex)
					{
						BaseDatos.Errores.Insertar.Mensaje("Pendientes Actualizar Streaming 2", ex);
					}
				}
			}         
        }

        public static async Task PlataformaAmazon(string idAmazon, int idJuego)
        {
            if (idJuego > 0)
            {
				string busqueda = "SELECT idAmazon FROM juegos WHERE id=@id";

				try
				{
					string idExistente = await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { id = idJuego });
					});

					bool yaPuesto = !string.IsNullOrEmpty(idExistente);

					if (yaPuesto == false)
					{
						string actualizar = "UPDATE juegos " +
							"SET idAmazon=@idAmazon WHERE id=@id";

						await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
						{
							return await conexion.ExecuteAsync(actualizar, new { id = idJuego, idAmazon }, transaction: sentencia);
						});

						yaPuesto = true;
					}
					
					if (yaPuesto == true)
					{
						await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
						{
							return await conexion.ExecuteAsync("DELETE FROM temporalAmazonJuegos WHERE id=@id", new { id = idAmazon }, transaction: sentencia);
						});
					}
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Plataforma Amazon", ex);
				}
			}
		}

		public static async Task PlataformaEpic(string idEpic, int idJuego)
		{
			if (idJuego > 0)
			{
				string busqueda = "SELECT exeEpic FROM juegos WHERE id=@id";

				try
				{
					string idExistente = await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { id = idJuego });
					});

					bool yaPuesto = !string.IsNullOrEmpty(idExistente);

					if (yaPuesto == false)
					{
						string actualizar = "UPDATE juegos " +
							"SET exeEpic=@exeEpic WHERE id=@id";

						await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
						{
							return await conexion.ExecuteAsync(actualizar, new { id = idJuego, exeEpic = idEpic }, transaction: sentencia);
						});

						yaPuesto = true;
					}

					if (yaPuesto == true)
					{
						await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
						{
							return await conexion.ExecuteAsync("DELETE FROM temporalEpicJuegos WHERE id=@id", new { id = idEpic }, transaction: sentencia);
						});
					}
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Plataforma Epic", ex);
				}
			}
		}

		public static async Task PlataformaUbisoft(string idUbisoft, int idJuego)
		{
			if (idJuego > 0)
			{
				string busqueda = "SELECT exeUbisoft FROM juegos WHERE id=@id";

				try
				{
					string idExistente = await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { id = idJuego });
					});

					bool yaPuesto = !string.IsNullOrEmpty(idExistente);

					if (yaPuesto == false)
					{
						string actualizar = "UPDATE juegos " +
							"SET exeUbisoft=@exeUbisoft WHERE id=@id";

						await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
						{
							return await conexion.ExecuteAsync(actualizar, new { id = idJuego, exeUbisoft = idUbisoft }, transaction: sentencia);
						});

						yaPuesto = true;
					}

					if (yaPuesto == true)
					{
						await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
						{
							return await conexion.ExecuteAsync("DELETE FROM temporalUbisoftJuegos WHERE id=@id", new { id = idUbisoft }, transaction: sentencia);
						});
					}
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Plataforma Ubisoft", ex);
				}
			}
		}

		public static async Task PlataformaEA(string idEa, int idJuego)
		{
			if (idJuego > 0)
			{
				string busqueda = "SELECT exeEA FROM juegos WHERE id=@id";

				try
				{
					string idExistente = await Herramientas.BaseDatos.Select(async conexion =>
					{
						return await conexion.QueryFirstOrDefaultAsync<string>(busqueda, new { id = idJuego });
					});

					bool yaPuesto = !string.IsNullOrEmpty(idExistente);

					if (yaPuesto == false)
					{
						string actualizar = "UPDATE juegos " +
							"SET exeEA=@exeEA WHERE id=@id";

						await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
						{
							return await conexion.ExecuteAsync(actualizar, new { id = idJuego, exeEA = idEa }, transaction: sentencia);
						});

						yaPuesto = true;
					}

					if (yaPuesto == true)
					{
						await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
						{
							return await conexion.ExecuteAsync("DELETE FROM temporalEaJuegos WHERE id=@id", new { id = idEa }, transaction: sentencia);
						});
					}
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Plataforma EA", ex);
				}
			}
		}

		public static async Task DescartarTienda(string idTienda, string enlace)
		{
			string sqlActualizar = "UPDATE tienda" + idTienda + " " +
					"SET descartado=@descartado WHERE enlace=@enlace";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlActualizar, new
					{
						descartado = true,
						enlace
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Descartar Tienda", ex);
			}
		}

		public static async Task DescartarSuscripcion(string tablaInsertar, string tablaTemporal, string enlace)
		{
			string sqlInsertar = @$"
				INSERT INTO {tablaInsertar}
				(enlace, idJuegos, descartado) 
				VALUES 
				(@Enlace, @IdJuegos, @Descartado)
			";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlInsertar, new
					{
						Enlace = enlace,
						IdJuegos = "0",
						Descartado = "si"
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Descartar Suscripcion 1 " + tablaInsertar, ex);
			}

			try
			{
				string sqlBorrar = $"DELETE FROM temporal{tablaTemporal} WHERE enlace = @enlace";

				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlBorrar, new
					{
						enlace
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Descartar Suscripcion 2 " + tablaInsertar, ex);
			}
		}

        public static async Task DescartarStreaming(string idStreaming, string nombreCodigo)
        {
			if (idStreaming.ToLower() == "amazonluna" || idStreaming.ToLower() == "geforcenow")
			{
				string sqlActualizar = "UPDATE streaming" + idStreaming + " " +
					"SET descartado=@descartado WHERE nombreCodigo=@nombreCodigo";

				try
				{
					await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(sqlActualizar, new
						{
							descartado = 1,
							nombreCodigo
						}, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Pendientes Descartar Streaming 1 " + idStreaming, ex);
				}
			}
			else
			{
				string sqlActualizar2 = "UPDATE streaming" + idStreaming + " " +
					"SET descartado=@descartado WHERE id=@id";

				try
				{
					await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
					{
						return await conexion.ExecuteAsync(sqlActualizar2, new
						{
							descartado = 1,
							id = nombreCodigo
						}, transaction: sentencia);
					});
				}
				catch (Exception ex)
				{
					BaseDatos.Errores.Insertar.Mensaje("Pendientes Descartar Streaming 2 " + idStreaming, ex);
				}
			}
		}

        public static async Task DescartarPlataforma(string idPlataforma, string idJuego)
        {
			string sqlBorrar = "DELETE FROM temporal" + idPlataforma + "Juegos WHERE id=@id";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlBorrar, new
					{
						id = idJuego
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Descartar Plataforma 1 " + idPlataforma, ex);
			}

			string sqlInsertar = "INSERT INTO " + idPlataforma + "Descartes " +
							"(id) VALUES " +
							"(@id) ";

			try
			{
				await Herramientas.BaseDatos.RestoOperaciones(async (conexion, sentencia) =>
				{
					return await conexion.ExecuteAsync(sqlInsertar, new
					{
						id = idJuego
					}, transaction: sentencia);
				});
			}
			catch (Exception ex)
			{
				BaseDatos.Errores.Insertar.Mensaje("Pendientes Descartar Plataforma 2 " + idPlataforma, ex);
			}
		}
    }
}
