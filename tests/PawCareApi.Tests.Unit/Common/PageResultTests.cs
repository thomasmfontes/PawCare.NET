using FluentAssertions;
using PawCareApi.Domain.Common;

namespace PawCareApi.Tests.Unit.Common
{
    public class PageResultTests
    {
        [Fact]
        public void PageResult_ComItensEMetadados_DeveCalcularPaginacaoCorretamente()
        {
            var itens = new List<string> { "Item 1", "Item 2", "Item 3" };
            var page = new PageResult<string>(itens, 25, 1, 10);

            page.Items.Should().HaveCount(3);
            page.TotalItems.Should().Be(25);
            page.PageNumber.Should().Be(1);
            page.PageSize.Should().Be(10);
            page.TotalPages.Should().Be(3);
            page.HasPreviousPage.Should().BeFalse();
            page.HasNextPage.Should().BeTrue();
        }

        [Fact]
        public void PageResult_NaUltimaPagina_HasNextPageDeveSerFalse()
        {
            var itens = new List<string> { "Item 21", "Item 22" };
            var page = new PageResult<string>(itens, 22, 3, 10);

            page.TotalPages.Should().Be(3);
            page.HasPreviousPage.Should().BeTrue();
            page.HasNextPage.Should().BeFalse();
        }
    }
}

