using MusicTube.Domain.Domain.Subdomain;
using MusicTube.Domain.Enumerations;
using MusicTube.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.Domain
{
    public class Media : BaseEntity
    {
        [Required]
        public String Name { get; set; }
        public String Description { get; set; }
        [Required]
        public String Label { get; set; }
        [Required]
        public Genre Genre { get; set; }

        public string CreatorId { get; set; }
        [Required]
        public virtual Creator Creator { get; set; }

        public virtual List<UserFeedback> Feedbacks { get; set; }
        public virtual List<Review> Reviews { get; set; }
    }
}
