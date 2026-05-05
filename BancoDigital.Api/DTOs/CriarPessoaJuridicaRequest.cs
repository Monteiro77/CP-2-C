using System.ComponentModel.DataAnnotations;

namespace BancoDigital.Api.DTOs;

public record CriarPessoaJuridicaRequest(
    [Required, MaxLength(200)] string Nome,
    [Required, EmailAddress, MaxLength(200)] string Email,
    [MaxLength(20)] string Telefone,
    [Required] long AgenciaId,
    [Required, StringLength(14, MinimumLength = 14)] string Cnpj,
    [Required, MaxLength(300)] string RazaoSocial
);
