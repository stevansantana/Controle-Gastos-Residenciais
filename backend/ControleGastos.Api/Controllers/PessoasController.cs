using Microsoft.AspNetCore.Mvc;
using ControleGastos.Api.Dtos;
using ControleGastos.Api.Services;

namespace ControleGastos.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // -> api/pessoas
public class PessoasController : ControllerBase
{
    private readonly PessoaService _pessoaService;

    public PessoasController(PessoaService pessoaService)
    {
        _pessoaService = pessoaService;
    }

    /// <summary>GET /api/pessoas — lista todas as pessoas cadastradas.</summary>
    [HttpGet]
    public async Task<ActionResult<List<PessoaDto>>> Listar()
    {
        var pessoas = await _pessoaService.ListarAsync();
        return Ok(pessoas);
    }

    /// <summary>POST /api/pessoas — cria uma nova pessoa.</summary>
    [HttpPost]
    public async Task<ActionResult<PessoaDto>> Criar([FromBody] CriarPessoaDto dto)
    {
        try
        {
            var pessoa = await _pessoaService.CriarAsync(dto);
            // 201 Created + header "Location" apontando para o novo recurso (boa prática REST)
            return CreatedAtAction(nameof(Listar), new { id = pessoa.Id }, pessoa);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// DELETE /api/pessoas/{id} — remove uma pessoa e, em cascata,
    /// todas as suas transações (regra implementada no banco de dados).
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var removida = await _pessoaService.DeletarAsync(id);
        if (!removida)
            return NotFound(new { mensagem = "Pessoa não encontrada." });

        return NoContent(); // 204: sucesso, sem corpo de resposta
    }
}
