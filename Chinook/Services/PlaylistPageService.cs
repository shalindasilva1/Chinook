using Chinook.ClientModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Chinook.Services
{
    public class PlaylistPageService
    {
        /// <summary>
        /// Chinook Database Factory.
        /// </summary>
        private readonly IDbContextFactory<ChinookContext> _dbContextFactory;
        /// <summary>
        /// Chinook Database Context.
        /// </summary>
        private readonly ChinookContext _chinookContext;

        public PlaylistPageService(IDbContextFactory<ChinookContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _chinookContext = _dbContextFactory.CreateDbContext();
        }

        /// <summary>
        /// Get playlist by playlist id.
        /// </summary>
        /// <param name="PlaylistId">Playlist's id.</param>
        /// <param name="CurrentUserId">Loged in user's id.</param>
        /// <returns>Playlist model.</returns>
        public async Task<Playlist> GetPlaylistByPlaylistId(long PlaylistId, string CurrentUserId)
        {
            var Playlist = await _chinookContext.Playlists
            .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
            .Where(p => p.PlaylistId == PlaylistId)
            .Select(p => new Playlist()
            {
                Name = p.Name,
                Tracks = p.Tracks.Select(t => new ClientModels.PlaylistTrack()
                {
                    AlbumTitle = t.Album.Title,
                    ArtistName = t.Album.Artist.Name,
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == CurrentUserId && up.Playlist.Name == "Favorites")).Any()
                }).ToList()
            })
            .FirstOrDefaultAsync();
            return Playlist;
        }
    }
}
