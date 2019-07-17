namespace Soccer.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Score247.Shared.Base;

    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(string id);

        Task<IReadOnlyList<T>> ListAllAsync();

        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

        Task<int> CountAsync(ISpecification<T> spec);

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(params object[] ids);
    }

    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly SoccerContext soccerContext;

        public BaseRepository(SoccerContext soccerContext)
        {
            this.soccerContext = soccerContext;
        }

        public virtual async Task<T> GetByIdAsync(string id)
            => await soccerContext.Set<T>().FindAsync(id);

        public async Task<IReadOnlyList<T>> ListAllAsync()
            => await soccerContext.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
            => await ApplySpecification(spec).ToListAsync();

        public async Task<int> CountAsync(ISpecification<T> spec)
             => await ApplySpecification(spec).CountAsync();

        public async Task<T> AddAsync(T entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.ModifiedTime = DateTime.UtcNow;

            soccerContext.Set<T>().Add(entity);
            await soccerContext.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            entity.ModifiedTime = DateTime.UtcNow;

            soccerContext.Entry(entity).State = EntityState.Modified;

            await soccerContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(params object[] ids)
        {
            var table = soccerContext.Set<T>();
            T existing = await table.FindAsync(ids);

            if (existing != null)
            {
                table.Remove(existing);

                await soccerContext.SaveChangesAsync();
            }
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
            => SpecificationEvaluator<T>.GetQuery(soccerContext.Set<T>().AsQueryable(), spec);
    }
}