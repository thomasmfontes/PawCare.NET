using Microsoft.Extensions.Logging;
using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.DTOs.Pets;
using PawCareApi.Application.Interfaces;
using PawCareApi.Application.Mappers;
using PawCareApi.Domain.Common;
using PawCareApi.Domain.Interfaces;

namespace PawCareApi.Application.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly IHateoasService _hateoasService;
        private readonly ILogger<PetService> _logger;

        public PetService(
            IPetRepository petRepository,
            IHateoasService hateoasService,
            ILogger<PetService> logger)
        {
            _petRepository = petRepository;
            _hateoasService = hateoasService;
            _logger = logger;
        }

        public async Task<OperationResult<PageResult<PetResponseDto>>> ObterPaginadoAsync(
            PagedRequestDto request, string? nome, string? especie, long? racaId, string? tutorCpf, string baseUrl)
        {
            _logger.LogInformation("Consultando pets paginados: Página {Page}, Tamanho {Size}", request.PageNumber, request.PageSize);

            var pageResult = await _petRepository.ObterPaginadoAsync(
                request.PageNumber, request.PageSize, nome, especie, racaId, tutorCpf);

            var dtos = pageResult.Items.Select(p => p.ToResponseDto()).ToList();
            var pagedDtoResult = new PageResult<PetResponseDto>(dtos, pageResult.TotalItems, pageResult.PageNumber, pageResult.PageSize);

            _hateoasService.EnriquecerListaPets(pagedDtoResult, baseUrl);

            return OperationResult<PageResult<PetResponseDto>>.Success(
                pagedDtoResult, "Lista paginada de pets recuperada com sucesso.");
        }

        public async Task<OperationResult<IEnumerable<PetResponseDto>>> ObterTodosAsync(string baseUrl)
        {
            var pets = await _petRepository.ObterTodosAsync();
            var dtos = pets.Select(p => _hateoasService.EnriquecerPet(p.ToResponseDto(), baseUrl)).ToList();

            return OperationResult<IEnumerable<PetResponseDto>>.Success(dtos, "Pets recuperados com sucesso.");
        }

        public async Task<OperationResult<PetResponseDto>> ObterPorIdAsync(long id, string baseUrl)
        {
            var pet = await _petRepository.ObterPorIdAsync(id);
            if (pet == null)
            {
                return OperationResult<PetResponseDto>.NotFound($"Pet com ID {id} não foi encontrado.");
            }

            var dto = _hateoasService.EnriquecerPet(pet.ToResponseDto(), baseUrl);
            return OperationResult<PetResponseDto>.Success(dto, "Pet recuperado com sucesso.");
        }

        public async Task<OperationResult<PetResponseDto>> CriarAsync(PetRequestDto dto, string baseUrl)
        {
            _logger.LogInformation("Cadastrando novo pet: {Nome}", dto.Nome);

            var entity = dto.ToEntity();
            var criado = await _petRepository.CriarAsync(entity);

            var responseDto = _hateoasService.EnriquecerPet(criado.ToResponseDto(), baseUrl);
            return OperationResult<PetResponseDto>.Created(responseDto, "Pet cadastrado com sucesso.");
        }

        public async Task<OperationResult<PetResponseDto>> AtualizarAsync(long id, PetRequestDto dto, string baseUrl)
        {
            _logger.LogInformation("Atualizando pet ID {Id}", id);

            var pet = await _petRepository.ObterPorIdAsync(id);
            if (pet == null)
            {
                return OperationResult<PetResponseDto>.NotFound($"Pet com ID {id} não foi encontrado.");
            }

            pet.AtualizarDados(dto.Nome, dto.DataNascimento, dto.Peso, dto.StatusLongevidade, dto.RacaId, dto.TutorCpf);
            var atualizado = await _petRepository.AtualizarAsync(pet);

            var responseDto = _hateoasService.EnriquecerPet(atualizado.ToResponseDto(), baseUrl);
            return OperationResult<PetResponseDto>.Success(responseDto, "Pet atualizado com sucesso.");
        }

        public async Task<OperationResult> RemoverAsync(long id)
        {
            _logger.LogInformation("Removendo pet ID {Id}", id);

            var sucesso = await _petRepository.RemoverAsync(id);
            if (!sucesso)
            {
                return OperationResult.NotFound($"Pet com ID {id} não foi encontrado para remoção.");
            }

            return OperationResult.NoContent("Pet removido com sucesso.");
        }
    }
}

