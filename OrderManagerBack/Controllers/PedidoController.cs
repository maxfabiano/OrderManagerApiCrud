using OrderManager.Helpers;
using Database.Repository;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using Database.Domain;

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
        var tokens = tokenfor.GetAndStoreTokens(HttpContext);
        return Ok(new { Token = tokens.RequestToken });
    }
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> CreatePedidom([FromBody] createPedidoCommand command)
    {
        try
        {

            ValidarDados validarDados = new ValidarDados();
            validarDados.validarPedido(command.Pedido);

            var pedidos = await mediador.Send(command);
            return Ok(pedidos);

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePedidoa(int id, [FromBody] updatePedidoCommand command)
    {

        try
        {

        ValidarDados validarDados = new ValidarDados();
        validarDados.validarPedido(command.Pedido);
        command.Pedido.setId(id);

        var pedido = await mediador.Send(command);

            if (pedido == null) { 
                return NotFound();
            }
        return Ok(pedido);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePedidox(int id)
    {
        var success = await mediador.Send(new deletePedidoCommand(id));
        if (!success) { 
            return NotFound(); 
        }

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPedidoByIdm(int id, string? nome=null, string? data = null)
    {
        IEnumerable<Pedido> pedido;
        if(id < 0)
        {
            return BadRequest();
        }

        if (nome != null) {
            pedido = await mediador.Send(new getPedidoId(id,nome));
            return Ok(pedido);

        }
        if (data != null)
        {
            pedido = await mediador.Send(new getPedidoId(id, data));
            return Ok(pedido);

        }
        else
        {
            pedido = await mediador.Send(new getPedidoId(id));
        }
        if (pedido == null)
        {
            return NotFound();
        }
        return Ok(pedido);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPedidosax()
    {
        var pedidos = await mediador.Send(new getAllPedidos());

        return Ok(pedidos);
    }
}
