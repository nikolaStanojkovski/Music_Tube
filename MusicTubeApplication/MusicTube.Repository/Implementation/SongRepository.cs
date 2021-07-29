using Microsoft.EntityFrameworkCore;
using MusicTube.Domain.Domain;
using MusicTube.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Repository.Implementation
{
    public class SongRepository : ISongRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Song> entities;

        public SongRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Song>();
        }

        public List<Song> ReadAllSongs()
        {
            return entities
                .Include(z => z.Reviews)
                .Include("Reviews.Listener")
                .Include(z => z.Feedbacks)
                .Include("Feedbacks.User")
                .Include(z => z.Creator)
                .Include(z => z.Album)
                .ToListAsync().Result;
        }

        // One Entity

        public Song ReadSong(Guid? id)
        {
            return entities
                .Include(z => z.Reviews)
                .Include("Reviews.Listener")
                .Include(z => z.Feedbacks)
                .Include("Feedbacks.User")
                .Include(z => z.Creator)
                .SingleOrDefaultAsync(z => z.Id.Equals(id)).Result;
        }

        public void CreateSong(Song entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Add(entity);
            context.SaveChanges();
        }

        public void UpdateSong(Song entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Update(entity);
            context.SaveChanges();
        }

        public void DeleteSong(Song entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Remove(entity);
            context.SaveChanges();
        }
    }
}
