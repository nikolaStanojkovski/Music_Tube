using MusicTube.Domain.Domain;
using MusicTube.Domain.Domain.Subdomain;
using MusicTube.Domain.Enumerations;
using MusicTube.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.DTO.DomainDTO
{
    public class SongDto
    {
        public Guid Id { get; set; }

        [Required]
        public String Name { get; set; }
        [Required]
        public String Description { get; set; }
        [Required]
        public String Label { get; set; }
        [Required]
        public Genre Genre { get; set; }

        public Creator Creator { get; set; }
        public String AudioURL { get; set; }

        public Album? Album { get; set; }
        public Guid? AlbumId { get; set; }

        public List<Album> AllAlbums { get; set; }

        public List<UserFeedback> Feedbacks { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
