using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient; /// Importación para trabajar con provedor de Datos 

namespace Lester.Models
{
    public class DataAccessDAL
    {
        private string cadena;

        public DataAccessDAL() 
        {
            cadena = System.Configuration.ConfigurationManager.ConnectionStrings["Lester"].ConnectionString; 
        }

        public bool ProbarConexion() // Función para probar la conexión atravez de una consulta
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cadena))
                {
                    connection.Open();
                    string countCommand = "SELECT COUNT (*) FROM tblRFID_CodiCaptEmbarques";
                    using (SqlCommand command = new SqlCommand(countCommand, connection))
                    {
                        int count = (int)command.ExecuteScalar();
                        connection.Close();
                        return true;
                    }
                }
                
            }
            catch (Exception ex) {
                Console.WriteLine("Error:" + ex.Message);
                return false;
            }
        }


        public List<Embarques> GeneradorDeEmabrquesPorRango(DateTime FechaInicio, DateTime FechaDeFinalizacion)
        {
            List<Embarques> embarquesList = new List<Embarques>();
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    string SqlQueryComnad = "SELECT * FROM tblRFID_CodiCaptEmbarques WHERE fechaLectura BETWEEN @FechaInicio AND @FechaDeFinalizacion ";
                    SqlCommand command = new SqlCommand(SqlQueryComnad, conexion);
                    command.Parameters.AddWithValue("@FechaInicio", FechaInicio);
                    command.Parameters.AddWithValue("@FechaDeFinalizacion", FechaDeFinalizacion);
                    conexion.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read()) {
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
    }
}