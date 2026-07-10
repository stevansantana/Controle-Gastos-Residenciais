import { useState } from "react";
import { PessoasPanel } from "./components/PessoasPanel";
import { TransacoesPanel } from "./components/TransacoesPanel";
import { TotaisPanel } from "./components/TotaisPanel";
import "./App.css";

type Aba = "pessoas" | "transacoes" | "totais";

function App() {
  const [aba, setAba] = useState<Aba>("pessoas");

  // Contador incrementado sempre que pessoas ou transações mudam.
  // Painéis que dependem desses dados (ex.: Totais) o usam como
  // dependência de efeito para saber quando refazer o cálculo.
  const [versao, setVersao] = useState(0);
  const bump = () => setVersao((v) => v + 1);

  return (
    <div className="app">
      <header className="cabecalho">
        <div className="cabecalho-marca">
          <span className="cabecalho-selo">$</span>
          <div>
            <h1>Livro de Contas do Domicílio</h1>
            <p>Controle de gastos residenciais</p>
          </div>
        </div>

        <nav className="abas">
          <button
            className={aba === "pessoas" ? "aba aba--ativa" : "aba"}
            onClick={() => setAba("pessoas")}
          >
            Pessoas
          </button>
          <button
            className={aba === "transacoes" ? "aba aba--ativa" : "aba"}
            onClick={() => setAba("transacoes")}
          >
            Transações
          </button>
          <button
            className={aba === "totais" ? "aba aba--ativa" : "aba"}
            onClick={() => setAba("totais")}
          >
            Totais
          </button>
        </nav>
      </header>

      <main className="conteudo">
        {aba === "pessoas" && <PessoasPanel onChange={bump} />}
        {aba === "transacoes" && (
          <TransacoesPanel versaoPessoas={versao} onChange={bump} />
        )}
        {aba === "totais" && <TotaisPanel refreshKey={versao} />}
      </main>
    </div>
  );
}

export default App;
