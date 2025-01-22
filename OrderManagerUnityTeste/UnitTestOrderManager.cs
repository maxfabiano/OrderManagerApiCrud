using AutoMapper;
using Database.Domain;
using Database.Repository;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace OrderManagerUnityTeste
{
    public class PedidoControllerTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PedidoController _controller;

        public PedidoControllerTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PedidoController(_mediatorMock.Object);
        }

        [Fact]
        public async Task CreatePedido_ShouldReturnOk_WhenCommandIsValid()
        {
            Iten iten = new Iten() { valor = 10, quantidade = 20 };

            Pedido pedido = new Pedido { nome = "Pedido Atualizado", itens = [iten] };            // Arrange
            var command = new CreatePedidoCommand(pedido);
            var Pedido = new Pedido { nome = "Pedido Teste" };
            Pedido.setId(1);
            _mediatorMock.Setup(x => x.Send(It.IsAny<CreatePedidoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Pedido);

            // Act
            var result = await _controller.CreatePedido(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(Pedido, okResult.Value);
        }

        [Fact]
        public async Task UpdatePedido_ShouldReturnNotFound_WhenPedidoNaoEncontrado()
        {
            // Arrange
            var id = 1;
            Iten iten = new Iten() { valor = 10, quantidade = 20};

            Pedido pedido = new Pedido { nome = "Pedido Atualizado", itens = [iten] };
            pedido.setId(id);
            var command = new UpdatePedidoCommand(pedido);
            _mediatorMock.Setup(x => x.Send(It.IsAny<UpdatePedidoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Pedido)null);

            // Act
            var result = await _controller.UpdatePedido(id, command);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        

        [Fact]
        public async Task DeletePedido_ShouldReturnNoContent_WhenSuccess()
        {
            // Arrange
            var id = 1;
            _mediatorMock.Setup(x => x.Send(It.IsAny<DeletePedidoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePedido(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePedido_ShouldReturnNotFound_WhenPedidoNaoEncontrado()
        {
            // Arrange
            var id = 1;
            _mediatorMock.Setup(x => x.Send(It.IsAny<DeletePedidoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeletePedido(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPedidoById_ShouldReturnOk_WhenPedidoEncontrado()
        {
            // Arrange
            var id = 1;
            var Pedido = new Pedido {  nome = "Pedido Teste" };
            Pedido.setId(id);

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetPedidoByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Pedido);

            // Act
            var result = await _controller.GetPedidoById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(Pedido, okResult.Value);
        }

        [Fact]
        public async Task GetPedidoById_ShouldReturnNotFound_WhenPedidoNaoEncontrado()
        {
            // Arrange
            var id = 1;
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetPedidoByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Pedido)null);

            // Act
            var result = await _controller.GetPedidoById(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAllPedidos_ShouldReturnOk()
        {
            // Arrange
            var Pedidos = new List<Pedido>
            {
                new Pedido {  nome = "Pedido 1" },
                new Pedido { nome = "Pedido 2" }
            };
            foreach (var Pedido in Pedidos)
            {
                Pedido.setId(Pedidos.IndexOf(Pedido) + 1);
            }

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllPedidosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Pedidos);

            // Act
            var result = await _controller.GetAllPedidos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(Pedidos, okResult.Value);
        }
    }
}