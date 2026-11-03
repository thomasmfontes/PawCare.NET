using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.DTOs.Pets;
using PawCareApi.Domain.Common;

namespace PawCareApi.Application.Interfaces
{
    public interface IPetService
    {
        Task<OperationResult<PageResult<PetResponseDto>>> ObterPaginadoAsync(PagedRequestDto request, string? nome, string? especie, long? racaId, string? tutorCpf, string baseUrl);
        Task<OperationResult<IEnumerable<PetResponseDto>>> ObterTodosAsync(string baseUrl);
        Task<OperationResult<PetResponseDto>> ObterPorIdAsync(long id, string baseUrl);
        Task<OperationResult<PetResponseDto>> CriarAsync(PetRequestDto dto, string baseUrl);
        Task<OperationResult<PetResponseDto>> AtualizarAsync(long id, PetRequestDto dto, string baseUrl);
        Task<OperationResult> RemoverAsync(long id);
    }
}

