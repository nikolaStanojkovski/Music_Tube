using MusicTube.Domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.DTO
{
    public class SongAlbumDto
    {
        public Guid SongId { get; set; }
        public Song Song { get; set; }

        public Guid AlbumId { get; set; }

        public List<Album> AllAlbums { get; set; }
    }
}
