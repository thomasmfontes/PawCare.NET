using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

/// <summary>
/// Endpoints para gerenciamento dos pets cadastrados no sistema PawCare.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PetsController : ControllerBase
{
    private readonly PawCareContext _context;

    public PetsController(PawCareContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Recupera a lista completa de todos os pets cadastrados.
    /// </summary>
    /// <remarks>
    /// Retorna todos os pets incluindo os dados vinculados do Tutor e da Raça.
    /// </remarks>
    /// <returns>Coleção de pets com seus tutores e raças.</returns>
    /// <response code="200">Lista de pets obtida com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Pet>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Pet>>> GetAll()
    {
        var pets = await _context.Pets
            .Include(p => p.Tutor)
            .Include(p => p.Raca)
            .ToListAsync();

        return Ok(pets);
    }

    /// <summary>
    /// Recupera os dados detalhados de um pet específico a partir de seu ID.
    /// </summary>
    /// <param name="id">Identificador numérico do pet.</param>
    /// <returns>Dados do pet encontrado.</returns>
    /// <response code="200">Pet localizado com sucesso.</response>
    /// <response code="404">Nenhum pet encontrado para o ID fornecido.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Pet), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Pet>> GetById(long id)
    {
        var pet = await _context.Pets
            .Include(p => p.Tutor)
            .Include(p => p.Raca)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pet == null)
            return NotFound("Pet não encontrado.");

        return Ok(pet);
    }

    /// <summary>
    /// Lista todos os pets associados ao CPF de um tutor específico.
    /// </summary>
    /// <param name="cpf">CPF do tutor (somente números ou formato padrão).</param>
    /// <returns>Lista de pets pertencentes ao tutor.</returns>
    /// <response code="200">Pets do tutor encontrados com sucesso.</response>
    /// <response code="404">Tutor não encontrado ou tutor sem pets cadastrados.</response>
    [HttpGet("tutor/{cpf}")]
    [ProducesResponseType(typeof(IEnumerable<Pet>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByTutor(string cpf)
    {
        var tutor = await _context.Tutores.FindAsync(cpf);

        if (tutor == null)
            return NotFound("Tutor não encontrado.");

        var pets = await _context.Pets
            .Include(p => p.Raca)
            .Where(p => p.TutorCpf == cpf)
            .ToListAsync();

        if (!pets.Any())
            return NotFound("Nenhum pet encontrado para este tutor.");

        return Ok(pets);
    }

    /// <summary>
    /// Lista todos os pets cadastrados de uma determinada raça.
    /// </summary>
    /// <param name="racaId">Identificador numérico da raça.</param>
    /// <returns>Lista de pets pertencentes à raça especificada.</returns>
    /// <response code="200">Pets da raça encontrados com sucesso.</response>
    /// <response code="404">Raça não encontrada ou nenhum pet cadastrado para a raça.</response>
    [HttpGet("raca/{racaId:long}")]
    [ProducesResponseType(typeof(IEnumerable<Pet>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByRaca(long racaId)
    {
        var raca = await _context.Racas.FindAsync(racaId);

        if (raca == null)
            return NotFound("Raça não encontrada.");

        var pets = await _context.Pets
            .Include(p => p.Tutor)
            .Where(p => p.RacaId == racaId)
            .ToListAsync();

        if (!pets.Any())
            return NotFound("Nenhum pet encontrado para esta raça.");

        return Ok(pets);
    }

    /// <summary>
    /// Filtra pets com base no status de longevidade (ex: Filhote, Adulto, Idoso).
    /// </summary>
    /// <param name="status">Descrição do status de longevidade.</param>
    /// <returns>Coleção de pets com o status indicado.</returns>
    /// <response code="200">Pets filtrados com sucesso.</response>
    /// <response code="404">Nenhum pet localizado com o status informado.</response>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<Pet>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByStatusLongevidade(string status)
    {
        var pets = await _context.Pets
            .Include(p => p.Tutor)
            .Include(p => p.Raca)
            .Where(p => p.StatusLongevidade.ToLower() == status.ToLower())
            .ToListAsync();

        if (!pets.Any())
            return NotFound("Nenhum pet encontrado para este status de longevidade.");

        return Ok(pets);
    }

    /// <summary>
    /// Cadastra um novo pet no sistema e atualiza a quantidade de pets do tutor.
    /// </summary>
    /// <param name="pet">Objeto com os dados do pet a ser cadastrado.</param>
    /// <returns>Pet cadastrado com seu ID gerado.</returns>
    /// <response code="201">Pet criado com sucesso.</response>
    /// <response code="400">Dados inválidos, tutor informado inexistente ou raça inexistente.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Pet), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Pet>> Create(Pet pet)
    {
        var tutor = await _context.Tutores.FindAsync(pet.TutorCpf);

        if (tutor == null)
            return BadRequest("Tutor informado não existe.");

        var raca = await _context.Racas.FindAsync(pet.RacaId);

        if (raca == null)
            return BadRequest("Raça informada não existe.");

        _context.Pets.Add(pet);

        tutor.QtdPets += 1;

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
    }

    /// <summary>
    /// Atualiza os dados de um pet existente.
    /// </summary>
    /// <param name="id">ID do pet na URL.</param>
    /// <param name="pet">Dados atualizados do pet.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Pet atualizado com sucesso.</response>
    /// <response code="400">Inconsistência entre o ID da URL e o ID do corpo, ou dependência inválida.</response>
    /// <response code="404">Pet não encontrado no banco de dados.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, Pet pet)
    {
        if (id != pet.Id)
            return BadRequest("O ID da URL não confere com o ID enviado.");

        var petExistente = await _context.Pets.FindAsync(id);

        if (petExistente == null)
            return NotFound("Pet não encontrado.");

        var tutor = await _context.Tutores.FindAsync(pet.TutorCpf);

        if (tutor == null)
            return BadRequest("Tutor informado não existe.");

        var raca = await _context.Racas.FindAsync(pet.RacaId);

        if (raca == null)
            return BadRequest("Raça informada não existe.");

        petExistente.Nome = pet.Nome;
        petExistente.DataNascimento = pet.DataNascimento;
        petExistente.Peso = pet.Peso;
        petExistente.StatusLongevidade = pet.StatusLongevidade;
        petExistente.RacaId = pet.RacaId;
        petExistente.TutorCpf = pet.TutorCpf;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Remove um pet do sistema e decrementa a contagem de pets do tutor associado.
    /// </summary>
    /// <param name="id">ID do pet a ser excluído.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Pet excluído com sucesso.</response>
    /// <response code="404">Pet não localizado no sistema.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        var pet = await _context.Pets.FindAsync(id);

        if (pet == null)
            return NotFound("Pet não encontrado.");

        var tutor = await _context.Tutores.FindAsync(pet.TutorCpf);

        if (tutor != null && tutor.QtdPets > 0)
            tutor.QtdPets -= 1;

        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}