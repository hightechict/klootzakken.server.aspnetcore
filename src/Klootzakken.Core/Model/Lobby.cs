using System.Collections.Generic;

namespace Klootzakken.Server.Model
{
    public class Lobby : Entity
    {
        public Lobby(Entity srcEntity, IEnumerable<User> users, bool isListed) : base(srcEntity)
        {
            IsListed = isListed;
            Users = users.AsArray();
        }

        public Lobby(string id, string name, User owner, bool isListed) : base(id, name)
        {
            IsListed = isListed;
            Users = new[] {owner};
        }

        public User[] Users { get; }
        public bool IsListed { get; }
    }
}
