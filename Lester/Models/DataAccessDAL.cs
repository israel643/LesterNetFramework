using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
/// Importación para trabajar con provedor de Datos 

namespace Lester.Models
{
    public class DataAccessDAL
    {
        private string cadena;

        public DataAccessDAL()
        {
            cadena = System.Configuration.ConfigurationManager.ConnectionStrings["Lester"].ConnectionString;
        }

        /// Modelo Creado con una consulta basica en SQL 
        public List<Embarques> GeneradorDeEmabrquesPorRango(DateTime? FechaInicio, DateTime? FechaDeFinalizacion)
        {
            List<Embarques> embarquesList = new List<Embarques>();
            using (SqlConnection conexion = new SqlConnection(cadena))
            {
                // Construir la consulta SQL dinámicamente
                string SqlQueryComnad = "SELECT * FROM tblRFID_CodiCaptEmbarques WHERE 1=1";

                if (FechaInicio.HasValue)
                {
                    SqlQueryComnad += " AND fechaLectura >= @FechaInicio";
                }
                if (FechaDeFinalizacion.HasValue)
                {
                    SqlQueryComnad += " AND fechaLectura <= @FechaDeFinalizacion";
                }

                SqlCommand command = new SqlCommand(SqlQueryComnad, conexion);

                if (FechaInicio.HasValue)
                {
                    command.Parameters.AddWithValue("@FechaInicio", FechaInicio.Value);
                }
                if (FechaDeFinalizacion.HasValue)
                {
                    command.Parameters.AddWithValue("@FechaDeFinalizacion", FechaDeFinalizacion.Value);
                }

                conexion.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Embarques embarque = new Embarques()
                    {
                        id = (int)reader["id"],
                        codebar = reader["codebar"].ToString(),
                        acronimo = reader["acronimo"].ToString(),
                        fechaLectura = (DateTime)reader["fechaLectura"],
                        objReferencia = reader["objReferencia"].ToString(),
                        tipo = (int)reader["tipo"],
                        Viaje = reader["Viaje"].ToString()
                    };
                    embarquesList.Add(embarque);
                }
                conexion.Close();
            }
            return embarquesList;

        }

        /// Ejemplo de consulta por Procedimiento Almacenado
        public List<Agrupamiento> GeneratorAddFilterByCount(DateTime? FechaInicio, DateTime? FechaDeFinalizacion)
        {
            List<Agrupamiento> agrupamientoList = new List<Agrupamiento>();
            using (SqlConnection conexion = new SqlConnection(cadena))
            {
                using (SqlCommand command = new SqlCommand("FilterByTravel", conexion))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DateFrom", (Object)FechaInicio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DateTo", (Object)FechaDeFinalizacion ?? DBNull.Value);

                    conexion.Open();
                    using (SqlDataReader read = command.ExecuteReader())
                    {
                        while (read.Read())
                        {
                            Agrupamiento agrupamiento = new Agrupamiento()
                            {
                                acronimo = read["acronimo"].ToString(),
                                cantidad = (int)read["cantidad"],
                                Viaje = read["Viaje"].ToString()
                            };
                            agrupamientoList.Add(agrupamiento);
                        }
                    }
                    conexion.Close();
                }
            }
            return agrupamientoList;

        }

        public List<TotalItemsCargados> GetTotalItemsCargados(DateTime? FechaInicio, DateTime? FechaDeFinalizacion)
        {
            List<TotalItemsCargados> totalItemsList = new List<TotalItemsCargados>();
            using (SqlConnection conexion = new SqlConnection(cadena))
            {
                // Construir la consulta SQL dinámicamente
                string query = @"
            SELECT
                Viaje,
                SUM(CASE WHEN tipo > 0 THEN 1 ELSE 0 END) AS TotalItems
            FROM
                tblRFID_CodiCaptEmbarques
            WHERE
                1=1";

                if (FechaInicio.HasValue)
                {
                    query += " AND fechaLectura >= @FechaInicio";
                }
                if (FechaDeFinalizacion.HasValue)
                {
                    query += " AND fechaLectura <= @FechaDeFinalizacion";
                }

                query += @"
            GROUP BY
                Viaje
            ORDER BY
                Viaje";

                using (SqlCommand command = new SqlCommand(query, conexion))
                {
                    if (FechaInicio.HasValue)
                    {
                        command.Parameters.AddWithValue("@FechaInicio", FechaInicio.Value);
                    }
                    if (FechaDeFinalizacion.HasValue)
                    {
                        command.Parameters.AddWithValue("@FechaDeFinalizacion", FechaDeFinalizacion.Value);
                    }

                    conexion.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TotalItemsCargados totalItem = new TotalItemsCargados()
                            {
                                Viaje = reader["Viaje"].ToString(),
                                TotalItems = (int)reader["TotalItems"]
                            };
                            totalItemsList.Add(totalItem);
                        }
                    }
                    conexion.Close();
                }
            }
            return totalItemsList;
        }





    }


}