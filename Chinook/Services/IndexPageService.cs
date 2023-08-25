using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class IndexPageService
    {
        /// <summary>
        /// Chinook Database Factory.
        /// </summary>
        private readonly IDbContextFactory<ChinookContext> _dbContextFactory;
        /// <summary>
        /// Chinook Database Context.
        /// </summary>
        private readonly ChinookContext _chinookContext;

        public IndexPageService(IDbContextFactory<ChinookContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _chinookContext = _dbContextFactory.CreateDbContext();
        }

        /// <summary>
        /// Get all artists.
        /// </summary>
        /// <returns>List of Artist Model.</returns>
        public async Task<List<Artist>> GetArtists()
        {
            // Included albums to the same database call for perfomance optimization.
            return await _chinookContext.Artists.Include(x => x.Albums).ToListAsync();
        }
    }
}
