using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

/// <summary>
/// Endpoints para gerenciamento dos prontuários e históricos clínicos dos pets com suporte a observações de IA.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HistoricosClinicosController : ControllerBase
{
    private readonly PawCareContext _context;

    public HistoricosClinicosController(PawCareContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos os históricos clínicos registrados no sistema.
    /// </summary>
    /// <returns>Lista completa de históricos clínicos acompanhados do respectivo evento.</returns>
    /// <response code="200">Históricos clínicos obtidos com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HistoricoClinico>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HistoricoClinico>>> GetAll()
    {
        var historicos = await _context.HistoricosClinicos
            .Include(h => h.Evento)
            .ToListAsync();

        return Ok(historicos);
    }

    /// <summary>
    /// Recupera um histórico clínico específico a partir de seu ID.
    /// </summary>
    /// <param name="id">ID numérico do histórico clínico.</param>
    /// <returns>Dados detalhados do histórico clínico.</returns>
    /// <response code="200">Histórico localizado com sucesso.</response>
    /// <response code="404">Histórico não encontrado.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(HistoricoClinico), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HistoricoClinico>> GetById(long id)
    {
        var historico = await _context.HistoricosClinicos
            .Include(h => h.Evento)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (historico == null)
            return NotFound("Histórico clínico não encontrado.");

        return Ok(historico);
    }

    /// <summary>
    /// Busca o histórico clínico vinculado a um evento de atendimento específico.
    /// </summary>
    /// <param name="idEvento">ID numérico do evento.</param>
    /// <returns>Histórico clínico do evento correspondente.</returns>
    /// <response code="200">Histórico clínico do evento obtido com sucesso.</response>
    /// <response code="404">Nenhum histórico encontrado para o evento informado.</response>
    [HttpGet("evento/{idEvento:long}")]
    [ProducesResponseType(typeof(HistoricoClinico), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HistoricoClinico>> GetByEvento(long idEvento)
    {
        var historico = await _context.HistoricosClinicos
            .Include(h => h.Evento)
            .FirstOrDefaultAsync(h => h.IdEvento == idEvento);

        if (historico == null)
            return NotFound("Histórico clínico não encontrado para este evento.");

        return Ok(historico);
    }

    /// <summary>
    /// Filtra históricos clínicos pelo status do procedimento ou retorno (ex: Pendente, Concluído).
    /// </summary>
    /// <param name="status">Descrição do status.</param>
    /// <returns>Coleção de históricos com o status solicitado.</returns>
    /// <response code="200">Históricos filtrados com sucesso.</response>
    /// <response code="404">Nenhum histórico encontrado para este status.</response>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<HistoricoClinico>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<HistoricoClinico>>> GetByStatus(string status)
    {
        var historicos = await _context.HistoricosClinicos
            .Where(h => h.Status.ToLower() == status.ToLower())
            .ToListAsync();

        if (!historicos.Any())
            return NotFound("Nenhum histórico clínico encontrado para este status.");

        return Ok(historicos);
    }

    /// <summary>
    /// Cadastra um novo registro de histórico clínico para um evento.
    /// </summary>
    /// <param name="historico">Dados do histórico clínico a ser criado.</param>
    /// <returns>Histórico clínico criado com seu identificador.</returns>
    /// <response code="201">Histórico clínico criado com sucesso.</response>
    /// <response code="400">Evento inexistente ou o evento já possui histórico cadastrado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(HistoricoClinico), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Atualiza um histórico clínico existente.
    /// </summary>
    /// <param name="id">ID do histórico clínico na URL.</param>
    /// <param name="historico">Dados atualizados do histórico.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Histórico clínico atualizado com sucesso.</response>
    /// <response code="400">IDs divergentes ou evento informado inexistente.</response>
    /// <response code="404">Histórico clínico não encontrado.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Exclui um registro de histórico clínico da base de dados.
    /// </summary>
    /// <param name="id">ID do histórico a ser removido.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Histórico excluído com sucesso.</response>
    /// <response code="404">Histórico não encontrado.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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