using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawCareApi.Domain.Common;
using PawCareApi.Domain.Entities;
using PawCareApi.Domain.Interfaces;
using PawCareApi.Infrastructure.Data;

namespace PawCareApi.Infrastructure.Repositories
{
    public class TutorRepository : ITutorRepository
    {
        private readonly PawCareContext _context;
        private readonly ILogger<TutorRepository> _logger;

        public TutorRepository(PawCareContext context, ILogger<TutorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PageResult<Tutor>> ObterPaginadoAsync(int pageNumber, int pageSize, string? nome = null, string? email = null)
        {
            var query = _context.Tutores
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                query = query.Where(t => t.Nome.ToLower().Contains(nome.Trim().ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(t => t.Email.ToLower().Contains(email.Trim().ToLower()));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(t => t.Nome)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResult<Tutor>(items, totalItems, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Tutor>> ObterTodosAsync()
        {
            return await _context.Tutores
                .AsNoTracking()
                .OrderBy(t => t.Nome)
                .ToListAsync();
        }

        public async Task<Tutor?> ObterPorCpfAsync(string cpf)
        {
            var cleanCpf = Tutor.LimparCpf(cpf);
            return await _context.Tutores
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Cpf == cleanCpf || t.Cpf == cpf);
        }

        public async Task<Tutor> CriarAsync(Tutor tutor)
        {
            await _context.Tutores.AddAsync(tutor);
            await _context.SaveChangesAsync();
            return tutor;
        }

        public async Task<Tutor> AtualizarAsync(Tutor tutor)
        {
            _context.Tutores.Update(tutor);
            await _context.SaveChangesAsync();
            return tutor;
        }

        public async Task<bool> RemoverAsync(string cpf)
        {
            var tutor = await ObterPorCpfAsync(cpf);
            if (tutor == null) return false;

            _context.Tutores.Remove(tutor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteAsync(string cpf)
        {
            var cleanCpf = Tutor.LimparCpf(cpf);
            return await _context.Tutores.AnyAsync(t => t.Cpf == cleanCpf || t.Cpf == cpf);
        }
    }
}

