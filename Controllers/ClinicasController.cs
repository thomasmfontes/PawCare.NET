using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawCareApi.Application.DTOs.Clinicas;
using PawCareApi.Application.DTOs.Common;
using PawCareApi.Application.Interfaces;
using PawCareApi.Domain.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace PawCareApi.Controllers
{
    [Route("api/[controller]")]
    public class ClinicasController : ApiControllerBase
    {
        private readonly IClinicaService _clinicaService;

        public ClinicasController(IClinicaService clinicaService)
        {
            _clinicaService = clinicaService;
        }

        /// <summary>
        /// Obtém a listagem paginada de clínicas veterinárias com filtros e HATEOAS.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lista clínicas paginadas com filtros e HATEOAS", Description = "Retorna lista de clínicas veterinárias paginada utilizando PageResult.")]
        [ProducesResponseType(typeof(OperationResult<PageResult<ClinicaResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPaginado(
            [FromQuery] PagedRequestDto request,
            [FromQuery] string? cidade = null,
            [FromQuery] bool? atendimento24h = null)
        {
            var baseUrl = GetBaseUrl();
            var result = await _clinicaService.ObterPaginadoAsync(request, cidade, atendimento24h, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtém todas as clínicas veterinárias cadastradas.
        /// </summary>
        [HttpGet("todas")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lista todas as clínicas", Description = "Retorna a coleção de clínicas veterinárias com links hipermídia.")]
        [ProducesResponseType(typeof(OperationResult<IEnumerable<ClinicaResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodas()
        {
            var baseUrl = GetBaseUrl();
            var result = await _clinicaService.ObterTodasAsync(baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Obtém uma clínica específica pelo ID com HATEOAS.
        /// </summary>
        [HttpGet("{id:long}", Name = "ObterClinicaPorId")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Obtém clínica por ID com HATEOAS", Description = "Retorna detalhes da clínica e seus hiperlinks.")]
        [ProducesResponseType(typeof(OperationResult<ClinicaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(long id)
        {
            var baseUrl = GetBaseUrl();
            var result = await _clinicaService.ObterPorIdAsync(id, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Cadastra uma nova clínica veterinária. Requer perfil de Administrador.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Cadastra nova clínica [Requer Admin]", Description = "Criação de clínica com validação de dados geográficos e de contato.")]
        [ProducesResponseType(typeof(OperationResult<ClinicaResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Criar([FromBody] ClinicaRequestDto dto)
        {
            var baseUrl = GetBaseUrl();
            var result = await _clinicaService.CriarAsync(dto, baseUrl);
            return HandleResult(result, "ObterClinicaPorId", new { id = result.Data?.Id });
        }

        /// <summary>
        /// Atualiza os dados de uma clínica existente. Requer perfil de Administrador.
        /// </summary>
        [HttpPut("{id:long}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Atualiza clínica existente [Requer Admin]", Description = "Atualização cadastral de clínica veterinária.")]
        [ProducesResponseType(typeof(OperationResult<ClinicaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Atualizar(long id, [FromBody] ClinicaRequestDto dto)
        {
            var baseUrl = GetBaseUrl();
            var result = await _clinicaService.AtualizarAsync(id, dto, baseUrl);
            return HandleResult(result);
        }

        /// <summary>
        /// Remove uma clínica pelo ID. Requer perfil de Administrador.
        /// </summary>
        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Remove uma clínica pelo ID [Requer Admin]", Description = "Exclusão de clínica veterinária do sistema.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Remover(long id)
        {
            var result = await _clinicaService.RemoverAsync(id);
            return HandleResult(result);
        }
    }
}
