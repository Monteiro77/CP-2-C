namespace BancoDigital.Api.DTOs;

public record AgenciaResponse(
    long Id,
    string Nome,
    string Codigo,
    string Endereco
);
