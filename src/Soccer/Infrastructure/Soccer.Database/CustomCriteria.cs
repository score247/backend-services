using System;
using Fanex.Data.Repository;

namespace Soccer.Database
{
    public abstract class CustomCriteria : CriteriaBase
    {
        protected CustomCriteria(DateTime eventDate = default)
        {
            EventDate = eventDate == default ? DateTime.Now : eventDate;
        }

        protected DateTime EventDate { get; }
    }
}
