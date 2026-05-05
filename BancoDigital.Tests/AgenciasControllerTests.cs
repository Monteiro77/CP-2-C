using BancoDigital.Tests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace BancoDigital.Tests;

public class AgenciasControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AgenciasControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CadastrarAgencia_DeveCriarComSucesso()
    {
        var response = await _client.PostAsJsonAsync("/api/agencias", new
        {
            Nome = "Agência São Paulo",
            Codigo = "0042",
            Endereco = "Av. Paulista, 1000"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<JsonNode>();
        body!["nome"]!.GetValue<string>().Should().Be("Agência São Paulo");
        body!["codigo"]!.GetValue<string>().Should().Be("0042");
    }

    [Fact]
    public async Task BuscarAgencia_ExistenteDeveRetornarOk()
    {
        var criarResponse = await _client.PostAsJsonAsync("/api/agencias", new
        {
            Nome = "Agência Rio",
            Codigo = "0099",
            Endereco = "Rua do Ouvidor, 50"
        });
        var criada = await criarResponse.Content.ReadFromJsonAsync<JsonNode>();
        var agenciaId = criada!["id"]!.GetValue<long>();

        var response = await _client.GetAsync($"/api/agencias/{agenciaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task BuscarAgencia_InexistenteDeveRetornarNotFound()
    {
        var response = await _client.GetAsync("/api/agencias/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
