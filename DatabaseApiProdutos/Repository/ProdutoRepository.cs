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
    public class ProdutoRepository : IProdutoRepository,
        IRequestHandler<CreateProdutoCommand, Produto>,
        IRequestHandler<UpdateProdutoCommand, Produto>,
        IRequestHandler<DeleteProdutoCommand, bool>,
        IRequestHandler<GetProdutoByIdQuery, Produto>,
        IRequestHandler<GetAllProdutosQuery, IEnumerable<Produto>>
    {
        private readonly string _connectionString;

        public ProdutoRepository(IConfiguration configuration)
        {
            var databasePath = Path.Combine(Directory.GetCurrentDirectory(), "Banco.sqlite");

            if (!File.Exists(databasePath))
            {
                File.WriteAllTextAsync(databasePath, "");
                CreatedatabaseSchema();
            }

            _connectionString = $"data Source={databasePath};";
        }

        private void CreatedatabaseSchema()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            const string schema = @"
                CREATE TABLE IF NOT EXISTS Produto (
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
                    produtoId INTEGER NOT NULL,
                    FOREIGN KEY (produtoId) REFERENCES Produto (Id) ON DELETE CASCADE
                );
            ";

            connection.Execute(schema);
        }

        #region Handlers

        public async Task<Produto> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
        {
            const string produtoSql = @"
                INSERT INTO Produto (nome, data, valorTotal) 
                VALUES (@nome, @data, @valorTotal);
                SELECT last_insert_rowid();";

            const string itensSql = @"
                INSERT INTO itens (nome, valor, quantidade, produtoId) 
                VALUES (@nome, @valor, @quantidade, @produtoId);";

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            using var transaction = connection.BeginTransaction();
            try
            {
                var idProduto = await connection.QuerySingleAsync<int>(produtoSql, new
                {
                    request.produto.nome,
                    request.produto.data,
                    request.produto.valorTotal
                }, transaction);

                foreach (var item in request.produto.itens)
                {
                    await connection.ExecuteAsync(itensSql, new
                    {
                        item.nome,
                        item.valor,
                        item.quantidade,
                        produtoId = idProduto
                    }, transaction);
                }

                transaction.Commit();
                return new Produto
                {
                    nome = request.produto.nome,
                    data = request.produto.data,
                    itens = request.produto.itens
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<Produto> Handle(UpdateProdutoCommand request, CancellationToken cancellationToken)
        {
            const string produtoSql = @"
                UPDATE Produto 
                SET nome = @nome, data = @data, valorTotal = @valorTotal
                WHERE id = @id";

            const string deleteitensSql = "DELETE FROM itens WHERE produtoId = @produtoId";

            const string itensSql = @"
                INSERT INTO itens (nome, valor, quantidade, produtoId) 
                VALUES (@nome, @valor, @quantidade, @produtoId);";

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            using var transaction = connection.BeginTransaction();
            try
            {
                var rowsAffected = await connection.ExecuteAsync(produtoSql, new
                {
                    request.produto.nome,
                    request.produto.data,
                    request.produto.valorTotal,
                    id = request.produto.id
                }, transaction);

                if (rowsAffected == 0) return null;

                await connection.ExecuteAsync(deleteitensSql, new { produtoId = request.produto.id }, transaction);

                foreach (var item in request.produto.itens)
                {
                    await connection.ExecuteAsync(itensSql, new
                    {
                        item.nome,
                        item.valor,
                        item.quantidade,
                        produtoId = request.produto.id
                    }, transaction);
                }

                transaction.Commit();
                return new Produto
                {
                    nome = request.produto.nome,
                    data = request.produto.data,
                    itens = request.produto.itens
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> Handle(DeleteProdutoCommand request, CancellationToken cancellationToken)
        {
            const string sql = "DELETE FROM Produto WHERE id = @id";

            using var connection = new SqliteConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new { request.id });
            return rowsAffected > 0;
        }

        public async Task<Produto> Handle(GetProdutoByIdQuery request, CancellationToken cancellationToken)
        {
            const string produtoSql = "SELECT * FROM Produto WHERE id = @id";
            const string itensSql = "SELECT * FROM itens WHERE produtoId = @produtoId";

            using var connection = new SqliteConnection(_connectionString);

            var produto = await connection.QuerySingleOrDefaultAsync<Produto>(produtoSql, new { id = request.id });
            if (produto != null)
            {
                var itens = await connection.QueryAsync<Iten>(itensSql, new { produtoId = produto.id });
                produto.itens = itens.ToList();
            }

            return produto;
        }

        public async Task<IEnumerable<Produto>> Handle(GetAllProdutosQuery request, CancellationToken cancellationToken)
        {
            const string produtosSql = "SELECT * FROM Produto";
            const string itensSql = "SELECT * FROM itens WHERE produtoId = @produtoId";

            using var connection = new SqliteConnection(_connectionString);

            var produtos = await connection.QueryAsync<Produto>(produtosSql);

            foreach (var produto in produtos)
            {
                var itens = await connection.QueryAsync<Iten>(itensSql, new { produtoId = produto.id });
                produto.itens = itens.ToList();
            }

            return produtos;
        }

        #endregion
    }
}
