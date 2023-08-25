using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Services.Interfaces
{
    public interface IArtistPageService
    {
        Task<Artist> GetArtistByIdAsync(long artistId);
        Task<List<PlaylistTrack>> GetTracksByArtistId(long artistId, string currentUserId);
    }
}
