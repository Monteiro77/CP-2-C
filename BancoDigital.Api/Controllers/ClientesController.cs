using BancoDigital.Api.Data;
using BancoDigital.Api.Domain.Entities;
using BancoDigital.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly BancoContext _db;

    public ClientesController(BancoContext db) => _db = db;

    [HttpPost("pf")]
    public async Task<IActionResult> CadastrarPF([FromBody] CriarPessoaFisicaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var agencia = await _db.Agencias.FindAsync(request.AgenciaId);
        if (agencia is null) return NotFound(new { Mensagem = "Agência não encontrada." });

        var cpfExiste = await _db.PessoasFisicas.CountAsync(p => p.Cpf == request.Cpf) > 0;
        if (cpfExiste) return Conflict(new { Mensagem = "CPF já cadastrado." });

        var pf = new PessoaFisica
        {
            Nome = request.Nome,
            Email = request.Email,
            Telefone = request.Telefone,
            AgenciaId = request.AgenciaId,
            Cpf = request.Cpf,
            DataNascimento = request.DataNascimento
        };

        _db.PessoasFisicas.Add(pf);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = pf.Id }, ToResponse(pf));
    }

    [HttpPost("pj")]
    public async Task<IActionResult> CadastrarPJ([FromBody] CriarPessoaJuridicaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var agencia = await _db.Agencias.FindAsync(request.AgenciaId);
        if (agencia is null) return NotFound(new { Mensagem = "Agência não encontrada." });

        var cnpjExiste = await _db.PessoasJuridicas.CountAsync(p => p.Cnpj == request.Cnpj) > 0;
        if (cnpjExiste) return Conflict(new { Mensagem = "CNPJ já cadastrado." });

        var pj = new PessoaJuridica
        {
            Nome = request.Nome,
            Email = request.Email,
            Telefone = request.Telefone,
            AgenciaId = request.AgenciaId,
            Cnpj = request.Cnpj,
            RazaoSocial = request.RazaoSocial
        };

        _db.PessoasJuridicas.Add(pj);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = pj.Id }, ToResponse(pj));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> BuscarPorId(long id)
    {
        var cliente = await _db.Clientes.FindAsync(id);
        if (cliente is null) return NotFound(new { Mensagem = "Cliente não encontrado." });
        return Ok(ToResponse(cliente));
    }

    private static ClienteResponse ToResponse(Cliente c) => c switch
    {
        PessoaFisica pf => new ClienteResponse(pf.Id, pf.Nome, pf.Email, pf.Telefone, pf.AgenciaId,
            "PF", pf.Cpf, pf.DataNascimento, null, null),
        PessoaJuridica pj => new ClienteResponse(pj.Id, pj.Nome, pj.Email, pj.Telefone, pj.AgenciaId,
            "PJ", null, null, pj.Cnpj, pj.RazaoSocial),
        _ => throw new InvalidOperationException("Tipo de cliente desconhecido.")
    };
}
