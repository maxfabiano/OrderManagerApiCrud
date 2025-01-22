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
    public class PedidoRepository : IPedidoRepository,
        IRequestHandler<CreatePedidoCommand, Pedido>,
        IRequestHandler<UpdatePedidoCommand, Pedido>,
        IRequestHandler<DeletePedidoCommand, bool>,
        IRequestHandler<GetPedidoByIdQuery, Pedido>,
        IRequestHandler<GetAllPedidosQuery, IEnumerable<Pedido>>
    {
        private readonly string _connectionString;

        public PedidoRepository(IConfiguration configuration)
        {
            var databasePath = Path.Combine(Directory.GetCurrentDirectory(), "Banco.sqlite");
           
            if (!File.Exists(databasePath))
            {
                File.WriteAllTextAsync(databasePath, "");
                
            }
            _connectionString = configuration.GetConnectionString("DefaultConnection");

            CreatedatabaseSchemaAsync();
        }

        private async Task CreatedatabaseSchemaAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            const string schema = @"
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
                var result = await connection.ExecuteAsync(schema); 
            
            }
            catch (Exception ex){
                throw new Exception(ex.Message);
            
            }
        }

        #region Handlers

        public async Task<Pedido> Handle(CreatePedidoCommand request, CancellationToken cancellationToken)
        {
            const string PedidoSql = @"
                INSERT INTO Pedido (nome, data, valorTotal) 
                VALUES (@nome, @data, @valorTotal);
                SELECT last_insert_rowid();";

            const string itensSql = @"
                INSERT INTO itens (nome, valor, quantidade, PedidoId) 
                VALUES (@nome, @valor, @quantidade, @PedidoId);";

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            using var transaction = connection.BeginTransaction();
            try
            {
                var idPedido = await connection.QuerySingleAsync<int>(PedidoSql, new
                {
                    request.Pedido.nome,
                    request.Pedido.data,
                    request.Pedido.valorTotal
                }, transaction);

                foreach (var item in request.Pedido.itens)
                {
                    await connection.ExecuteAsync(itensSql, new
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
                    nome = request.Pedido.nome,
                    data = request.Pedido.data,
                    itens = request.Pedido.itens
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<Pedido> Handle(UpdatePedidoCommand request, CancellationToken cancellationToken)
        {
            const string PedidoSql = @"
                UPDATE Pedido 
                SET nome = @nome, data = @data, valorTotal = @valorTotal
                WHERE id = @id";

            const string deleteitensSql = "DELETE FROM itens WHERE PedidoId = @PedidoId";

            const string itensSql = @"
                INSERT INTO itens (nome, valor, quantidade, PedidoId) 
                VALUES (@nome, @valor, @quantidade, @PedidoId);";

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            using var transaction = connection.BeginTransaction();
            try
            {
                var rowsAffected = await connection.ExecuteAsync(PedidoSql, new
                {
                    request.Pedido.nome,
                    request.Pedido.data,
                    request.Pedido.valorTotal,
                    id = request.Pedido.id
                }, transaction);

                if (rowsAffected == 0) return null;

                await connection.ExecuteAsync(deleteitensSql, new { PedidoId = request.Pedido.id }, transaction);

                foreach (var item in request.Pedido.itens)
                {
                    await connection.ExecuteAsync(itensSql, new
                    {
                        item.nome,
                        item.valor,
                        item.quantidade,
                        PedidoId = request.Pedido.id
                    }, transaction);
                }

                transaction.Commit();
                return new Pedido
                {
                    nome = request.Pedido.nome,
                    data = request.Pedido.data,
                    itens = request.Pedido.itens
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> Handle(DeletePedidoCommand request, CancellationToken cancellationToken)
        {
            const string sql = "DELETE FROM Pedido WHERE id = @id";

            using var connection = new SqliteConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new { request.id });
            return rowsAffected > 0;
        }

        public async Task<Pedido> Handle(GetPedidoByIdQuery request, CancellationToken cancellationToken)
        {
            const string PedidoSql = "SELECT * FROM Pedido WHERE id = @id";
            const string itensSql = "SELECT * FROM itens WHERE PedidoId = @PedidoId";

            using var connection = new SqliteConnection(_connectionString);

            var Pedido = await connection.QuerySingleOrDefaultAsync<Pedido>(PedidoSql, new { id = request.id });
            if (Pedido != null)
            {
                var itens = await connection.QueryAsync<Iten>(itensSql, new { PedidoId = Pedido.id });
                Pedido.itens = itens.ToList();
            }

            return Pedido;
        }

        public async Task<IEnumerable<Pedido>> Handle(GetAllPedidosQuery request, CancellationToken cancellationToken)
        {
            const string PedidosSql = "SELECT * FROM Pedido";
            const string itensSql = "SELECT * FROM itens WHERE PedidoId = @PedidoId";

            using var connection = new SqliteConnection(_connectionString);

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
