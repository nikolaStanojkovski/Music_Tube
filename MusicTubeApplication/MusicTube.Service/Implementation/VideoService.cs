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
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository videoRepository;
        private readonly ISongRepository songRepository;
        private readonly IUserRepository userRepository;

        public VideoService(IVideoRepository _videoRepository,
            ISongRepository _songRepository,
            IUserRepository _userRepository)
        {
            this.videoRepository = _videoRepository;
            this.songRepository = _songRepository;
            this.userRepository = _userRepository;
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
    }
}
