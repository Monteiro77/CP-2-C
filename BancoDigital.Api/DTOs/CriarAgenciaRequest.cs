using System.ComponentModel.DataAnnotations;

namespace BancoDigital.Api.DTOs;

public record CriarAgenciaRequest(
    [Required, MaxLength(200)] string Nome,
    [Required, StringLength(6, MinimumLength = 4)] string Codigo,
    [MaxLength(500)] string Endereco
);
