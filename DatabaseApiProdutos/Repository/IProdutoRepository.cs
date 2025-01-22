using Database.Domain;
using Database.Repository;
using MediatR;

namespace Repository;

public interface IPedidoRepository
{
   Task<Pedido> Handle(createPedidoCommand comando, CancellationToken tokenCanceel);

   Task<Pedido> Handle(updatePedidoCommand comando, CancellationToken tokenCanceel);

   Task<bool> Handle(deletePedidoCommand comando, CancellationToken tokenCanceel);
    Task<IEnumerable<Pedido>> Handle(getPedidoId comando, CancellationToken tokenCanceel);
   Task<IEnumerable<Pedido>> Handle(getAllPedidos comando, CancellationToken tokenCanceel);


}