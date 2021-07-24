using MusicTube.Domain.Domain;
using MusicTube.Domain.Domain.Subdomain;
using MusicTube.Domain.DTO;
using MusicTube.Domain.Identity;
using MusicTube.Repository.Interface;
using MusicTube.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Service.Implementation
{
    public class AlbumService : IAlbumService
    {
        private readonly IRepository<Album> albumRepository;
        private readonly IRepository<PremiumPlan> premiumPlanRepository;
        private readonly IUserRepository userRepository;

        public AlbumService(IRepository<Album> _albumRepository,
            IUserRepository _userRepository,
            IRepository<PremiumPlan> _premiumPlanRepository)
        {
            this.albumRepository = _albumRepository;
            this.userRepository = _userRepository;
            this.premiumPlanRepository = _premiumPlanRepository;
        }

        public List<Album> GetAllAlbums()
        {
            return albumRepository.ReadAll();
        }

        public List<Album> GetAlbumsForUser(Creator user)
        {
            user = (Creator) userRepository.ReadCreatorInformation(user.Id);
            List<Album> userAlbums = albumRepository.ReadAll()
                .Where(z => z.PremiumUserId.Equals(user.PremiumPlanId))
                .ToList();

            return userAlbums;
        }

        public AlbumDto GetAlbumDto(Creator user)
        {
            return new AlbumDto()
            {
                PremiumUserId = user.PremiumPlanId
            };
        }

        public Album AddNewAlbum(Creator user, AlbumDto model)
        {
            user = userRepository.ReadCreatorInformation(user.Id);

            Album album = new Album()
            {
                Id = Guid.NewGuid(),

                AlbumArranger = model.AlbumArranger,
                AlbumComposer = model.AlbumComposer,
                AlbumGenre = model.AlbumGenre,
                AlbumMixMasterEngineer = model.AlbumMixMasterEngineer,
                AlbumName = model.AlbumName,
                AlbumProducer = model.AlbumProducer,
                AlbumCoverArt = model.AlbumCoverArt,

                PremiumUser = user.PremiumPlan,
                PremiumUserId = user.PremiumPlanId,
                Songs = new List<Song>()
            };

            albumRepository.Create(album);
            userRepository.UpdateUser(user);

            return album;
        }

        public Album DeleteAlbum(Guid? albumId)
        {
            Album album = albumRepository.Read(albumId);
            albumRepository.Delete(album);

            return album;
        }
    }
}
