using System.Threading.Tasks;

namespace Soccer.EventProcessors._Shared.Filters
{
    public interface IFilter<in T, TResult>
    {
        Task<TResult> FilterAsync(T data);
    }
}