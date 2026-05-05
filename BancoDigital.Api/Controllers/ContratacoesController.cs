using BancoDigital.Api.Data;
using BancoDigital.Api.Domain.Entities;
using BancoDigital.Api.Domain.Enums;
using BancoDigital.Api.DTOs;
using BancoDigital.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.Api.Controllers;

[ApiController]
[Route("api/contratacoes")]
public class ContratacoesController : ControllerBase
{
    private readonly BancoContext _db;
    private readonly IRabbitMqPublisher _publisher;

    public ContratacoesController(BancoContext db, IRabbitMqPublisher publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    [HttpPost]
    public async Task<IActionResult> Solicitar([FromBody] CriarContratacaoRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var cliente = await _db.Clientes.FindAsync(request.ClienteId);
        if (cliente is null) return NotFound(new { Mensagem = "Cliente não encontrado." });

        var produto = await _db.Produtos.FindAsync(request.ProdutoId);
        if (produto is null) return NotFound(new { Mensagem = "Produto não encontrado." });

        var contratacao = new Contratacao
        {
            ClienteId = request.ClienteId,
            ProdutoId = request.ProdutoId,
            ValorSolicitado = request.ValorSolicitado,
            Status = StatusContratacao.Pendente,
            DataSolicitacao = DateTime.UtcNow
        };

        _db.Contratacoes.Add(contratacao);
        await _db.SaveChangesAsync();

        _publisher.Publish("contratacoes", contratacao.Id.ToString());

        return Accepted(new { contratacao.Id, Status = contratacao.Status.ToString(), Mensagem = "Contratação recebida e enviada para análise." });
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> ConsultarStatus(long id)
    {
        var contratacao = await _db.Contratacoes
            .Include(c => c.Cliente)
            .Include(c => c.Produto)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contratacao is null) return NotFound(new { Mensagem = "Contratação não encontrada." });

        return Ok(new ContratacaoResponse(
            contratacao.Id,
            contratacao.ClienteId,
            contratacao.Cliente.Nome,
            contratacao.ProdutoId,
            contratacao.Produto.Nome,
            contratacao.Status.ToString(),
            contratacao.ValorSolicitado,
            contratacao.DataSolicitacao,
            contratacao.DataProcessamento,
            contratacao.ObservacaoProcessamento,
            contratacao.ScoreCredito
        ));
    }
}
