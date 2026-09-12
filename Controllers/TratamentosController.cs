using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

/// <summary>
/// Endpoints para gerenciamento dos tratamentos e prescrições medicamentosas dos pets.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TratamentosController : ControllerBase
{
    private readonly PawCareContext _context;

    public TratamentosController(PawCareContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos os tratamentos cadastrados com os dados do pet associado.
    /// </summary>
    /// <returns>Coleção de tratamentos cadastrados.</returns>
    /// <response code="200">Lista obtida com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Tratamento>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Tratamento>>> GetAll()
    {
        var tratamentos = await _context.Tratamentos
            .Include(t => t.Pet)
            .ToListAsync();

        return Ok(tratamentos);
    }

    /// <summary>
    /// Busca os dados de um tratamento específico através de seu ID.
    /// </summary>
    /// <param name="id">ID numérico do tratamento.</param>
    /// <returns>Dados do tratamento localizado.</returns>
    /// <response code="200">Tratamento localizado com sucesso.</response>
    /// <response code="404">Tratamento não encontrado.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Tratamento), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Tratamento>> GetById(long id)
    {
        var tratamento = await _context.Tratamentos
            .Include(t => t.Pet)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tratamento == null)
            return NotFound("Tratamento não encontrado.");

        return Ok(tratamento);
    }

    /// <summary>
    /// Lista todos os tratamentos prescritos para um pet em particular.
    /// </summary>
    /// <param name="idPet">ID numérico do pet.</param>
    /// <returns>Lista de tratamentos do pet.</returns>
    /// <response code="200">Tratamentos do pet obtidos com sucesso.</response>
    /// <response code="404">Pet não encontrado ou nenhum tratamento vinculado ao pet.</response>
    [HttpGet("pet/{idPet:long}")]
    [ProducesResponseType(typeof(IEnumerable<Tratamento>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Cadastra um novo tratamento ou prescrição medicamentosa para um pet.
    /// </summary>
    /// <param name="tratamento">Dados do tratamento a ser cadastrado.</param>
    /// <returns>Tratamento criado com ID gerado.</returns>
    /// <response code="201">Tratamento criado com sucesso.</response>
    /// <response code="400">Pet informado não existe ou dados inválidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Tratamento), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Tratamento>> Create(Tratamento tratamento)
    {
        var pet = await _context.Pets.FindAsync(tratamento.IdPet);

        if (pet == null)
            return BadRequest("Pet informado não existe.");

        _context.Tratamentos.Add(tratamento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = tratamento.Id }, tratamento);
    }

    /// <summary>
    /// Atualiza as informações de dosagem, medicamento ou datas de um tratamento existente.
    /// </summary>
    /// <param name="id">ID do tratamento na URL.</param>
    /// <param name="tratamento">Novos dados do tratamento.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Tratamento atualizado com sucesso.</response>
    /// <response code="400">Divergência de IDs ou pet informado inexistente.</response>
    /// <response code="404">Tratamento não encontrado.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Remove um registro de tratamento do sistema.
    /// </summary>
    /// <param name="id">ID do tratamento a ser excluído.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Tratamento excluído com sucesso.</response>
    /// <response code="404">Tratamento não encontrado.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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