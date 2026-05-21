using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly PawCareContext _context;

    public EventosController(PawCareContext context)
    {
        _context = context;
    }

    [HttpGet]
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

    [HttpGet("{id:long}")]
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

    [HttpGet("pet/{idPet:long}")]
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

    [HttpGet("tutor/{cpf}")]
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

    [HttpGet("tipo/{tipo}")]
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

    [HttpPost]
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

    [HttpPut("{id:long}")]
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

    [HttpDelete("{id:long}")]
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