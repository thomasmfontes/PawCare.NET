using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Tests.Integration.Fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Ativa o flag para usar banco em memória no Program.cs
        builder.UseSetting("UseInMemoryDatabase", "true");

        builder.ConfigureServices(services =>
        {
            // Obtém o ServiceProvider e popula os dados iniciais de teste
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PawCareContext>();

            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    private static void SeedTestData(PawCareContext db)
    {
        if (!db.Tutores.Any())
        {
            var tutor = new Tutor
            {
                Cpf = "11122233344",
                Nome = "Tutor Teste Integracao",
                Email = "integracao@pawcare.com",
                Telefone = "11912345678",
                QtdPets = 1
            };

            var raca = new Raca
            {
                Id = 100,
                Nome = "Pastor Alemão",
                Especie = "Canina"
            };

            var pet = new Pet
            {
                Id = 500,
                Nome = "Max",
                DataNascimento = new DateTime(2020, 1, 1),
                Peso = 30.0,
                StatusLongevidade = "Adulto",
                TutorCpf = tutor.Cpf,
                RacaId = raca.Id
            };

            var clinica = new Clinica
            {
                Id = 200,
                NomeCnpj = "Hospital Veterinário São Paulo",
                Telefone = "1133334444",
                Cidade = "São Paulo",
                Estado = "SP",
                Atendimento24h = true
            };

            db.Tutores.Add(tutor);
            db.Racas.Add(raca);
            db.Pets.Add(pet);
            db.Clinicas.Add(clinica);

            db.SaveChanges();
        }
    }
}
