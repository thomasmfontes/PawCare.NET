using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HistoricosClinicosController : ControllerBase
{
    private readonly PawCareContext _context;

    public HistoricosClinicosController(PawCareContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HistoricoClinico>>> GetAll()
    {
        var historicos = await _context.HistoricosClinicos
            .Include(h => h.Evento)
            .ToListAsync();

        return Ok(historicos);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<HistoricoClinico>> GetById(long id)
    {
        var historico = await _context.HistoricosClinicos
            .Include(h => h.Evento)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (historico == null)
            return NotFound("Histórico clínico não encontrado.");

        return Ok(historico);
    }

    [HttpGet("evento/{idEvento:long}")]
    public async Task<ActionResult<HistoricoClinico>> GetByEvento(long idEvento)
    {
        var historico = await _context.HistoricosClinicos
            .Include(h => h.Evento)
            .FirstOrDefaultAsync(h => h.IdEvento == idEvento);

        if (historico == null)
            return NotFound("Histórico clínico não encontrado para este evento.");

        return Ok(historico);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<HistoricoClinico>>> GetByStatus(string status)
    {
        var historicos = await _context.HistoricosClinicos
            .Where(h => h.Status.ToLower() == status.ToLower())
            .ToListAsync();

        if (!historicos.Any())
            return NotFound("Nenhum histórico clínico encontrado para este status.");

        return Ok(historicos);
    }

    [HttpPost]
    public async Task<ActionResult<HistoricoClinico>> Create(HistoricoClinico historico)
    {
        var evento = await _context.Eventos.FindAsync(historico.IdEvento);

        if (evento == null)
            return BadRequest("Evento informado não existe.");

        var historicoExistente = await _context.HistoricosClinicos
            .FirstOrDefaultAsync(h => h.IdEvento == historico.IdEvento);

        if (historicoExistente != null)
            return BadRequest("Este evento já possui histórico clínico.");

        _context.HistoricosClinicos.Add(historico);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = historico.Id }, historico);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, HistoricoClinico historico)
    {
        if (id != historico.Id)
            return BadRequest("O ID da URL não confere com o ID enviado.");

        var historicoExistente = await _context.HistoricosClinicos.FindAsync(id);

        if (historicoExistente == null)
            return NotFound("Histórico clínico não encontrado.");

        var evento = await _context.Eventos.FindAsync(historico.IdEvento);

        if (evento == null)
            return BadRequest("Evento informado não existe.");

        historicoExistente.IdEvento = historico.IdEvento;
        historicoExistente.DataEvento = historico.DataEvento;
        historicoExistente.DataVencimento = historico.DataVencimento;
        historicoExistente.Status = historico.Status;
        historicoExistente.ObservacoesIa = historico.ObservacoesIa;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var historico = await _context.HistoricosClinicos.FindAsync(id);

        if (historico == null)
            return NotFound("Histórico clínico não encontrado.");

        _context.HistoricosClinicos.Remove(historico);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}