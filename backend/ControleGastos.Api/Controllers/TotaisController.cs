using Microsoft.AspNetCore.Mvc;
using ControleGastos.Api.Dtos;
using ControleGastos.Api.Services;

namespace ControleGastos.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // -> api/totais
public class TotaisController : ControllerBase
{
    private readonly TotaisService _totaisService;

    public TotaisController(TotaisService totaisService)
    {
        _totaisService = totaisService;
    }

    /// <summary>
    /// GET /api/totais — retorna, para cada pessoa, o total de receitas,
    /// despesas e saldo; e ao final o total geral de todas as pessoas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<TotaisResponseDto>> Obter()
    {
        var totais = await _totaisService.ObterTotaisAsync();
        return Ok(totais);
    }
}
