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

        private DataAccessDAL() 
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
                    using (SqlCommand command = new SqlCommand("SELECT COUNT (*) FROM tblRFID_CodiCaptEmbarques", connection))
                    {
                        int count = (int)command.ExecuteScalar();
                        return true;
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Error:" + ex.Message);
                return false;
            }
            }
    }
}