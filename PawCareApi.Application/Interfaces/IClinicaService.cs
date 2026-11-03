using PawCareApi.Application.DTOs.Clinicas;
using PawCareApi.Application.DTOs.Common;
using PawCareApi.Domain.Common;

namespace PawCareApi.Application.Interfaces
{
    public interface IClinicaService
    {
        Task<OperationResult<PageResult<ClinicaResponseDto>>> ObterPaginadoAsync(PagedRequestDto request, string? cidade, bool? atendimento24h, string baseUrl);
        Task<OperationResult<IEnumerable<ClinicaResponseDto>>> ObterTodasAsync(string baseUrl);
        Task<OperationResult<ClinicaResponseDto>> ObterPorIdAsync(long id, string baseUrl);
        Task<OperationResult<ClinicaResponseDto>> CriarAsync(ClinicaRequestDto dto, string baseUrl);
        Task<OperationResult<ClinicaResponseDto>> AtualizarAsync(long id, ClinicaRequestDto dto, string baseUrl);
        Task<OperationResult> RemoverAsync(long id);
    }
}

