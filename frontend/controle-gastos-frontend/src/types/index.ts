// Tipos que espelham os DTOs do back-end (C#), garantindo que o
// front-end saiba exatamente o formato dos dados trocados com a API.

export interface Pessoa {
  id: string;
  nome: string;
  idade: number;
  ehMenorDeIdade: boolean;
}

export type TipoTransacao = "Despesa" | "Receita";

export interface Transacao {
  id: string;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  pessoaId: string;
  pessoaNome: string;
}

export interface TotalPorPessoa {
  pessoaId: string;
  pessoaNome: string;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
}

export interface TotaisResponse {
  porPessoa: TotalPorPessoa[];
  totalGeralReceitas: number;
  totalGeralDespesas: number;
  saldoGeral: number;
}

/** Formato padrão de erro devolvido pela API (BadRequest / NotFound). */
export interface ApiErro {
  mensagem: string;
}
