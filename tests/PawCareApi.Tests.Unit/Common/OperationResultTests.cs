using FluentAssertions;
using PawCareApi.Domain.Common;

namespace PawCareApi.Tests.Unit.Common
{
    public class OperationResultTests
    {
        [Fact]
        public void Success_DeveCriarResultadoComSucessoEStatusCode200()
        {
            var result = OperationResult.Success("Operação concluída");

            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Operação concluída");
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Created_ComDados_DeveCriarResultadoComSucessoEStatusCode201()
        {
            var data = new { Id = 1, Nome = "Item" };
            var result = OperationResult<object>.Created(data, "Recurso cadastrado");

            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(201);
            result.Data.Should().Be(data);
        }

        [Fact]
        public void NotFound_DeveRetornarIsSuccessFalseEStatusCode404()
        {
            var result = OperationResult.NotFound("Não localizado");

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("Não localizado");
        }

        [Fact]
        public void Failure_ComErros_DeveConterListaDeErros()
        {
            var result = OperationResult.Failure("Falha na validação", 400, "Erro 1", "Erro 2");

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Errors.Should().HaveCount(2);
            result.Errors.Should().Contain("Erro 1");
        }
    }
}

