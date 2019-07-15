using System;
using System.Collections.Generic;
using System.Text;

namespace Soccer.Core.Models
{
    public class Entity<TId, TName>
#pragma warning restore S1694 // An abstract class should have both abstract and concrete methods
    {
        public TId Id { get; set; }

        public TName Name { get; set; }
    }
}