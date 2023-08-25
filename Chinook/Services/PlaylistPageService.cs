using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace Chinook.Services
{
    public class PlaylistPageService : IPlaylistPageService
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
        /// Event to handle new navigation menu items.
        /// </summary>
        public event Action OnNavigationUpdated;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<PlaylistPageService> _logger;
        /// <summary>
        /// navigation bar menu item list.
        /// </summary>
        private List<Models.Playlist> NavbarList;

        public PlaylistPageService(
            IDbContextFactory<ChinookContext> dbContextFactory, 
            IConfiguration configuration,
            ILogger<PlaylistPageService> logger
        )
        {
            _dbContextFactory = dbContextFactory;
            _chinookContext = _dbContextFactory.CreateDbContext();
            _configuration = configuration;
            _logger = logger;
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
        public async Task<ClientModels.Playlist> GetPlaylistByPlaylistId(long playlistId, string currentUserId)
        {
            try
            {
                if (playlistId <= 0)
                {
                    throw new ArgumentException("Invalid PlaylistId");
                }

                var playlist = await _chinookContext.Playlists
                    .Include(p => p.Tracks)
                    .ThenInclude(t => t.Album)
                    .Where(p => p.PlaylistId == playlistId)
                    .Select(p => new ClientModels.Playlist
                    {
                        Name = p.Name,
                        Tracks = p.Tracks.Select(t => new PlaylistTrack
                        {
                            AlbumTitle = t.Album.Title,
                            ArtistName = t.Album.Artist.Name,
                            TrackId = t.TrackId,
                            TrackName = t.Name,
                            IsFavorite = t.Playlists.Any(p => p.Name == _configuration["AppSettings:FavoritePlaylistName"])
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (playlist == null)
                {
                    return null;
                }

                return playlist;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the playlist.");
                throw;
            }
        }
        /// <summary>
        /// Save new playlist item.
        /// </summary>
        /// <param name="playlist">Playlist model.</param>
        public void SavePlaylist(Models.Playlist playlist, Track track = null)
        {
            if (playlist == null || _chinookContext == null)
            {
                return;
            }

            try
            {
                if (track != null)
                {
                    playlist.Tracks.Add(track);
                }

                _chinookContext.Playlists.Add(playlist);
                _chinookContext.SaveChanges();

                UpdateNavbarList(playlist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the playlist.");
                throw;
            }
        }
        /// <summary>
        /// Get navigation menu items.
        /// </summary>
        /// <returns>Playlist model.</returns>
        public List<Models.Playlist> GetNavigationItems()
        {
            var fevPlaylist = NavbarList.FirstOrDefault(p => p.Name.Equals(_configuration["AppSettings:FavoritePlaylistName"]));
            if (fevPlaylist != null)
            {
                // Bring favorite play list to top all the time
                NavbarList.Remove(fevPlaylist);
                NavbarList.Insert(0, fevPlaylist);
            }
            return NavbarList;
        }
        /// <summary>
        /// Add tracks to playlists.
        /// </summary>
        /// <param name="playlistTrack">Playlist model.</param>
        /// <param name="playlistId">Playlist id.</param>
        /// <param name="isFavorite">True if needs to be added to the favorite playlist</param>
        public async Task AddTracksToPlaylist(PlaylistTrack playlistTrack, long playlistId = 0, bool isFavorite = false)
        {
            try
            {
                if (playlistTrack == null || _chinookContext == null)
                {
                    return;
                }

                Models.Playlist playlist = null;
                Track track = null;
                if (playlistTrack.TrackId > 0)
                {
                    track = await _chinookContext.Tracks.FirstOrDefaultAsync(t => t.TrackId == playlistTrack.TrackId);
                }
                if (isFavorite && playlistId == 0)
                {
                    playlist = await _chinookContext.Playlists.FirstOrDefaultAsync(p => p.Name == _configuration["AppSettings:FavoritePlaylistName"]);

                    if (playlist == null)
                    {
                        playlist = CreateFavoritePlaylist(track);
                        _chinookContext.Playlists.Add(playlist);
                    }
                }
                else if (playlistId != 0)
                {
                    playlist = await _chinookContext.Playlists.FirstOrDefaultAsync(p => p.PlaylistId == playlistId);
                }

                if (track != null)
                {
                    playlist.Tracks.Add(track);

                    // Save changes once.
                    await _chinookContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding tracks to the playlist.");
                throw;
            }
        }
        /// <summary>
        /// Remomve tracks from playlist
        /// </summary>
        /// <param name="playlistTrack">Playlist model.</param>
        public async Task RemoveTracksFromPlaylist(PlaylistTrack playlistTrack)
        {
            try
            {
                if (playlistTrack == null || _chinookContext == null)
                {
                    return;
                }

                Models.Playlist playlist = null;
                Track track = null;
                if (playlistTrack.TrackId > 0)
                {
                    track = await _chinookContext.Tracks.FirstOrDefaultAsync(t => t.TrackId == playlistTrack.TrackId);
                }
                playlist = await _chinookContext.Playlists.Include(t => t.Tracks).FirstOrDefaultAsync(p => p.Name == _configuration["AppSettings:FavoritePlaylistName"]);

                if (track != null)
                {
                    playlist.Tracks.Remove(track);
                    // Save changes once.
                    await _chinookContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding tracks to the playlist.");
                throw;
            }
        }
        /// <summary>
        /// Get track by trackId.
        /// </summary>
        /// <param name="trackId">Track's id.</param>
        /// <returns></returns>
        public async Task<Track> GetTrackByTrackId(long trackId)
        {
            return await _chinookContext.Tracks.Where(t => t.TrackId == trackId).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Get OnNavigationUpdated evrnt
        /// </summary>
        /// <returns>Evnt</returns>
        public Action GetOnNavigationUpdated()
        {
            return OnNavigationUpdated;
        }
        /// <summary>
        /// Create favorite playlist.
        /// </summary>
        /// <returns>Playlist model.</returns>
        private Models.Playlist CreateFavoritePlaylist(Track track)
        {
            Random random = new Random();
            var addedPlaylist = new Models.Playlist()
            {
                PlaylistId = (long)random.Next(),
                Name = _configuration["AppSettings:FavoritePlaylistName"]
            };
            SavePlaylist(addedPlaylist, track);
            return addedPlaylist;
        }
        /// <summary>
        /// Update navigation bar
        /// </summary>
        /// <param name="playlist"></param>
        private void UpdateNavbarList(Models.Playlist playlist)
        {
            NavbarList.Insert(0, playlist);
            OnNavigationUpdated?.Invoke();
        }

        
    }
}
