using MusicTube.Domain.Domain;
using MusicTube.Domain.DTO;
using MusicTube.Domain.DTO.DomainDTO;
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

        public Task<Song> CreateSong(Creator user, SongDto song, String songURL);

        public Song ReadSong(Guid? songId);

        public Task<Song> EditSong(Creator user, SongDto songToEdit);

        public Task<Song> DeleteSong(Guid? songId);


        public SongDto GetCreateDto(Creator user);

        public SongDto GetEditDto(Creator user, Guid? songId);

        public SongDto GetDetailsDto(Guid? songId);

        public SongAlbumDto GetSongAlbumDto(Creator user, Guid? songId);

        public void AddSongToAlbum(SongAlbumDto model);

        public void CreateFeedbackForSong(MusicTubeUser user, Boolean liking, Guid songId, String comment);

        public void UpdateReviewForSong(Listener user, Guid songId, String summary, String description, String rating);


        public List<Song> SearchSongs(String text);

        public List<Song> FilterSongs(Genre genreFilter, String nameFilter, String descriptionFilter, String labelFilter);

        public List<Song> SortSongs(Boolean sortCondition);


        public List<Song> GetSongsForArtist(string? artistId);
    }
}
