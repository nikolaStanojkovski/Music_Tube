﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.Domain
{
    public class Song : Media
    {
        [Required]
        public Int64 Length { get; set; } // in seconds
        [Required]
        public Int64 AudioQuality { get; set; } // in kbps
        [Required]
        public Int64 AudioSampleRate { get; set; } // in kHz
        [Required]
        public String AudioURL { get; set; }

        public Guid AlbumId { get; set; }
        [Required]
        public virtual Album Album { get; set; }

        public virtual List<Video> VideosAppearedIn { get; set; }
    }
}
