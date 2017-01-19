using System.Collections.Generic;
using System.Linq;

namespace Klootzakken.Server.Model
{
    public class Player
    {
        public Player(User user, IEnumerable<Play> playsThisRound, IEnumerable<Card> cardsInHand, IEnumerable<Play> possibleActions, Rank newRank, Play exchangedCards)
        {
            User = user;
            PlaysThisRound = playsThisRound.ToArray();
            NewRank = newRank;
            ExchangedCards = exchangedCards;
            CardsInHand = cardsInHand.OrderBy(c => c.Value).ToArray();
            PossibleActions = possibleActions.ToArray();
        }

        public User User { get; }
        public Play[] PlaysThisRound { get; }
        public Rank NewRank { get; }
        public Card[] CardsInHand { get; }
        public Play[] PossibleActions { get; }
        public Play ExchangedCards { get; }
    }
}