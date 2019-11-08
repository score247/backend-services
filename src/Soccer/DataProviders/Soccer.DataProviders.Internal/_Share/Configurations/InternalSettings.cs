namespace Soccer.DataProviders.Internal._Share.Configurations
{
    public interface IInternalSettings
    {
        string ServiceUrl { get; }
    }

    public class InternalProviderSettings : IInternalSettings
    {
        public string ServiceUrl { get; set; }
    }
}