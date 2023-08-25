using Chinook.ClientModels;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;
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
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Event to handle new navigation menu items.
        /// </summary>
        public event Action OnNavigationUpdated;
        /// <summary>
        /// navigation bar menu item list.
        /// </summary>
        private List<Models.Playlist> NavbarList;

        public PlaylistPageService(IDbContextFactory<ChinookContext> dbContextFactory, IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _chinookContext = _dbContextFactory.CreateDbContext();
            _configuration = configuration;
        }

        /// <summary>
        /// Get all playlists.
        /// </summary>
        /// <returns>List of Playlist model.</returns>
        public async Task<List<Models.Playlist>> GetPlaylists()
        {
            NavbarList = await _chinookContext.Playlists.ToListAsync();
            return NavbarList;
        }
        /// <summary>
        /// Get playlist by playlist id.
        /// </summary>
        /// <param name="PlaylistId">Playlist's id.</param>
        /// <param name="CurrentUserId">Logged in user's id.</param>
        /// <returns>Playlist model.</returns>
        public async Task<ClientModels.Playlist> GetPlaylistByPlaylistId(long PlaylistId, string CurrentUserId)
        {
            var Playlist = await _chinookContext.Playlists
            .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
            .Where(p => p.PlaylistId == PlaylistId)
            .Select(p => new ClientModels.Playlist()
            {
                Name = p.Name,
                Tracks = p.Tracks.Select(t => new PlaylistTrack()
                {
                    AlbumTitle = t.Album.Title,
                    ArtistName = t.Album.Artist.Name,
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists
                        .Where(p => p.UserPlaylists
                            .Any(up => up.UserId == CurrentUserId && up.Playlist.Name == "Favorites"))
                        .Any()
                }).ToList()
            })
            .FirstOrDefaultAsync();
            return Playlist;
        }
        /// <summary>
        /// Save new playlist item.
        /// </summary>
        /// <param name="playlist">Playlist model.</param>
        public void SavePlaylist(Models.Playlist playlist, Track track)
        {
            if (playlist != null)
            {
                
                playlist = _chinookContext.Playlists.Add(playlist).Entity;
                playlist.Tracks.Add(track);
                _chinookContext.SaveChanges();
                _chinookContext.Update(playlist);
                _chinookContext.SaveChanges();
                NavbarList.Insert(0, playlist);
                OnNavigationUpdated?.Invoke();
            }
        }
        /// <summary>
        /// Get navigation menu items.
        /// </summary>
        /// <returns>Playlist model.</returns>
        public List<Models.Playlist> GetNavigationItems()
        {
            return NavbarList;
        }
        /// <summary>
        /// Add tracks to playlists.
        /// </summary>
        /// <param name="playlistTrack">Playlist model.</param>
        /// <param name="playlistId">Playlist id.</param>
        /// <param name="isFavorite">True if needs to be added to the favorite playlist</param>
        public async void AddTracksToPlaylist(PlaylistTrack playlistTrack, long playlistId = 0 , bool isFavorite = false)
        {
            var playlist = new Models.Playlist();
            if (isFavorite && playlistId == 0)
            {
                var fevPlaylistName = _configuration["AppSettings:FavoritePlaylistName"];
                playlist = await _chinookContext.Playlists.Where(p => p.Name == fevPlaylistName).FirstOrDefaultAsync();
                if(playlist == null) 
                {
                    playlist = CreateFavoritePlaylist();
                }
            }
            else
            {
                playlist = await _chinookContext.Playlists.Where(p => p.PlaylistId == playlistId).FirstOrDefaultAsync();
            }
            var track = await _chinookContext.Tracks.Where(t => t.TrackId == playlistTrack.TrackId).FirstOrDefaultAsync();
            if (playlist != null && track != null)
            {
                playlist.Tracks.Add(track);
                _chinookContext.Playlists.Update(playlist);
                _chinookContext.SaveChanges();
            }
        }
        /// <summary>
        /// Create favorite playlist.
        /// </summary>
        /// <returns>Playlist model.</returns>
        public Models.Playlist CreateFavoritePlaylist()
        {
            var addedPlaylist = _chinookContext.Playlists.Add(new Models.Playlist()
            {
                Name = _configuration["AppSettings:FavoritePlaylistName"]
            });
            _chinookContext.SaveChanges();
            NavbarList.Insert(0, addedPlaylist.Entity);
            OnNavigationUpdated?.Invoke();
            return addedPlaylist.Entity;
        }

        public async Task<Track> GetTrackByTrackId(long trackId)
        {
            return await _chinookContext.Tracks.Where(t => t.TrackId == trackId).FirstOrDefaultAsync();
        }
    }
}
