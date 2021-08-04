using MusicTube.Domain;
using MusicTube.Domain.Domain;
using MusicTube.Domain.Domain.Subdomain;
using MusicTube.Domain.DTO;
using MusicTube.Domain.DTO.DomainDTO;
using MusicTube.Domain.Enumerations;
using MusicTube.Domain.Identity;
using MusicTube.Repository.Interface;
using MusicTube.Service.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Service.Implementation
{
    public class SongService : ISongService
    {
        private readonly IUserRepository userRepository;
        private readonly ISongRepository songRepository;
        private readonly IRepository<Album> albumRepository;
        private readonly IRepository<UserFeedback> feedbackRepository;
        private readonly IRepository<Review> reviewRepository;

        // for newsletters
        private readonly IRepository<EmailMessage> emailRepository;
        private readonly EmailSettings emailSettings;
        private readonly BackgroundEmailSender emailSender;

        public SongService(ISongRepository _songRepository,
            IRepository<Album> _albumRepository,
            IUserRepository _userRepository,
            IRepository<UserFeedback> _feedbackRepository,
            IRepository<Review> _reviewRepository,

            EmailSettings _emailSettings,
            IRepository<EmailMessage> _emailRepository)
        {
            this.songRepository = _songRepository;
            this.userRepository = _userRepository;
            this.albumRepository = _albumRepository;
            this.feedbackRepository = _feedbackRepository;
            this.reviewRepository = _reviewRepository;

            this.emailRepository = _emailRepository;
            this.emailSettings = _emailSettings;
            this.emailSender = new BackgroundEmailSender(new EmailService(_emailSettings), emailRepository);
        }

        public List<Song> GetAllSongs()
        {
            return songRepository.ReadAllSongs();
        }

        public Song ReadSong(Guid? songId)
        {
            return songRepository.ReadSong(songId);
        }

        public async Task<Song> CreateSong(Creator user, SongDto song, string songURL)
        {
            Album album = null;
            Song songToCreate;
            if (song.AlbumId != Guid.Empty)
            {
                album = albumRepository.Read(song.AlbumId);

                songToCreate = new Song
                {
                    Id = Guid.NewGuid(),

                    // Media Specific
                    Name = song.Name,
                    Description = song.Description,
                    Label = song.Label,
                    Genre = song.Genre,
                    Creator = user,
                    CreatorId = user.Id,
                    Feedbacks = new List<UserFeedback>(),
                    Reviews = new List<Review>(),

                    // Song specific

                    AudioURL = songURL,

                    Album = album,
                    AlbumId = song.AlbumId,
                    VideosAppearedIn = new List<Video>()
                };
            }
            else
            {
                songToCreate = new Song
                {
                    Id = Guid.NewGuid(),

                    // Media Specific
                    Name = song.Name,
                    Description = song.Description,
                    Label = song.Label,
                    Genre = song.Genre,
                    Creator = user,
                    CreatorId = user.Id,
                    Feedbacks = new List<UserFeedback>(),
                    Reviews = new List<Review>(),

                    // Song specific

                    AudioURL = songURL,

                    VideosAppearedIn = new List<Video>()
                };
            }

            songRepository.CreateSong(songToCreate);
            userRepository.UpdateUser(user);

            await SendMail(songToCreate, "created");

            return songToCreate;
        }

        public async Task<Song> EditSong(Creator user, SongDto songToEdit)
        {
            user = userRepository.ReadCreatorInformation(user.Id);
            Song song = songRepository.ReadSong(songToEdit.Id);

            if (songToEdit.Name != null && !songToEdit.Name.Equals(""))
                song.Name = songToEdit.Name;

            if (songToEdit.Label != null && !songToEdit.Label.Equals(""))
                song.Label = songToEdit.Label;

            if (songToEdit.Description != null && !songToEdit.Description.Equals(""))
                song.Description = songToEdit.Description;

            if (songToEdit.Genre != null)
                song.Genre = songToEdit.Genre;

            if (songToEdit.AlbumId != null)
            {
                Album album = albumRepository.Read(songToEdit.AlbumId);
                if (song != null)
                {
                    song.Album = album;
                    song.AlbumId = song.AlbumId;
                }
            }

            songRepository.UpdateSong(song);
            await SendMail(song, "updated");

            return song;
        }

        public async Task<Song> DeleteSong(Guid? songId)
        {
            Song songToDelete = ReadSong(songId);
            songRepository.DeleteSong(songToDelete);

            string rootFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\custom\\files\\audio";
            File.Delete(Path.Combine(rootFolder, songToDelete.AudioURL)); // delete the file with the song in it

            await SendMail(songToDelete, "deleted");

            return songToDelete;
        }



        public SongDto GetCreateDto(Creator user)
        {
            List<Album> currentUserAlbums = new List<Album>();
            user = userRepository.ReadCreatorInformation(user.Id);
            if (user.PremiumPlan != null)
                currentUserAlbums = albumRepository.ReadAll()
                    .Where(z => z.PremiumUserId.Equals(user.PremiumPlanId))
                    .ToList();

            return new SongDto()
            {
                AllAlbums = currentUserAlbums,
                Creator = user
            };
        }

        public SongDto GetDetailsDto(Guid? songId)
        {
            var song = songRepository.ReadSong(songId);
            Album album = null;
            if(song.AlbumId != null)
                album = albumRepository.Read(song.AlbumId);

            if(album != null)
            {
                return new SongDto()
                {
                    Id = song.Id,
                    Name = song.Name,
                    Label = song.Label,
                    Description = song.Description,
                    Creator = song.Creator,
                    Feedbacks = song.Feedbacks,
                    Genre = song.Genre,
                    Reviews = song.Reviews,
                    Album = album,
                    AlbumId = album.Id,
                    AudioURL = song.AudioURL
                };
            } else
            {
                return new SongDto()
                {
                    Id = song.Id,
                    Name = song.Name,
                    Label = song.Label,
                    Description = song.Description,
                    Creator = song.Creator,
                    Feedbacks = song.Feedbacks,
                    Genre = song.Genre,
                    Reviews = song.Reviews,
                    AudioURL = song.AudioURL
                };
            }
        }

        public SongDto GetEditDto(Creator user, Guid? songId)
        {
            user = userRepository.ReadCreatorInformation(user.Id);
            Song existingSong = ReadSong(songId);
            List<Album> allAlbums = albumRepository.ReadAll()
                .Where(z => z.PremiumUserId.Equals(user.PremiumPlanId)).ToList();

            return new SongDto()
            {
                Id = existingSong.Id,
                Name = existingSong.Name,
                Label = existingSong.Label,
                Description = existingSong.Description,
                Genre = existingSong.Genre,
                Creator = user,
                AudioURL = existingSong.AudioURL,

                AlbumId = existingSong.AlbumId,
                Album = existingSong.Album,

                AllAlbums = allAlbums
            };
        }

        public SongAlbumDto GetSongAlbumDto(Creator user, Guid? songId)
        {
            Song song = songRepository.ReadSong(songId);
            List<Album> allAlbums = albumRepository.ReadAll()
                .Where(z => z.PremiumUserId.Equals(user.PremiumPlanId))
                .ToList();

            return new SongAlbumDto()
            {
                AllAlbums = allAlbums,
                Song = song,
                SongId = songId.Value
            };
        }



        public void AddSongToAlbum(SongAlbumDto model)
        {
            Song songToAdd = songRepository.ReadSong(model.SongId);
            Album album = albumRepository.Read(model.AlbumId);
            if(album != null)
            {
                songToAdd.Album = album;
                songToAdd.AlbumId = album.Id;

                songRepository.UpdateSong(songToAdd);
                albumRepository.Update(album);
            }
        }

        public void CreateFeedbackForSong(MusicTubeUser user, bool liking, Guid songId, string comment)
        {
            user = userRepository.ReadUserInformation(user.Id);
            Song song = songRepository.ReadSong(songId);

            UserFeedback newFeedback = new UserFeedback()
            {
                Id = Guid.NewGuid(),

                IsLiked = liking,
                IsDisliked = !liking,
                Comment = comment,

                User = user,
                UserId = user.Id,
                Media = song,
                MediaId = song.Id
            };
            user.Feedbacks.Add(newFeedback);

            feedbackRepository.Create(newFeedback);

            userRepository.UpdateUser(user);
            songRepository.UpdateSong(song);
        }

        public void UpdateReviewForSong(Listener user, Guid songId, string summary, string description, string rating)
        {
            user = userRepository.ReadListenerInformation(user.Id);
            Song song = songRepository.ReadSong(songId);

            Review existingReview = reviewRepository.ReadAll().Where(z => z.MediaId.Equals(songId) 
                    && z.ListenerId.Equals(user.Id)).SingleOrDefault();
            if (existingReview != null)
            {
                if(summary != null && !summary.Equals(""))
                    existingReview.Summary = summary;

                if(description != null && !description.Equals(""))
                    existingReview.Description = description;

                if(rating != null && !rating.Equals("") && !rating.Equals("0"))
                    existingReview.Rating = Int64.Parse(rating);

                reviewRepository.Update(existingReview);
            }
            else
            {
                Review newReview = new Review()
                {
                    Id = Guid.NewGuid(),

                    Summary = summary,
                    Description = description,
                    Rating = Int64.Parse(rating),

                    Listener = user,
                    ListenerId = user.Id,
                    Media = song,
                    MediaId = song.Id
                };
                user.Reviews.Add(newReview);

                reviewRepository.Create(newReview);
            }

            userRepository.UpdateUser(user);
            songRepository.UpdateSong(song);
        }


        public List<Song> SearchSongs(string text)
        {
            List<Song> allSongs = GetAllSongs();
            List<Song> filteredSongs = new List<Song>();

            if (allSongs.Where(z => z.Name.Contains(text)).Count() != 0)
                filteredSongs.AddRange(allSongs.Where(z => z.Name.Contains(text)).ToList());
            // name

            if (allSongs.Where(z => z.Description.Contains(text)).Count() != 0)
                filteredSongs.AddRange(allSongs.Where(z => z.Description.Contains(text)
                && filteredSongs.Where(t => t.Id.Equals(z.Id)).Count() == 0).ToList());
            // description

            if (allSongs.Where(z => z.Label.Contains(text)).Count() != 0)
                filteredSongs.AddRange(allSongs.Where(z => z.Label.Contains(text)
                && filteredSongs.Where(t => t.Id.Equals(z.Id)).Count() == 0).ToList());
            // label

            if (allSongs.Where(z => z.Genre.Equals(text)).Count() != 0)
                filteredSongs.AddRange(allSongs.Where(z => z.Genre.Equals(text)
                && filteredSongs.Where(t => t.Id.Equals(z.Id)).Count() == 0).ToList());
            // genre

            return filteredSongs;
        }

        public List<Song> FilterSongs(Genre genreFilter, String nameFilter, String descriptionFilter, String labelFilter)
        {
            List<Song> allSongs = GetAllSongs();
            List<Song> filteredSongs = new List<Song>();

            if (nameFilter == null)
                nameFilter = "";
            if (descriptionFilter == null)
                descriptionFilter = "";
            if (labelFilter == null)
                labelFilter = "";

            filteredSongs = allSongs.Where(z => z.Genre == genreFilter
                        && z.Name.Contains(nameFilter)
                        && z.Description.Contains(descriptionFilter)
                        && z.Label.Contains(labelFilter)).ToList();

            return filteredSongs;
        }

        public List<Song> SortSongs(Boolean sortCondition) // popularity = true, rating = false
        {
            List<Song> allSongs = GetAllSongs();
            List<Song> filteredSongs = new List<Song>();

            if(sortCondition)
            {
                filteredSongs = allSongs.OrderByDescending(z => z.Feedbacks.Count).ToList();
            } else
            {
                filteredSongs = allSongs.OrderByDescending(z => (z.Reviews != null && 
                    z.Reviews.Count != 0 ) ? z.Reviews.Average(t => t.Rating) : 0).ToList();
            }

            return filteredSongs;
        }


        public List<Song> GetSongsForArtist(string artistId)
        {
            return GetAllSongs().Where(z => z.CreatorId.Equals(artistId)).ToList();
        }
    
        
        private async Task<bool> SendMail(Song song, String action)
        {
            List<MusicTubeUser> subscribedUsers = userRepository.GetAll().Where(z => z.NewsletterSubscribed && z.FavouriteArtistId != null && z.FavouriteArtistId.Equals(song.CreatorId)).ToList();
            if (subscribedUsers != null && subscribedUsers.Count != 0)
            {
                StringBuilder content = new StringBuilder();
                content.AppendLine("Artist name: " + song.Creator.Name);
                content.AppendLine("Song name: " + song.Name);
                content.AppendLine("Song description: " + song.Description);
                content.AppendLine("Label published on: " + song.Label);

                foreach (var musicTubeUser in subscribedUsers)
                {
                    EmailMessage message = new EmailMessage()
                    {
                        Id = Guid.NewGuid(),
                        MailTo = musicTubeUser.Email,
                        Subject = "Song " + action + " by " + song.Creator.Name + " " + song.Creator.Surname + " (" + song.Creator.ArtistName + ")",
                        Status = false,
                        Content = content.ToString()
                    };

                    emailRepository.Create(message);
                } // send the mail to everyone subscribed
            }

            await emailSender.DoWork();

            return true;
        }

    }
}
