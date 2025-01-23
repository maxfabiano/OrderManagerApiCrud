using AutoMapper;
using Database.Domain;
using Database.Repository;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace OrderManagerUnityTeste
{
    public class PedidoControllerTest
    {
         Mock<IMediator> _mediatorMock;
         PedidoController pedidoAi;
         Mock<IAntiforgery> antFo;

        public PedidoControllerTest()
        {
            _mediatorMock = new Mock<IMediator>();
            pedidoAi = new PedidoController(_mediatorMock.Object, antFo.Object); 
        }

        [Fact]
        public async Task criarPedidoComandoValido()
        {
            Iten iten = new Iten() { valor = 10, quantidade = 20 };

            Pedido pedido = new Pedido();
            pedido.nome = "Pedido Teste";
            pedido.itens = [iten];
            var command = new createPedidoCommand(pedido);
            var Pedido = new Pedido { nome = "Pedido Teste" };
            Pedido.setId(1);
            _mediatorMock.Setup(x => x.Send(It.IsAny<createPedidoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Pedido);
            var result = await pedidoAi.CreatePedidom(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(Pedido, okResult.Value);
        }

        [Fact]
        public async Task atualizarPedidoComandoInvalido()
        {
            var id = 1;
            Iten iten = new Iten();
            iten.valor = 10;
            iten.quantidade = 20;

            Pedido pedido = new Pedido();
            pedido.nome = "Pedido Atualizado";
            pedido.itens = [iten];
            pedido.setId(id);
            var command = new updatePedidoCommand(pedido);
            _mediatorMock.Setup(x => x.Send(It.IsAny<updatePedidoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Pedido)null);

            var result = await pedidoAi.UpdatePedidoa(id, command);

            Assert.IsType<NotFoundResult>(result);
        }

        

        [Fact]
        public async Task deletarPedidoComandoValido()
        {
            var id = 1;
            _mediatorMock.Setup(x => x.Send(It.IsAny<deletePedidoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await pedidoAi.DeletePedidox(id);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletarPedidoComandoInvalido()
        {
            var id = 1;
            _mediatorMock.Setup(x => x.Send(It.IsAny<deletePedidoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var result = await pedidoAi.DeletePedidox(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task pegarPedidoInvalido()
        {
            var id = 1;
            var pedido = new Pedido ();
            pedido.nome ="Pedido Teste";
            pedido.setId(id);

            _mediatorMock.Setup(x => x.Send(It.IsAny<getPdidoId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pedido);

            var result = await pedidoAi.GetPedidoByIdm(id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(pedido, okResult.Value);
        }

        [Fact]
        public async Task pegarPedidoIdInvalido()
        {
            var id = 1;
            _mediatorMock.Setup(x => x.Send(It.IsAny<getPdidoId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Pedido)null);

            var result = await pedidoAi.GetPedidoByIdm(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task pegarTodosPedidosInvalido()
        {
            var pedidos = new List<Pedido>();
            pedidos.Add(new Pedido { nome = "Pedido Teste" });
            pedidos.Add(new Pedido { nome = "Pedido Teste 2" });
            foreach (var pedido in pedidos)
            {
                pedido.setId(pedidos.IndexOf(pedido) + 1);
            }

            _mediatorMock.Setup(x => x.Send(It.IsAny<getAllPedidos>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pedidos);
            var result = await pedidoAi.GetAllPedidosax();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(pedidos, okResult.Value);
        }
    }
}