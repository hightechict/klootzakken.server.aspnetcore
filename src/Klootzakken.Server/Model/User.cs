using System;
using JetBrains.Annotations;

namespace Klootzakken.Server.Model
{
    public class User : IEquatable<User>
    {
        public bool Equals(User other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id, StringComparison.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as User;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.InvariantCulture.GetHashCode(Id);
        }

        public User([NotNull]string id, [NotNull]string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }

        [NotNull]
        public string DisplayName { get; }
        [NotNull]
        public string Id { get; }
    }
}