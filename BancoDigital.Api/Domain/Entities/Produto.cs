namespace BancoDigital.Api.Domain.Entities;

public abstract class Produto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public ICollection<Contratacao> Contratacoes { get; set; } = new List<Contratacao>();
}
