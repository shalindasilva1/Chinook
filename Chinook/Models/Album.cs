﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Chinook.Models
{
    public partial class Album
    {
        public Album()
        {
            Tracks = new HashSet<Track>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AlbumId { get; set; }
        public string Title { get; set; } = null!;
        public long ArtistId { get; set; }

        public virtual Artist Artist { get; set; } = null!;
        public virtual ICollection<Track> Tracks { get; set; }
    }
}
