using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosEspecialistasController : ControllerBase
{
    private readonly PawCareContext _context;

    public MedicosEspecialistasController(PawCareContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicoEspecialista>>> GetAll()
    {
        var medicos = await _context.MedicosEspecialistas.ToListAsync();
        return Ok(medicos);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<MedicoEspecialista>> GetById(long id)
    {
        var medico = await _context.MedicosEspecialistas.FindAsync(id);

        if (medico == null)
            return NotFound("Médico especialista não encontrado.");

        return Ok(medico);
    }

    [HttpGet("especialidade/{especialidade}")]
    public async Task<ActionResult<IEnumerable<MedicoEspecialista>>> GetByEspecialidade(string especialidade)
    {
        var medicos = await _context.MedicosEspecialistas
            .Where(m => m.Especialidade.ToLower() == especialidade.ToLower())
            .ToListAsync();

        if (!medicos.Any())
            return NotFound("Nenhum médico especialista encontrado para esta especialidade.");

        return Ok(medicos);
    }

    [HttpPost]
    public async Task<ActionResult<MedicoEspecialista>> Create(MedicoEspecialista medico)
    {
        _context.MedicosEspecialistas.Add(medico);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = medico.Id }, medico);
    }

    [HttpPut("{id:long}")]
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

    [HttpDelete("{id:long}")]
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