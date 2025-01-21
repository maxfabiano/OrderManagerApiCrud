using Database.Domain;
using MediatR;

namespace Database.Repository
{
    public record CreateProdutoCommand(Produto produto) : IRequest<Produto>;

    public record UpdateProdutoCommand(Produto produto) : IRequest<Produto>;

    public record DeleteProdutoCommand(int id) : IRequest<bool>;
}
