using Microsoft.EntityFrameworkCore;
using ControleGastos.Api.Data;
using ControleGastos.Api.Dtos;
using ControleGastos.Api.Models;

namespace ControleGastos.Api.Services;

/// <summary>
/// Responsável por consolidar os totais de receitas, despesas e saldo
/// por pessoa, e o total geral do sistema.
/// </summary>
public class TotaisService
{
    private readonly AppDbContext _context;

    public TotaisService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TotaisResponseDto> ObterTotaisAsync()
    {
        // Carregamos todas as pessoas já com suas transações para calcular
        // tudo em memória (o volume de dados de um controle doméstico é
        // pequeno, então isso é simples e eficiente o suficiente).
        var pessoas = await _context.Pessoas
            .Include(p => p.Transacoes)
            .OrderBy(p => p.Nome)
            .ToListAsync();

        var resposta = new TotaisResponseDto();

        foreach (var pessoa in pessoas)
        {
            var totalReceitas = pessoa.Transacoes
                .Where(t => t.Tipo == TipoTransacao.Receita)
                .Sum(t => t.Valor);

            var totalDespesas = pessoa.Transacoes
                .Where(t => t.Tipo == TipoTransacao.Despesa)
                .Sum(t => t.Valor);

            resposta.PorPessoa.Add(new TotalPorPessoaDto
            {
                PessoaId = pessoa.Id,
                PessoaNome = pessoa.Nome,
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas
            });

            resposta.TotalGeralReceitas += totalReceitas;
            resposta.TotalGeralDespesas += totalDespesas;
        }

        return resposta;
    }
}