using System.Linq;
using Klootzakken.Server.Model;

namespace Klootzakken.Server.ApiModel
{
    public class You : PlayerBase
    {
        public You(Player src) : base(src)
        {
            CardsInHand = src.CardsInHand.Serialized();
            ExchangedCards = src.ExchangedCards?.PlayedCards?.Serialized();
            PossibleActions = src.PossibleActions.Select( pa => new PlayView(pa)).ToArray();
        }

        public string[] CardsInHand { get; }
        public PlayView[] PossibleActions { get; }
        public string[] ExchangedCards { get; }
    }
}