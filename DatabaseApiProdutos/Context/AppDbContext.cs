using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Context
{
    public class AppDbContexto
    {

        string conexaostring;
        SqliteConnection sqliteconection;
        public AppDbContexto(IConfiguration configuration)
        {
            try
            {

                var caminhoBanco = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Banco.sqlite");
                if (!File.Exists(caminhoBanco))
                {

                    File.WriteAllTextAsync(caminhoBanco,"");
                }

                conexaostring = $"Data Source={caminhoBanco};Version=3;";
                sqliteconection = new SqliteConnection(conexaostring);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public SqliteConnection Connect()
        {
            try
            {

                if (sqliteconection.State != System.Data.ConnectionState.Open)
                {

                    sqliteconection.Open();
                }
                return sqliteconection;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                throw;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (sqliteconection.State != System.Data.ConnectionState.Closed)
                {
                    sqliteconection.Close();

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
                throw;
            }
        }

        public string GetConnectionString()
        {

            return conexaostring;
        }
    }
}
