using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

/// <summary>
/// Endpoints para gerenciamento dos eventos e atendimentos clínicos (consultas, cirurgias, exames, vacinas).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EventosController : ControllerBase
{
    private readonly PawCareContext _context;

    public EventosController(PawCareContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos os eventos clínicos cadastrados com suas respectivas relações (Pet, Tutor, Médico, Clínica e Histórico).
    /// </summary>
    /// <returns>Lista completa de eventos clínicos.</returns>
    /// <response code="200">Eventos retornados com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Evento>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Evento>>> GetAll()
    {
        var eventos = await _context.Eventos
            .Include(e => e.Pet)
            .Include(e => e.Tutor)
            .Include(e => e.Medico)
            .Include(e => e.Clinica)
            .Include(e => e.HistoricoClinico)
            .ToListAsync();

        return Ok(eventos);
    }

    /// <summary>
    /// Busca um evento clínico detalhado pelo seu identificador (ID).
    /// </summary>
    /// <param name="id">ID numérico do evento.</param>
    /// <returns>Dados do evento clínico encontrado.</returns>
    /// <response code="200">Evento encontrado com sucesso.</response>
    /// <response code="404">Evento não localizado.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Evento), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Evento>> GetById(long id)
    {
        var evento = await _context.Eventos
            .Include(e => e.Pet)
            .Include(e => e.Tutor)
            .Include(e => e.Medico)
            .Include(e => e.Clinica)
            .Include(e => e.HistoricoClinico)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evento == null)
            return NotFound("Evento não encontrado.");

        return Ok(evento);
    }

    /// <summary>
    /// Lista todos os eventos clínicos associados a um determinado pet.
    /// </summary>
    /// <param name="idPet">ID numérico do pet.</param>
    /// <returns>Lista de eventos clínicos do pet.</returns>
    /// <response code="200">Eventos do pet retornados com sucesso.</response>
    /// <response code="404">Pet não encontrado ou nenhum evento registrado para o pet.</response>
    [HttpGet("pet/{idPet:long}")]
    [ProducesResponseType(typeof(IEnumerable<Evento>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Evento>>> GetByPet(long idPet)
    {
        var pet = await _context.Pets.FindAsync(idPet);

        if (pet == null)
            return NotFound("Pet não encontrado.");

        var eventos = await _context.Eventos
            .Include(e => e.Medico)
            .Include(e => e.Clinica)
            .Where(e => e.IdPet == idPet)
            .ToListAsync();

        if (!eventos.Any())
            return NotFound("Nenhum evento encontrado para este pet.");

        return Ok(eventos);
    }

    /// <summary>
    /// Lista todos os eventos clínicos associados ao CPF de um tutor.
    /// </summary>
    /// <param name="cpf">CPF do tutor.</param>
    /// <returns>Lista de eventos clínicos associados ao tutor.</returns>
    /// <response code="200">Eventos do tutor retornados com sucesso.</response>
    /// <response code="404">Tutor não encontrado ou nenhum evento vinculado ao tutor.</response>
    [HttpGet("tutor/{cpf}")]
    [ProducesResponseType(typeof(IEnumerable<Evento>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Evento>>> GetByTutor(string cpf)
    {
        var tutor = await _context.Tutores.FindAsync(cpf);

        if (tutor == null)
            return NotFound("Tutor não encontrado.");

        var eventos = await _context.Eventos
            .Include(e => e.Pet)
            .Include(e => e.Medico)
            .Include(e => e.Clinica)
            .Where(e => e.IdTutor == cpf)
            .ToListAsync();

        if (!eventos.Any())
            return NotFound("Nenhum evento encontrado para este tutor.");

        return Ok(eventos);
    }

    /// <summary>
    /// Filtra eventos clínicos pelo seu tipo (ex: Consulta, Vacinação, Cirurgia).
    /// </summary>
    /// <param name="tipo">Tipo do evento clínico.</param>
    /// <returns>Coleção de eventos do tipo especificado.</returns>
    /// <response code="200">Eventos filtrados com sucesso.</response>
    /// <response code="404">Nenhum evento encontrado para o tipo informado.</response>
    [HttpGet("tipo/{tipo}")]
    [ProducesResponseType(typeof(IEnumerable<Evento>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Evento>>> GetByTipo(string tipo)
    {
        var eventos = await _context.Eventos
            .Include(e => e.Pet)
            .Include(e => e.Medico)
            .Include(e => e.Clinica)
            .Where(e => e.Tipo.ToLower() == tipo.ToLower())
            .ToListAsync();

        if (!eventos.Any())
            return NotFound("Nenhum evento encontrado para este tipo.");

        return Ok(eventos);
    }

    /// <summary>
    /// Registra um novo evento clínico, validando existência do Pet, Tutor, Médico e Clínica.
    /// </summary>
    /// <param name="evento">Dados do evento a ser cadastrado.</param>
    /// <returns>Evento registrado com identificador gerado.</returns>
    /// <response code="201">Evento registrado com sucesso.</response>
    /// <response code="400">Entidades relacionadas inexistentes ou incoerência entre o pet e tutor informado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Evento), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Evento>> Create(Evento evento)
    {
        var pet = await _context.Pets.FindAsync(evento.IdPet);

        if (pet == null)
            return BadRequest("Pet informado não existe.");

        var tutor = await _context.Tutores.FindAsync(evento.IdTutor);

        if (tutor == null)
            return BadRequest("Tutor informado não existe.");

        var medico = await _context.MedicosEspecialistas.FindAsync(evento.IdMedico);

        if (medico == null)
            return BadRequest("Médico informado não existe.");

        var clinica = await _context.Clinicas.FindAsync(evento.IdClinica);

        if (clinica == null)
            return BadRequest("Clínica informada não existe.");

        if (pet.TutorCpf != evento.IdTutor)
            return BadRequest("O pet informado não pertence ao tutor informado.");

        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = evento.Id }, evento);
    }

    /// <summary>
    /// Atualiza as informações de um evento clínico existente.
    /// </summary>
    /// <param name="id">ID do evento na URL.</param>
    /// <param name="evento">Dados atualizados do evento.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Evento atualizado com sucesso.</response>
    /// <response code="400">Divergência de IDs ou entidades relacionadas inválidas.</response>
    /// <response code="404">Evento não encontrado.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, Evento evento)
    {
        if (id != evento.Id)
            return BadRequest("O ID da URL não confere com o ID enviado.");

        var eventoExistente = await _context.Eventos.FindAsync(id);

        if (eventoExistente == null)
            return NotFound("Evento não encontrado.");

        var pet = await _context.Pets.FindAsync(evento.IdPet);

        if (pet == null)
            return BadRequest("Pet informado não existe.");

        var tutor = await _context.Tutores.FindAsync(evento.IdTutor);

        if (tutor == null)
            return BadRequest("Tutor informado não existe.");

        var medico = await _context.MedicosEspecialistas.FindAsync(evento.IdMedico);

        if (medico == null)
            return BadRequest("Médico informado não existe.");

        var clinica = await _context.Clinicas.FindAsync(evento.IdClinica);

        if (clinica == null)
            return BadRequest("Clínica informada não existe.");

        if (pet.TutorCpf != evento.IdTutor)
            return BadRequest("O pet informado não pertence ao tutor informado.");

        eventoExistente.Tipo = evento.Tipo;
        eventoExistente.IdPet = evento.IdPet;
        eventoExistente.IdTutor = evento.IdTutor;
        eventoExistente.IdMedico = evento.IdMedico;
        eventoExistente.IdClinica = evento.IdClinica;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Exclui um evento clínico (apenas se não houver histórico clínico vinculado).
    /// </summary>
    /// <param name="id">ID do evento clínico.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Evento excluído com sucesso.</response>
    /// <response code="400">Não é permitido excluir um evento que já possui histórico clínico registrado.</response>
    /// <response code="404">Evento não encontrado.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        var evento = await _context.Eventos
            .Include(e => e.HistoricoClinico)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evento == null)
            return NotFound("Evento não encontrado.");

        if (evento.HistoricoClinico != null)
            return BadRequest("Não é possível remover um evento que possui histórico clínico.");

        _context.Eventos.Remove(evento);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}