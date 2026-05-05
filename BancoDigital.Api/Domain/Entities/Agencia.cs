namespace BancoDigital.Api.Domain.Entities;

public class Agencia
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
