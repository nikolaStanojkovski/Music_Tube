using MusicTube.Domain.Domain;
using MusicTube.Domain.Domain.Subdomain;
using MusicTube.Domain.DTO;
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

        public SongService(ISongRepository _songRepository,
            IRepository<Album> _albumRepository,
            IUserRepository _userRepository,
            IRepository<UserFeedback> _feedbackRepository,
            IRepository<Review> _reviewRepository)
        {
            this.songRepository = _songRepository;
            this.userRepository = _userRepository;
            this.albumRepository = _albumRepository;
            this.feedbackRepository = _feedbackRepository;
            this.reviewRepository = _reviewRepository;
        }

        public List<Song> GetAllSongs()
        {
            return songRepository.ReadAllSongs();
        }

        public SongDto GetSongDto(Creator user)
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

        public Song CreateSong(Creator user, SongDto song, string songURL)
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
            } else
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

            return songToCreate;
        }

        public Song ReadSong(Guid? songId)
        {
            return songRepository.ReadSong(songId);
        }

        public Song DeleteSong(Guid? songId)
        {
            Song songToDelete = ReadSong(songId);
            songRepository.DeleteSong(songToDelete);

            string rootFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\custom\\files\\audio";
            File.Delete(Path.Combine(rootFolder, songToDelete.AudioURL)); // delete the file with the song in it

            return songToDelete;
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

        public void UpdateFeedbackForSong(MusicTubeUser user, bool liking, Guid songId, string comment)
        {
            user = userRepository.ReadUserInformation(user.Id);
            Song song = songRepository.ReadSong(songId);
            
            UserFeedback existingFeedback = feedbackRepository.ReadAll().Where(z => z.MediaId.Equals(songId) && z.UserId.Equals(user.Id)).SingleOrDefault();
            if (existingFeedback != null)
            {
                existingFeedback.IsLiked = liking;
                existingFeedback.IsDisliked = !liking;
                existingFeedback.Comment = comment;

                feedbackRepository.Update(existingFeedback);
            } else
            {
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
            }

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
                existingReview.Summary = summary;
                existingReview.Description = description;
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
    }
}
