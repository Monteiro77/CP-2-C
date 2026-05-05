using BancoDigital.Tests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace BancoDigital.Tests;

public class ClientesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ClientesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<long> CriarAgenciaAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/agencias", new
        {
            Nome = "Agência Central",
            Codigo = "0001",
            Endereco = "Rua Principal, 100"
        });
        var body = await response.Content.ReadFromJsonAsync<JsonNode>();
        return body!["id"]!.GetValue<long>();
    }

    [Fact]
    public async Task CadastrarPF_DeveCriarComSucesso()
    {
        var agenciaId = await CriarAgenciaAsync();

        var response = await _client.PostAsJsonAsync("/api/clientes/pf", new
        {
            Nome = "João Silva",
            Email = "joao@email.com",
            Telefone = "11999999999",
            AgenciaId = agenciaId,
            Cpf = "12345678901",
            DataNascimento = new DateTime(1990, 5, 15)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<JsonNode>();
        body!["tipoCliente"]!.GetValue<string>().Should().Be("PF");
    }

    [Fact]
    public async Task CadastrarPF_CpfDuplicado_DeveRetornarConflict()
    {
        var agenciaId = await CriarAgenciaAsync();
        var payload = new
        {
            Nome = "Maria Santos",
            Email = "maria@email.com",
            Telefone = "11988888888",
            AgenciaId = agenciaId,
            Cpf = "98765432100",
            DataNascimento = new DateTime(1985, 3, 20)
        };

        await _client.PostAsJsonAsync("/api/clientes/pf", payload);
        var response = await _client.PostAsJsonAsync("/api/clientes/pf", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CadastrarPJ_DeveCriarComSucesso()
    {
        var agenciaId = await CriarAgenciaAsync();

        var response = await _client.PostAsJsonAsync("/api/clientes/pj", new
        {
            Nome = "Empresa LTDA",
            Email = "empresa@email.com",
            Telefone = "1133334444",
            AgenciaId = agenciaId,
            Cnpj = "12345678000195",
            RazaoSocial = "Empresa Teste LTDA"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<JsonNode>();
        body!["tipoCliente"]!.GetValue<string>().Should().Be("PJ");
    }

    [Fact]
    public async Task CadastrarPJ_CnpjDuplicado_DeveRetornarConflict()
    {
        var agenciaId = await CriarAgenciaAsync();
        var payload = new
        {
            Nome = "Outra Empresa",
            Email = "outra@email.com",
            Telefone = "1155556666",
            AgenciaId = agenciaId,
            Cnpj = "98765432000181",
            RazaoSocial = "Outra Empresa LTDA"
        };

        await _client.PostAsJsonAsync("/api/clientes/pj", payload);
        var response = await _client.PostAsJsonAsync("/api/clientes/pj", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CadastrarPF_AgenciaInexistente_DeveRetornarNotFound()
    {
        var response = await _client.PostAsJsonAsync("/api/clientes/pf", new
        {
            Nome = "Carlos",
            Email = "carlos@email.com",
            Telefone = "11977777777",
            AgenciaId = 99999L,
            Cpf = "11122233344",
            DataNascimento = new DateTime(1992, 1, 1)
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task BuscarPorId_ClienteExistente_DeveRetornarOk()
    {
        var agenciaId = await CriarAgenciaAsync();

        var criarResponse = await _client.PostAsJsonAsync("/api/clientes/pf", new
        {
            Nome = "Pedro",
            Email = "pedro@email.com",
            Telefone = "11966666666",
            AgenciaId = agenciaId,
            Cpf = "55566677788",
            DataNascimento = new DateTime(1988, 7, 10)
        });
        var criado = await criarResponse.Content.ReadFromJsonAsync<JsonNode>();
        var clienteId = criado!["id"]!.GetValue<long>();

        var response = await _client.GetAsync($"/api/clientes/{clienteId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task BuscarPorId_ClienteInexistente_DeveRetornarNotFound()
    {
        var response = await _client.GetAsync("/api/clientes/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
