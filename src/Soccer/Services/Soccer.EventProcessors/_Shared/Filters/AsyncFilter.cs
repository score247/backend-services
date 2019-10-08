using System.Threading.Tasks;

namespace Soccer.EventProcessors._Shared.Filters
{
    public interface IAsyncFilter<in T, TResult>
    {
        Task<TResult> Filter(T data);
    }
}
