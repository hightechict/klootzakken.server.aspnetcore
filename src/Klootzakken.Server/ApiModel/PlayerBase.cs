using System.Linq;
using Klootzakken.Server.Model;

namespace Klootzakken.Server.ApiModel
{
    public abstract class PlayerBase
    {
        protected PlayerBase(Player src)
        {
            User = src.User;
            PlaysThisRound = src.PlaysThisRound.Select(pl => new PlayView(pl)).ToArray();
            NewRank = src.NewRank.Serialized();
        }

        public User User { get; }
        public PlayView[] PlaysThisRound { get; }
        public string NewRank { get; }
    }
}