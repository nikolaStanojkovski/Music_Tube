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
        public String VideoURL { get; set; }

        public Guid? SongId { get; set; }
        public virtual Song? Song { get; set; }
    }
}
