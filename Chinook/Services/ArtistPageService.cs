using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace Chinook.Services
{
    public class ArtistPageService : IArtistPageService
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
        /// Configurations
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<ArtistPageService> _logger;
        public ArtistPageService(
            IDbContextFactory<ChinookContext> dbContextFactory, 
            IConfiguration configuration,
            ILogger<ArtistPageService> logger
        )
        {
            _dbContextFactory = dbContextFactory;
            _chinookContext = _dbContextFactory.CreateDbContext();
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Get Artist by artist id.
        /// </summary>
        /// <param name="ArtistId">Artist's id.</param>
        /// <returns>Artist Model.</returns>
        public async Task<Artist> GetArtistByIdAsync(long artistId)
        {
            if (artistId <= 0)
            {
                throw new ArgumentException("Invalid artistId");
            }

            try
            {
                return await _chinookContext.Artists
                    .FirstOrDefaultAsync(a => a.ArtistId == artistId);
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, and possibly rethrow it if necessary.
                _logger.LogError(ex, "An error occurred while getting the artist.");
                throw;
            }
        }

        /// <summary>
        /// Get tracks in aplaylist.
        /// </summary>
        /// <param name="ArtistId">Artist's id</param>
        /// <param name="CurrentUserId">Loged in user's id.</param>
        /// <returns>List of PlaylistTrack model.</returns>
        public async Task<List<PlaylistTrack>> GetTracksByArtistId(long artistId, string caurrentUserId)
        {
            try
            {
                if (artistId <= 0 || string.IsNullOrWhiteSpace(caurrentUserId) || _chinookContext == null || _configuration == null)
                {
                    return new List<PlaylistTrack>();
                }

                var tracks = await _chinookContext.Tracks
                    .Include(t => t.Album)
                    .Include(t => t.Playlists)
                    .Where(t => t.Album.ArtistId == artistId)
                    .Select(t => new PlaylistTrack
                    {
                        AlbumTitle = t.Album == null ? "-" : t.Album.Title,
                        TrackId = t.TrackId,
                        TrackName = t.Name,
                        IsFavorite = t.Playlists.Any(p => p.Name == _configuration["AppSettings:FavoritePlaylistName"])
                    })
                    .ToListAsync();

                return tracks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting tracks by artist.");
                throw;
            }
        }
    }
}
