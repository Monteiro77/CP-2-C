using BancoDigital.Api.Domain.Enums;

namespace BancoDigital.Api.Domain.Entities;

public class Contratacao
{
    public long Id { get; set; }
    public long ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public long ProdutoId { get; set; }
    public Produto Produto { get; set; } = null!;
    public StatusContratacao Status { get; set; }
    public decimal ValorSolicitado { get; set; }
    public DateTime DataSolicitacao { get; set; }
    public DateTime? DataProcessamento { get; set; }
    public string? ObservacaoProcessamento { get; set; }
    public int ScoreCredito { get; set; }
}
