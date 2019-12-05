namespace Score247.Shared.Base
{
    using System;
    using MessagePack;

    /// <summary>
    /// <![CDATA[https://enterprisecraftsmanship.com/2014/11/08/domain-object-base-class/]]>
    /// </summary>
    [MessagePackObject(keyAsPropertyName: true)]
    public class BaseEntity
    {
        protected BaseEntity(string id)
        {
            Id = id;
        }

        public string Id { get; protected set; }

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

#pragma warning disable S3875 // "operator==" should not be overloaded on reference types

        public static bool operator ==(BaseEntity a, BaseEntity b)
#pragma warning restore S3875 // "operator==" should not be overloaded on reference types
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