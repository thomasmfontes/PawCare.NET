using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

/// <summary>
/// Endpoints para gerenciamento das clínicas veterinárias parceiras.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClinicasController : ControllerBase
{
    private readonly PawCareContext _context;

    public ClinicasController(PawCareContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todas as clínicas veterinárias cadastradas.
    /// </summary>
    /// <returns>Lista de clínicas cadastradas.</returns>
    /// <response code="200">Lista recuperada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Clinica>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Clinica>>> GetAll()
    {
        var clinicas = await _context.Clinicas.ToListAsync();
        return Ok(clinicas);
    }

    /// <summary>
    /// Busca uma clínica veterinária pelo seu identificador único (ID).
    /// </summary>
    /// <param name="id">ID numérico da clínica.</param>
    /// <returns>Dados da clínica correspondente.</returns>
    /// <response code="200">Clínica encontrada com sucesso.</response>
    /// <response code="404">Clínica não encontrada para o ID informado.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Clinica), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Clinica>> GetById(long id)
    {
        var clinica = await _context.Clinicas.FindAsync(id);

        if (clinica == null)
            return NotFound("Clínica não encontrada.");

        return Ok(clinica);
    }

    /// <summary>
    /// Filtra clínicas veterinárias localizadas em uma determinada cidade.
    /// </summary>
    /// <param name="cidade">Nome da cidade pesquisada.</param>
    /// <returns>Lista de clínicas situadas na cidade informada.</returns>
    /// <response code="200">Clínicas localizadas com sucesso.</response>
    /// <response code="404">Nenhuma clínica encontrada para a cidade especificada.</response>
    [HttpGet("cidade/{cidade}")]
    [ProducesResponseType(typeof(IEnumerable<Clinica>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Clinica>>> GetByCidade(string cidade)
    {
        var clinicas = await _context.Clinicas
            .Where(c => c.Cidade.ToLower() == cidade.ToLower())
            .ToListAsync();

        if (!clinicas.Any())
            return NotFound("Nenhuma clínica encontrada para esta cidade.");

        return Ok(clinicas);
    }

    /// <summary>
    /// Lista todas as clínicas que oferecem atendimento de emergência 24 horas.
    /// </summary>
    /// <returns>Coleção de clínicas com suporte 24h.</returns>
    /// <response code="200">Clínicas 24h encontradas com sucesso.</response>
    /// <response code="404">Nenhuma clínica com atendimento 24h encontrada.</response>
    [HttpGet("atendimento-24h")]
    [ProducesResponseType(typeof(IEnumerable<Clinica>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Clinica>>> GetAtendimento24h()
    {
        var clinicas = await _context.Clinicas.ToListAsync();

        var clinicas24h = clinicas
            .Where(c => c.Atendimento24h)
            .ToList();

        if (!clinicas24h.Any())
            return NotFound("Nenhuma clínica com atendimento 24h encontrada.");

        return Ok(clinicas24h);
    }

    /// <summary>
    /// Cadastra uma nova clínica veterinária no sistema.
    /// </summary>
    /// <param name="clinica">Dados da clínica a ser criada.</param>
    /// <returns>Clínica criada com seu ID gerado.</returns>
    /// <response code="201">Clínica cadastrada com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos no corpo da requisição.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Clinica), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Clinica>> Create(Clinica clinica)
    {
        _context.Clinicas.Add(clinica);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = clinica.Id }, clinica);
    }

    /// <summary>
    /// Atualiza os dados cadastrais de uma clínica existente.
    /// </summary>
    /// <param name="id">ID da clínica a ser atualizada.</param>
    /// <param name="clinica">Novos dados da clínica.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Clínica atualizada com sucesso.</response>
    /// <response code="400">ID da rota diverge do ID do corpo da requisição.</response>
    /// <response code="404">Clínica não encontrada no sistema.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, Clinica clinica)
    {
        if (id != clinica.Id)
            return BadRequest("O ID da URL não confere com o ID enviado.");

        var clinicaExistente = await _context.Clinicas.FindAsync(id);

        if (clinicaExistente == null)
            return NotFound("Clínica não encontrada.");

        clinicaExistente.NomeCnpj = clinica.NomeCnpj;
        clinicaExistente.Telefone = clinica.Telefone;
        clinicaExistente.Latitude = clinica.Latitude;
        clinicaExistente.Longitude = clinica.Longitude;
        clinicaExistente.Bairro = clinica.Bairro;
        clinicaExistente.Cidade = clinica.Cidade;
        clinicaExistente.Estado = clinica.Estado;
        clinicaExistente.Atendimento24h = clinica.Atendimento24h;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Exclui uma clínica veterinária (apenas se não houver eventos/consultas associados).
    /// </summary>
    /// <param name="id">ID da clínica a ser excluída.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Clínica excluída com sucesso.</response>
    /// <response code="400">A clínica não pode ser excluída pois possui eventos/consultas vinculados.</response>
    /// <response code="404">Clínica não encontrada.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        var clinica = await _context.Clinicas
            .Include(c => c.Eventos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (clinica == null)
            return NotFound("Clínica não encontrada.");

        if (clinica.Eventos.Any())
            return BadRequest("Não é possível remover uma clínica vinculada a eventos.");

        _context.Clinicas.Remove(clinica);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}