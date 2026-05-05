using BancoDigital.Tests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace BancoDigital.Tests;

public class ContratacoesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ContratacoesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<long> CriarAgenciaAsync(string codigo)
    {
        var response = await _client.PostAsJsonAsync("/api/agencias", new
        {
            Nome = "Agência Teste",
            Codigo = codigo,
            Endereco = "Rua Teste, 1"
        });
        var body = await response.Content.ReadFromJsonAsync<JsonNode>();
        return body!["id"]!.GetValue<long>();
    }

    private async Task<long> CriarClientePFAsync(long agenciaId, string cpf)
    {
        var response = await _client.PostAsJsonAsync("/api/clientes/pf", new
        {
            Nome = "Cliente Teste",
            Email = $"{cpf}@email.com",
            Telefone = "11999999999",
            AgenciaId = agenciaId,
            Cpf = cpf,
            DataNascimento = new DateTime(1990, 1, 1)
        });
        var body = await response.Content.ReadFromJsonAsync<JsonNode>();
        return body!["id"]!.GetValue<long>();
    }

    [Fact]
    public async Task SolicitarContratacao_Valida_DeveRetornarAccepted()
    {
        var agenciaId = await CriarAgenciaAsync("2001");
        var clienteId = await CriarClientePFAsync(agenciaId, "10020030041");

        var response = await _client.PostAsJsonAsync("/api/contratacoes", new
        {
            ClienteId = clienteId,
            ProdutoId = 1L,
            ValorSolicitado = 5000m
        });

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var body = await response.Content.ReadFromJsonAsync<JsonNode>();
        body!["status"]!.GetValue<string>().Should().Be("Pendente");
    }

    [Fact]
    public async Task SolicitarContratacao_ClienteInexistente_DeveRetornarNotFound()
    {
        var response = await _client.PostAsJsonAsync("/api/contratacoes", new
        {
            ClienteId = 999999L,
            ProdutoId = 1L,
            ValorSolicitado = 5000m
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SolicitarContratacao_ProdutoInexistente_DeveRetornarNotFound()
    {
        var agenciaId = await CriarAgenciaAsync("3001");
        var clienteId = await CriarClientePFAsync(agenciaId, "20030040052");

        var response = await _client.PostAsJsonAsync("/api/contratacoes", new
        {
            ClienteId = clienteId,
            ProdutoId = 999999L,
            ValorSolicitado = 5000m
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ConsultarStatus_ContratacaoExistente_DeveRetornarOk()
    {
        var agenciaId = await CriarAgenciaAsync("4001");
        var clienteId = await CriarClientePFAsync(agenciaId, "30040050063");

        var criarResponse = await _client.PostAsJsonAsync("/api/contratacoes", new
        {
            ClienteId = clienteId,
            ProdutoId = 1L,
            ValorSolicitado = 10000m
        });
        var criada = await criarResponse.Content.ReadFromJsonAsync<JsonNode>();
        var contratacaoId = criada!["id"]!.GetValue<long>();

        var response = await _client.GetAsync($"/api/contratacoes/{contratacaoId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonNode>();
        body!["id"]!.GetValue<long>().Should().Be(contratacaoId);
        body!["valorSolicitado"]!.GetValue<decimal>().Should().Be(10000m);
    }

    [Fact]
    public async Task ConsultarStatus_ContratacaoInexistente_DeveRetornarNotFound()
    {
        var response = await _client.GetAsync("/api/contratacoes/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
