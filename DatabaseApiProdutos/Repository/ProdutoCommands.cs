using Database.Domain;
using MediatR;

namespace Database.Repository
{

    public record createPedidoCommand(Pedido Pedido) : IRequest<Pedido>;

    public record updatePedidoCommand(Pedido Pedido) : IRequest<Pedido>;
    public record deletePedidoCommand(int id) : IRequest<bool>;
}
