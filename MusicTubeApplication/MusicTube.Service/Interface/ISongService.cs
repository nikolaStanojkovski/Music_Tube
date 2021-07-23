using MusicTube.Domain.Domain;
using MusicTube.Domain.DTO;
using MusicTube.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Service.Interface
{
    public interface ISongService
    {
        public List<Song> GetAllSongs();

        public SongDto GetSongDto(Creator user);

        public Song CreateNewSong(Creator user, Song song, String songURL);
    }
}
