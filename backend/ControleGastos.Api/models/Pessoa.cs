namespace ControleGastos.Api.Models;

/// <summary>
/// Representa uma pessoa do domicílio que pode ter transações financeiras associadas.
/// </summary>
public class Pessoa
{
    /// <summary>
    /// Identificador único, gerado automaticamente pelo banco (Guid).
    /// Usamos Guid em vez de int autoincremental para evitar colisões
    /// e não depender da ordem de inserção.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Nome { get; set; } = string.Empty;

    public int Idade { get; set; }

    /// <summary>
    /// Propriedade de navegação do EF Core: lista das transações desta pessoa.
    /// Configuramos "delete em cascata" no AppDbContext, então ao remover
    /// a Pessoa, todas as Transações relacionadas são apagadas automaticamente.
    /// </summary>
    public List<Transacao> Transacoes { get; set; } = new();

    /// <summary>
    /// Regra de negócio: pessoa é considerada menor de idade se tiver menos de 18 anos.
    /// Colocar essa regra aqui (e não espalhada pelo código) facilita manutenção:
    /// se um dia a maioridade mudar de critério, só muda em um lugar.
    /// </summary>
    public bool EhMenorDeIdade => Idade < 18;
}
