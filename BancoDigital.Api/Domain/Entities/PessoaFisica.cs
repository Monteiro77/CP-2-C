namespace BancoDigital.Api.Domain.Entities;

public class PessoaFisica : Cliente
{
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
}
