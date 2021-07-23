using Microsoft.AspNetCore.Identity;
using MusicTube.Domain.DTO;
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

        public UserRegistrationDto GetUserRegistrationDto(UserManager<MusicTubeUser> userManager);
    }
}
