using BancoDigital.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.Api.Data;

public class BancoContext : DbContext
{
    public BancoContext(DbContextOptions<BancoContext> options) : base(options) { }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<PessoaFisica> PessoasFisicas { get; set; }
    public DbSet<PessoaJuridica> PessoasJuridicas { get; set; }
    public DbSet<Agencia> Agencias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Emprestimo> Emprestimos { get; set; }
    public DbSet<Contratacao> Contratacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(e =>
        {
            e.ToTable("TB_CLIENTE");
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasColumnName("ID_CLIENTE").ValueGeneratedOnAdd();
            e.Property(c => c.Nome).HasColumnName("NM_NOME").HasMaxLength(200).IsRequired();
            e.Property(c => c.Email).HasColumnName("DS_EMAIL").HasMaxLength(200).IsRequired();
            e.Property(c => c.Telefone).HasColumnName("DS_TELEFONE").HasMaxLength(20);
            e.Property(c => c.AgenciaId).HasColumnName("ID_AGENCIA");
            e.HasDiscriminator<string>("TP_CLIENTE")
                .HasValue<PessoaFisica>("PF")
                .HasValue<PessoaJuridica>("PJ");
        });

        modelBuilder.Entity<PessoaFisica>(e =>
        {
            e.Property(p => p.Cpf).HasColumnName("NR_CPF").HasMaxLength(11);
            e.Property(p => p.DataNascimento).HasColumnName("DT_NASCIMENTO");
            e.HasIndex(p => p.Cpf).IsUnique();
        });

        modelBuilder.Entity<PessoaJuridica>(e =>
        {
            e.Property(p => p.Cnpj).HasColumnName("NR_CNPJ").HasMaxLength(14);
            e.Property(p => p.RazaoSocial).HasColumnName("DS_RAZAO_SOCIAL").HasMaxLength(300);
            e.HasIndex(p => p.Cnpj).IsUnique();
        });

        modelBuilder.Entity<Agencia>(e =>
        {
            e.ToTable("TB_AGENCIA");
            e.HasKey(a => a.Id);
            e.Property(a => a.Id).HasColumnName("ID_AGENCIA").ValueGeneratedOnAdd();
            e.Property(a => a.Nome).HasColumnName("NM_AGENCIA").HasMaxLength(200).IsRequired();
            e.Property(a => a.Codigo).HasColumnName("NR_CODIGO").HasMaxLength(10).IsRequired();
            e.Property(a => a.Endereco).HasColumnName("DS_ENDERECO").HasMaxLength(500);
        });

        modelBuilder.Entity<Produto>(e =>
        {
            e.ToTable("TB_PRODUTO");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("ID_PRODUTO").ValueGeneratedOnAdd();
            e.Property(p => p.Nome).HasColumnName("NM_PRODUTO").HasMaxLength(200).IsRequired();
            e.Property(p => p.Descricao).HasColumnName("DS_PRODUTO").HasMaxLength(500);
            e.HasDiscriminator<string>("TP_PRODUTO")
                .HasValue<Emprestimo>("EMPRESTIMO")
                .HasValue<MaquinaDeCartao>("MAQUINA_CARTAO")
                .HasValue<ReceberSalario>("RECEBER_SALARIO");
        });

        modelBuilder.Entity<Emprestimo>(e =>
        {
            e.Property(p => p.ValorMinimo).HasColumnName("VL_MINIMO").HasColumnType("NUMBER(18,2)");
            e.Property(p => p.ValorMaximo).HasColumnName("VL_MAXIMO").HasColumnType("NUMBER(18,2)");
            e.Property(p => p.TaxaJurosMensal).HasColumnName("PC_TAXA_JUROS").HasColumnType("NUMBER(5,4)");
        });

        modelBuilder.Entity<MaquinaDeCartao>(e =>
        {
            e.Property(p => p.TaxaMdr).HasColumnName("PC_TAXA_MDR").HasColumnType("NUMBER(5,4)");
        });

        modelBuilder.Entity<ReceberSalario>(e =>
        {
            e.Property(p => p.NomeEmpresa).HasColumnName("NM_EMPRESA").HasMaxLength(300);
        });

        modelBuilder.Entity<Contratacao>(e =>
        {
            e.ToTable("TB_CONTRATACAO");
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasColumnName("ID_CONTRATACAO").ValueGeneratedOnAdd();
            e.Property(c => c.ClienteId).HasColumnName("ID_CLIENTE");
            e.Property(c => c.ProdutoId).HasColumnName("ID_PRODUTO");
            e.Property(c => c.Status).HasColumnName("TP_STATUS").HasConversion<string>().HasMaxLength(20);
            e.Property(c => c.ValorSolicitado).HasColumnName("VL_SOLICITADO").HasColumnType("NUMBER(18,2)");
            e.Property(c => c.DataSolicitacao).HasColumnName("DT_SOLICITACAO");
            e.Property(c => c.DataProcessamento).HasColumnName("DT_PROCESSAMENTO");
            e.Property(c => c.ObservacaoProcessamento).HasColumnName("DS_OBSERVACAO").HasMaxLength(1000);
            e.Property(c => c.ScoreCredito).HasColumnName("NR_SCORE");

            e.HasOne(c => c.Cliente).WithMany(cl => cl.Contratacoes).HasForeignKey(c => c.ClienteId);
            e.HasOne(c => c.Produto).WithMany(p => p.Contratacoes).HasForeignKey(c => c.ProdutoId);
        });

        modelBuilder.Entity<Emprestimo>().HasData(new Emprestimo
        {
            Id = 1,
            Nome = "Empréstimo Pessoal",
            Descricao = "Empréstimo pessoal com análise de crédito automatizada",
            ValorMinimo = 1000m,
            ValorMaximo = 50000m,
            TaxaJurosMensal = 0.0299m
        });
    }
}
