using Microsoft.EntityFrameworkCore;
using ControleGastos.Api.Models;

namespace ControleGastos.Api.Data;

/// <summary>
/// Contexto do banco de dados. Cada DbSet<T> vira uma tabela.
/// O EF Core traduz nossas operações em C# (LINQ) para comandos SQL.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configura o relacionamento 1:N entre Pessoa e Transacao.
        // OnDelete(DeleteBehavior.Cascade) implementa exatamente a regra pedida:
        // "ao deletar uma pessoa, todas as suas transações devem ser apagadas".
        modelBuilder.Entity<Transacao>()
            .HasOne(t => t.Pessoa)
            .WithMany(p => p.Transacoes)
            .HasForeignKey(t => t.PessoaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Garante precisão/escala consistente para valores monetários no SQLite.
        modelBuilder.Entity<Transacao>()
            .Property(t => t.Valor)
            .HasColumnType("decimal(18,2)");

        base.OnModelCreating(modelBuilder);
    }
}
