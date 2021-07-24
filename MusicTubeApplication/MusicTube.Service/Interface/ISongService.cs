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

        public Song CreateSong(Creator user, SongDto song, String songURL);

        public Song ReadSong(Guid? songId);

        public Song DeleteSong(Guid? songId);


        public SongDto GetSongDto(Creator user);

        public SongAlbumDto GetSongAlbumDto(Creator user, Guid? songId);

        public void AddSongToAlbum(SongAlbumDto model);
    }
}
