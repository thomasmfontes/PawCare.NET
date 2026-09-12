using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

/// <summary>
/// Endpoints para gerenciamento do corpo clínico de médicos veterinários especialistas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MedicosEspecialistasController : ControllerBase
{
    private readonly PawCareContext _context;

    public MedicosEspecialistasController(PawCareContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos os médicos especialistas cadastrados na rede de atendimento.
    /// </summary>
    /// <returns>Coleção de médicos veterinários especialistas.</returns>
    /// <response code="200">Lista obtida com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MedicoEspecialista>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MedicoEspecialista>>> GetAll()
    {
        var medicos = await _context.MedicosEspecialistas.ToListAsync();
        return Ok(medicos);
    }

    /// <summary>
    /// Busca um médico especialista através de seu ID identificador.
    /// </summary>
    /// <param name="id">ID numérico do médico especialista.</param>
    /// <returns>Dados do médico especialista.</returns>
    /// <response code="200">Médico especialista localizado com sucesso.</response>
    /// <response code="404">Médico especialista não encontrado.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(MedicoEspecialista), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicoEspecialista>> GetById(long id)
    {
        var medico = await _context.MedicosEspecialistas.FindAsync(id);

        if (medico == null)
            return NotFound("Médico especialista não encontrado.");

        return Ok(medico);
    }

    /// <summary>
    /// Filtra médicos especialistas por área de atuação ou especialidade (ex: Dermatologia, Cardiologia, Cirurgia).
    /// </summary>
    /// <param name="especialidade">Nome da especialidade médica veterinária.</param>
    /// <returns>Lista de médicos especialistas cadastrados na especialidade.</returns>
    /// <response code="200">Médicos localizados com sucesso.</response>
    /// <response code="404">Nenhum médico encontrado para a especialidade solicitada.</response>
    [HttpGet("especialidade/{especialidade}")]
    [ProducesResponseType(typeof(IEnumerable<MedicoEspecialista>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MedicoEspecialista>>> GetByEspecialidade(string especialidade)
    {
        var medicos = await _context.MedicosEspecialistas
            .Where(m => m.Especialidade.ToLower() == especialidade.ToLower())
            .ToListAsync();

        if (!medicos.Any())
            return NotFound("Nenhum médico especialista encontrado para esta especialidade.");

        return Ok(medicos);
    }

    /// <summary>
    /// Cadastra um novo médico especialista no sistema.
    /// </summary>
    /// <param name="medico">Dados do médico especialista a ser cadastrado.</param>
    /// <returns>Médico cadastrado com ID gerado.</returns>
    /// <response code="201">Médico cadastrado com sucesso.</response>
    /// <response code="400">Dados inválidos informados.</response>
    [HttpPost]
    [ProducesResponseType(typeof(MedicoEspecialista), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MedicoEspecialista>> Create(MedicoEspecialista medico)
    {
        _context.MedicosEspecialistas.Add(medico);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = medico.Id }, medico);
    }

    /// <summary>
    /// Atualiza os dados de um médico especialista existente.
    /// </summary>
    /// <param name="id">ID do médico especialista na URL.</param>
    /// <param name="medico">Novos dados do médico especialista.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Médico atualizado com sucesso.</response>
    /// <response code="400">ID da URL diverge do ID do corpo da requisição.</response>
    /// <response code="404">Médico especialista não encontrado.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, MedicoEspecialista medico)
    {
        if (id != medico.Id)
            return BadRequest("O ID da URL não confere com o ID enviado.");

        var medicoExistente = await _context.MedicosEspecialistas.FindAsync(id);

        if (medicoExistente == null)
            return NotFound("Médico especialista não encontrado.");

        medicoExistente.Nome = medico.Nome;
        medicoExistente.Especialidade = medico.Especialidade;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Exclui um médico especialista (apenas se não possuir eventos/atendimentos vinculados).
    /// </summary>
    /// <param name="id">ID do médico especialista a ser excluído.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Médico excluído com sucesso.</response>
    /// <response code="400">Médico possui eventos ou atendimentos registrados e não pode ser excluído.</response>
    /// <response code="404">Médico especialista não encontrado.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        var medico = await _context.MedicosEspecialistas
            .Include(m => m.Eventos)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (medico == null)
            return NotFound("Médico especialista não encontrado.");

        if (medico.Eventos.Any())
            return BadRequest("Não é possível remover um médico vinculado a eventos.");

        _context.MedicosEspecialistas.Remove(medico);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}