using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClinicasController : ControllerBase
{
    private readonly PawCareContext _context;

    public ClinicasController(PawCareContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Clinica>>> GetAll()
    {
        var clinicas = await _context.Clinicas.ToListAsync();
        return Ok(clinicas);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Clinica>> GetById(long id)
    {
        var clinica = await _context.Clinicas.FindAsync(id);

        if (clinica == null)
            return NotFound("Clínica não encontrada.");

        return Ok(clinica);
    }

    [HttpGet("cidade/{cidade}")]
    public async Task<ActionResult<IEnumerable<Clinica>>> GetByCidade(string cidade)
    {
        var clinicas = await _context.Clinicas
            .Where(c => c.Cidade.ToLower() == cidade.ToLower())
            .ToListAsync();

        if (!clinicas.Any())
            return NotFound("Nenhuma clínica encontrada para esta cidade.");

        return Ok(clinicas);
    }

    [HttpGet("atendimento-24h")]
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

    [HttpPost]
    public async Task<ActionResult<Clinica>> Create(Clinica clinica)
    {
        _context.Clinicas.Add(clinica);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = clinica.Id }, clinica);
    }

    [HttpPut("{id:long}")]
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

    [HttpDelete("{id:long}")]
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