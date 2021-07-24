using MusicTube.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.Domain.Subdomain
{
    public class Review : BaseEntity
    {
        [Required]
        public String Summary { get; set; }
        [Required]
        public String Description { get; set; }
        [Required]
        [Range(1, 5)]
        public Double Rating { get; set; }

        public string ListenerId { get; set; }
        [Required]
        public Listener Listener { get; set; }

        public Guid MediaId { get; set; }
        [Required]
        public Media Media { get; set; }
    }
}
