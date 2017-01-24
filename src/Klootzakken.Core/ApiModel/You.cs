using Klootzakken.Server.Model;

namespace Klootzakken.Server.ApiModel
{
    public class You : PlayerBase
    {
        public You(Player src) : base(src)
        {
            ExchangedCards = src.ExchangedCards;
            CardsInHand = src.CardsInHand;
            PossibleActions = src.PossibleActions;
        }

        public Card[] CardsInHand { get; }
        public Play[] PossibleActions { get; }
        public Play ExchangedCards { get; }
    }
}