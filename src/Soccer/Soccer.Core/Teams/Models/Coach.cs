namespace Soccer.Core.Domain.Teams.Models
{
    using Score247.Shared.Base;

    public class Coach : BaseModel
    {
        public string Nationality { get; set; }

        public string CountryCode { get; set; }
    }
}