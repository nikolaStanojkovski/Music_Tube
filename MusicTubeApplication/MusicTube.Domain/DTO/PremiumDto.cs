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
    public class PremiumDto
    {
        public String Name { get; set; }
        public String Surname { get; set; }
        public String ArtistName { get; set; }

        public string CreatorId { get; set; }
        public Creator Creator { get; set; }

        [Required]
        public SubscriptionPlan SubscriptionPlan { get; set; }
    }
}
