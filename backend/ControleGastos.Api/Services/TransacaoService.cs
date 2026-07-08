using Microsoft.EntityFrameworkCore;
using ControleGastos.Api.Data;
using ControleGastos.Api.Dtos;
using ControleGastos.Api.Models;

namespace ControleGastos.Api.Services;

/// <summary>
/// Exceção específica para violações de regra de negócio.
/// Usar um tipo próprio (em vez de Exception genérica) permite que o
/// Controller devolva um HTTP 400 com uma mensagem clara para o front-end,
/// diferenciando de erros inesperados (que seriam HTTP 500).
/// </summary>
public class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string message) : base(message) { }
}

/// <summary>
/// Regras de negócio relacionadas ao cadastro de Transações.
/// </summary>
public class TransacaoService
{
    private readonly AppDbContext _context;

    public TransacaoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransacaoDto>> ListarAsync()
    {
        return await _context.Transacoes
            .Include(t => t.Pessoa)
            .OrderByDescending(t => t.Id)
            .Select(t => new TransacaoDto
            {
                Id = t.Id,
                Descricao = t.Descricao,
                Valor = t.Valor,
                Tipo = t.Tipo,
                PessoaId = t.PessoaId,
                PessoaNome = t.Pessoa!.Nome
            })
            .ToListAsync();
    }

    public async Task<TransacaoDto> CriarAsync(CriarTransacaoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Descricao))
            throw new RegraDeNegocioException("A descrição da transação é obrigatória.");

        if (dto.Valor <= 0)
            throw new RegraDeNegocioException("O valor da transação deve ser maior que zero.");

        // Regra: a pessoa informada precisa existir no cadastro.
        var pessoa = await _context.Pessoas.FindAsync(dto.PessoaId);
        if (pessoa is null)
            throw new RegraDeNegocioException("Pessoa informada não encontrada. Verifique o identificador.");

        // Regra: pessoa menor de idade (< 18 anos) só pode cadastrar despesas.
        if (pessoa.EhMenorDeIdade && dto.Tipo == TipoTransacao.Receita)
            throw new RegraDeNegocioException(
                $"{pessoa.Nome} é menor de idade ({pessoa.Idade} anos) e por isso só pode registrar despesas, não receitas.");

        var transacao = new Transacao
        {
            Descricao = dto.Descricao.Trim(),
            Valor = dto.Valor,
            Tipo = dto.Tipo,
            PessoaId = dto.PessoaId
        };

        _context.Transacoes.Add(transacao);
        await _context.SaveChangesAsync();

        return new TransacaoDto
        {
            Id = transacao.Id,
            Descricao = transacao.Descricao,
            Valor = transacao.Valor,
            Tipo = transacao.Tipo,
            PessoaId = transacao.PessoaId,
            PessoaNome = pessoa.Nome
        };
    }
}
