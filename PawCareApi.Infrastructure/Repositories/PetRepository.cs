using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawCareApi.Domain.Common;
using PawCareApi.Domain.Entities;
using PawCareApi.Domain.Interfaces;
using PawCareApi.Infrastructure.Data;

namespace PawCareApi.Infrastructure.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly PawCareContext _context;
        private readonly ILogger<PetRepository> _logger;

        public PetRepository(PawCareContext context, ILogger<PetRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PageResult<Pet>> ObterPaginadoAsync(
            int pageNumber, int pageSize, string? nome = null, string? especie = null, long? racaId = null, string? tutorCpf = null)
        {
            var query = _context.Pets
                .AsNoTracking()
                .Include(p => p.Raca)
                .Include(p => p.Tutor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                query = query.Where(p => p.Nome.ToLower().Contains(nome.Trim().ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(especie))
            {
                query = query.Where(p => p.Raca != null && p.Raca.Especie.ToLower().Contains(especie.Trim().ToLower()));
            }

            if (racaId.HasValue && racaId.Value > 0)
            {
                query = query.Where(p => p.RacaId == racaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(tutorCpf))
            {
                var cleanCpf = Tutor.LimparCpf(tutorCpf);
                query = query.Where(p => p.TutorCpf == cleanCpf || p.TutorCpf == tutorCpf);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Nome)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResult<Pet>(items, totalItems, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Pet>> ObterTodosAsync()
        {
            return await _context.Pets
                .AsNoTracking()
                .Include(p => p.Raca)
                .Include(p => p.Tutor)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<Pet?> ObterPorIdAsync(long id)
        {
            return await _context.Pets
                .AsNoTracking()
                .Include(p => p.Raca)
                .Include(p => p.Tutor)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pet> CriarAsync(Pet pet)
        {
            await _context.Pets.AddAsync(pet);
            await _context.SaveChangesAsync();
            return await ObterPorIdAsync(pet.Id) ?? pet;
        }

        public async Task<Pet> AtualizarAsync(Pet pet)
        {
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();
            return await ObterPorIdAsync(pet.Id) ?? pet;
        }

        public async Task<bool> RemoverAsync(long id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null) return false;

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteAsync(long id)
        {
            return await _context.Pets.AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Pet>> ObterPorTutorCpfAsync(string tutorCpf)
        {
            var cleanCpf = Tutor.LimparCpf(tutorCpf);
            return await _context.Pets
                .AsNoTracking()
                .Include(p => p.Raca)
                .Where(p => p.TutorCpf == cleanCpf || p.TutorCpf == tutorCpf)
                .ToListAsync();
        }
    }
}

