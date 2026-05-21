using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TratamentosController : ControllerBase
{
    private readonly PawCareContext _context;

    public TratamentosController(PawCareContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tratamento>>> GetAll()
    {
        var tratamentos = await _context.Tratamentos
            .Include(t => t.Pet)
            .ToListAsync();

        return Ok(tratamentos);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Tratamento>> GetById(long id)
    {
        var tratamento = await _context.Tratamentos
            .Include(t => t.Pet)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tratamento == null)
            return NotFound("Tratamento não encontrado.");

        return Ok(tratamento);
    }

    [HttpGet("pet/{idPet:long}")]
    public async Task<ActionResult<IEnumerable<Tratamento>>> GetByPet(long idPet)
    {
        var pet = await _context.Pets.FindAsync(idPet);

        if (pet == null)
            return NotFound("Pet não encontrado.");

        var tratamentos = await _context.Tratamentos
            .Where(t => t.IdPet == idPet)
            .ToListAsync();

        if (!tratamentos.Any())
            return NotFound("Nenhum tratamento encontrado para este pet.");

        return Ok(tratamentos);
    }

    [HttpPost]
    public async Task<ActionResult<Tratamento>> Create(Tratamento tratamento)
    {
        var pet = await _context.Pets.FindAsync(tratamento.IdPet);

        if (pet == null)
            return BadRequest("Pet informado não existe.");

        _context.Tratamentos.Add(tratamento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = tratamento.Id }, tratamento);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, Tratamento tratamento)
    {
        if (id != tratamento.Id)
            return BadRequest("O ID da URL não confere com o ID enviado.");

        var tratamentoExistente = await _context.Tratamentos.FindAsync(id);

        if (tratamentoExistente == null)
            return NotFound("Tratamento não encontrado.");

        var pet = await _context.Pets.FindAsync(tratamento.IdPet);

        if (pet == null)
            return BadRequest("Pet informado não existe.");

        tratamentoExistente.IdPet = tratamento.IdPet;
        tratamentoExistente.NomeMedicamento = tratamento.NomeMedicamento;
        tratamentoExistente.Frequencia = tratamento.Frequencia;
        tratamentoExistente.DataInicio = tratamento.DataInicio;
        tratamentoExistente.DataFinal = tratamento.DataFinal;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var tratamento = await _context.Tratamentos.FindAsync(id);

        if (tratamento == null)
            return NotFound("Tratamento não encontrado.");

        _context.Tratamentos.Remove(tratamento);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}