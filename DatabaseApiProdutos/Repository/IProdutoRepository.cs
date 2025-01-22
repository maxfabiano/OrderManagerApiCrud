using Database.Domain;
using Database.Repository;
using MediatR;

namespace Repository;

public interface IPedidoRepository
{
   Task<Pedido> Handle(CreatePedidoCommand request, CancellationToken cancellationToken);
   Task<Pedido> Handle(UpdatePedidoCommand request, CancellationToken cancellationToken);
   Task<bool> Handle(DeletePedidoCommand request, CancellationToken cancellationToken);
   Task<Pedido> Handle(GetPedidoByIdQuery request, CancellationToken cancellationToken);
   Task<IEnumerable<Pedido>> Handle(GetAllPedidosQuery request, CancellationToken cancellationToken);

}