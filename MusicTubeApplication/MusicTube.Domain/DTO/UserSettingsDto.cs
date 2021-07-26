using MusicTube.Domain.Domain;
using MusicTube.Domain.Domain.Subdomain;
using MusicTube.Domain.Enumerations;
using MusicTube.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.DTO
{
    public class UserSettingsDto
    {
        public MusicTubeUser CurrentUser { get; set; }

        public String Name { get; set; }
        public String Mail { get; set; }
        public String Surname { get; set; }
        public String ImageURL { get; set; }
        public Boolean NewsletterSubscribed { get; set; }
        public Genre FavouriteGenre { get; set; }
        public string FavouriteArtistId { get; set; }
        public Creator FavouriteArtist { get; set; }
        public List<Creator> AllCreators { get; set; }

        // Creator specific properties
        public String ArtistName { get; set; }
        public String ArtistDescription { get; set; }
        public PremiumPlan? PremiumPlan { get; set; }
        public List<MusicTubeUser> Fans { get; set; }
        public List<Media> Content { get; set; }

        // Listener specific properties
        public List<Review> Reviews { get; set; }
    }
}
