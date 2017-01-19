using Klootzakken.Server.Model;

namespace Klootzakken.Server.ApiModel
{
    public class LobbyView
    {
        public string Id { get; }
        public string Name { get; }
        public User Owner { get; }
        public User[] AllUsers { get; }
    }
}