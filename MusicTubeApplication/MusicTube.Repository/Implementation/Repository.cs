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
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext context;
        private DbSet<T> entities;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            this.entities = context.Set<T>();
        }

        // For one entity

        public void Create(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            this.entities.Add(entity);
            context.SaveChanges();
        }

        public T Read(Guid? id)
        {
            return this.entities.SingleOrDefault(s => s.Id == id);
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            this.entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            this.entities.Remove(entity);
            context.SaveChanges();
        }

        // For multiple entities

        public void CreateAll(List<T> entities)
        {
            if (entities.Count == 0)
                throw new Exception("List is empty when trying to create entities in database");
            if (entities == null)
                throw new ArgumentNullException("entities");

            this.entities.AddRange(entities);
            context.SaveChanges();
        }

        public List<T> ReadAll()
        {
            return this.entities.ToListAsync().Result;
        }

        public void UpdateAll(List<T> entities)
        {
            if (entities.Count == 0)
                throw new Exception("List is empty when trying to update entities in database");
            if (entities == null)
                throw new ArgumentNullException("entities");

            this.entities.UpdateRange(entities);
            context.SaveChanges();
        }

        public void DeleteAll(List<T> entities)
        {
            if (entities.Count == 0)
                throw new Exception("List is empty when trying to delete entities in database");
            if (entities == null)
                throw new ArgumentNullException("entities");

            this.entities.RemoveRange(entities);
            context.SaveChanges();
        }
    }
}
