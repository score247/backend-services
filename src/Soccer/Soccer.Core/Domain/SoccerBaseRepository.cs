namespace Soccer.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Score247.Shared.Base;

    public class SoccerBaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly SoccerContext soccerContext;

        public SoccerBaseRepository(SoccerContext soccerContext)
        {
            this.soccerContext = soccerContext;
        }

        public virtual async Task<T> GetById(string id)
            => await soccerContext.Set<T>().FindAsync(id);

        public async Task<IReadOnlyList<T>> ListAll()
            => await soccerContext.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> List(ISpecification<T> spec)
            => await ApplySpecification(spec).ToListAsync();

        public async Task<int> Count(ISpecification<T> spec)
             => await ApplySpecification(spec).CountAsync();

        public async Task<T> Add(T entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.ModifiedTime = DateTime.UtcNow;

            await soccerContext.Set<T>().AddAsync(entity);
            await soccerContext.SaveChangesAsync();

            return entity;
        }

        public async Task<IEnumerable<T>> AddRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedTime = DateTime.UtcNow;
                entity.ModifiedTime = DateTime.UtcNow;
            }

            await soccerContext.Set<T>().AddRangeAsync(entities);
            await soccerContext.SaveChangesAsync();

            return entities;
        }

        public async Task Update(T entity)
        {
            entity.ModifiedTime = DateTime.UtcNow;

            soccerContext.Entry(entity).State = EntityState.Modified;

            await soccerContext.SaveChangesAsync();
        }

        public async Task UpdateRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.ModifiedTime = DateTime.UtcNow;
            }

            soccerContext.UpdateRange(entities);
            await soccerContext.SaveChangesAsync();
        }

        public async Task Delete(params object[] ids)
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