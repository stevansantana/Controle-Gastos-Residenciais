using Microsoft.EntityFrameworkCore;
using ControleGastos.Api.Data;
using ControleGastos.Api.Dtos;
using ControleGastos.Api.Models;

namespace ControleGastos.Api.Services;

/// <summary>
/// Regras de negócio relacionadas ao cadastro de Pessoas.
/// </summary>
public class PessoaService
{
    private readonly AppDbContext _context;

    public PessoaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PessoaDto>> ListarAsync()
    {
        return await _context.Pessoas
            .OrderBy(p => p.Nome)
            .Select(p => new PessoaDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Idade = p.Idade,
                EhMenorDeIdade = p.EhMenorDeIdade
            })
            .ToListAsync();
    }

    public async Task<PessoaDto> CriarAsync(CriarPessoaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("O nome da pessoa é obrigatório.");

        if (dto.Idade < 0)
            throw new ArgumentException("A idade não pode ser negativa.");

        var pessoa = new Pessoa
        {
            Nome = dto.Nome.Trim(),
            Idade = dto.Idade
        };

        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        return new PessoaDto
        {
            Id = pessoa.Id,
            Nome = pessoa.Nome,
            Idade = pessoa.Idade,
            EhMenorDeIdade = pessoa.EhMenorDeIdade
        };
    }

    /// <summary>
    /// Remove uma pessoa. Graças ao "OnDelete(DeleteBehavior.Cascade)"
    /// configurado no AppDbContext, o próprio banco de dados já apaga
    /// automaticamente todas as transações associadas a essa pessoa.
    /// </summary>
    /// <returns>false se a pessoa não existir.</returns>
    public async Task<bool> DeletarAsync(Guid id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);
        if (pessoa is null) return false;

        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();
        return true;
    }
}
