using System.Collections.Generic;
using System.Linq;

namespace Klootzakken.Server.Model
{
    public class Lobby
    {
        public Lobby(IEnumerable<User> users)
        {
            Users = users.ToArray();
        }

        public User[] Users { get; }
    }
}
