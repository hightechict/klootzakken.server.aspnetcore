using Klootzakken.Server.Model;

namespace Klootzakken.Server.ApiModel
{
    public class OtherPlayer : PlayerBase
    {
        public OtherPlayer(Player src) : base(src)
        {
            CardCount = src.CardsInHand.Length;
            ExchangedCardsCount = src.ExchangedCards?.PlayedCards?.Length ?? 0;
            IsActive = src.PossibleActions.Length != 0;
        }

        public int CardCount { get; }
        public int ExchangedCardsCount { get; }
        public bool IsActive { get; set; }
    }
}