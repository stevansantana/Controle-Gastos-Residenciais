namespace ControleGastos.Api.Dtos;

/// <summary>
/// Totais consolidados de uma única pessoa.
/// </summary>
public class TotalPorPessoaDto
{
    public Guid PessoaId { get; set; }
    public string PessoaNome { get; set; } = string.Empty;
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }

    /// <summary>Saldo = Receitas - Despesas.</summary>
    public decimal Saldo => TotalReceitas - TotalDespesas;
}

/// <summary>
/// Resposta completa da consulta de totais: totais por pessoa + total geral.
/// </summary>
public class TotaisResponseDto
{
    public List<TotalPorPessoaDto> PorPessoa { get; set; } = new();

    public decimal TotalGeralReceitas { get; set; }
    public decimal TotalGeralDespesas { get; set; }
    public decimal SaldoGeral => TotalGeralReceitas - TotalGeralDespesas;
}
