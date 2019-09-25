namespace Soccer.EventProcessors._Shared.Filters
{
    public interface IFilter<T>
    {
        T Filter(T data);
    }
}
