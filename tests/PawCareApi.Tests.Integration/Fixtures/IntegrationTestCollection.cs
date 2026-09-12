namespace PawCareApi.Tests.Integration.Fixtures;

[CollectionDefinition("IntegrationTestCollection")]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    // Definição da Collection Fixture compartilhada entre as classes de teste de integração
}
