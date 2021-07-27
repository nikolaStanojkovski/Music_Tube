﻿using MusicTube.Domain.Domain;
using MusicTube.Domain.Enumerations;
using MusicTube.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.DTO
{
    public class VideoDto
    {
        [Required]
        public String Name { get; set; }
        [Required]
        public String Description { get; set; }
        [Required]
        public String Label { get; set; }
        [Required]
        public Genre Genre { get; set; }

        public String VideoURL { get; set; }

        public Guid? SongId { get; set; }
        public Creator Creator { get; set; }
        public List<Song> AllSongs { get; set; }
    }
}
