using Microsoft.AspNetCore.Identity;
using MusicTube.Domain.Domain;
using MusicTube.Domain.Domain.Subdomain;
using MusicTube.Domain.DTO.DomainDTO;
using MusicTube.Domain.DTO.IdentityDTO;
using MusicTube.Domain.Enumerations;
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
        private readonly ISongRepository songRepository;
        private readonly IRepository<Review> reviewRepository;
        private readonly IRepository<PremiumPlan> premiumPlanRepository;

        public UserService(IUserRepository _userRepository,
            IRepository<Review> _reviewRepository,
            IRepository<PremiumPlan> _premiumPlanRepository,
            ISongRepository _songRepository)
        {
            this.userRepository = _userRepository;
            this.reviewRepository = _reviewRepository;
            this.premiumPlanRepository = _premiumPlanRepository;
            this.songRepository = _songRepository;
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
                    PremiumPlanId = null,
                    Fans = new List<MusicTubeUser>(),
                    Content = new List<Media>()
                };
            }

            return user;
        }

        public UserRegistrationDto GetUserRegistrationDto(MusicTubeUser user, UserManager<MusicTubeUser> userManager)
        {
            List<Creator> chosenCreators = GetAllCreators(null, userManager);

            return new UserRegistrationDto
            {
                AllCreators = chosenCreators
            };
        }

        public UserSettingsDto GetUserSettings(MusicTubeUser user, UserManager<MusicTubeUser> userManager)
        {
            UserSettingsDto model = new UserSettingsDto();
            if (user.GetType().Name.Equals("Creator"))
            {
                user = userRepository.ReadCreatorInformation(user.Id);
                PremiumPlan premiumPlan = premiumPlanRepository.Read(((Creator)user).PremiumPlanId);
                model = new UserSettingsDto()
                {
                    CurrentUser = user,

                    Mail = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    ImageURL = user.ImageURL,
                    NewsletterSubscribed = user.NewsletterSubscribed,
                    FavouriteGenre = user.FavouriteGenre,
                    FavouriteArtist = user.FavouriteArtist,
                    FavouriteArtistId = user.FavouriteArtistId,
                    AllCreators = GetAllCreators(user.Id, userManager),

                    ArtistName = ((Creator)user).ArtistName,
                    ArtistDescription = ((Creator)user).ArtistDescription,
                    PremiumPlan = premiumPlan,
                    Fans = ((Creator)user).Fans,
                    Content = ((Creator)user).Content,
                    Reviews = null
                };
            } else
            {
                user = userRepository.ReadListenerInformation(user.Id);
                model = new UserSettingsDto()
                {
                    CurrentUser = user,

                    Mail = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    ImageURL = user.ImageURL,
                    NewsletterSubscribed = user.NewsletterSubscribed,
                    FavouriteGenre = user.FavouriteGenre,
                    FavouriteArtist = user.FavouriteArtist,
                    FavouriteArtistId = user.FavouriteArtistId,
                    AllCreators = GetAllCreators(user.Id, userManager),

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

        public void RemoveReview(Guid? reviewId)
        {
            Review review = reviewRepository.Read(reviewId);
            reviewRepository.Delete(review);
        }

        public void UpdateUserPersonalInformation(MusicTubeUser user, UserSettingsDto model)
        {
            if (model.Name != null && !model.Name.Equals(""))
                user.Name = model.Name;

            if (model.Surname != null && !model.Surname.Equals(""))
                user.Surname = model.Surname;

            if (model.ImageURL != null && !model.ImageURL.Equals(""))
                user.ImageURL = model.ImageURL;

            if (model.NewsletterSubscribed != null)
                user.NewsletterSubscribed = model.NewsletterSubscribed;

            if (model.FavouriteGenre != null)
                user.FavouriteGenre = model.FavouriteGenre;

            if(model.FavouriteArtistId != "0")
            {
                var favouriteArtist = userRepository.ReadUser(model.FavouriteArtistId);
                user.FavouriteArtist = (Creator)favouriteArtist;
                user.FavouriteArtistId = model.FavouriteArtistId;
            }

            if (model.ArtistName != null && !model.ArtistName.Equals(""))
                ((Creator)user).ArtistName = model.ArtistName;

            if (model.ArtistDescription != null && !model.ArtistDescription.Equals(""))
                ((Creator)user).ArtistDescription = model.ArtistDescription;

            userRepository.UpdateUser(user);
        }

        public PremiumDto GetPremiumDto(Creator user)
        {
            user = userRepository.ReadCreatorInformation(user.Id);

            PremiumDto model = new PremiumDto()
            {
                ArtistName = user.ArtistName,
                CreatorId = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Creator = user
            };

            return model;
        }

        public void SetPremium(Creator user, Int64 sum)
        {
            user = userRepository.ReadCreatorInformation(user.Id);

            SubscriptionPlan subPlan = SubscriptionPlan.BRONZE;

            if (sum == 10)
                subPlan = SubscriptionPlan.BRONZE;
            else if (sum == 20)
                subPlan = SubscriptionPlan.SILVER;
            else if (sum == 50)
                subPlan = SubscriptionPlan.GOLD;
            else if (sum == 150)
                subPlan = SubscriptionPlan.DIAMOND;

            if (user.PremiumPlan != null)
            {
                PremiumPlan premium = premiumPlanRepository.Read(user.PremiumPlanId);
                foreach (var album in user.PremiumPlan.Albums)
                    album.IsFromCurrentPlan = false; // making all previous added albums false, so we can count only the ones in this premium plan

                premium.SubscriptionPlan = subPlan;

                premiumPlanRepository.Update(premium);
            } else
            {
                PremiumPlan premiumPlan = new PremiumPlan()
                {
                    Id = Guid.NewGuid(),

                    SubscriptionPlan = subPlan,
                    Creator = user,
                    CreatorId = user.Id,
                    Albums = new List<Album>()
                };

                user.PremiumPlan = premiumPlan;
                user.PremiumPlanId = premiumPlan.Id;

                premiumPlanRepository.Create(premiumPlan);
            }

            userRepository.UpdateUser(user);
        }

        private List<Creator> GetAllCreators(string userId, UserManager<MusicTubeUser> userManager)
        {
            List<MusicTubeUser> allCreators = userRepository.GetAll();
            List<Creator> chosenCreators = new List<Creator>();
            foreach (var artist in allCreators)
            {
                foreach (var role in userManager.GetRolesAsync(artist).Result)
                {
                    if (role.Equals("Creator"))
                    { // not putting the current user in the list
                        if(userId != null)
                        {
                            if(!userId.Equals(artist.Id))
                            {
                                chosenCreators.Add((Creator)artist);
                                break;
                            }
                        } else
                        {
                            chosenCreators.Add((Creator)artist);
                            break;
                        }
                    }
                }
            }

            return chosenCreators;
        }

        public Creator ReadPremiumUser(Guid? userId)
        {
            var premium = premiumPlanRepository.Read(userId);
            var user = userRepository.ReadCreatorInformation(premium.CreatorId);
            return user;
        }

        public List<Creator> GetAllCreators()
        {
            List<MusicTubeUser> users = userRepository.GetAll()
                .Where(z => z.GetType().Name.Equals("Creator")).ToList();
            List<Creator> creators = new List<Creator>();

            foreach (var user in users)
                creators.Add((Creator)userRepository.ReadCreatorInformation(user.Id));

            return creators;
        }

        public Creator GetCreator(string artistId)
        {
            return userRepository.ReadCreatorInformation(artistId);
        }

        public List<Creator> FilterCreatorsByGenre(Genre genre)
        {
            List<MusicTubeUser> users = userRepository.GetAll()
                .Where(z => z.GetType().Name.Equals("Creator") && z.FavouriteGenre == genre).ToList();
            List<Creator> creators = new List<Creator>();

            foreach (var user in users)
                creators.Add((Creator)userRepository.ReadCreatorInformation(user.Id));

            return creators;
        }

        public String GetAlbumsAsString(Creator user)
        {
            PremiumPlan premiumPlan = user.PremiumPlan;
            
            if(premiumPlan == null)
            {
                return "The user has no premium subscription so he can upload albums";
            } else
            {
                if (premiumPlan.Albums != null && premiumPlan.Albums.Count != 0)
                {
                    StringBuilder sb = new StringBuilder();
                    var counter = 1;
                    foreach (var album in premiumPlan.Albums)
                    {
                        sb.AppendLine(counter + ". " + album.AlbumName + " - Genre: " + album.AlbumGenre);
                    }

                    return sb.ToString();
                }
                else
                {
                    return "The creator has no albums.";
                }
            }
        }

        public String GetSongsAsString(Creator user)
        {
            List<Media> medias = user.Content.Where(z => z.GetType().Name.Equals("Song")).ToList();
            List<Song> songs = new List<Song>();
            foreach (var media in medias)
                songs.Add((Song)media);

            if(songs != null && songs.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                var counter = 1;
                foreach (var song in songs)
                {
                    sb.AppendLine(counter + ". " + song.Name + " - Genre: " + song.Genre + ", Label: " + song.Label);
                    counter++;
                }

                return sb.ToString();
            } else
            {
                return "The creator has no songs.";
            }
        }

        public String GetVideosAsString(Creator user)
        {
            List<Media> medias = user.Content.Where(z => z.GetType().Name.Equals("Video")).ToList();
            List<Video> videos = new List<Video>();
            foreach (var media in medias)
                videos.Add((Video)media);

            if (videos != null && videos.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                var counter = 1;
                foreach (var video in videos)
                {
                    if(video.Song != null)
                    {
                        sb.AppendLine(counter + ". " + video.Name + ", Song: " + video.Song.Name);
                    } else
                    {
                        sb.AppendLine(counter + ". " + video.Name);
                    }
                    counter++;
                }

                return sb.ToString();
            }
            else
            {
                return "The creator has no songs.";
            }
        }
    }
}
