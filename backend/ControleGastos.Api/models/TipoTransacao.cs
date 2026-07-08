namespace ControleGastos.Api.Models;

/// <summary>
/// Tipo de uma transação financeira.
/// Usar um enum (em vez de string livre) evita erros de digitação
/// como "despesa" vs "Despesa" vs "expense" e facilita validação.
/// </summary>
public enum TipoTransacao
{
    Despesa = 0,
    Receita = 1
}
