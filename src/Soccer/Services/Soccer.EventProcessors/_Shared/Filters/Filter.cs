using System.Threading.Tasks;

namespace Soccer.EventProcessors._Shared.Filters
{
    public interface IFilter<T>
    {
        Task<T> FilterAsync(T data);
    }
}