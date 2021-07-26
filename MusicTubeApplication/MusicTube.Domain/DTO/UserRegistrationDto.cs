using MusicTube.Domain.Domain.Subdomain;
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
    public class UserRegistrationDto
    {
        [Required]
        public String Name { get; set; }
        [Required]
        public String Surname { get; set; }
        [Required]
        public Boolean NewsletterSubscribed { get; set; }
        [Required]
        public Genre FavouriteGenre { get; set; }

        public string? FavouriteArtistId { get; set; }
        public Creator? FavouriteArtist { get; set; }

        public List<Creator> AllCreators { get; set; }

        public String Role { get; set; }

        public String ArtistName { get; set; }
        public String ArtistDescription { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [Compare("Password", ErrorMessage = "The password and Confirm password do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
