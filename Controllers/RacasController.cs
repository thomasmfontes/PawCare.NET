using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

/// <summary>
/// Endpoints para gerenciamento das raças e espécies dos animais (canina, felina, etc.).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RacasController : ControllerBase
{
    private readonly PawCareContext _context;

    public RacasController(PawCareContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todas as raças cadastradas no sistema.
    /// </summary>
    /// <returns>Coleção de raças cadastradas.</returns>
    /// <response code="200">Lista obtida com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Raca>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Raca>>> GetAll()
    {
        var racas = await _context.Racas.ToListAsync();
        return Ok(racas);
    }

    /// <summary>
    /// Recupera os dados de uma raça através de seu ID.
    /// </summary>
    /// <param name="id">Identificador numérico da raça.</param>
    /// <returns>Dados da raça encontrada.</returns>
    /// <response code="200">Raça localizada com sucesso.</response>
    /// <response code="404">Raça não encontrada.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Raca), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Raca>> GetById(long id)
    {
        var raca = await _context.Racas.FindAsync(id);

        if (raca == null)
            return NotFound("Raça não encontrada.");

        return Ok(raca);
    }

    /// <summary>
    /// Filtra raças pertencentes a uma espécie específica (ex: Cachorro, Gato, Ave).
    /// </summary>
    /// <param name="especie">Nome da espécie a ser filtrada.</param>
    /// <returns>Lista de raças pertencentes à espécie.</returns>
    /// <response code="200">Raças localizadas com sucesso.</response>
    /// <response code="404">Nenhuma raça encontrada para a espécie informada.</response>
    [HttpGet("especie/{especie}")]
    [ProducesResponseType(typeof(IEnumerable<Raca>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Raca>>> GetByEspecie(string especie)
    {
        var racas = await _context.Racas
            .Where(r => r.Especie.ToLower() == especie.ToLower())
            .ToListAsync();

        if (!racas.Any())
            return NotFound("Nenhuma raça encontrada para esta espécie.");

        return Ok(racas);
    }

    /// <summary>
    /// Cadastra uma nova raça no sistema.
    /// </summary>
    /// <param name="raca">Dados da raça (Nome e Espécie são obrigatórios).</param>
    /// <returns>Raça criada com ID gerado.</returns>
    /// <response code="201">Raça cadastrada com sucesso.</response>
    /// <response code="400">Nome ou espécie ausentes ou em branco.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Raca), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Atualiza os dados de uma raça existente.
    /// </summary>
    /// <param name="id">ID da raça na URL.</param>
    /// <param name="raca">Novos dados da raça.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Raça atualizada com sucesso.</response>
    /// <response code="400">Divergência de IDs ou validação de campos obrigatórios falhou.</response>
    /// <response code="404">Raça não encontrada.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Exclui uma raça cadastrada (apenas se não houver pets associados a ela).
    /// </summary>
    /// <param name="id">ID da raça a ser excluída.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Raça excluída com sucesso.</response>
    /// <response code="400">Não é possível excluir uma raça com pets associados.</response>
    /// <response code="404">Raça não encontrada.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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