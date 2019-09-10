using MessagePack;

namespace Score247.Shared.Base
{
    using System;
    using MessagePack;

    /// <summary>
    /// <![CDATA[https://enterprisecraftsmanship.com/2014/11/08/domain-object-base-class/]]>
    /// </summary>
    [MessagePackObject]
    public abstract class BaseEntity
    {
        protected BaseEntity()
        { }

        [Key(0)]
        public string Id { get; set; }

        [IgnoreMember]
        public DateTimeOffset CreatedTime { get; set; }

        [IgnoreMember]
        public DateTimeOffset ModifiedTime { get; set; }

        [IgnoreMember]
        protected virtual object Actual => this;

        public override bool Equals(object obj)
        {
            var other = obj as BaseEntity;

            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (Actual.GetType() != other.Actual.GetType())
            {
                return false;
            }

            return !(Id?.Length == 0 || other.Id?.Length == 0) && Id == other.Id;
        }

        public static bool operator ==(BaseEntity a, BaseEntity b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            return !(a is null || b is null) && a.Equals(b);
        }

        public static bool operator !=(BaseEntity a, BaseEntity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (Actual.GetType() + Id).GetHashCode();
        }
    }
}