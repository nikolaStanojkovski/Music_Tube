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
        private readonly ISongRepository songRepository;

        public AlbumService(IRepository<Album> _albumRepository,
            IUserRepository _userRepository,
            IRepository<PremiumPlan> _premiumPlanRepository,
            ISongRepository _songRepository)
        {
            this.albumRepository = _albumRepository;
            this.userRepository = _userRepository;
            this.premiumPlanRepository = _premiumPlanRepository;
            this.songRepository = _songRepository;
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

                IsFromCurrentPlan = true,

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

        public bool CheckAlbumLimit(Creator user)
        {
            PremiumPlan userPremiumPlan = premiumPlanRepository.Read(user.PremiumPlanId);
            user = userRepository.ReadCreatorInformation(user.Id);
            var noAlbumsUploaded = 0;
            if(user.PremiumPlan.Albums != null)
                noAlbumsUploaded = user.PremiumPlan.Albums.Where(z => z.IsFromCurrentPlan == true).Count();

            if (userPremiumPlan.SubscriptionPlan == Domain.Enumerations.SubscriptionPlan.BRONZE)
                return noAlbumsUploaded >= 2;
            else if (userPremiumPlan.SubscriptionPlan == Domain.Enumerations.SubscriptionPlan.SILVER)
                return noAlbumsUploaded >= 5;
            else if (userPremiumPlan.SubscriptionPlan == Domain.Enumerations.SubscriptionPlan.GOLD)
                return noAlbumsUploaded >= 15;
            else // diamond has unlimited uploads
                return false;
        }

        public List<Song> GetSongsForAlbum(Guid? albumId)
        {
            return songRepository.ReadAllSongs()
                .Where(z => z.AlbumId.Equals(albumId)).ToList();
        }
    }
}
