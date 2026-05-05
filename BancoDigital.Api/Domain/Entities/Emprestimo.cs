namespace BancoDigital.Api.Domain.Entities;

public class Emprestimo : Produto
{
    public decimal ValorMinimo { get; set; }
    public decimal ValorMaximo { get; set; }
    public decimal TaxaJurosMensal { get; set; }
}
