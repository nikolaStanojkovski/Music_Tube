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
    public interface IAlbumService
    {
        public List<Album> GetAllAlbums();

        public List<Album> GetAlbumsForUser(Creator user);

        public AlbumDto GetAlbumDto(Creator user);

        public Album AddNewAlbum(Creator user, AlbumDto model);

        public Album DeleteAlbum(Guid? albumId);

        public Boolean CheckAlbumLimit(Creator user);

        public List<Song> GetSongsForAlbum(Guid? albumId);
    }
}
