using Database.Domain;
using MediatR;

namespace Database.Repository
{
    public record CreatePedidoCommand(Pedido Pedido) : IRequest<Pedido>;

    public record UpdatePedidoCommand(Pedido Pedido) : IRequest<Pedido>;

    public record DeletePedidoCommand(int id) : IRequest<bool>;
}
