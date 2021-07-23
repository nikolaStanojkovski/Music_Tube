using MusicTube.Domain.Domain;
using MusicTube.Domain.Domain.Subdomain;
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
        private readonly IRepository<Song> songRepository;

        public SongService(IRepository<Song> _songRepository,
            IUserRepository _userRepository)
        {
            this.songRepository = _songRepository;
            this.userRepository = _userRepository;
        }

        public List<Song> GetAllSongs()
        {
            return songRepository.ReadAll();
        }

        public Song CreateNewSong(Creator user, Song song, string songURL)
        {
            Song songToCreate = new Song
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

                AudioURL = song.AudioURL,

                Album = song.Album,
                AlbumId = song.Album.Id,
                VideosAppearedIn = new List<Video>()
            };

            user.Content.Add(songToCreate);

            userRepository.UpdateUser(user);
            songRepository.Create(songToCreate);

            return songToCreate;
        }
    }
}
