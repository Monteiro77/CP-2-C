using BancoDigital.Api.Data;
using BancoDigital.Api.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BancoDigital.Tests.Helpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var dbName = "TestDb_" + Guid.NewGuid();

        builder.ConfigureServices(services =>
        {
            var dbDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<BancoContext>));
            if (dbDescriptor != null) services.Remove(dbDescriptor);

            services.AddDbContext<BancoContext>(options =>
                options.UseInMemoryDatabase(dbName));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BancoContext>();
            db.Database.EnsureCreated();
            SeedDatabase(db);
        });
    }

    private static void SeedDatabase(BancoContext db)
    {
        if (!db.Emprestimos.Any())
        {
            db.Emprestimos.Add(new Emprestimo
            {
                Id = 1,
                Nome = "Empréstimo Pessoal",
                Descricao = "Empréstimo pessoal com análise de crédito automatizada",
                ValorMinimo = 1000m,
                ValorMaximo = 50000m,
                TaxaJurosMensal = 0.0299m
            });
            db.SaveChanges();
        }
    }
}
