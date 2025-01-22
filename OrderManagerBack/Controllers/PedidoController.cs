using OrderManager.Helpers;
using Database.Repository;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;

[ApiController]
[Route("orders")]
public class PedidoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAntiforgery _antiforgery;

    public PedidoController(IMediator mediator, IAntiforgery antiforgery)
    {
        _mediator = mediator;
        _antiforgery = antiforgery;
    }
    [HttpGet("GetToken")]
    public IActionResult GetToken()
    {
        // Gera o token antiforgery
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new { Token = tokens.RequestToken });
    }
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> CreatePedido([FromBody] CreatePedidoCommand command)
    {
        try
        {
            ValidarDados validarDados = new ValidarDados();
            validarDados.validarPedido(command.Pedido);
            var Pedido = await _mediator.Send(command);
            return Ok(Pedido);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePedido(int id, [FromBody] UpdatePedidoCommand command)
    {
        try
        {
        ValidarDados validarDados = new ValidarDados();
        validarDados.validarPedido(command.Pedido);
        command.Pedido.setId(id);
        var Pedido = await _mediator.Send(command);
        if (Pedido == null) return NotFound();
        return Ok(Pedido);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePedido(int id)
    {
        var success = await _mediator.Send(new DeletePedidoCommand(id));
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPedidoById(int id)
    {
        var Pedido = await _mediator.Send(new GetPedidoByIdQuery(id));
        if (Pedido == null) return NotFound();
        return Ok(Pedido);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPedidos()
    {
        var Pedidos = await _mediator.Send(new GetAllPedidosQuery());
        return Ok(Pedidos);
    }
}
