using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Models
{
    public partial class MediaType
    {
        public MediaType()
        {
            Tracks = new HashSet<Track>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MediaTypeId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }
}
