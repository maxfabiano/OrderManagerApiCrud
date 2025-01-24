using Database.Domain;
using Database.Repository;
using MediatR;

namespace Repository;

public interface IPedidoRepository
{
   Task<Pedido> Handle(createPedidoCommand comando, CancellationToken cancelamentoToken);

   Task<Pedido> Handle(updatePedidoCommand comando, CancellationToken cancelamentoToken);

   Task<bool> Handle(deletePedidoCommand comando, CancellationToken cancelamentoToken);
    Task<IEnumerable<Pedido>> Handle(getPedidoId comando, CancellationToken cancelamentoToken);
   Task<IEnumerable<Pedido>> Handle(getAllPedidos comando, CancellationToken cancelamentoToken);


}