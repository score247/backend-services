namespace Soccer.DataProviders.EyeFootball._Shared.Configurations
{
    public interface IEyeFootballSettings
    {
        string ServiceUrl { get; }
    }

    public class EyeFootballSettings : IEyeFootballSettings
    {
        public string ServiceUrl { get; set; }
    }
}
