using MusicTube.Domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Repository.Interface
{
    public interface IVideoRepository
    {
        // For one entity

        void CreateVideo(Video entity);

        Video ReadVideo(Guid? id);

        void UpdateVideo(Video entity);

        void DeleteVideo(Video entity);

        // For multiple entities

        List<Video> ReadAllVideos();
    }
}
