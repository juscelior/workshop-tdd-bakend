# Projeto

Projeto base para a construção do desafio do workshop de TDD para desenvolvedor backend
 
## Sumário

* [Desenvolvimento, por onde começar](#desenvolvimento-por-onde-começar)
* [Dependências](#dependências)
* [Bases de dados](#bases-de-dados)
* [Builds e testes](#builds-e-testes)
* [CI/CD](#ci/cd)
* [Deploy](#deploy)
* [Contribuição](#contribuição)
 
## Desenvolvimento, por onde começar

Passos para execução do projeto:

1. Executar projeto Wiz.Biblioteca.API definido como projeto inicial.
2. Utilizar documentação que esta na pasta [docs](doc/docs/Desafio.md)
  + Hml: Será criado um ambiente para o seu projeto em especifico.
3. Executar testes via dotnet test
 
## Dependências

* Projetos dependentes
  + SSO
  + [Open Library](doc/docs/Open-Library-Books-API.md)
  + Via CEP
 
## Bases de dados

* Servidores: 
  + Base local
 
## Build e Testes

 * Comandos para geração de testes:
  + Debug: Executar via Test Explorer (adicionar breakpoint)
  + Release: Executar via Test Explorer (não adicionar breakpoint)
* Comandos para geração de relatório de testes:

  + **PowerShell (Windows):**

  1. Executar comando: 
  ```sh
  Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope LocalMachine
  ```
    2. Executar testes e relatório de testes:
  ```sh
  .\code_coverage.ps1
  ```
  + **Shell (Linux/Mac):**
  1. Executar testes e relatório de testes:
  ```sh
  ./code_coverage.sh
  ```
 O relatório dos testes são gerados na pasta: **code_coverage**

## CI/CD
* Seguir o processo apresentado no treinamento de DevOps para desenvolvedores.
 
## Deploy 
* Temos como objetivo que cada desenvolvedor consiga executar a configuração do CI/CD para nosso ambiente de homologação do AKS.