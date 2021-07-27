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
    public class VideoRepository : IVideoRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Video> entities;

        public VideoRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Video>();
        }

        public List<Video> ReadAllVideos()
        {
            return entities
                .Include(z => z.Reviews)
                .Include(z => z.Song)
                .Include(z => z.Creator)
                .ToListAsync().Result;
        }

        // One Entity

        public Video ReadVideo(Guid? id)
        {
            return entities
                .Include(z => z.Reviews)
                .Include(z => z.Song)
                .Include(z => z.Creator)
                .SingleOrDefaultAsync(z => z.Id.Equals(id)).Result;
        }

        public void CreateVideo(Video entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Add(entity);
            context.SaveChanges();
        }

        public void UpdateVideo(Video entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Update(entity);
            context.SaveChanges();
        }

        public void DeleteVideo(Video entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Remove(entity);
            context.SaveChanges();
        }
    }
}
