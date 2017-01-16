using System.Linq;
using Klootzakken.Server;
using Klootzakken.Server.Model;
using Xunit;

namespace Klootzakker.Server.Tests
{
    public class LogicTests
    {
        [Fact]
        public void InitialGameStateHasNoCenterCard()
        {
            var sut = DealFourPlayerGame();
            Assert.Null(sut.CenterCard);
        }

        [Fact]
        public void InitialGameStateActivePlayerIsBetween0AndThree()
        {
            for (int q = 0; q < 250; q++)
                Assert.InRange(DealFourPlayerGame().ActivePlayer, 0, 3);
        }

        [Fact]
        public void InitialGameStateAllPlayersHaveEightCards()
        {
            var game = DealFourPlayerGame();
            Assert.All(game.Players, player => Assert.Equal(8, player.CardsInHand.Length));
        }

        [Fact]
        public void InitialGameStateAllPlayersHaveCardsOfSevenOrHigher()
        {
            var game = DealFourPlayerGame();
            Assert.All(game.Players,
                player => Assert.All(player.CardsInHand, c => Assert.InRange(c.Value, CardValue.Seven, CardValue.Ace)));
        }

        [Fact]
        public void InitialGameStateActivePlayerIsDistributedEvenly()
        {
            var timesStartPlayer = new int[4];
            for (int q = 0; q < 400; q++)
                timesStartPlayer[DealFourPlayerGame().ActivePlayer]++;
            Assert.All( timesStartPlayer, times => Assert.InRange(times, 75, 125));
        }

        [Fact]
        public void InitialGameStateActivePlayerHasSevenOfClubsInHand()
        {
            var game = DealFourPlayerGame();
            var activePlayer = game.Players[game.ActivePlayer];
            Assert.Contains(new Card(CardSuit.Clubs, CardValue.Seven), activePlayer.CardsInHand);
        }

        [Fact]
        public void InitialGameStateActivePlayerCannotPass()
        {
            var game = DealFourPlayerGame();
            var activePlayer = game.Players[game.ActivePlayer];
            Assert.DoesNotContain(new Play(new Card[0]), activePlayer.PossibleActions);
        }

        [Fact]
        public void InitialGameStateActivePlayerHasOptions()
        {
            var game = DealFourPlayerGame();
            var activePlayer = game.Players[game.ActivePlayer];
            Assert.NotEmpty(activePlayer.PossibleActions);
        }

        [Fact]
        public void StreetResultsInEightStartOptions()
        {
            var cardsInHand = HandOfOnlySingles;
            var possiblePlays = cardsInHand.StartOptions();
            Assert.Equal(8, possiblePlays.Length);
            Assert.All(possiblePlays, play => Assert.Equal(1, play.PlayedCards.Length));
        }

        [Fact]
        public void OnlyPairsResultsInTwelveStartOptions()
        {
            var cardsInHand = HandOfFourPairs;
            var possiblePlays = cardsInHand.StartOptions();
            Assert.Equal(12, possiblePlays.Length);
            Assert.Equal(8, possiblePlays.Count(play => play.PlayedCards.Length == 1));
            Assert.Equal(4, possiblePlays.Count(play => play.PlayedCards.Length == 2));
        }

        [Fact]
        public void TriplesAndAPairResultsInSeventeenStartOptions()
        {
            var cardsInHand = HandOfTwoTriplesAndAPair;
            var possiblePlays = cardsInHand.StartOptions();
            Assert.Equal(17, possiblePlays.Length);
            Assert.Equal(8, possiblePlays.Count(play => play.PlayedCards.Length == 1));
            Assert.Equal(7, possiblePlays.Count(play => play.PlayedCards.Length == 2));
            Assert.Equal(2, possiblePlays.Count(play => play.PlayedCards.Length == 3));
        }

        [Fact]
        public void TwoFoursResultsInThirtyStartOptions()
        {
            var cardsInHand = HandOfTwoFoursomes;
            var possiblePlays = cardsInHand.StartOptions();
            Assert.Equal(30, possiblePlays.Length);
            Assert.Equal(8, possiblePlays.Count(play => play.PlayedCards.Length == 1));
            Assert.Equal(12, possiblePlays.Count(play => play.PlayedCards.Length == 2));
            Assert.Equal(8, possiblePlays.Count(play => play.PlayedCards.Length == 3));
            Assert.Equal(2, possiblePlays.Count(play => play.PlayedCards.Length == 4));
        }

        private static Card[] HandOfOnlySingles
        {
            get { return Enumerable.Range(7, 8).Cast<CardValue>().Select(v => new Card(CardSuit.Hearts, v)).ToArray(); }
        }

        private static Card[] HandOfFourPairs
        {
            get
            {
                return Enumerable.Range(7, 4)
                    .Cast<CardValue>()
                    .SelectMany(v => new[] {new Card(CardSuit.Hearts, v), new Card(CardSuit.Spades, v)})
                    .ToArray();
            }
        }

        private static Card[] HandOfTwoTriplesAndAPair
        {
            get
            {
                return Enumerable.Range(7, 3)
                    .Cast<CardValue>()
                    .SelectMany(v => new[] { new Card(CardSuit.Hearts, v), new Card(CardSuit.Diamonds, v), new Card(CardSuit.Spades, v) })
                    .Take(8)
                    .ToArray();
            }
        }

        private static Card[] HandOfTwoFoursomes
        {
            get
            {
                return Enumerable.Range(7, 2)
                    .Cast<CardValue>()
                    .SelectMany(v => new[] { new Card(CardSuit.Hearts, v), new Card(CardSuit.Diamonds, v), new Card(CardSuit.Spades, v), new Card(CardSuit.Clubs, v) })
                    .ToArray();
            }
        }

        private static GameState DealFourPlayerGame()
        {
            var lobby = new Lobby(new[] {"HDB", "HDS", "HDM", "HDb"});
            var actual = lobby.Deal();
            return actual;
        }
    }
}
