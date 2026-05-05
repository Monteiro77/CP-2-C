using BancoDigital.Api.Data;
using BancoDigital.Api.Domain.Entities;
using BancoDigital.Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BancoDigital.Api.Controllers;

[ApiController]
[Route("api/agencias")]
public class AgenciasController : ControllerBase
{
    private readonly BancoContext _db;

    public AgenciasController(BancoContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CriarAgenciaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var agencia = new Agencia
        {
            Nome = request.Nome,
            Codigo = request.Codigo,
            Endereco = request.Endereco
        };

        _db.Agencias.Add(agencia);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = agencia.Id }, ToResponse(agencia));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> BuscarPorId(long id)
    {
        var agencia = await _db.Agencias.FindAsync(id);
        if (agencia is null) return NotFound(new { Mensagem = "Agência não encontrada." });
        return Ok(ToResponse(agencia));
    }

    private static AgenciaResponse ToResponse(Agencia a) =>
        new(a.Id, a.Nome, a.Codigo, a.Endereco);
}
