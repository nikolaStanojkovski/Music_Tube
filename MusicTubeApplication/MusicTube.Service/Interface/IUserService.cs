using Microsoft.AspNetCore.Identity;
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
    public interface IUserService
    {
        public MusicTubeUser CreateNewUser(UserRegistrationDto request);

        public UserRegistrationDto GetUserRegistrationDto(MusicTubeUser user, UserManager<MusicTubeUser> userManager);

        public UserSettingsDto GetUserSettings(MusicTubeUser user, UserManager<MusicTubeUser> userManager);

        public void UpdateUserPersonalInformation(MusicTubeUser user, UserSettingsDto model);

        public void RemoveReview(Guid? reviewId);

        public PremiumDto GetPremiumDto(Creator user);

        public void SetPremium(Creator user, Int64 sum);
    }
}
