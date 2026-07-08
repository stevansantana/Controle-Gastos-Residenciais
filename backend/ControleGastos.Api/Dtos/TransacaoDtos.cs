using ControleGastos.Api.Models;

namespace ControleGastos.Api.Dtos;

/// <summary>
/// Dados recebidos do cliente para criar uma transação.
/// </summary>
public class CriarTransacaoDto
{
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public TipoTransacao Tipo { get; set; }
    public Guid PessoaId { get; set; }
}

/// <summary>
/// Dados devolvidos ao cliente ao listar/criar uma transação.
/// Incluímos o nome da pessoa para facilitar a exibição na tela,
/// sem o front precisar cruzar os dados manualmente.
/// </summary>
public class TransacaoDto
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public TipoTransacao Tipo { get; set; }
    public Guid PessoaId { get; set; }
    public string PessoaNome { get; set; } = string.Empty;
}
