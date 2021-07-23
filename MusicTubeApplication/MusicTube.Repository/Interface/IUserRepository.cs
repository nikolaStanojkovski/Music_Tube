using MusicTube.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Repository.Interface
{
    public interface IUserRepository
    {
        List<MusicTubeUser> GetAll();

        // For one entity

        void CreateUser(MusicTubeUser entity);

        MusicTubeUser ReadUser(string? id);

        MusicTubeUser ReadUserInformation(string? id); // with included virtual properties

        Creator ReadCreatorInformation(string? id);

        void UpdateUser(MusicTubeUser entity);

        void DeleteUser(MusicTubeUser entity);
    }
}
