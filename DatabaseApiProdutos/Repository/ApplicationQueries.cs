using Database.Domain;
using MediatR;

namespace Database.Repository
{
    public record GetPedidoByIdQuery(int id) : IRequest<Pedido>;

    public record GetAllPedidosQuery() : IRequest<IEnumerable<Pedido>>;
}
