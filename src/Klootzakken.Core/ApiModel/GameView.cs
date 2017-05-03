using System.Linq;
using Klootzakken.Core.Model;

namespace Klootzakken.Core.ApiModel
{
    public class GameView
    {
        public GameView( Game src, User user)
        {
            You = new You(src.GetPlayer(user));
            var index = src.FindPlayer(user);
            Others =
                src.Players.Concat(src.Players)
                    .Skip(index + 1)
                    .Take(src.Players.Length - 1)
                    .Select(pl => new OtherPlayer(pl))
                    .ToArray();
            CenterCard = src.CenterCard?.Serialized();
        }

        public You You { get; }
        public OtherPlayer[] Others { get; }
        public string CenterCard { get; }
    }
}