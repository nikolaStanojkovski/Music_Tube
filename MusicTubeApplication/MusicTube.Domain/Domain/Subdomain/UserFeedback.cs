using MusicTube.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.Domain.Subdomain
{
    public class UserFeedback : BaseEntity
    {
        public Boolean IsLiked { get; set; }
        public Boolean IsDisliked { get; set; }
        public String Comment { get; set; }

        public string UserId { get; set; }
        [Required]
        public virtual MusicTubeUser User { get; set; }

        public Guid MediaId { get; set; }
        [Required]
        public virtual Media Media { get; set; }
    }
}
