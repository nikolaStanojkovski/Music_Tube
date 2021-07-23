using Microsoft.AspNetCore.Identity;
using MusicTube.Domain.Domain;
using MusicTube.Domain.Domain.Subdomain;
using MusicTube.Domain.DTO;
using MusicTube.Domain.Identity;
using MusicTube.Repository.Interface;
using MusicTube.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository _userRepository)
        {
            this.userRepository = _userRepository;
        }

        public MusicTubeUser CreateNewUser(UserRegistrationDto request)
        {
            Creator favouriteArtist = null;
            string favouriteArtistId = null;
            if(request.FavouriteArtistId != null)
            {
                favouriteArtist = userRepository.ReadCreatorInformation(request.FavouriteArtistId);
                favouriteArtistId = favouriteArtist.Id;
            }

            var user = new MusicTubeUser();
            if(request.Role.Equals("Listener"))
            {
                user = new Listener
                {
                    UserName = request.Email,
                    NormalizedUserName = request.Email,
                    Email = request.Email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    ImageURL = "default-account.png",

                    Name = request.Name,
                    Surname = request.Surname,
                    NewsletterSubscribed = request.NewsletterSubscribed,
                    FavouriteGenre = request.FavouriteGenre,
                    FavouriteArtist = favouriteArtist,
                    FavouriteArtistId = favouriteArtistId,
                    Feedbacks = new List<UserFeedback>(),

                    // Listener specific properties
                    Reviews = new List<Review>()
                };
            } else if(request.Role.Equals("Creator"))
            {
                user = new Creator
                {
                    UserName = request.Email,
                    NormalizedUserName = request.Email,
                    Email = request.Email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    ImageURL = "default-account.png",

                    Name = request.Name,
                    Surname = request.Surname,
                    NewsletterSubscribed = request.NewsletterSubscribed,
                    FavouriteGenre = request.FavouriteGenre,
                    FavouriteArtist = favouriteArtist,
                    FavouriteArtistId = favouriteArtistId,
                    Feedbacks = new List<UserFeedback>(),

                    // Creator specific properties
                    ArtistName = request.ArtistName,
                    ArtistDescription = request.ArtistDescription,
                    PremiumPlan = null,
                    PremiumPlanId = Guid.Empty,
                    Fans = new List<MusicTubeUser>(),
                    Content = new List<Media>()
                };
            }

            return user;
        }

        public UserRegistrationDto GetUserRegistrationDto(UserManager<MusicTubeUser> userManager)
        {
            List<MusicTubeUser> allCreators = userRepository.GetAll();
            List<Creator> chosenCreators = new List<Creator>();
            foreach(var artist in allCreators)
            {
                foreach(var role in userManager.GetRolesAsync(artist).Result)
                {
                    if(role.Equals("Creator"))
                    {
                        chosenCreators.Add((Creator)artist);
                        break;
                    }
                }
            }

            return new UserRegistrationDto
            {
                AllCreators = chosenCreators
            };
        }

        public UserSettingsDto GetUserSettings(MusicTubeUser user)
        {
            UserSettingsDto model = new UserSettingsDto();
            if (user.GetType().Name.Equals("Creator"))
            {
                user = userRepository.ReadCreatorInformation(user.Id);
                model = new UserSettingsDto()
                {
                    Mail = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    ImageURL = user.ImageURL,
                    NewsletterSubscribed = user.NewsletterSubscribed,
                    FavouriteGenre = user.FavouriteGenre,
                    FavouriteArtist = user.FavouriteArtist,

                    ArtistName = ((Creator)user).ArtistName,
                    ArtistDescription = ((Creator)user).ArtistDescription,
                    PremiumPlan = ((Creator)user).PremiumPlan,
                    Fans = ((Creator)user).Fans,
                    Content = ((Creator)user).Content,
                    Reviews = null
                };
            } else
            {
                user = userRepository.ReadUserInformation(user.Id);
                model = new UserSettingsDto()
                {
                    Mail = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    ImageURL = user.ImageURL,
                    NewsletterSubscribed = user.NewsletterSubscribed,
                    FavouriteGenre = user.FavouriteGenre,
                    FavouriteArtist = user.FavouriteArtist,

                    ArtistName = null,
                    ArtistDescription = null,
                    PremiumPlan = null,
                    Fans = null,
                    Content = null,
                    Reviews = ((Listener)user).Reviews
                };
            }

            return model;
        }

        public void UpdateUserPersonalInformation(MusicTubeUser user, UserSettingsDto model)
        {
            if (model.Name != null && !model.Name.Equals(""))
                user.Name = model.Name;

            if (model.Surname != null && !model.Surname.Equals(""))
                user.Surname = model.Surname;

            if (model.ImageURL != null && !model.ImageURL.Equals(""))
                user.ImageURL = model.ImageURL;

            userRepository.UpdateUser(user);
        }
    }
}
