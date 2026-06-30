# Desafio Técnico - Desenvolvedor(a) Backend .NET - FASE I

## 1. Apresentação

Obrigado por participar do nosso processo seletivo.

Neste desafio, você receberá uma API em .NET já funcional para cadastro de clientes e deverá evolui-la com a camada de banco de dados.

Prazo de entrega: **7 dias corridos** a partir do recebimento deste documento.

## 2. Objetivo do desafio

Implementar persistência em banco de dados para a API existente, mantendo as regras de negócio atuais e boas práticas de desenvolvimento. 

Pode escolher **um** dos bancos abaixo:
- SQL Server
- PostgreSQL
- MySQL

**Você é totalmente livre para fazer a modelagem do banco como preferir.

**Importante: NÃO UTILIZAR ORM OU DAPPER

## 3. Overview da API atual

A API consiste em cadastro de clientes e cálculo de um score de confiança, usado para classificá-los como bons, regulares ou maus clientes.

Atualmente a API permite:
- Cadastrar cliente
- Buscar cliente por CPF
- Listar todos os clientes
- Atualizar cliente por CPF

Os dados são persistidos em **SQL Server**, via ADO.NET puro (sem ORM ou Dapper).

### Entidade Cliente

Campos principais:

- `nome`
- `email`
- `dataNascimento`
- `telefone` (`ddd` e `numero`)
- `cpf`
- `endereco` (`logradouro`, `numero`, `complemento`, `cep`, `uf`)
- `rendaAnual`

### Regras de negócio

Campos obrigatórios:

- `endereco.uf`
- `telefone.ddd`
- `telefone.numero`
- `cpf`
- `rendaAnual`

Validações:

- CPF deve ser válido
- CPF deve ser único
- UF deve conter 2 caracteres
- Data de nascimento não pode ser futura
- Renda anual não pode ser negativa

### Regra de score

O score final e a soma de dois componentes: **renda anual + idade**.

Faixa de renda anual:

- acima de 120.000: `+300`
- entre 60.000 e 120.000: `+200`
- abaixo de 60.000: `+100`

Faixa de idade:

- acima de 40 anos: `+200`
- entre 25 e 40 anos: `+150`
- abaixo de 25 anos: `+50`

## 4. Endpoints disponíveis

- `POST /api/clientes` - cadastra cliente e retorna score
- `GET /api/clientes/{cpf}` - busca cliente por CPF com score
- `GET /api/clientes` - lista todos os clientes com score
- `PUT /api/clientes/{cpf}` - atualiza dados do cliente por CPF e retorna score recalculado

## 5. Como executar a API localmente

Pré-requisitos:

- .NET10 SDK instalado
- SQL Server LocalDB instalado (incluso na instalação padrão do Visual Studio com a carga de trabalho "Desenvolvimento para a Web")

Passos:

1. Entrar na pasta do projeto:

   `cd Teste.ScoreAPI`

2. Restaurar dependencias:

   `dotnet restore`

3. Ajustar a connection string em `appsettings.json` (`ConnectionStrings:DefaultConnection`) caso não use SQL Server LocalDB com a instância padrão.

4. Executar a aplicacao:

   `dotnet run`

   O banco de dados e a tabela `Customers` são criados automaticamente na primeira execução, caso ainda não existam.

5. Acessar documentação Swagger:

   `http://localhost:<porta>/swagger`

## 6. Entrega

Envie o material em ate **7 dias corridos** por e-mail.

### O que enviar

- Link do repositorio (GitHub/GitLab/Bitbucket) ou arquivo zipado com o código.

---

Boa sorte e bom desenvolvimento!