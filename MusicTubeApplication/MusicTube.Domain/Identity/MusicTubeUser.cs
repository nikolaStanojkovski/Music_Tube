using MusicTube.Domain.Enumerations;
using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicTube.Domain.Domain;
using MusicTube.Domain.Domain.Subdomain;

namespace MusicTube.Domain.Identity
{
    public class MusicTubeUser : IdentityUser
    {
        public String Name { get; set; }
        public String Surname { get; set; }
        public String ImageURL { get; set; }
        public Boolean NewsletterSubscribed { get; set; }
        public Genre FavouriteGenre { get; set; }

        public string? FavouriteArtistId { get; set; }
        public virtual Creator? FavouriteArtist { get; set; }

        public virtual List<UserFeedback> Feedbacks { get; set; }
    }
}
