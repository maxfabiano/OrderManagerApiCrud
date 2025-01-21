using CadastroDeProdutosNovo.Helpers;
using Database.Repository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("orders")]
public class ProdutoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProdutoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduto([FromBody] CreateProdutoCommand command)
    {
        try
        {
            ValidarDados validarDados = new ValidarDados();
            validarDados.validarProduto(command.produto);
            var produto = await _mediator.Send(command);
            return Ok(produto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduto(int id, [FromBody] UpdateProdutoCommand command)
    {
        try
        {
        ValidarDados validarDados = new ValidarDados();
        validarDados.validarProduto(command.produto);
        command.produto.setId(id);
        var produto = await _mediator.Send(command);
        if (produto == null) return NotFound();
        return Ok(produto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduto(int id)
    {
        var success = await _mediator.Send(new DeleteProdutoCommand(id));
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProdutoById(int id)
    {
        var produto = await _mediator.Send(new GetProdutoByIdQuery(id));
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProdutos()
    {
        var produtos = await _mediator.Send(new GetAllProdutosQuery());
        return Ok(produtos);
    }
}
