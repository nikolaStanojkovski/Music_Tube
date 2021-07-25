using Microsoft.EntityFrameworkCore;
using MusicTube.Domain.Identity;
using MusicTube.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<MusicTubeUser> entities;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<MusicTubeUser>();
        }

        public List<MusicTubeUser> GetAll()
        {
            return entities.ToListAsync().Result;
        }

        // One Entity

        public MusicTubeUser ReadUser(string id)
        {
            return entities
                .SingleOrDefaultAsync(z => z.Id.Equals(id)).Result;
        }

        public MusicTubeUser ReadUserInformation(string id)
        {
            return entities
                .Include(z => z.FavouriteArtist)
                .Include(z => z.Feedbacks)
                .SingleOrDefaultAsync(z => z.Id.Equals(id)).Result;
        }

        public Listener ReadListenerInformation(string id)
        {
            return (Listener)entities
                .Include(z => z.FavouriteArtist)
                .Include(z => z.Feedbacks)
                .Include(z => ((Listener)z).Reviews)
                .Include("Reviews.Media")
                .SingleOrDefaultAsync(z => z.Id.Equals(id)).Result;
        }

        public Creator ReadCreatorInformation(string id)
        {
            return (Creator)entities
                .Include(z => z.FavouriteArtist)
                .Include(z => z.Feedbacks)
                .Include(z => ((Creator)z).Fans)
                .Include(z => ((Creator)z).Content)
                .Include(z => ((Creator)z).PremiumPlan)
                .Include("PremiumPlan.Albums")
                .Include("Content.Reviews")
                .SingleOrDefaultAsync(z => z.Id.Equals(id)).Result;
        }

        public void CreateUser(MusicTubeUser entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Add(entity);
            context.SaveChanges();
        }

        public void UpdateUser(MusicTubeUser entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Update(entity);
            context.SaveChanges();
        }

        public void DeleteUser(MusicTubeUser entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Remove(entity);
            context.SaveChanges();
        }
    }
}
