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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Service.Implementation
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository videoRepository;
        private readonly ISongRepository songRepository;
        private readonly IUserRepository userRepository;
        private readonly IRepository<UserFeedback> feedbackRepository;
        private readonly IRepository<Review> reviewRepository;

        public VideoService(IVideoRepository _videoRepository,
            ISongRepository _songRepository,
            IUserRepository _userRepository,
            IRepository<UserFeedback> _feedbackRepository,
            IRepository<Review> _reviewRepository)
        {
            this.videoRepository = _videoRepository;
            this.songRepository = _songRepository;
            this.userRepository = _userRepository;
            this.feedbackRepository = _feedbackRepository;
            this.reviewRepository = _reviewRepository;
        }

        public List<Video> GetAllVideos()
        {
            return videoRepository.ReadAllVideos();
        }

        public Video ReadVideo(Guid? videoId)
        {
            return videoRepository.ReadVideo(videoId);
        }

        public Video CreateVideo(Creator user, VideoDto video, string videoURL)
        {
            Song song = null;
            Video videoToCreate;

            user = userRepository.ReadCreatorInformation(user.Id);
            if (video.SongId != null && video.SongId != Guid.Empty)
            {
                song = songRepository.ReadSong(video.SongId);

                videoToCreate = new Video
                {
                    Id = Guid.NewGuid(),

                    // Media Specific
                    Name = video.Name,
                    Description = video.Description,
                    Label = video.Label,
                    Genre = video.Genre,
                    Creator = user,
                    CreatorId = user.Id,
                    Feedbacks = new List<UserFeedback>(),
                    Reviews = new List<Review>(),

                    // Video specific

                    VideoURL = videoURL,

                    Song = song,
                    SongId = song.Id
                };
            }
            else
            {
                videoToCreate = new Video
                {
                    Id = Guid.NewGuid(),

                    // Media Specific
                    Name = video.Name,
                    Description = video.Description,
                    Label = video.Label,
                    Genre = video.Genre,
                    Creator = user,
                    CreatorId = user.Id,
                    Feedbacks = new List<UserFeedback>(),
                    Reviews = new List<Review>(),

                    // Video specific

                    VideoURL = videoURL
                };
            }

            videoRepository.CreateVideo(videoToCreate);
            userRepository.UpdateUser(user);

            return videoToCreate;
        }

        public Video DeleteVideo(Guid? videoId)
        {
            Video video = ReadVideo(videoId);
            videoRepository.DeleteVideo(video);

            return video;
        }

        public List<Video> GetVideosForArtist(string artistId)
        {
            List<Video> videos = videoRepository.ReadAllVideos()
                .Where(z => z.CreatorId.Equals(artistId)).ToList();

            return videos;
        }

        public VideoDto GetVideoDto(Creator user)
        {
            user = userRepository.ReadCreatorInformation(user.Id);

            List<Song> currentUserSongs = songRepository.ReadAllSongs()
                .Where(z => z.CreatorId.Equals(user.Id)).ToList();

            return new VideoDto()
            {
                AllSongs = currentUserSongs,
                Creator = user
            };
        }

        public VideoDto GetDetailsDto(Guid? videoId)
        {
            var video = videoRepository.ReadVideo(videoId);

            return new VideoDto()
            {
                Id = video.Id,
                Name = video.Name,
                Label = video.Label,
                Description = video.Description,
                Creator = video.Creator,
                Feedbacks = video.Feedbacks,
                Genre = video.Genre,
                Reviews = video.Reviews,
                Song = video.Song,
                SongId = video.SongId,
                VideoURL = video.VideoURL
            };
        }

        public void CreateFeedbackForVideo(MusicTubeUser user, bool liking, Guid videoId, string comment)
        {
            user = userRepository.ReadUserInformation(user.Id);
            Video video = videoRepository.ReadVideo(videoId);

            UserFeedback newFeedback = new UserFeedback()
            {
                Id = Guid.NewGuid(),

                IsLiked = liking,
                IsDisliked = !liking,
                Comment = comment,

                User = user,
                UserId = user.Id,
                Media = video,
                MediaId = video.Id
            };
            user.Feedbacks.Add(newFeedback);

            feedbackRepository.Create(newFeedback);

            userRepository.UpdateUser(user);
            videoRepository.UpdateVideo(video);
        }

        public void UpdateReviewForVideo(Listener user, Guid videoId, string summary, string description, string rating)
        {
            user = userRepository.ReadListenerInformation(user.Id);
            Video video = videoRepository.ReadVideo(videoId);

            Review existingReview = reviewRepository.ReadAll().Where(z => z.MediaId.Equals(videoId)
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
                    Media = video,
                    MediaId = video.Id
                };
                user.Reviews.Add(newReview);

                reviewRepository.Create(newReview);
            }

            userRepository.UpdateUser(user);
            videoRepository.UpdateVideo(video);
        }

        public List<Video> SearchVideos(string text)
        {
            List<Video> allVideos = GetAllVideos();
            List<Video> filteredVideos = new List<Video>();

            if(text != null)
            {
                if (allVideos.Where(z => z.Name.Contains(text)).Count() != 0)
                    filteredVideos.AddRange(allVideos.Where(z => z.Name.Contains(text)).ToList());
                // name

                if (allVideos.Where(z => z.Description.Contains(text)).Count() != 0)
                    filteredVideos.AddRange(allVideos.Where(z => z.Description.Contains(text)
                    && filteredVideos.Where(t => t.Id.Equals(z.Id)).Count() == 0).ToList());
                // description

                if (allVideos.Where(z => z.Label.Contains(text)).Count() != 0)
                    filteredVideos.AddRange(allVideos.Where(z => z.Label.Contains(text)
                    && filteredVideos.Where(t => t.Id.Equals(z.Id)).Count() == 0).ToList());
                // label

                if (allVideos.Where(z => z.Genre.Equals(text)).Count() != 0)
                    filteredVideos.AddRange(allVideos.Where(z => z.Genre.Equals(text)
                    && filteredVideos.Where(t => t.Id.Equals(z.Id)).Count() == 0).ToList());
                // genre
            }

            return filteredVideos;
        }

        public List<Video> FilterVideos(Genre genreFilter, Guid? songFilter, string nameFilter, string descriptionFilter, string labelFilter)
        {
            List<Video> allVideos = GetAllVideos();
            List<Video> filteredVideos = new List<Video>();

            Song song = songRepository.ReadSong(songFilter);

            if (nameFilter == null)
                nameFilter = "";
            if (descriptionFilter == null)
                descriptionFilter = "";
            if (labelFilter == null)
                labelFilter = "";
            if (song != null)
            {
                filteredVideos = allVideos.Where(z => z.Genre == genreFilter
                        && z.Name.Contains(nameFilter)
                        && z.Description.Contains(descriptionFilter)
                        && z.Label.Contains(labelFilter)
                        && z.Song.Id.Equals(song.Id)).ToList();
            }
            else
            {
                filteredVideos = allVideos.Where(z => z.Genre == genreFilter
                        && z.Name.Contains(nameFilter)
                        && z.Description.Contains(descriptionFilter)
                        && z.Label.Contains(labelFilter)).ToList();
            }

            return filteredVideos;
        }

        public List<Video> SortVideos(Boolean? sortCondition)
        {
            List<Video> allVideos = GetAllVideos();
            List<Video> filteredVideos = new List<Video>();

            if(sortCondition != null)
            {
                if (sortCondition.Value)
                {
                    filteredVideos = allVideos.OrderByDescending(z => z.Feedbacks.Count).ToList();
                }
                else
                {
                    filteredVideos = allVideos.OrderByDescending(z => (z.Reviews != null &&
                        z.Reviews.Count != 0) ? z.Reviews.Average(t => t.Rating) : 0).ToList();
                }
            }

            return filteredVideos;
        }
    }
}
