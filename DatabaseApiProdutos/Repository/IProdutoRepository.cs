using Database.Domain;
using Database.Repository;
using MediatR;

namespace Repository;

public interface IProdutoRepository
{
   Task<Produto> Handle(CreateProdutoCommand request, CancellationToken cancellationToken);
   Task<Produto> Handle(UpdateProdutoCommand request, CancellationToken cancellationToken);
   Task<bool> Handle(DeleteProdutoCommand request, CancellationToken cancellationToken);
   Task<Produto> Handle(GetProdutoByIdQuery request, CancellationToken cancellationToken);
   Task<IEnumerable<Produto>> Handle(GetAllProdutosQuery request, CancellationToken cancellationToken);

}