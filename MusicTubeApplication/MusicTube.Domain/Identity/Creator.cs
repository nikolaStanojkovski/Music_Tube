using MusicTube.Domain.Domain;
using MusicTube.Domain.Domain.Subdomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.Identity
{
    public class Creator : MusicTubeUser
    {
        public String ArtistName { get; set; }
        public String ArtistDescription { get; set; }

        public Guid? PremiumPlanId { get; set; }
        public virtual PremiumPlan? PremiumPlan { get; set; }

        public virtual List<MusicTubeUser> Fans { get; set; }
        public virtual List<Media> Content { get; set; }
    }
}
