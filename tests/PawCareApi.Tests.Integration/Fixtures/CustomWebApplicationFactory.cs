using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PawCareApi.Domain.Entities;
using PawCareApi.Infrastructure.Data;

namespace PawCareApi.Tests.Integration.Fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Ativa o flag para usar banco em memória no Program.cs
        builder.UseSetting("UseInMemoryDatabase", "true");

        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PawCareContext>();

            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    private static void SeedTestData(PawCareContext db)
    {
        if (!db.Tutores.Any(t => t.Cpf == "11122233344"))
        {
            var tutor = new Tutor("11122233344", "Tutor Teste Integracao", "11912345678", "integracao@pawcare.com", 1);
            var raca = new Raca("Pastor Alemão", "Canina") { Id = 100 };
            var pet = new Pet("Max", new DateTime(2020, 1, 1), 30.0, "Adulto", raca.Id, tutor.Cpf) { Id = 500 };
            var clinica = new Clinica("Hospital Veterinário São Paulo", "1133334444", -23.5, -46.6, "Centro", "São Paulo", "SP", true) { Id = 200 };

            db.Tutores.Add(tutor);
            if (!db.Racas.Any(r => r.Id == 100)) db.Racas.Add(raca);
            if (!db.Pets.Any(p => p.Id == 500)) db.Pets.Add(pet);
            if (!db.Clinicas.Any(c => c.Id == 200)) db.Clinicas.Add(clinica);

            db.SaveChanges();
        }
    }
}
