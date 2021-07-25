using MusicTube.Domain.Domain;
using MusicTube.Domain.DTO;
using MusicTube.Domain.Enumerations;
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

        public void UpdateFeedbackForSong(MusicTubeUser user, Boolean liking, Guid songId, String comment);

        public void UpdateReviewForSong(Listener user, Guid songId, String summary, String description, String rating);


        public List<Song> SearchSongs(String text);

        public List<Song> FilterSongs(Genre genreFilter, String nameFilter, String descriptionFilter, String labelFilter);
    }
}
