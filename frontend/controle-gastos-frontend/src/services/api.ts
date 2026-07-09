import type {
  Pessoa,
  Transacao,
  TotaisResponse,
  TipoTransacao,
} from "../types";

// URL base da API .NET. Ajuste aqui caso rode o backend em outra porta.
const BASE_URL = "http://localhost:5197/api";

/**
 * Função auxiliar central para chamadas HTTP.
 * Lança um Error com a mensagem vinda da API quando a resposta não é OK,
 * para que os componentes possam exibir a mensagem de erro de negócio
 * (ex.: "menor de idade só pode cadastrar despesas").
 */
async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  if (!response.ok) {
    let mensagem = `Erro ${response.status} ao chamar ${path}`;
    try {
      const corpo = await response.json();
      if (corpo?.mensagem) mensagem = corpo.mensagem;
    } catch {
      // resposta sem corpo JSON (ex.: 404 simples) - mantém mensagem padrão
    }
    throw new Error(mensagem);
  }

  // Respostas 204 (No Content) não têm corpo para converter em JSON.
  if (response.status === 204) return undefined as T;

  return response.json() as Promise<T>;
}

export const pessoasApi = {
  listar: () => request<Pessoa[]>("/pessoas"),

  criar: (nome: string, idade: number) =>
    request<Pessoa>("/pessoas", {
      method: "POST",
      body: JSON.stringify({ nome, idade }),
    }),

  deletar: (id: string) =>
    request<void>(`/pessoas/${id}`, { method: "DELETE" }),
};

export const transacoesApi = {
  listar: () => request<Transacao[]>("/transacoes"),

  criar: (
    descricao: string,
    valor: number,
    tipo: TipoTransacao,
    pessoaId: string,
  ) =>
    request<Transacao>("/transacoes", {
      method: "POST",
      body: JSON.stringify({ descricao, valor, tipo, pessoaId }),
    }),
};

export const totaisApi = {
  obter: () => request<TotaisResponse>("/totais"),
};
