namespace BancoDigital.Api.DTOs;

public record ContratacaoResponse(
    long Id,
    long ClienteId,
    string NomeCliente,
    long ProdutoId,
    string NomeProduto,
    string Status,
    decimal ValorSolicitado,
    DateTime DataSolicitacao,
    DateTime? DataProcessamento,
    string? ObservacaoProcessamento,
    int ScoreCredito
);
