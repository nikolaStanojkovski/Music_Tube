using MusicTube.Domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Repository.Interface
{
    public interface IRepository<T> where T : BaseEntity
    {
        // For one entity

        void Create(T entity);

        T Read(Guid? id);

        void Update(T entity);

        void Delete(T entity);

        // For multiple entities

        void CreateAll(List<T> entities);

        List<T> ReadAll();

        void UpdateAll(List<T> entities);

        void DeleteAll(List<T> entities);
    }
}
