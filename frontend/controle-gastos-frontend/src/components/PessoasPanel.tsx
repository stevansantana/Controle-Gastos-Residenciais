import { useEffect, useState, type FormEvent } from "react";
import type { Pessoa } from "../types";
import { pessoasApi } from "../services/api";

interface Props {
  /** Avisa o componente pai que a lista de pessoas mudou (criar/deletar),
   *  para que outras telas (Transações, Totais) possam se atualizar. */
  onChange: () => void;
}

export function PessoasPanel({ onChange }: Props) {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [nome, setNome] = useState("");
  const [idade, setIdade] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);

  async function carregar() {
    setPessoas(await pessoasApi.listar());
  }

  useEffect(() => {
    carregar();
  }, []);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErro(null);

    if (!nome.trim() || idade === "") {
      setErro("Preencha nome e idade.");
      return;
    }

    setCarregando(true);
    try {
      await pessoasApi.criar(nome.trim(), Number(idade));
      setNome("");
      setIdade("");
      await carregar();
      onChange();
    } catch (err) {
      setErro(err instanceof Error ? err.message : "Erro ao criar pessoa.");
    } finally {
      setCarregando(false);
    }
  }

  async function handleDeletar(id: string, nomePessoa: string) {
    const confirmar = window.confirm(
      `Remover "${nomePessoa}"? Todas as transações dessa pessoa também serão apagadas.`,
    );
    if (!confirmar) return;

    try {
      await pessoasApi.deletar(id);
      await carregar();
      onChange();
    } catch (err) {
      setErro(err instanceof Error ? err.message : "Erro ao remover pessoa.");
    }
  }

  return (
    <section className="painel">
      <h2 className="painel-titulo">Pessoas do domicílio</h2>

      <form className="formulario" onSubmit={handleSubmit}>
        <div className="campo">
          <label htmlFor="nome">Nome</label>
          <input
            id="nome"
            type="text"
            value={nome}
            onChange={(e) => setNome(e.target.value)}
            placeholder="Ex.: Maria Silva"
          />
        </div>

        <div className="campo campo--curto">
          <label htmlFor="idade">Idade</label>
          <input
            id="idade"
            type="number"
            min={0}
            value={idade}
            onChange={(e) => setIdade(e.target.value)}
            placeholder="Ex.: 34"
          />
        </div>

        <button
          type="submit"
          className="botao botao--primario"
          disabled={carregando}
        >
          {carregando ? "Salvando..." : "Adicionar pessoa"}
        </button>
      </form>

      {erro && <p className="mensagem-erro">{erro}</p>}

      <table className="tabela">
        <thead>
          <tr>
            <th>Nome</th>
            <th>Idade</th>
            <th>Situação</th>
            <th aria-label="ações"></th>
          </tr>
        </thead>
        <tbody>
          {pessoas.length === 0 && (
            <tr>
              <td colSpan={4} className="tabela-vazia">
                Nenhuma pessoa cadastrada ainda.
              </td>
            </tr>
          )}
          {pessoas.map((p) => (
            <tr key={p.id}>
              <td>{p.nome}</td>
              <td>{p.idade} anos</td>
              <td>
                {p.ehMenorDeIdade ? (
                  <span className="etiqueta etiqueta--atencao">
                    Menor de idade
                  </span>
                ) : (
                  <span className="etiqueta">Maior de idade</span>
                )}
              </td>
              <td>
                <button
                  className="botao botao--perigo botao--pequeno"
                  onClick={() => handleDeletar(p.id, p.nome)}
                >
                  Remover
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
