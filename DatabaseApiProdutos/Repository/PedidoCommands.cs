using Database.Domain;
using MediatR;

namespace Database.Repository
{

    public record createPedidoCommand(Pedido pedido) : IRequest<Pedido>;

    public record updatePedidoCommand(Pedido pedido) : IRequest<Pedido>;
    public record deletePedidoCommand(int id) : IRequest<bool>;
}
