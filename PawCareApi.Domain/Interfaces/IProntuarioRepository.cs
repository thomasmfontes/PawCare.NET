using PawCareApi.Domain.Common;
using PawCareApi.Domain.Entities;

namespace PawCareApi.Domain.Interfaces
{
    public interface IProntuarioRepository
    {
        Task<PageResult<ProntuarioNoSql>> ObterPaginadoAsync(int pageNumber, int pageSize, long? petId = null, string? tipoAtendimento = null, string? sintoma = null);
        Task<IEnumerable<ProntuarioNoSql>> ObterPorPetIdAsync(long petId);
        Task<ProntuarioNoSql?> ObterPorIdAsync(string id);
        Task<ProntuarioNoSql> CriarAsync(ProntuarioNoSql prontuario);
        Task<ProntuarioNoSql> AtualizarAsync(string id, ProntuarioNoSql prontuario);
        Task<bool> RemoverAsync(string id);
        Task<bool> PingAsync();
    }
}

