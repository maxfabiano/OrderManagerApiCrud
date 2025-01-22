using OrderManager.Helpers;
using Database.Repository;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;

[ApiController]
[Route("orders")]
public class PedidoController : ControllerBase
{
     IMediator mediador;
     IAntiforgery tokenfor;

    public PedidoController(IMediator media, IAntiforgery tokenantfo)
    {

        mediador = media;
        tokenfor = tokenantfo;
    }
    [HttpGet("GetToken")]
    public IActionResult GetToken()
    {
        // Gera o token antiforgery
        var tokens = tokenfor.GetAndStoreTokens(HttpContext);
        return Ok(new { Token = tokens.RequestToken });
    }
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> CreatePedido([FromBody] createPedidoCommand command)
    {
        try
        {

            ValidarDados validarDados = new ValidarDados();
            validarDados.validarPedido(command.Pedido);

            var Pedido = await mediador.Send(command);
            return Ok(Pedido);

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePedido(int id, [FromBody] updatePedidoCommand command)
    {

        try
        {

        ValidarDados validarDados = new ValidarDados();
        validarDados.validarPedido(command.Pedido);
        command.Pedido.setId(id);

        var Pedido = await mediador.Send(command);

            if (Pedido == null) { 
                return NotFound();
            }
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
        var success = await mediador.Send(new deletePedidoCommand(id));
        if (!success) { 
            return NotFound(); 
        }

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPedidoById(int id)
    {
        var Pedido = await mediador.Send(new getPdidoId(id));

        if (Pedido == null)
        {
            return NotFound();
        }
        return Ok(Pedido);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPedidos()
    {
        var Pedidos = await mediador.Send(new getAllPedidos());

        return Ok(Pedidos);
    }
}
