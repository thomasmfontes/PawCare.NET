using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using PawCareApi.Domain.Common;
using PawCareApi.Domain.Entities;
using PawCareApi.Domain.Interfaces;

namespace PawCareApi.Infrastructure.NoSql
{
    public class ProntuarioRepository : IProntuarioRepository
    {
        private readonly MongoDbContext _context;
        private readonly ILogger<ProntuarioRepository> _logger;

        // Armazenamento em memória concorrente resiliente para testes e desenvolvimento sem daemon MongoDB ativo
        private static readonly ConcurrentDictionary<string, ProntuarioNoSql> _inMemoryStore = new();

        public ProntuarioRepository(MongoDbContext context, ILogger<ProntuarioRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PageResult<ProntuarioNoSql>> ObterPaginadoAsync(
            int pageNumber, int pageSize, long? petId = null, string? tipoAtendimento = null, string? sintoma = null)
        {
            try
            {
                if (_context.Prontuarios != null)
                {
                    var builder = Builders<ProntuarioNoSql>.Filter;
                    var filter = builder.Empty;

                    if (petId.HasValue && petId.Value > 0)
                    {
                        filter &= builder.Eq(p => p.PetId, petId.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(tipoAtendimento))
                    {
                        filter &= builder.Regex(p => p.TipoAtendimento, new BsonRegularExpression(tipoAtendimento, "i"));
                    }

                    if (!string.IsNullOrWhiteSpace(sintoma))
                    {
                        filter &= builder.Regex("Sintomas", new BsonRegularExpression(sintoma, "i"));
                    }

                    var totalItems = (int)await _context.Prontuarios.CountDocumentsAsync(filter);
                    var items = await _context.Prontuarios.Find(filter)
                        .SortByDescending(p => p.DataAtendimento)
                        .Skip((pageNumber - 1) * pageSize)
                        .Limit(pageSize)
                        .ToListAsync();

                    return new PageResult<ProntuarioNoSql>(items, totalItems, pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MongoDB indisponível ou inacessível. Utilizando armazenamento resiliente em memória.");
            }

            // Fallback em memória
            var query = _inMemoryStore.Values.AsQueryable();

            if (petId.HasValue && petId.Value > 0)
            {
                query = query.Where(p => p.PetId == petId.Value);
            }

            if (!string.IsNullOrWhiteSpace(tipoAtendimento))
            {
                query = query.Where(p => p.TipoAtendimento.Contains(tipoAtendimento, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(sintoma))
            {
                query = query.Where(p => p.Sintomas.Any(s => s.Contains(sintoma, StringComparison.OrdinalIgnoreCase)));
            }

            var memTotal = query.Count();
            var memItems = query.OrderByDescending(p => p.DataAtendimento)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PageResult<ProntuarioNoSql>(memItems, memTotal, pageNumber, pageSize);
        }

        public async Task<IEnumerable<ProntuarioNoSql>> ObterPorPetIdAsync(long petId)
        {
            try
            {
                if (_context.Prontuarios != null)
                {
                    return await _context.Prontuarios
                        .Find(p => p.PetId == petId)
                        .SortByDescending(p => p.DataAtendimento)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MongoDB indisponível. Utilizando armazenamento resiliente em memória.");
            }

            return _inMemoryStore.Values
                .Where(p => p.PetId == petId)
                .OrderByDescending(p => p.DataAtendimento)
                .ToList();
        }

        public async Task<ProntuarioNoSql?> ObterPorIdAsync(string id)
        {
            try
            {
                if (_context.Prontuarios != null)
                {
                    return await _context.Prontuarios.Find(p => p.Id == id).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MongoDB indisponível. Utilizando armazenamento resiliente em memória.");
            }

            _inMemoryStore.TryGetValue(id, out var prontuario);
            return prontuario;
        }

        public async Task<ProntuarioNoSql> CriarAsync(ProntuarioNoSql prontuario)
        {
            if (string.IsNullOrWhiteSpace(prontuario.Id))
            {
                prontuario.Id = ObjectId.GenerateNewId().ToString();
            }

            try
            {
                if (_context.Prontuarios != null)
                {
                    await _context.Prontuarios.InsertOneAsync(prontuario);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MongoDB indisponível. Registrando no armazenamento em memória.");
            }

            _inMemoryStore[prontuario.Id] = prontuario;
            return prontuario;
        }

        public async Task<ProntuarioNoSql> AtualizarAsync(string id, ProntuarioNoSql prontuario)
        {
            prontuario.Id = id;

            try
            {
                if (_context.Prontuarios != null)
                {
                    await _context.Prontuarios.ReplaceOneAsync(p => p.Id == id, prontuario);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MongoDB indisponível. Atualizando no armazenamento em memória.");
            }

            _inMemoryStore[id] = prontuario;
            return prontuario;
        }

        public async Task<bool> RemoverAsync(string id)
        {
            var removido = false;

            try
            {
                if (_context.Prontuarios != null)
                {
                    var result = await _context.Prontuarios.DeleteOneAsync(p => p.Id == id);
                    removido = result.DeletedCount > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MongoDB indisponível. Removendo do armazenamento em memória.");
            }

            var memRemovido = _inMemoryStore.TryRemove(id, out _);
            return removido || memRemovido;
        }

        public async Task<bool> PingAsync()
        {
            return await _context.PingAsync();
        }
    }
}
