using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.DTOs.Tutores;
using PawCareApi.Application.Interfaces;
using PawCareApi.Domain.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace PawCareApi.Controllers
{
    [Route("api/[controller]")]
    public class TutoresController : ApiControllerBase
    {
        private readonly ITutorService _tutorService;

        public TutoresController(ITutorService tutorService)
        {
            _tutorService = tutorService;
        }

        /// <summary>
        /// Obtém a listagem paginada de tutores com filtros e HATEOAS.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lista tutores paginados com filtros e HATEOAS", Description = "Retorna lista de tutores paginada utilizando PageResult.")]
        [ProducesResponseType(typeof(OperationResult<PageResult<TutorResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPaginado([FromQuery] PagedRequestDto request, [FromQuery] string? nome = null, [FromQuery] string? email = null)
        {
            var baseUrl = GetBaseUrl();
            var result = await _tutorService.ObterPaginadoAsync(request, nome, email, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtém todos os tutores cadastrados.
        /// </summary>
        [HttpGet("todos")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lista todos os tutores", Description = "Retorna todos os tutores com hiperlinks.")]
        [ProducesResponseType(typeof(OperationResult<IEnumerable<TutorResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodos()
        {
            var baseUrl = GetBaseUrl();
            var result = await _tutorService.ObterTodosAsync(baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtém um tutor específico pelo seu CPF com HATEOAS.
        /// </summary>
        [HttpGet("{cpf}", Name = "ObterTutorPorCpf")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Obtém tutor por CPF com HATEOAS", Description = "Retorna detalhes do tutor cadastrado e seus links hipermídia.")]
        [ProducesResponseType(typeof(OperationResult<TutorResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorCpf(string cpf)
        {
            var baseUrl = GetBaseUrl();
            var result = await _tutorService.ObterPorCpfAsync(cpf, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Cadastra um novo tutor no sistema. Requer autenticação (Roles: Admin ou Veterinario).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Veterinario")]
        [SwaggerOperation(Summary = "Cadastra novo tutor [Requer Admin/Veterinario]", Description = "Criação de tutor com validações de unicidade de CPF e consistência de dados.")]
        [ProducesResponseType(typeof(OperationResult<TutorResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Criar([FromBody] TutorRequestDto dto)
        {
            var baseUrl = GetBaseUrl();
            var result = await _tutorService.CriarAsync(dto, baseUrl);
            return HandleResult(result, "ObterTutorPorCpf", new { cpf = result.Data?.Cpf });
        }

        /// <summary>
        /// Atualiza os dados de um tutor existente. Requer autenticação (Roles: Admin ou Veterinario).
        /// </summary>
        [HttpPut("{cpf}")]
        [Authorize(Roles = "Admin,Veterinario")]
        [SwaggerOperation(Summary = "Atualiza tutor existente [Requer Admin/Veterinario]", Description = "Atualiza telefone, email, nome e quantidade de pets.")]
        [ProducesResponseType(typeof(OperationResult<TutorResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Atualizar(string cpf, [FromBody] TutorRequestDto dto)
        {
            var baseUrl = GetBaseUrl();
            var result = await _tutorService.AtualizarAsync(cpf, dto, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Remove um tutor pelo CPF. Requer perfil de Administrador.
        /// </summary>
        [HttpDelete("{cpf}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Remove um tutor pelo CPF [Requer Admin]", Description = "Exclusão permanente de um tutor cadastrado.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Remover(string cpf)
        {
            var result = await _tutorService.RemoverAsync(cpf);
            return HandleResult(result);
        }
    }
}
