import { useEffect, useState, type FormEvent } from "react";
import type { Pessoa, Transacao, TipoTransacao } from "../types";
import { pessoasApi, transacoesApi } from "../services/api";

interface Props {
  /** Muda toda vez que uma pessoa é criada/removida em outra tela,
   *  para mantermos a lista de pessoas do formulário atualizada. */
  versaoPessoas: number;
  /** Avisa o componente pai que uma transação foi criada (afeta os Totais). */
  onChange: () => void;
}

export function TransacoesPanel({ versaoPessoas, onChange }: Props) {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);

  const [descricao, setDescricao] = useState("");
  const [valor, setValor] = useState("");
  const [tipo, setTipo] = useState<TipoTransacao>("Despesa");
  const [pessoaId, setPessoaId] = useState("");

  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);

  useEffect(() => {
    pessoasApi.listar().then(setPessoas);
  }, [versaoPessoas]);

  useEffect(() => {
    transacoesApi.listar().then(setTransacoes);
  }, []);

  const pessoaSelecionada = pessoas.find((p) => p.id === pessoaId);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);

    if (!descricao.trim() || !valor || !pessoaId) {
      setErro("Preencha descrição, valor e selecione uma pessoa.");
      return;
    }

    setCarregando(true);
    try {
      await transacoesApi.criar(
        descricao.trim(),
        Number(valor),
        tipo,
        pessoaId,
      );
      setDescricao("");
      setValor("");
      setTransacoes(await transacoesApi.listar());
      onChange();
    } catch (err) {
      // Aqui chega, por exemplo, a mensagem de negócio:
      // "Fulano é menor de idade (15 anos) e só pode registrar despesas..."
      setErro(err instanceof Error ? err.message : "Erro ao criar transação.");
    } finally {
      setCarregando(false);
    }
  }

  return (
    <section className="painel">
      <h2 className="painel-titulo">Transações</h2>

      <form className="formulario" onSubmit={handleSubmit}>
        <div className="campo">
          <label htmlFor="pessoa">Pessoa</label>
          <select
            id="pessoa"
            value={pessoaId}
            onChange={(e) => setPessoaId(e.target.value)}
          >
            <option value="">Selecione...</option>
            {pessoas.map((p) => (
              <option key={p.id} value={p.id}>
                {p.nome} {p.ehMenorDeIdade ? "(menor de idade)" : ""}
              </option>
            ))}
          </select>
        </div>

        <div className="campo">
          <label htmlFor="descricao">Descrição</label>
          <input
            id="descricao"
            type="text"
            value={descricao}
            onChange={(e) => setDescricao(e.target.value)}
            placeholder="Ex.: Supermercado"
          />
        </div>

        <div className="campo campo--curto">
          <label htmlFor="valor">Valor (R$)</label>
          <input
            id="valor"
            type="number"
            min={0}
            step="0.01"
            value={valor}
            onChange={(e) => setValor(e.target.value)}
            placeholder="0,00"
          />
        </div>

        <div className="campo campo--curto">
          <label htmlFor="tipo">Tipo</label>
          <select
            id="tipo"
            value={tipo}
            onChange={(e) => setTipo(e.target.value as TipoTransacao)}
            // Trava visualmente a opção "Receita" quando a pessoa é menor de idade,
            // reforçando na interface a regra que também é validada no back-end.
            disabled={pessoaSelecionada?.ehMenorDeIdade && tipo === "Receita"}
          >
            <option value="Despesa">Despesa</option>
            <option
              value="Receita"
              disabled={pessoaSelecionada?.ehMenorDeIdade}
            >
              Receita
            </option>
          </select>
        </div>

        <button
          type="submit"
          className="botao botao--primario"
          disabled={carregando}
        >
          {carregando ? "Salvando..." : "Adicionar transação"}
        </button>
      </form>

      {pessoaSelecionada?.ehMenorDeIdade && (
        <p className="mensagem-aviso">
          {pessoaSelecionada.nome} é menor de idade: apenas despesas podem ser
          cadastradas.
        </p>
      )}

      {erro && <p className="mensagem-erro">{erro}</p>}

      <table className="tabela">
        <thead>
          <tr>
            <th>Pessoa</th>
            <th>Descrição</th>
            <th>Tipo</th>
            <th>Valor</th>
          </tr>
        </thead>
        <tbody>
          {transacoes.length === 0 && (
            <tr>
              <td colSpan={4} className="tabela-vazia">
                Nenhuma transação cadastrada ainda.
              </td>
            </tr>
          )}
          {transacoes.map((t) => (
            <tr key={t.id}>
              <td>{t.pessoaNome}</td>
              <td>{t.descricao}</td>
              <td>
                <span
                  className={
                    t.tipo === "Receita"
                      ? "etiqueta etiqueta--receita"
                      : "etiqueta etiqueta--despesa"
                  }
                >
                  {t.tipo}
                </span>
              </td>
              <td className="valor-monetario">
                {t.valor.toLocaleString("pt-BR", {
                  style: "currency",
                  currency: "BRL",
                })}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
