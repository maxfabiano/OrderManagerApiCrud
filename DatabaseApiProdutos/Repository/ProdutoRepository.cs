using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Domain;
using Database.Repository;
using Dapper;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Repository;

namespace Database.Repository
{
    public class PedidoRepository : IPedidoRepository, IRequestHandler<createPedidoCommand, Pedido>,
        IRequestHandler<updatePedidoCommand, Pedido>,
        IRequestHandler<deletePedidoCommand, bool>, IRequestHandler<getPdidoId, Pedido>,
        IRequestHandler<getAllPedidos, IEnumerable<Pedido>>
    {
        string _stringConexao;
        public PedidoRepository(IConfiguration configuration)
        {
            var databasePath = Path.Combine(Directory.GetCurrentDirectory(), "Banco.sqlite");
            if (!File.Exists(databasePath))
            {
                File.WriteAllTextAsync(databasePath, "");


            }
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
            CreatedatabaseSchemaAsync();
        }

        private async Task CreatedatabaseSchemaAsync()
        {
            using (var connection = new SqliteConnection(_stringConexao)) {
                connection.Open();
                string queryy = @"
                CREATE TABLE IF NOT EXISTS Pedido (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    nome TEXT NOT NULL,
                    data TEXT NOT NULL,
                    valorTotal REAL NOT NULL
                );

                CREATE TABLE IF NOT EXISTS itens (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    nome TEXT NOT NULL,
                    valor REAL NOT NULL,
                    quantidade INTEGER NOT NULL,
                    PedidoId INTEGER NOT NULL,
                    FOREIGN KEY (PedidoId) REFERENCES Pedido (id) ON DELETE CASCADE
                );
            ";

                try {

                    var result = await connection.ExecuteAsync(queryy);

                }
                catch (Exception ex) {
                    throw new Exception(ex.Message);


                }
            }
        }

        #region Handlers

        public async Task<Pedido> Handle(createPedidoCommand comando, CancellationToken cancellationToken)
        {
            string queyPedido = @"
                INSERT INTO Pedido (nome, data, valorTotal) 
                VALUES (@nome, @data, @valorTotal);
                SELECT last_insert_rowid();";
            string queryIten = @"
                INSERT INTO itens (nome, valor, quantidade, PedidoId) 
                VALUES (@nome, @valor, @quantidade, @PedidoId);";
            using (var connection = new SqliteConnection(_stringConexao))
            {
                await connection.OpenAsync(cancellationToken);

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var idPedido = await connection.QuerySingleAsync<int>(queyPedido, new
                        {
                            comando.Pedido.nome,
                            comando.Pedido.data,
                            comando.Pedido.valorTotal
                        }, transaction);

                        foreach (var item in comando.Pedido.itens)
                        {
                            await connection.ExecuteAsync(queryIten, new
                            {

                                item.nome,
                                item.valor,
                                item.quantidade,
                                PedidoId = idPedido
                            }, transaction);

                        }
                        transaction.Commit();
                        return new Pedido
                        {

                            nome = comando.Pedido.nome,
                            data = comando.Pedido.data,
                            itens = comando.Pedido.itens
                        };
                    }
                    catch
                    {

                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<Pedido> Handle(updatePedidoCommand comando, CancellationToken cancellationToken)
        {
            string PedidoSql = @"
                UPDATE Pedido 
                SET nome = @nome, data = @data, valorTotal = @valorTotal
                WHERE id = @id";
            string deleteitensSql = "DELETE FROM itens WHERE PedidoId = @PedidoId";
            string itensSql = @"
                INSERT INTO itens (nome, valor, quantidade, PedidoId) 
                VALUES (@nome, @valor, @quantidade, @PedidoId);";
            using (var connection = new SqliteConnection(_stringConexao))
            {
                await connection.OpenAsync(cancellationToken);

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {

                        var rowsAffected = await connection.ExecuteAsync(PedidoSql, new
                        {

                            comando.Pedido.nome,
                            comando.Pedido.data,
                            comando.Pedido.valorTotal,
                            id = comando.Pedido.id
                        }, transaction);
                        if (rowsAffected == 0) return null;


                        await connection.ExecuteAsync(deleteitensSql, new { PedidoId = comando.Pedido.id }, transaction);
                        foreach (var item in comando.Pedido.itens)
                        {

                            await connection.ExecuteAsync(itensSql, new
                            {

                                item.nome,
                                item.valor,
                                item.quantidade,
                                PedidoId = comando.Pedido.id
                            }, transaction);
                        }
                        transaction.Commit();
                        return new Pedido
                        {
                            nome = comando.Pedido.nome,
                            data = comando.Pedido.data,
                            itens = comando.Pedido.itens
                        };
                    }
                    catch
                    {

                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> Handle(deletePedidoCommand comando, CancellationToken cancellationToken)
        {
            string sql = "DELETE FROM Pedido WHERE id = @id";
            using (var connection = new SqliteConnection(_stringConexao)) {

                var rowsAffected = await connection.ExecuteAsync(sql, new { comando.id });
                return rowsAffected > 0;

            }
        }

        public async Task<Pedido> Handle(getPdidoId comando, CancellationToken cancellationToken)
        {
            string PedidoSql = "SELECT * FROM Pedido WHERE id = @id";
            string itensSql = "SELECT * FROM itens WHERE PedidoId = @PedidoId";

            using (var connection = new SqliteConnection(_stringConexao)) {
                var Pedido = await connection.QuerySingleOrDefaultAsync<Pedido>(PedidoSql, new { id = comando.id })){
                    if (Pedido != null)
                    {
                        var itens = await connection.QueryAsync<Iten>(itensSql, new { PedidoId = Pedido.id });
                        Pedido.itens = itens.ToList();

                    }

                    return Pedido;
                }
            }
        }

        public async Task<IEnumerable<Pedido>> Handle(getAllPedidos comando, CancellationToken cancellationToken)
        {
            string PedidosSql = "SELECT * FROM Pedido";
            string itensSql = "SELECT * FROM itens WHERE PedidoId = @PedidoId";
            using (var connection = new SqliteConnection(_stringConexao)) {

                var Pedidos = await connection.QueryAsync<Pedido>(PedidosSql);
                foreach (var Pedido in Pedidos)
                {
                    var itens = await connection.QueryAsync<Iten>(itensSql, new { PedidoId = Pedido.id });
                    Pedido.itens = itens.ToList();
                }
                return Pedidos;
            }

            #endregion
        }
    }
}
