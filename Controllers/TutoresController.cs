using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

/// <summary>
/// Endpoints para gerenciamento dos tutores e responsáveis pelos pets.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TutoresController : ControllerBase
{
    private readonly PawCareContext _context;

    public TutoresController(PawCareContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos os tutores cadastrados na base de dados com seus respectivos pets.
    /// </summary>
    /// <returns>Coleção de tutores cadastrados.</returns>
    /// <response code="200">Lista de tutores retornada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Tutor>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Tutor>>> GetAll()
    {
        var tutores = await _context.Tutores
            .Include(t => t.Pets)
            .ToListAsync();

        return Ok(tutores);
    }

    /// <summary>
    /// Busca os detalhes de um tutor pelo seu CPF.
    /// </summary>
    /// <param name="cpf">Número do CPF do tutor.</param>
    /// <returns>Dados completos do tutor e lista de pets.</returns>
    /// <response code="200">Tutor encontrado com sucesso.</response>
    /// <response code="404">Nenhum tutor localizado com o CPF informado.</response>
    [HttpGet("{cpf}")]
    [ProducesResponseType(typeof(Tutor), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Tutor>> GetByCpf(string cpf)
    {
        var tutor = await _context.Tutores
            .Include(t => t.Pets)
            .FirstOrDefaultAsync(t => t.Cpf == cpf);

        if (tutor == null)
            return NotFound("Tutor não encontrado.");

        return Ok(tutor);
    }

    /// <summary>
    /// Busca um tutor pelo seu endereço de e-mail cadastrado.
    /// </summary>
    /// <param name="email">E-mail do tutor.</param>
    /// <returns>Dados do tutor correspondente.</returns>
    /// <response code="200">Tutor localizado com sucesso.</response>
    /// <response code="404">Nenhum tutor cadastrado com este e-mail.</response>
    [HttpGet("email/{email}")]
    [ProducesResponseType(typeof(Tutor), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Tutor>> GetByEmail(string email)
    {
        var tutor = await _context.Tutores
            .FirstOrDefaultAsync(t => t.Email == email);

        if (tutor == null)
            return NotFound("Tutor não encontrado.");

        return Ok(tutor);
    }

    /// <summary>
    /// Cadastra um novo tutor na plataforma PawCare.
    /// </summary>
    /// <param name="tutor">Dados cadastrais do novo tutor.</param>
    /// <returns>Dados do tutor recém-criado.</returns>
    /// <response code="201">Tutor cadastrado com sucesso.</response>
    /// <response code="400">CPF já cadastrado ou campos obrigatórios inválidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Tutor), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Tutor>> Create(Tutor tutor)
    {
        var tutorExistente = await _context.Tutores.FindAsync(tutor.Cpf);

        if (tutorExistente != null)
            return BadRequest("Já existe um tutor cadastrado com este CPF.");

        _context.Tutores.Add(tutor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByCpf), new { cpf = tutor.Cpf }, tutor);
    }

    /// <summary>
    /// Atualiza as informações de cadastro de um tutor existente.
    /// </summary>
    /// <param name="cpf">CPF do tutor especificado na rota.</param>
    /// <param name="tutor">Dados atualizados do tutor.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Tutor atualizado com sucesso.</response>
    /// <response code="400">Divergência entre o CPF da URL e o CPF do corpo da requisição.</response>
    /// <response code="404">Tutor não encontrado.</response>
    [HttpPut("{cpf}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string cpf, Tutor tutor)
    {
        if (cpf != tutor.Cpf)
            return BadRequest("O CPF da URL não confere com o CPF enviado.");

        var tutorExistente = await _context.Tutores.FindAsync(cpf);

        if (tutorExistente == null)
            return NotFound("Tutor não encontrado.");

        tutorExistente.Nome = tutor.Nome;
        tutorExistente.Telefone = tutor.Telefone;
        tutorExistente.Email = tutor.Email;
        tutorExistente.QtdPets = tutor.QtdPets;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Exclui o cadastro de um tutor do sistema (caso não possua pets ativos).
    /// </summary>
    /// <param name="cpf">CPF do tutor a ser excluído.</param>
    /// <returns>Sem conteúdo em caso de sucesso.</returns>
    /// <response code="204">Tutor removido com sucesso.</response>
    /// <response code="400">Tentativa de remover tutor que possui pets cadastrados.</response>
    /// <response code="404">Tutor não encontrado.</response>
    [HttpDelete("{cpf}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string cpf)
    {
        var tutor = await _context.Tutores
            .Include(t => t.Pets)
            .FirstOrDefaultAsync(t => t.Cpf == cpf);

        if (tutor == null)
            return NotFound("Tutor não encontrado.");

        if (tutor.Pets.Any())
            return BadRequest("Não é possível remover um tutor que possui pets cadastrados.");

        _context.Tutores.Remove(tutor);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}