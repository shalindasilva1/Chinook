using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Services.Interfaces
{
    public interface IPlaylistPageService
    {
        Task<List<Models.Playlist>> GetPlaylists();
        Task<ClientModels.Playlist> GetPlaylistByPlaylistId(long playlistId, string currentUserId);
        void SavePlaylist(Models.Playlist playlist, Track track);
        List<Models.Playlist> GetNavigationItems();
        Task AddTracksToPlaylist(PlaylistTrack playlistTrack, long playlistId = 0, bool isFavorite = false);
        Task RemoveTracksFromPlaylist(PlaylistTrack playlistTrack);
        Task<Track> GetTrackByTrackId(long trackId);
        event Action OnNavigationUpdated;
    }
}
