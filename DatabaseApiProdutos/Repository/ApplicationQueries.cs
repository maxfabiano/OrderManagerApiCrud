using Database.Domain;
using MediatR;

namespace Database.Repository
{
    public record getPdidoId(int id) : IRequest<Pedido>;
    public record getAllPedidos() : IRequest<IEnumerable<Pedido>>;

}
