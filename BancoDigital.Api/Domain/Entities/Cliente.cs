namespace BancoDigital.Api.Domain.Entities;

public abstract class Cliente
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public long AgenciaId { get; set; }
    public Agencia Agencia { get; set; } = null!;
    public ICollection<Contratacao> Contratacoes { get; set; } = new List<Contratacao>();
}
