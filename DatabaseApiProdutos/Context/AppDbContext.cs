using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Context
{
    public class AppDbContexto
    {
        private readonly string _connectionString;
        private SqliteConnection _sqliteConnection;

        public AppDbContexto(IConfiguration configuration)
        {
            try
            {
                // Define o caminho do banco de dados local
                var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Banco.sqlite");

                // Verifica se o banco já existe, caso contrário, cria um novo
                if (!File.Exists(databasePath))
                {
                    File.WriteAllTextAsync(databasePath,"");
                    Debug.WriteLine("Banco de dados SQLite criado em: " + databasePath);
                }

                // Configura a string de conexão
                _connectionString = $"Data Source={databasePath};Version=3;";
                _sqliteConnection = new SqliteConnection(_connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao configurar o contexto do banco: {ex.Message}", ex);
            }
        }

        public SqliteConnection Connect()
        {
            try
            {
                if (_sqliteConnection.State != System.Data.ConnectionState.Open)
                {
                    _sqliteConnection.Open();
                }
                return _sqliteConnection;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erro ao abrir conexão: " + ex.Message);
                throw;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_sqliteConnection.State != System.Data.ConnectionState.Closed)
                {
                    _sqliteConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erro ao fechar conexão: " + ex.Message);
                throw;
            }
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
