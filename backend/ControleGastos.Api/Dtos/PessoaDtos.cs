namespace ControleGastos.Api.Dtos;

/// <summary>
/// Dados recebidos do cliente para criar uma pessoa.
/// Não incluímos "Id" aqui porque ele é gerado pelo servidor.
/// </summary>
public class CriarPessoaDto
{
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
}

/// <summary>
/// Dados devolvidos ao cliente ao listar/criar uma pessoa.
/// Separar do model evita, por exemplo, referências circulares
/// (Pessoa -> Transacoes -> Pessoa -> ...) na serialização JSON.
/// </summary>
public class PessoaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
    public bool EhMenorDeIdade { get; set; }
}
