using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawCareApi.Domain.Common;
using PawCareApi.Domain.Entities;
using PawCareApi.Domain.Interfaces;
using PawCareApi.Infrastructure.Data;

namespace PawCareApi.Infrastructure.Repositories
{
    public class ClinicaRepository : IClinicaRepository
    {
        private readonly PawCareContext _context;
        private readonly ILogger<ClinicaRepository> _logger;

        public ClinicaRepository(PawCareContext context, ILogger<ClinicaRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PageResult<Clinica>> ObterPaginadoAsync(int pageNumber, int pageSize, string? cidade = null, bool? atendimento24h = null)
        {
            var query = _context.Clinicas
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(cidade))
            {
                query = query.Where(c => c.Cidade.ToLower().Contains(cidade.Trim().ToLower()));
            }

            if (atendimento24h.HasValue)
            {
                query = query.Where(c => c.Atendimento24h == atendimento24h.Value);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.NomeCnpj)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResult<Clinica>(items, totalItems, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Clinica>> ObterTodasAsync()
        {
            return await _context.Clinicas
                .AsNoTracking()
                .OrderBy(c => c.NomeCnpj)
                .ToListAsync();
        }

        public async Task<Clinica?> ObterPorIdAsync(long id)
        {
            return await _context.Clinicas
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Clinica> CriarAsync(Clinica clinica)
        {
            await _context.Clinicas.AddAsync(clinica);
            await _context.SaveChangesAsync();
            return clinica;
        }

        public async Task<Clinica> AtualizarAsync(Clinica clinica)
        {
            _context.Clinicas.Update(clinica);
            await _context.SaveChangesAsync();
            return clinica;
        }

        public async Task<bool> RemoverAsync(long id)
        {
            var clinica = await _context.Clinicas.FindAsync(id);
            if (clinica == null) return false;

            _context.Clinicas.Remove(clinica);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteAsync(long id)
        {
            return await _context.Clinicas.AnyAsync(c => c.Id == id);
        }
    }
}

