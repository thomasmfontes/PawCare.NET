using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.DTOs.Pets;
using PawCareApi.Application.Interfaces;
using PawCareApi.Domain.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace PawCareApi.Controllers
{
    [Route("api/[controller]")]
    public class PetsController : ApiControllerBase
    {
        private readonly IPetService _petService;

        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        /// <summary>
        /// Obtém a listagem paginada de pets cadastrados com metadados estruturados (PageResult) e links hipermídia (HATEOAS).
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lista pets paginados com filtros e HATEOAS", Description = "Retorna uma lista paginada utilizando PageResult com metadados de navegação e hipermídia.")]
        [ProducesResponseType(typeof(OperationResult<PageResult<PetResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPaginado(
            [FromQuery] PagedRequestDto request,
            [FromQuery] string? nome = null,
            [FromQuery] string? especie = null,
            [FromQuery] long? racaId = null,
            [FromQuery] string? tutorCpf = null)
        {
            var baseUrl = GetBaseUrl();
            var result = await _petService.ObterPaginadoAsync(request, nome, especie, racaId, tutorCpf, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtém todos os pets cadastrados sem paginação, enriquecidos com HATEOAS.
        /// </summary>
        [HttpGet("todos")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lista todos os pets cadastrados", Description = "Retorna a coleção completa de pets com seus relacionamentos.")]
        [ProducesResponseType(typeof(OperationResult<IEnumerable<PetResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodos()
        {
            var baseUrl = GetBaseUrl();
            var result = await _petService.ObterTodosAsync(baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Recupera os dados detalhados de um pet específico a partir de seu ID, enriquecido com HATEOAS.
        /// </summary>
        [HttpGet("{id:long}", Name = "ObterPetPorId")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Obtém pet por ID com HATEOAS", Description = "Retorna os detalhes de um pet específico e links de navegação para seus recursos.")]
        [ProducesResponseType(typeof(OperationResult<PetResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(long id)
        {
            var baseUrl = GetBaseUrl();
            var result = await _petService.ObterPorIdAsync(id, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Cadastra um novo pet no sistema. Requer autenticação (Roles: Admin ou Veterinario).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Veterinario")]
        [SwaggerOperation(Summary = "Cadastra novo pet [Requer Admin/Veterinario]", Description = "Criação de pet com validações de domínio e retorno de erro ou sucesso via OperationResult.")]
        [ProducesResponseType(typeof(OperationResult<PetResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Criar([FromBody] PetRequestDto dto)
        {
            var baseUrl = GetBaseUrl();
            var result = await _petService.CriarAsync(dto, baseUrl);
            return HandleResult(result, "ObterPetPorId", new { id = result.Data?.Id });
        }

        /// <summary>
        /// Atualiza os dados de um pet existente. Requer autenticação (Roles: Admin ou Veterinario).
        /// </summary>
        [HttpPut("{id:long}")]
        [Authorize(Roles = "Admin,Veterinario")]
        [SwaggerOperation(Summary = "Atualiza pet existente [Requer Admin/Veterinario]", Description = "Atualização completa de dados cadastrais do pet com validação de domínio.")]
        [ProducesResponseType(typeof(OperationResult<PetResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Atualizar(long id, [FromBody] PetRequestDto dto)
        {
            var baseUrl = GetBaseUrl();
            var result = await _petService.AtualizarAsync(id, dto, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Remove um pet do sistema pelo ID. Requer perfil de Administrador.
        /// </summary>
        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Remove um pet pelo ID [Requer Admin]", Description = "Exclusão permanente de um pet da base de dados.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Remover(long id)
        {
            var result = await _petService.RemoverAsync(id);
            return HandleResult(result);
        }
    }
}
