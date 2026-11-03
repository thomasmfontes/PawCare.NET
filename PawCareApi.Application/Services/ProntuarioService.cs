using Microsoft.Extensions.Logging;
using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.DTOs.Prontuarios;
using PawCareApi.Application.Interfaces;
using PawCareApi.Application.Mappers;
using PawCareApi.Domain.Common;
using PawCareApi.Domain.Interfaces;

namespace PawCareApi.Application.Services
{
    public class ProntuarioService : IProntuarioService
    {
        private readonly IProntuarioRepository _prontuarioRepository;
        private readonly IHateoasService _hateoasService;
        private readonly ILogger<ProntuarioService> _logger;

        public ProntuarioService(
            IProntuarioRepository prontuarioRepository,
            IHateoasService hateoasService,
            ILogger<ProntuarioService> logger)
        {
            _prontuarioRepository = prontuarioRepository;
            _hateoasService = hateoasService;
            _logger = logger;
        }

        public async Task<OperationResult<PageResult<ProntuarioResponseDto>>> ObterPaginadoAsync(
            PagedRequestDto request, long? petId, string? tipoAtendimento, string? sintoma, string baseUrl)
        {
            _logger.LogInformation("Consultando prontuários NoSQL paginados: Página {Page}, Tamanho {Size}", request.PageNumber, request.PageSize);

            var pageResult = await _prontuarioRepository.ObterPaginadoAsync(
                request.PageNumber, request.PageSize, petId, tipoAtendimento, sintoma);

            var dtos = pageResult.Items.Select(p => p.ToResponseDto()).ToList();
            var pagedDtoResult = new PageResult<ProntuarioResponseDto>(dtos, pageResult.TotalItems, pageResult.PageNumber, pageResult.PageSize);

            _hateoasService.EnriquecerListaProntuarios(pagedDtoResult, baseUrl);
            return OperationResult<PageResult<ProntuarioResponseDto>>.Success(
                pagedDtoResult, "Prontuários NoSQL recuperados com sucesso.");
        }

        public async Task<OperationResult<IEnumerable<ProntuarioResponseDto>>> ObterPorPetIdAsync(long petId, string baseUrl)
        {
            var prontuarios = await _prontuarioRepository.ObterPorPetIdAsync(petId);
            var dtos = prontuarios.Select(p => _hateoasService.EnriquecerProntuario(p.ToResponseDto(), baseUrl)).ToList();

            return OperationResult<IEnumerable<ProntuarioResponseDto>>.Success(
                dtos, $"Prontuários do pet {petId} recuperados com sucesso.");
        }

        public async Task<OperationResult<ProntuarioResponseDto>> ObterPorIdAsync(string id, string baseUrl)
        {
            var prontuario = await _prontuarioRepository.ObterPorIdAsync(id);
            if (prontuario == null)
            {
                return OperationResult<ProntuarioResponseDto>.NotFound($"Prontuário NoSQL com ID '{id}' não foi encontrado.");
            }

            var dto = _hateoasService.EnriquecerProntuario(prontuario.ToResponseDto(), baseUrl);
            return OperationResult<ProntuarioResponseDto>.Success(dto, "Prontuário NoSQL recuperado com sucesso.");
        }

        public async Task<OperationResult<ProntuarioResponseDto>> CriarAsync(ProntuarioRequestDto dto, string baseUrl)
        {
            _logger.LogInformation("Criando novo prontuário NoSQL para o pet {PetId} - {NomePet}", dto.PetId, dto.NomePet);

            var entity = dto.ToEntity();
            var criado = await _prontuarioRepository.CriarAsync(entity);

            var responseDto = _hateoasService.EnriquecerProntuario(criado.ToResponseDto(), baseUrl);
            return OperationResult<ProntuarioResponseDto>.Created(responseDto, "Prontuário NoSQL registrado com sucesso.");
        }

        public async Task<OperationResult<ProntuarioResponseDto>> AtualizarAsync(string id, ProntuarioRequestDto dto, string baseUrl)
        {
            _logger.LogInformation("Atualizando prontuário NoSQL ID {Id}", id);

            var existente = await _prontuarioRepository.ObterPorIdAsync(id);
            if (existente == null)
            {
                return OperationResult<ProntuarioResponseDto>.NotFound($"Prontuário NoSQL com ID '{id}' não foi encontrado para atualização.");
            }

            existente.Atualizar(
                dto.VeterinarioNome,
                dto.TipoAtendimento,
                dto.QueixaPrincipal,
                dto.Diagnostico,
                dto.SinaisVitais,
                dto.Sintomas,
                dto.Prescricoes,
                dto.Observacoes
            );

            var atualizado = await _prontuarioRepository.AtualizarAsync(id, existente);
            var responseDto = _hateoasService.EnriquecerProntuario(atualizado.ToResponseDto(), baseUrl);

            return OperationResult<ProntuarioResponseDto>.Success(responseDto, "Prontuário NoSQL atualizado com sucesso.");
        }

        public async Task<OperationResult> RemoverAsync(string id)
        {
            _logger.LogInformation("Removendo prontuário NoSQL ID {Id}", id);

            var sucesso = await _prontuarioRepository.RemoverAsync(id);
            if (!sucesso)
            {
                return OperationResult.NotFound($"Prontuário NoSQL com ID '{id}' não foi encontrado para remoção.");
            }

            return OperationResult.NoContent("Prontuário NoSQL removido com sucesso.");
        }
    }
}

