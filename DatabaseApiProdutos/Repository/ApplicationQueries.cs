using Database.Domain;
using MediatR;

namespace Database.Repository
{
    public record GetProdutoByIdQuery(int id) : IRequest<Produto>;

    public record GetAllProdutosQuery() : IRequest<IEnumerable<Produto>>;
}
