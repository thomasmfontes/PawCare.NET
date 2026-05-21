using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RacasController : ControllerBase
{
    private readonly PawCareContext _context;

    public RacasController(PawCareContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Raca>>> GetAll()
    {
        var racas = await _context.Racas.ToListAsync();
        return Ok(racas);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Raca>> GetById(long id)
    {
        var raca = await _context.Racas.FindAsync(id);

        if (raca == null)
            return NotFound("Raça não encontrada.");

        return Ok(raca);
    }

    [HttpGet("especie/{especie}")]
    public async Task<ActionResult<IEnumerable<Raca>>> GetByEspecie(string especie)
    {
        var racas = await _context.Racas
            .Where(r => r.Especie.ToLower() == especie.ToLower())
            .ToListAsync();

        if (!racas.Any())
            return NotFound("Nenhuma raça encontrada para esta espécie.");

        return Ok(racas);
    }

    [HttpPost]
    public async Task<ActionResult<Raca>> Create(Raca raca)
    {
        if (string.IsNullOrWhiteSpace(raca.Nome))
            return BadRequest("O nome da raça é obrigatório.");

        if (string.IsNullOrWhiteSpace(raca.Especie))
            return BadRequest("A espécie da raça é obrigatória.");

        _context.Racas.Add(raca);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = raca.Id }, raca);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, Raca raca)
    {
        if (id != raca.Id)
            return BadRequest("O ID da URL não confere com o ID enviado.");

        if (string.IsNullOrWhiteSpace(raca.Nome))
            return BadRequest("O nome da raça é obrigatório.");

        if (string.IsNullOrWhiteSpace(raca.Especie))
            return BadRequest("A espécie da raça é obrigatória.");

        var racaExistente = await _context.Racas.FindAsync(id);

        if (racaExistente == null)
            return NotFound("Raça não encontrada.");

        racaExistente.Nome = raca.Nome;
        racaExistente.Especie = raca.Especie;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var raca = await _context.Racas
            .Include(r => r.Pets)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (raca == null)
            return NotFound("Raça não encontrada.");

        if (raca.Pets.Any())
            return BadRequest("Não é possível remover uma raça vinculada a pets.");

        _context.Racas.Remove(raca);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}