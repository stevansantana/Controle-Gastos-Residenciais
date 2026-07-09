using Microsoft.AspNetCore.Mvc;
using ControleGastos.Api.Dtos;
using ControleGastos.Api.Services;

namespace ControleGastos.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // -> api/transacoes
public class TransacoesController : ControllerBase
{
    private readonly TransacaoService _transacaoService;

    public TransacoesController(TransacaoService transacaoService)
    {
        _transacaoService = transacaoService;
    }

    /// <summary>GET /api/transacoes — lista todas as transações cadastradas.</summary>
    [HttpGet]
    public async Task<ActionResult<List<TransacaoDto>>> Listar()
    {
        var transacoes = await _transacaoService.ListarAsync();
        return Ok(transacoes);
    }

    /// <summary>
    /// POST /api/transacoes — cria uma nova transação.
    /// Valida: pessoa existente e regra do menor de idade (só despesa).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TransacaoDto>> Criar([FromBody] CriarTransacaoDto dto)
    {
        try
        {
            var transacao = await _transacaoService.CriarAsync(dto);
            return CreatedAtAction(nameof(Listar), new { id = transacao.Id }, transacao);
        }
        catch (RegraDeNegocioException ex)
        {
            // 400 Bad Request: erro de validação de regra de negócio, não um bug.
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
