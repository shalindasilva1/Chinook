using Chinook.Models;

namespace Chinook.Services.Interfaces
{
    public interface IIndexPageService
    {
        Task<List<Artist>> GetArtists();
    }
}
