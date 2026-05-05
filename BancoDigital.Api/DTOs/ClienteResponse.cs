namespace BancoDigital.Api.DTOs;

public record ClienteResponse(
    long Id,
    string Nome,
    string Email,
    string Telefone,
    long AgenciaId,
    string TipoCliente,
    string? Cpf,
    DateTime? DataNascimento,
    string? Cnpj,
    string? RazaoSocial
);
