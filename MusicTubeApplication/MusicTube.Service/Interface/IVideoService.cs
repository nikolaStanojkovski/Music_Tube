using MusicTube.Domain.Domain;
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
    public interface IVideoService
    {
        public List<Video> GetAllVideos();

        public Video ReadVideo(Guid? videoId);

        public Task<Video> CreateVideo(Creator user, VideoDto video, String videoURL);

        public Task<Video> EditVideo(Creator user, VideoDto videoToEdit);

        public Task<Video> DeleteVideo(Guid? videoId);


        public VideoDto GetVideoDto(Creator user);

        public VideoDto GetDetailsDto(Guid? videoId);

        public VideoDto GetEditDto(Creator user, Guid? videoId);


        public void CreateFeedbackForVideo(MusicTubeUser user, Boolean liking, Guid videoId, String comment);

        public void UpdateReviewForVideo(Listener user, Guid videoId, String summary, String description, String rating);


        public List<Video> SearchVideos(String text);

        public List<Video> FilterVideos(Genre genreFilter, Guid? songFilter, String nameFilter, String descriptionFilter, String labelFilter);

        public List<Video> SortVideos(Boolean? sortCondition);


        public List<Video> GetVideosForArtist(string? artistId);
    }
}
