# Banco Digital — API

## 1. Identificação

| Nome | RM |
|------|----|
| Vinícius Monteiro Araújo | RM555088 |
| Rafael Gaspar | RM557228 |


## 2. Produto bancário escolhido

**Empréstimo Pessoal**

O produto Empréstimo foi escolhido por ser o que melhor demonstra análise de crédito assíncrona — o ponto central da atividade. O consumer calcula um score (300–900) baseado no perfil do cliente e aprova ou reprova automaticamente, aplicando taxa de juros inversamente proporcional ao score.

Regras de negócio implementadas:
- Score < 600 → Reprovado
- Score 600–699 → Aprovado com taxa de 2,99% ao mês
- Score 700–799 → Aprovado com taxa de 2,49% ao mês
- Score ≥ 800 → Aprovado com taxa de 1,99% ao mês
- Valor fora do intervalo do produto (R$ 1.000 – R$ 50.000) → Reprovado independente do score

## 3. Decisão de modelagem de filas

**1 fila única: `contratacoes`**

Justificativa: como a atividade é individual e implementa 1 produto, uma única fila direct é suficiente e mais simples de operar. Quando o POST `/api/contratacoes` é chamado, o ID da contratação é publicado na fila. O consumer (BackgroundService) consome mensagens, processa a análise de crédito e atualiza o status no banco.

Exchange utilizado: default (direct, sem exchange nomeado), routing key = nome da fila.

## 4. Diagrama de classes

> Adicione a imagem `docs/diagrama-classes.png` após gerar no draw.io.

![Diagrama de Classes](docs/diagrama-classes.png)

## 5. Como rodar localmente

### Pré-requisitos
- .NET 8.0 SDK
- Docker (para RabbitMQ)
- Acesso à rede da FIAP (VPN ou presença física) para Oracle

### RabbitMQ via Docker
```bash
docker run -d --hostname rabbitmq --name rabbitmq \
  -p 5672:5672 -p 15672:15672 \
  rabbitmq:3-management
```
Painel de administração: http://localhost:15672 (guest/guest)

### Banco de dados
```bash
cd BancoDigital.Api
dotnet ef database update
```

### Executar a API
```bash
cd BancoDigital.Api
dotnet run
```
Swagger disponível em: https://localhost:{porta}/swagger

## 6. Endpoints disponíveis

### POST /api/agencias
```json
Request:
{
  "nome": "Agência Centro",
  "codigo": "0001",
  "endereco": "Av. Paulista, 1000 - São Paulo/SP"
}

Response 201:
{
  "id": 1,
  "nome": "Agência Centro",
  "codigo": "0001",
  "endereco": "Av. Paulista, 1000 - São Paulo/SP"
}
```

### GET /api/agencias/{id}
```json
Response 200:
{
  "id": 1,
  "nome": "Agência Centro",
  "codigo": "0001",
  "endereco": "Av. Paulista, 1000 - São Paulo/SP"
}
```

### POST /api/clientes/pf
```json
Request:
{
  "nome": "João da Silva",
  "email": "joao@email.com",
  "telefone": "11999999999",
  "agenciaId": 1,
  "cpf": "12345678901",
  "dataNascimento": "1990-05-15"
}

Response 201:
{
  "id": 1,
  "nome": "João da Silva",
  "email": "joao@email.com",
  "telefone": "11999999999",
  "agenciaId": 1,
  "tipoCliente": "PF",
  "cpf": "12345678901",
  "dataNascimento": "1990-05-15T00:00:00",
  "cnpj": null,
  "razaoSocial": null
}
```

### POST /api/clientes/pj
```json
Request:
{
  "nome": "Empresa LTDA",
  "email": "contato@empresa.com",
  "telefone": "1133334444",
  "agenciaId": 1,
  "cnpj": "12345678000195",
  "razaoSocial": "Empresa Teste LTDA"
}

Response 201:
{
  "id": 2,
  "nome": "Empresa LTDA",
  "email": "contato@empresa.com",
  "telefone": "1133334444",
  "agenciaId": 1,
  "tipoCliente": "PJ",
  "cpf": null,
  "dataNascimento": null,
  "cnpj": "12345678000195",
  "razaoSocial": "Empresa Teste LTDA"
}
```

### GET /api/clientes/{id}
```json
Response 200:
{
  "id": 1,
  "nome": "João da Silva",
  "email": "joao@email.com",
  "telefone": "11999999999",
  "agenciaId": 1,
  "tipoCliente": "PF",
  "cpf": "12345678901",
  "dataNascimento": "1990-05-15T00:00:00",
  "cnpj": null,
  "razaoSocial": null
}
```

### POST /api/contratacoes
```json
Request:
{
  "clienteId": 1,
  "produtoId": 1,
  "valorSolicitado": 15000.00
}

Response 202:
{
  "id": 1,
  "status": "Pendente",
  "mensagem": "Contratação recebida e enviada para análise."
}
```

### GET /api/contratacoes/{id}
```json
Response 200 (após processamento):
{
  "id": 1,
  "clienteId": 1,
  "nomeCliente": "João da Silva",
  "produtoId": 1,
  "nomeProduto": "Empréstimo Pessoal",
  "status": "Aprovada",
  "valorSolicitado": 15000.00,
  "dataSolicitacao": "2026-05-05T10:00:00Z",
  "dataProcessamento": "2026-05-05T10:00:02Z",
  "observacaoProcessamento": "Aprovado. Score: 720. Taxa mensal aplicada: 2,49%.",
  "scoreCredito": 720
}
```

## 7. Como executar os testes

```bash
cd BancoDigital.Tests
dotnet test --logger "console;verbosity=normal"
```

> Adicione aqui o print do resultado dos testes.

## 8. Print do painel RabbitMQ

> Adicione aqui o print do painel em http://localhost:15672 mostrando a fila `contratacoes` com mensagens processadas.

## 9. Print da API no Swagger

> Adicione aqui o print do Swagger com pelo menos uma contratação aprovada.
