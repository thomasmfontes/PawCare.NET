using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.DTOs.Prontuarios;
using PawCareApi.Application.Interfaces;
using PawCareApi.Domain.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace PawCareApi.Controllers
{
    /// <summary>
    /// Endpoints para gerenciamento NoSQL de Prontuários e Atendimentos Clínicos Veterinários (MongoDB).
    /// </summary>
    [Route("api/[controller]")]
    public class ProntuariosController : ApiControllerBase
    {
        private readonly IProntuarioService _prontuarioService;

        public ProntuariosController(IProntuarioService prontuarioService)
        {
            _prontuarioService = prontuarioService;
        }

        /// <summary>
        /// Obtém a listagem paginada de prontuários clínicos NoSQL (MongoDB) com filtros dinâmicos e HATEOAS.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lista prontuários NoSQL paginados com filtros e HATEOAS", Description = "Consulta documentos de prontuário clínico veterinário no MongoDB com filtros por pet, tipo de atendimento e sintomas.")]
        [ProducesResponseType(typeof(OperationResult<PageResult<ProntuarioResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPaginado(
            [FromQuery] PagedRequestDto request,
            [FromQuery] long? petId = null,
            [FromQuery] string? tipoAtendimento = null,
            [FromQuery] string? sintoma = null)
        {
            var baseUrl = GetBaseUrl();
            var result = await _prontuarioService.ObterPaginadoAsync(request, petId, tipoAtendimento, sintoma, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtém todos os prontuários clínicos NoSQL de um pet específico a partir de seu PetId relacional.
        /// </summary>
        [HttpGet("pet/{petId:long}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lista prontuários NoSQL de um pet", Description = "Recupera o histórico completo de atendimentos NoSQL vinculados a um pet.")]
        [ProducesResponseType(typeof(OperationResult<IEnumerable<ProntuarioResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPorPet(long petId)
        {
            var baseUrl = GetBaseUrl();
            var result = await _prontuarioService.ObterPorPetIdAsync(petId, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtém um prontuário clínico NoSQL específico pelo seu ID (MongoDB ObjectId).
        /// </summary>
        [HttpGet("{id}", Name = "ObterProntuarioPorId")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Obtém prontuário NoSQL por ID com HATEOAS", Description = "Retorna o documento detalhado com sinais vitais, sintomas, prescrições e links hipermídia.")]
        [ProducesResponseType(typeof(OperationResult<ProntuarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(string id)
        {
            var baseUrl = GetBaseUrl();
            var result = await _prontuarioService.ObterPorIdAsync(id, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Registra um novo prontuário clínico veterinário no MongoDB (NoSQL). Requer autenticação (Roles: Admin ou Veterinario).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Veterinario")]
        [SwaggerOperation(Summary = "Registra prontuário clínico NoSQL [Requer Admin/Veterinario]", Description = "Cria um novo documento de prontuário clínico semiestruturado no MongoDB.")]
        [ProducesResponseType(typeof(OperationResult<ProntuarioResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Criar([FromBody] ProntuarioRequestDto dto)
        {
            var baseUrl = GetBaseUrl();
            var result = await _prontuarioService.CriarAsync(dto, baseUrl);
            return HandleResult(result, "ObterProntuarioPorId", new { id = result.Data?.Id });
        }

        /// <summary>
        /// Atualiza os dados de um prontuário clínico NoSQL existente no MongoDB. Requer autenticação (Roles: Admin ou Veterinario).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Veterinario")]
        [SwaggerOperation(Summary = "Atualiza prontuário NoSQL existente [Requer Admin/Veterinario]", Description = "Atualiza anamnese, diagnóstico, sinais vitais e prescrições do documento.")]
        [ProducesResponseType(typeof(OperationResult<ProntuarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Atualizar(string id, [FromBody] ProntuarioRequestDto dto)
        {
            var baseUrl = GetBaseUrl();
            var result = await _prontuarioService.AtualizarAsync(id, dto, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Remove um prontuário clínico NoSQL do MongoDB. Requer perfil de Administrador.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Remove prontuário NoSQL pelo ID [Requer Admin]", Description = "Exclusão permanente de um prontuário da coleção NoSQL.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Remover(string id)
        {
            var result = await _prontuarioService.RemoverAsync(id);
            return HandleResult(result);
        }
    }
}

