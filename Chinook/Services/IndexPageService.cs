using Chinook.Models;
using Chinook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class IndexPageService : IIndexPageService
    {
        /// <summary>
        /// Chinook Database Factory.
        /// </summary>
        private readonly IDbContextFactory<ChinookContext> _dbContextFactory;
        /// <summary>
        /// Chinook Database Context.
        /// </summary>
        private readonly ChinookContext _chinookContext;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<IndexPageService> _logger;

        public IndexPageService(
            IDbContextFactory<ChinookContext> dbContextFactory, 
            ILogger<IndexPageService> logger
        )
        {
            _dbContextFactory = dbContextFactory;
            _chinookContext = _dbContextFactory.CreateDbContext();
            _logger = logger;
        }

        /// <summary>
        /// Get all artists.
        /// </summary>
        /// <returns>List of Artist Model.</returns>
        public async Task<List<Artist>> GetArtists()
        {
            try
            {
                // Include albums in the same database call for performance optimization.
                // This retrieves artists and their albums in a single query.
                return await _chinookContext.Artists.Include(x => x.Albums).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting artists.");
                throw;
            }
        }
    }
}
