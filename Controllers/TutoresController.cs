using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TutoresController : ControllerBase
{
    private readonly PawCareContext _context;

    public TutoresController(PawCareContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tutor>>> GetAll()
    {
        var tutores = await _context.Tutores
            .Include(t => t.Pets)
            .ToListAsync();

        return Ok(tutores);
    }

    [HttpGet("{cpf}")]
    public async Task<ActionResult<Tutor>> GetByCpf(string cpf)
    {
        var tutor = await _context.Tutores
            .Include(t => t.Pets)
            .FirstOrDefaultAsync(t => t.Cpf == cpf);

        if (tutor == null)
            return NotFound("Tutor não encontrado.");

        return Ok(tutor);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<Tutor>> GetByEmail(string email)
    {
        var tutor = await _context.Tutores
            .FirstOrDefaultAsync(t => t.Email == email);

        if (tutor == null)
            return NotFound("Tutor não encontrado.");

        return Ok(tutor);
    }

    [HttpPost]
    public async Task<ActionResult<Tutor>> Create(Tutor tutor)
    {
        var tutorExistente = await _context.Tutores.FindAsync(tutor.Cpf);

        if (tutorExistente != null)
            return BadRequest("Já existe um tutor cadastrado com este CPF.");

        _context.Tutores.Add(tutor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByCpf), new { cpf = tutor.Cpf }, tutor);
    }

    [HttpPut("{cpf}")]
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

    [HttpDelete("{cpf}")]
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