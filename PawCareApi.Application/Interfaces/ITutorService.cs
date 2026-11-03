using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.DTOs.Tutores;
using PawCareApi.Domain.Common;

namespace PawCareApi.Application.Interfaces
{
    public interface ITutorService
    {
        Task<OperationResult<PageResult<TutorResponseDto>>> ObterPaginadoAsync(PagedRequestDto request, string? nome, string? email, string baseUrl);
        Task<OperationResult<IEnumerable<TutorResponseDto>>> ObterTodosAsync(string baseUrl);
        Task<OperationResult<TutorResponseDto>> ObterPorCpfAsync(string cpf, string baseUrl);
        Task<OperationResult<TutorResponseDto>> CriarAsync(TutorRequestDto dto, string baseUrl);
        Task<OperationResult<TutorResponseDto>> AtualizarAsync(string cpf, TutorRequestDto dto, string baseUrl);
        Task<OperationResult> RemoverAsync(string cpf);
    }
}

