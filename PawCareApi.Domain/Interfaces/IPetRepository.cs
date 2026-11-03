using PawCareApi.Domain.Common;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Domain.Interfaces
{
    public interface IPetRepository
    {
        Task<PageResult<Pet>> ObterPaginadoAsync(int pageNumber, int pageSize, string? nome = null, string? especie = null, long? racaId = null, string? tutorCpf = null);
        Task<IEnumerable<Pet>> ObterTodosAsync();
        Task<Pet?> ObterPorIdAsync(long id);
        Task<Pet> CriarAsync(Pet pet);
        Task<Pet> AtualizarAsync(Pet pet);
        Task<bool> RemoverAsync(long id);
        Task<bool> ExisteAsync(long id);
        Task<IEnumerable<Pet>> ObterPorTutorCpfAsync(string tutorCpf);
    }
}

