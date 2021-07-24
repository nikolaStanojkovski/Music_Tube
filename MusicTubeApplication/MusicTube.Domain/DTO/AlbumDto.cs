using MusicTube.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.DTO
{
    public class AlbumDto
    {
        [Required]
        [Display(Name = "Album Name")]
        public String AlbumName { get; set; }
        [Required]
        [Display(Name = "Album Producer Name")]
        public String AlbumProducer { get; set; }
        [Required]
        [Display(Name = "Album Cover Art URL")]
        public String AlbumCoverArt { get; set; }
        [Required]
        [Display(Name = "Album Composer Name")]
        public String AlbumComposer { get; set; }
        [Required]
        [Display(Name = "Album Arranger Name")]
        public String AlbumArranger { get; set; }
        [Required]
        [Display(Name = "Album Mix/Master Engineer Name")]
        public String AlbumMixMasterEngineer { get; set; }
        [Required]
        [Display(Name = "Album Genre")]
        public Genre AlbumGenre { get; set; }

        public Guid PremiumUserId { get; set; }
    }
}
