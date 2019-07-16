namespace Soccer.Core.Domain.Teams.Models
{
    using Soccer.Core.Base;

    public class Coach : BaseEntity
    {
        public string Nationality { get; set; }

        public string CountryCode { get; set; }
    }
}