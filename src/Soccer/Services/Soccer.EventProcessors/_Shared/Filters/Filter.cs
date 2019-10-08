namespace Soccer.EventProcessors._Shared.Filters
{
    public interface IFilter<in T, out TResult>
    {
        TResult Filter(T data);
    }
}