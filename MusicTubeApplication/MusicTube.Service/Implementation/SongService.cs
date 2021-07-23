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
    public class SongService : ISongService
    {
        private readonly IUserRepository userRepository;
        private readonly ISongRepository songRepository;
        private readonly IRepository<Album> albumRepository;

        public SongService(ISongRepository _songRepository,
            IRepository<Album> _albumRepository,
            IUserRepository _userRepository)
        {
            this.songRepository = _songRepository;
            this.userRepository = _userRepository;
            this.albumRepository = _albumRepository;
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

        public Song CreateNewSong(Creator user, SongDto song, string songURL)
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
    }
}
