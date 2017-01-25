using Klootzakken.Server.Model;

namespace Klootzakken.Server.ApiModel
{
    public class LobbyView
    {
        public LobbyView(Lobby src)
        {
            Id = src.Id;
            Name = src.Name;
            Owner = src.Owner;
            AllUsers = src.Users;
            Public = src.IsListed;
        }
        public string Id { get; }
        public string Name { get; }
        public User Owner { get; }
        public User[] AllUsers { get; }
        public bool Public { get; }
    }
}