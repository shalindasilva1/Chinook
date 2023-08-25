using Chinook.ClientModels;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class ArtistPageService
    {
        /// <summary>
        /// Chinook Database Factory.
        /// </summary>
        private readonly IDbContextFactory<ChinookContext> _dbContextFactory;
        /// <summary>
        /// Chinook Database Context.
        /// </summary>
        private readonly ChinookContext _chinookContext;

        public ArtistPageService(IDbContextFactory<ChinookContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _chinookContext = _dbContextFactory.CreateDbContext();
        }

        /// <summary>
        /// Get Artist by artist id.
        /// </summary>
        /// <param name="ArtistId">Artist's id.</param>
        /// <returns>Artist Model.</returns>
        public async Task<Artist?> GetArtistyById(long ArtistId)
        {
            return await _chinookContext.Artists.SingleOrDefaultAsync(a => a.ArtistId == ArtistId);

        }

        /// <summary>
        /// Get tracks in aplaylist.
        /// </summary>
        /// <param name="ArtistId">Artist's id</param>
        /// <param name="CurrentUserId">Loged in user's id.</param>
        /// <returns>List of PlaylistTrack model.</returns>
        public async Task<List<PlaylistTrack>> GetTracksByArtistId(long ArtistId, string CurrentUserId)
        {
            var Tracks = await _chinookContext.Tracks.Where(a => a.Album.ArtistId == ArtistId)
            .Include(a => a.Album)
            .Select(t => new PlaylistTrack()
            {
                AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                TrackId = t.TrackId,
                TrackName = t.Name,
                IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == CurrentUserId && up.Playlist.Name == "Favorites")).Any()
            })
            .ToListAsync();
            return Tracks;
        }
    }
}
