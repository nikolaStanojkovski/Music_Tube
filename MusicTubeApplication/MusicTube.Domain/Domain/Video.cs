using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.Domain
{
    public class Video : Media
    {
        [Required]
        public Int64 Length { get; set; } // in seconds
        [Required]
        public Int64 VideoQuality { get; set; } // in PSNR
        [Required]
        public Int64 VideoFrameRate { get; set; } // in FPS
        [Required]
        public String VideoURL { get; set; }

        public Guid SongId { get; set; }
        [Required]
        public virtual Song Song { get; set; }
    }
}
