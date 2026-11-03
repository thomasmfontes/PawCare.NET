using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(PawCareContext context)
        {
            if (!await context.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new User("admin", HashPassword("Admin@123"), "Administrador Geral", "Admin"),
                    new User("thomas", HashPassword("Fiap@2026"), "Thomas Fontes (RM562254)", "Admin"),
                    new User("vet", HashPassword("Vet@123"), "Dr. Veterinário Responsável", "Veterinario"),
                    new User("user", HashPassword("User@123"), "Usuário Tutor", "User")
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }

            if (!await context.Racas.AnyAsync())
            {
                var racas = new List<Raca>
                {
                    new Raca("Pastor Alemão", "Canina") { Id = 1 },
                    new Raca("Golden Retriever", "Canina") { Id = 2 },
                    new Raca("Siamês", "Felina") { Id = 3 },
                    new Raca("Persa", "Felina") { Id = 4 },
                    new Raca("Poodle", "Canina") { Id = 5 }
                };

                await context.Racas.AddRangeAsync(racas);
                await context.SaveChangesAsync();
            }

            if (!await context.Tutores.AnyAsync())
            {
                var tutores = new List<Tutor>
                {
                    new Tutor("12345678901", "Thomas Fontes", "11987654321", "thomas@fiap.com.br", 2),
                    new Tutor("98765432100", "Mariana Silva", "11912345678", "mariana@gmail.com", 1),
                    new Tutor("11122233344", "Carlos Eduardo", "1199998888", "carlos@outlook.com", 1)
                };

                await context.Tutores.AddRangeAsync(tutores);
                await context.SaveChangesAsync();
            }

            if (!await context.Pets.AnyAsync())
            {
                var pets = new List<Pet>
                {
                    new Pet("Thor", DateTime.UtcNow.AddYears(-3), 28.5, "Adulto", 1, "12345678901") { Id = 1 },
                    new Pet("Luna", DateTime.UtcNow.AddYears(-1), 18.0, "Adulto", 2, "12345678901") { Id = 2 },
                    new Pet("Mimi", DateTime.UtcNow.AddMonths(-6), 3.2, "Filhote", 3, "98765432100") { Id = 3 },
                    new Pet("Simba", DateTime.UtcNow.AddYears(-8), 4.5, "Idoso", 4, "11122233344") { Id = 4 }
                };

                await context.Pets.AddRangeAsync(pets);
                await context.SaveChangesAsync();
            }

            if (!await context.Clinicas.AnyAsync())
            {
                var clinicas = new List<Clinica>
                {
                    new Clinica("Hospital Veterinário Central", "1130001000", -23.5505, -46.6333, "Bela Vista", "São Paulo", "SP", true) { Id = 1 },
                    new Clinica("Clínica PetCare Jardins", "1130002000", -23.5650, -46.6600, "Jardins", "São Paulo", "SP", false) { Id = 2 }
                };

                await context.Clinicas.AddRangeAsync(clinicas);
                await context.SaveChangesAsync();
            }
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }
    }
}

