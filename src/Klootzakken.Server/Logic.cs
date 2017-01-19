using System;
using System.Collections.Generic;
using System.Linq;
using Klootzakken.Server.Model;

namespace Klootzakken.Server
{
    public static class Logic
    {
        public static Card[] TopDownDeck { get; } = CreateTopDownDeck().AsArray();

        private static IEnumerable<Card> CreateTopDownDeck()
        {
            for (var cardValue = CardValue.Ace; cardValue >= CardValue.Two; cardValue--)
            {
                yield return new Card(CardSuit.Hearts, cardValue);
                yield return new Card(CardSuit.Spades, cardValue);
                yield return new Card(CardSuit.Diamonds, cardValue);
                yield return new Card(CardSuit.Clubs, cardValue);
            }
        }

        public static Game DealFirstGame(this Lobby lobby)
        {
            var playerCount = lobby.Users.Length;
            int playerThatGotLowestCard;
            var deal = DealCards(playerCount, out playerThatGotLowestCard);

            var startPlayer = playerThatGotLowestCard;
            var players = lobby.Users
                .Select((pl, i) => new Player(pl, new Play[0], deal[i].AsArray(), new Play[0], Rank.Unknown, null))
                .Select((pl, i) => i == startPlayer?pl.WithStartOptions():pl)
                .AsArray();
            return new Game(GamePhase.Playing, players, null);
        }

        public static Game Redeal(this Game game)
        {
            var playerCount = game.Players.Length;
            int ignorePlayerThatGotLowestCard;
            var deal = DealCards(playerCount, out ignorePlayerThatGotLowestCard);
            var players = game.Players.Select((pl,i) => pl.WithNewHand(deal[i]).WithCardExchangeOptions()).AsArray();
            return new Game(GamePhase.SwappingCards, players, null);
        }

        private static List<Card>[] DealCards(int playerCount, out int playerThatGotLastCard)
        {
            var deck = new Stack<Card>(TopDownDeck.Take(8 * playerCount).Reverse());
            var deal = new List<Card>[playerCount];
            for (int playerNo = 0; playerNo < playerCount; playerNo++)
                deal[playerNo] = new List<Card>();

            playerThatGotLastCard = 0;
            while (deck.Count != 0)
            {
                var dealtCard = deck.Pop();
                while (true)
                {
                    playerThatGotLastCard = Random(playerCount);
                    if (deal[playerThatGotLastCard].Count == 8)
                        continue;
                    deal[playerThatGotLastCard].Add(dealtCard);
                    break;
                }
            }
            return deal;
        }

        public static Rank NextRank(this Game game)
        {
            var position = game.Players.Count(pl => pl.NewRank != Rank.Unknown) + 1;
            if (position == 1) return Rank.President;
            if (position == 2 && game.Players.Length >= 4) return Rank.VicePresident;
            if (position == game.Players.Length - 1 && game.Players.Length >= 4) return Rank.ViezeKlootzak;
            if (position == game.Players.Length) return Rank.Klootzak;
            return Rank.Neutraal;
        }

        public static Game WhenPlaying(this Game game, User player, Play play)
        {
            switch (game.Phase)
            {
                case GamePhase.Playing:
                    return game.WhenPlayingActiveGame(player, play);
                case GamePhase.Ended:
                    return game.WhenPlayingEndedGame(player, play);
                case GamePhase.SwappingCards:
                    return game.WhenPlayingSwappingGame(player, play);
            }
            throw new ArgumentOutOfRangeException();
        }

        public static int FindPlayer(this Game game, User user) => game.Players.FindSingle(pl => Equals(pl.User, user));
        public static Player GetPlayer(this Game game, User user) => game.Players.Single(pl => Equals(pl.User, user));

        public static Game WhenPlayingSwappingGame(this Game game, User user, Play play)
        {
            var playerAfterPlay = game.GetPlayer(user).ThatSwapped();
            var newPlayers = game.Players.ReplacePlayer(playerAfterPlay).AsArray();
            var thisWasNotTheLastPlayerToSwap = game.Players.Count(pl => pl.PossibleActions.Length != 0) > 1;
            if (thisWasNotTheLastPlayerToSwap)
                return new Game(GamePhase.SwappingCards, newPlayers, null);

            var startPlayers = newPlayers.Select(pl => pl.TakeSwappedCards(newPlayers)).AsArray();
            return new Game(GamePhase.Playing, startPlayers, null);
        }

        public static Game WhenPlayingEndedGame(this Game game, User user, Play play)
        {
            if (!play.IsPass)
                throw new InvalidOperationException();
            return game.Redeal();
        }

        
        public static Game WhenPlayingActiveGame(this Game game, User user, Play play)
        {
            var playingPlayerNo = game.FindPlayer(user);
            var playingPlayer = game.Players[playingPlayerNo];
            if (!playingPlayer.PossibleActions.Contains(play))
                throw new InvalidOperationException();

            var changedPlayer = playingPlayer.ThatPlayed(play);
            if (!changedPlayer.CardsInHand.Any())
            {
                changedPlayer = changedPlayer.WithNewRank(game.NextRank());
            }

            var tempPlayers = game.Players.ReplacePlayer(changedPlayer).AsArray();

            var stillPlaying = tempPlayers.Where(pl => pl.NewRank == Rank.Unknown).ToList();
            if (stillPlaying.Count == 1)
            {
                var finalPlayers = tempPlayers.Select(pl => pl.WhenGameEnded());
                return new Game(GamePhase.Ended, finalPlayers, game.CenterCard);
            }

            var lastProperPlayPlayer = tempPlayers.WhoPutLastCardsDown();
            var lastProperPlay = tempPlayers[lastProperPlayPlayer].PlaysThisRound.Last();

            var nextPlayer = (playingPlayerNo + 1) % game.Players.Length;
            var roundEnded = lastProperPlayPlayer == nextPlayer;
            while (tempPlayers[nextPlayer].NewRank != Rank.Unknown)
            {
                nextPlayer = (nextPlayer + 1) % game.Players.Length;
                roundEnded |= lastProperPlayPlayer == nextPlayer;
            }

            var newActivePlayer = nextPlayer;
            if (roundEnded)
            {
                var players = tempPlayers.Select((pl, i) => pl.NewRank != Rank.Unknown ? pl : i == newActivePlayer ? pl.WithStartOptions() : pl.StartNewRound())
                    .AsArray();
                var newTopCard = lastProperPlay.PlayedCards.Last();

                return new Game(GamePhase.Playing, players, newTopCard);
            }
            else
            {
                var players = tempPlayers.Select((pl, i) => i == newActivePlayer ? pl.WithOptions(lastProperPlay) : pl)
                    .AsArray();

                return new Game(GamePhase.Playing, players, game.CenterCard);
            }
        }

        private static Random RandomGenerator { get; } = new Random();

        private static int Random(int playerCount)
        {
            return RandomGenerator.Next(playerCount);
        }
    }
}