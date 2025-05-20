# Feedback - Avaliação Geral

## Front End

### Navegação
  * Pontos positivos:
    - Views e controllers presentes para `Produto`, `Categoria`, `Vendedor` com rotas definidas.
    - Controllers MVC seguem a convenção de nomeação e navegação típica do ASP.NET MVC.

### Design
  - Atende aos propósitos

### Funcionalidade
  * Pontos positivos:
    - Funcionalidades básicas de cadastro e autenticação via Identity presentes.
    - Cadastro e associação de Produto a Vendedor implementados com controle de acesso.
    - Endpoints públicos e protegidos por JWT presentes conforme requisitos.
  
## Back End

### Arquitetura
  * Pontos positivos:
    - Projeto utiliza estrutura modularizada com separation of concerns clara.
    - Injeção de dependência e organização em `Configuration` bem definida.

  * Pontos negativos:
    - **Violação direta dos requisitos do projeto** ao incluir múltiplas camadas desnecessárias:
        - Camada `MLV.Business/Services`: separação exagerada para lógica mínima.
        - Camada `MLV.Core/Mediator`: uso de abstração `Mediator` que complica um CRUD simples.
    - Para um projeto de CRUD básico, o enunciado exige apenas 3 camadas (MVC, API e uma Core unificada). As camadas adicionais só aumentam a complexidade sem necessidade.

    **Recomendação:** Deixar o arsenal técnico avançado para projetos que realmente exijam essa complexidade. Manter simples é um mérito técnico.

### Funcionalidade
  * Pontos positivos:
    - CRUD completo de Produtos e Categorias implementado com validações.
    - Endpoints públicos e privados separados conforme necessidade do projeto.
    - Criação do `Vendedor` no momento do registro do usuário implementada no `AuthController.cs`, linhas 82–90.
    - Uso de SQLite no modo Development encontrado nos arquivos de configuração.

### Modelagem
  * Pontos positivos:
    - Entidades simples: `Produto`, `Categoria`, `Vendedor`.
    - Modelos estão anêmicos, com propriedades e relacionamentos bem definidos.
    - Uso direto do EF Core com contexto padrão.  
  
## Projeto

### Organização
  * Pontos positivos:
    - Uso da pasta `src/` na raiz.
    - Projeto `.sln` presente dentro da pasta.
    - Separação clara por responsabilidade dentro de `src`.

  * Pontos negativos:
    - Alguns arquivos `.db`, `.db-shm`, `.db-wal` na pasta `src` poderiam ser ignorados no `.gitignore`.
  
### Documentação
  * Pontos positivos:
    - Arquivo `README.md` presente com informações básicas.
    - Swagger implementado com autenticação JWT.
    - Arquivo `FEEDBACK.md` presente.

### Instalação
  * Pontos positivos:
    - Uso de SQLite como banco configurado por `appsettings.Development.json`.

---

# 📊 Matriz de Avaliação com Notas de 0 a 10

| Critério                         | Peso | Nota (0-10)  | Justificativa                                                                 |
|----------------------------------|------|-------------|------------------------------------------------------------------------------|
| **Funcionalidade**              | 30%  | 8            | Back-end bem entregue com exageros de implementacao                                    |
| **Qualidade do Código**         | 20%  | 9            | Organização limpa e padrão consistente.                                      |
| **Eficiência e Desempenho**     | 20%  | 5           | Arquitetura excessivamente fragmentada para um CRUD simples.                 |
| **Inovação e Diferenciais**     | 10%  | 8           | Cumpre bem o esperado, porém peca na falta de simplicidade.                                                       |
| **Documentação e Organização**  | 10%  | 10          | Estrutura sólida.                                     |
| **Resolução de Feedbacks**      | 10%  | 8           | Nem todas as observações foram atendidas, a arquitetura continuou complexa     |


---

🎯 **Nota Final: 7.8 / 10**
