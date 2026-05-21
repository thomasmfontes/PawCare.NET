using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCareApi.Data;
using PawCareApi.Models;

namespace PawCareApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController : ControllerBase
{
    private readonly PawCareContext _context;

    public PetsController(PawCareContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pet>>> GetAll()
    {
        var pets = await _context.Pets
            .Include(p => p.Tutor)
            .Include(p => p.Raca)
            .ToListAsync();

        return Ok(pets);
    }

    [HttpGet("{id:long}")]
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

    [HttpGet("tutor/{cpf}")]
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

    [HttpGet("raca/{racaId:long}")]
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

    [HttpGet("status/{status}")]
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

    [HttpPost]
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

    [HttpPut("{id:long}")]
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

    [HttpDelete("{id:long}")]
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