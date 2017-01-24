using Klootzakken.Server.Model;

namespace Klootzakken.Server.ApiModel
{
    public abstract class PlayerBase
    {
        protected PlayerBase(Player src)
        {
            User = src.User;
            PlaysThisRound = src.PlaysThisRound;
            NewRank = src.NewRank;
        }

        public User User { get; }
        public Play[] PlaysThisRound { get; }
        public Rank NewRank { get; }
    }
}