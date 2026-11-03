using Microsoft.Extensions.Logging;
using PawCareApi.Application.DTOs.Clinicas;
using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.Interfaces;
using PawCareApi.Application.Mappers;
using PawCareApi.Domain.Common;
using PawCareApi.Domain.Interfaces;

namespace PawCareApi.Application.Services
{
    public class ClinicaService : IClinicaService
    {
        private readonly IClinicaRepository _clinicaRepository;
        private readonly IHateoasService _hateoasService;
        private readonly ILogger<ClinicaService> _logger;

        public ClinicaService(
            IClinicaRepository clinicaRepository,
            IHateoasService hateoasService,
            ILogger<ClinicaService> logger)
        {
            _clinicaRepository = clinicaRepository;
            _hateoasService = hateoasService;
            _logger = logger;
        }

        public async Task<OperationResult<PageResult<ClinicaResponseDto>>> ObterPaginadoAsync(
            PagedRequestDto request, string? cidade, bool? atendimento24h, string baseUrl)
        {
            var pageResult = await _clinicaRepository.ObterPaginadoAsync(
                request.PageNumber, request.PageSize, cidade, atendimento24h);

            var dtos = pageResult.Items.Select(c => c.ToResponseDto()).ToList();
            var pagedDtoResult = new PageResult<ClinicaResponseDto>(dtos, pageResult.TotalItems, pageResult.PageNumber, pageResult.PageSize);

            _hateoasService.EnriquecerListaClinicas(pagedDtoResult, baseUrl);
            return OperationResult<PageResult<ClinicaResponseDto>>.Success(pagedDtoResult, "Lista paginada de clínicas recuperada com sucesso.");
        }

        public async Task<OperationResult<IEnumerable<ClinicaResponseDto>>> ObterTodasAsync(string baseUrl)
        {
            var clinicas = await _clinicaRepository.ObterTodasAsync();
            var dtos = clinicas.Select(c => _hateoasService.EnriquecerClinica(c.ToResponseDto(), baseUrl)).ToList();

            return OperationResult<IEnumerable<ClinicaResponseDto>>.Success(dtos, "Clínicas recuperadas com sucesso.");
        }

        public async Task<OperationResult<ClinicaResponseDto>> ObterPorIdAsync(long id, string baseUrl)
        {
            var clinica = await _clinicaRepository.ObterPorIdAsync(id);
            if (clinica == null)
            {
                return OperationResult<ClinicaResponseDto>.NotFound($"Clínica com ID {id} não foi encontrada.");
            }

            var dto = _hateoasService.EnriquecerClinica(clinica.ToResponseDto(), baseUrl);
            return OperationResult<ClinicaResponseDto>.Success(dto, "Clínica recuperada com sucesso.");
        }

        public async Task<OperationResult<ClinicaResponseDto>> CriarAsync(ClinicaRequestDto dto, string baseUrl)
        {
            _logger.LogInformation("Cadastrando nova clínica: {NomeCnpj}", dto.NomeCnpj);

            var entity = dto.ToEntity();
            var criada = await _clinicaRepository.CriarAsync(entity);

            var responseDto = _hateoasService.EnriquecerClinica(criada.ToResponseDto(), baseUrl);
            return OperationResult<ClinicaResponseDto>.Created(responseDto, "Clínica cadastrada com sucesso.");
        }

        public async Task<OperationResult<ClinicaResponseDto>> AtualizarAsync(long id, ClinicaRequestDto dto, string baseUrl)
        {
            var clinica = await _clinicaRepository.ObterPorIdAsync(id);
            if (clinica == null)
            {
                return OperationResult<ClinicaResponseDto>.NotFound($"Clínica com ID {id} não foi encontrada.");
            }

            clinica.AtualizarDados(dto.NomeCnpj, dto.Telefone, dto.Latitude, dto.Longitude, dto.Bairro, dto.Cidade, dto.Estado, dto.Atendimento24h);
            var atualizada = await _clinicaRepository.AtualizarAsync(clinica);

            var responseDto = _hateoasService.EnriquecerClinica(atualizada.ToResponseDto(), baseUrl);
            return OperationResult<ClinicaResponseDto>.Success(responseDto, "Clínica atualizada com sucesso.");
        }

        public async Task<OperationResult> RemoverAsync(long id)
        {
            var sucesso = await _clinicaRepository.RemoverAsync(id);
            if (!sucesso)
            {
                return OperationResult.NotFound($"Clínica com ID {id} não foi encontrada para remoção.");
            }

            return OperationResult.NoContent("Clínica removida com sucesso.");
        }
    }
}

