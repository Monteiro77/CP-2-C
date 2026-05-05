using System.ComponentModel.DataAnnotations;

namespace BancoDigital.Api.DTOs;

public record CriarPessoaFisicaRequest(
    [Required, MaxLength(200)] string Nome,
    [Required, EmailAddress, MaxLength(200)] string Email,
    [MaxLength(20)] string Telefone,
    [Required] long AgenciaId,
    [Required, StringLength(11, MinimumLength = 11)] string Cpf,
    [Required] DateTime DataNascimento
);
