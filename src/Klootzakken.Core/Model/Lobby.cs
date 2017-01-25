using System.Collections.Generic;

namespace Klootzakken.Core.Model
{
    public class Lobby : Entity
    {
        public Lobby(Entity srcEntity, User owner, IEnumerable<User> users, bool isListed) : base(srcEntity)
        {
            IsListed = isListed;
            Users = users.AsArray();
            Owner = owner;
        }

        public Lobby(string id, string name, User owner, bool isListed) : base(id, name)
        {
            Owner = owner;
            IsListed = isListed;
            Users = new[] {owner};
        }

        public User Owner { get; }
        public User[] Users { get; }
        public bool IsListed { get; }
    }
}
