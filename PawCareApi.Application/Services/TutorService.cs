using Microsoft.Extensions.Logging;
using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.DTOs.Tutores;
using PawCareApi.Application.Interfaces;
using PawCareApi.Application.Mappers;
using PawCareApi.Domain.Common;
using PawCareApi.Domain.Interfaces;

namespace PawCareApi.Application.Services
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IHateoasService _hateoasService;
        private readonly ILogger<TutorService> _logger;

        public TutorService(
            ITutorRepository tutorRepository,
            IHateoasService hateoasService,
            ILogger<TutorService> logger)
        {
            _tutorRepository = tutorRepository;
            _hateoasService = hateoasService;
            _logger = logger;
        }

        public async Task<OperationResult<PageResult<TutorResponseDto>>> ObterPaginadoAsync(
            PagedRequestDto request, string? nome, string? email, string baseUrl)
        {
            var pageResult = await _tutorRepository.ObterPaginadoAsync(request.PageNumber, request.PageSize, nome, email);
            var dtos = pageResult.Items.Select(t => t.ToResponseDto()).ToList();
            var pagedDtoResult = new PageResult<TutorResponseDto>(dtos, pageResult.TotalItems, pageResult.PageNumber, pageResult.PageSize);

            _hateoasService.EnriquecerListaTutores(pagedDtoResult, baseUrl);
            return OperationResult<PageResult<TutorResponseDto>>.Success(pagedDtoResult, "Lista paginada de tutores recuperada com sucesso.");
        }

        public async Task<OperationResult<IEnumerable<TutorResponseDto>>> ObterTodosAsync(string baseUrl)
        {
            var tutores = await _tutorRepository.ObterTodosAsync();
            var dtos = tutores.Select(t => _hateoasService.EnriquecerTutor(t.ToResponseDto(), baseUrl)).ToList();

            return OperationResult<IEnumerable<TutorResponseDto>>.Success(dtos, "Tutores recuperados com sucesso.");
        }

        public async Task<OperationResult<TutorResponseDto>> ObterPorCpfAsync(string cpf, string baseUrl)
        {
            var tutor = await _tutorRepository.ObterPorCpfAsync(cpf);
            if (tutor == null)
            {
                return OperationResult<TutorResponseDto>.NotFound($"Tutor com CPF {cpf} não foi encontrado.");
            }

            var dto = _hateoasService.EnriquecerTutor(tutor.ToResponseDto(), baseUrl);
            return OperationResult<TutorResponseDto>.Success(dto, "Tutor recuperado com sucesso.");
        }

        public async Task<OperationResult<TutorResponseDto>> CriarAsync(TutorRequestDto dto, string baseUrl)
        {
            _logger.LogInformation("Cadastrando novo tutor: {Nome} - CPF {Cpf}", dto.Nome, dto.Cpf);

            if (await _tutorRepository.ExisteAsync(dto.Cpf))
            {
                return OperationResult<TutorResponseDto>.Conflict($"Já existe um tutor cadastrado com o CPF {dto.Cpf}.");
            }

            var entity = dto.ToEntity();
            var criado = await _tutorRepository.CriarAsync(entity);

            var responseDto = _hateoasService.EnriquecerTutor(criado.ToResponseDto(), baseUrl);
            return OperationResult<TutorResponseDto>.Created(responseDto, "Tutor cadastrado com sucesso.");
        }

        public async Task<OperationResult<TutorResponseDto>> AtualizarAsync(string cpf, TutorRequestDto dto, string baseUrl)
        {
            var tutor = await _tutorRepository.ObterPorCpfAsync(cpf);
            if (tutor == null)
            {
                return OperationResult<TutorResponseDto>.NotFound($"Tutor com CPF {cpf} não foi encontrado.");
            }

            tutor.AtualizarDados(dto.Nome, dto.Telefone, dto.Email, dto.QtdPets);
            var atualizado = await _tutorRepository.AtualizarAsync(tutor);

            var responseDto = _hateoasService.EnriquecerTutor(atualizado.ToResponseDto(), baseUrl);
            return OperationResult<TutorResponseDto>.Success(responseDto, "Tutor atualizado com sucesso.");
        }

        public async Task<OperationResult> RemoverAsync(string cpf)
        {
            var sucesso = await _tutorRepository.RemoverAsync(cpf);
            if (!sucesso)
            {
                return OperationResult.NotFound($"Tutor com CPF {cpf} não foi encontrado para remoção.");
            }

            return OperationResult.NoContent("Tutor removido com sucesso.");
        }
    }
}

