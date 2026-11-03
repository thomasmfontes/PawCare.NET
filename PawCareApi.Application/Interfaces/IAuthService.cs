using PawCareApi.Application.DTOs.Auth;
using PawCareApi.Domain.Common;

namespace PawCareApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task<OperationResult<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    }
}

