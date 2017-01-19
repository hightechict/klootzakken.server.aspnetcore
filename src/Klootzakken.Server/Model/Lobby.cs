using System.Collections.Generic;

namespace Klootzakken.Server.Model
{
    public class Lobby
    {
        public Lobby(IEnumerable<User> users)
        {
            Users = users.AsArray();
        }

        public User[] Users { get; }
    }
}
