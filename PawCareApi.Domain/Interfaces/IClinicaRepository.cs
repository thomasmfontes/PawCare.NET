using PawCareApi.Domain.Common;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Domain.Interfaces
{
    public interface IClinicaRepository
    {
        Task<PageResult<Clinica>> ObterPaginadoAsync(int pageNumber, int pageSize, string? cidade = null, bool? atendimento24h = null);
        Task<IEnumerable<Clinica>> ObterTodasAsync();
        Task<Clinica?> ObterPorIdAsync(long id);
        Task<Clinica> CriarAsync(Clinica clinica);
        Task<Clinica> AtualizarAsync(Clinica clinica);
        Task<bool> RemoverAsync(long id);
        Task<bool> ExisteAsync(long id);
    }
}

