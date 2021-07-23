using MusicTube.Domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Repository.Interface
{
    public interface ISongRepository
    {
        // For one entity

        void CreateSong(Song entity);

        Song ReadSong(Guid? id);

        void UpdateSong(Song entity);

        void DeleteSong(Song entity);

        // For multiple entities

        List<Song> ReadAllSongs();
    }
}
