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
    public interface IVideoService
    {
        public List<Video> GetAllVideos();

        public Video CreateVideo(Creator user, VideoDto video, String videoURL);

        public Video ReadVideo(Guid? videoId);

        public Video DeleteVideo(Guid? videoId);

        public VideoDto GetVideoDto(Creator user);

        /* public List<Video> SearchVideos(String text);

        public List<Video> FilterVideos(Genre genreFilter, String nameFilter, String descriptionFilter, String labelFilter);

        public List<Video> SortVideos(Boolean sortCondition); */


        public List<Video> GetVideosForArtist(string? artistId);
    }
}
