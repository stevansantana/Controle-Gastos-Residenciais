namespace ControleGastos.Api.Models;

/// <summary>
/// Representa uma transação financeira (receita ou despesa) associada a uma pessoa.
/// </summary>
public class Transacao
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Valor monetário da transação. Sempre armazenado como um número positivo;
    /// o "sinal" (se soma ou subtrai do saldo) é dado pelo campo Tipo.
    /// </summary>
    public decimal Valor { get; set; }

    public TipoTransacao Tipo { get; set; }

    /// <summary>
    /// Chave estrangeira para a Pessoa dona da transação.
    /// </summary>
    public Guid PessoaId { get; set; }

    /// <summary>
    /// Propriedade de navegação (opcional de usar, mas o EF Core precisa dela
    /// para conseguir montar o relacionamento 1:N entre Pessoa e Transacao).
    /// </summary>
    public Pessoa? Pessoa { get; set; }
}
