using MusicTube.Domain.Domain;
using MusicTube.Domain.Enumerations;
using MusicTube.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.Domain.Subdomain
{
    public class PremiumPlan : BaseEntity
    {
        [Required]
        public SubscriptionPlan SubscriptionPlan { get; set; }

        public string CreatorId { get; set; }
        [Required]
        public virtual Creator Creator { get; set; }

        public virtual List<Album> Albums { get; set; }
    }
}
