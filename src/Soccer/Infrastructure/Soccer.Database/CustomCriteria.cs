using System;
using Fanex.Data.Repository;

namespace Soccer.Database
{
    public abstract class CustomCriteria : CriteriaBase
    {
        protected CustomCriteria(DateTimeOffset eventDate = default)
        {
            EventDate = eventDate == default ? DateTimeOffset.Now : eventDate;
        }

        protected DateTimeOffset EventDate { get; }
    }
}
