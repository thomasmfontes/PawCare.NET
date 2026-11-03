using PawCareApi.Domain.Common;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Domain.Interfaces
{
    public interface ITutorRepository
    {
        Task<PageResult<Tutor>> ObterPaginadoAsync(int pageNumber, int pageSize, string? nome = null, string? email = null);
        Task<IEnumerable<Tutor>> ObterTodosAsync();
        Task<Tutor?> ObterPorCpfAsync(string cpf);
        Task<Tutor> CriarAsync(Tutor tutor);
        Task<Tutor> AtualizarAsync(Tutor tutor);
        Task<bool> RemoverAsync(string cpf);
        Task<bool> ExisteAsync(string cpf);
    }
}

