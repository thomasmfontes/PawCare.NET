using Microsoft.EntityFrameworkCore;
using PawCareApi.Domain.Entities;
using PawCareApi.Infrastructure.Data;

namespace PawCareApi.Tests.Unit.Fixtures;

public class TestDbContextFixture : IDisposable
{
    public PawCareContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PawCareContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new PawCareContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
