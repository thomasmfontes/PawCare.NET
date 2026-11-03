using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.DTOs.Prontuarios;
using PawCareApi.Domain.Common;

namespace PawCareApi.Application.Interfaces
{
    public interface IProntuarioService
    {
        Task<OperationResult<PageResult<ProntuarioResponseDto>>> ObterPaginadoAsync(PagedRequestDto request, long? petId, string? tipoAtendimento, string? sintoma, string baseUrl);
        Task<OperationResult<IEnumerable<ProntuarioResponseDto>>> ObterPorPetIdAsync(long petId, string baseUrl);
        Task<OperationResult<ProntuarioResponseDto>> ObterPorIdAsync(string id, string baseUrl);
        Task<OperationResult<ProntuarioResponseDto>> CriarAsync(ProntuarioRequestDto dto, string baseUrl);
        Task<OperationResult<ProntuarioResponseDto>> AtualizarAsync(string id, ProntuarioRequestDto dto, string baseUrl);
        Task<OperationResult> RemoverAsync(string id);
    }
}

