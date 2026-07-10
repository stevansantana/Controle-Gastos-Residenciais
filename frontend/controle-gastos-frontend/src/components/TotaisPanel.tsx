import { useEffect, useState } from "react";
import type { TotaisResponse } from "../types";
import { totaisApi } from "../services/api";

interface Props {
  /** Muda sempre que uma pessoa ou transação é criada/removida,
   *  disparando um novo cálculo de totais. */
  refreshKey: number;
}

function formatarMoeda(valor: number) {
  return valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

export function TotaisPanel({ refreshKey }: Props) {
  const [totais, setTotais] = useState<TotaisResponse | null>(null);

  useEffect(() => {
    totaisApi.obter().then(setTotais);
  }, [refreshKey]);

  if (!totais) return <p className="painel">Carregando totais...</p>;

  return (
    <section className="painel">
      <h2 className="painel-titulo">Consulta de totais</h2>

      <table className="tabela">
        <thead>
          <tr>
            <th>Pessoa</th>
            <th>Receitas</th>
            <th>Despesas</th>
            <th>Saldo</th>
          </tr>
        </thead>
        <tbody>
          {totais.porPessoa.length === 0 && (
            <tr>
              <td colSpan={4} className="tabela-vazia">
                Nenhuma pessoa cadastrada ainda.
              </td>
            </tr>
          )}
          {totais.porPessoa.map((p) => (
            <tr key={p.pessoaId}>
              <td>{p.pessoaNome}</td>
              <td className="valor-monetario valor-monetario--receita">
                {formatarMoeda(p.totalReceitas)}
              </td>
              <td className="valor-monetario valor-monetario--despesa">
                {formatarMoeda(p.totalDespesas)}
              </td>
              <td
                className={`valor-monetario ${p.saldo >= 0 ? "valor-monetario--receita" : "valor-monetario--despesa"}`}
              >
                {formatarMoeda(p.saldo)}
              </td>
            </tr>
          ))}
        </tbody>
        <tfoot>
          <tr className="tabela-total">
            <td>Total geral</td>
            <td className="valor-monetario valor-monetario--receita">
              {formatarMoeda(totais.totalGeralReceitas)}
            </td>
            <td className="valor-monetario valor-monetario--despesa">
              {formatarMoeda(totais.totalGeralDespesas)}
            </td>
            <td
              className={`valor-monetario ${totais.saldoGeral >= 0 ? "valor-monetario--receita" : "valor-monetario--despesa"}`}
            >
              {formatarMoeda(totais.saldoGeral)}
            </td>
          </tr>
        </tfoot>
      </table>
    </section>
  );
}
