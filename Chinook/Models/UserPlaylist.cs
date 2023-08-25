using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Models;

public class UserPlaylist
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string UserId { get; set; }
    public long PlaylistId { get; set; }
    public ChinookUser User { get; set; }
    public Playlist Playlist { get; set; }
}
