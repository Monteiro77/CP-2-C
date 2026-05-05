using System.ComponentModel.DataAnnotations;

namespace BancoDigital.Api.DTOs;

public record CriarContratacaoRequest(
    [Required] long ClienteId,
    [Required] long ProdutoId,
    [Required, Range(1, double.MaxValue)] decimal ValorSolicitado
);
