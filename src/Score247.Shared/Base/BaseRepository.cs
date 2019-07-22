namespace Score247.Shared.Base
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<T> GetById(string id);

        Task<IReadOnlyList<T>> ListAll();

        Task<IReadOnlyList<T>> List(ISpecification<T> spec);

        Task<int> Count(ISpecification<T> spec);

        Task<T> Add(T entity);

        Task<IEnumerable<T>> AddRange(IEnumerable<T> entities);

        Task Update(T entity);

        Task UpdateRange(IEnumerable<T> entities);

        Task Delete(params object[] ids);
    }
}