namespace Score247.Shared.Base
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// <![CDATA[https://enterprisecraftsmanship.com/2014/11/08/domain-object-base-class/]]>
    /// </summary>
    public abstract class BaseEntity
    {
        public string Id { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public DateTimeOffset ModifiedTime { get; set; }

        protected virtual object Actual => this;

        public override bool Equals(object obj)
        {
            var other = obj as BaseEntity;

            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Actual.GetType() != other.Actual.GetType())
                return false;

            if (Id?.Length == 0 || other.Id?.Length == 0)
                return false;

            return Id == other.Id;
        }

        public static bool operator ==(BaseEntity a, BaseEntity b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(BaseEntity a, BaseEntity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (Actual.GetType().ToString() + Id).GetHashCode();
        }
    }
}