using Database.Domain;
using MediatR;

namespace Database.Repository
{
    public record getPedidoId(int id, string? nome = null, string? data = null) : IRequest<IEnumerable<Pedido>>;
    public record getAllPedidos() : IRequest<IEnumerable<Pedido>>;

}
