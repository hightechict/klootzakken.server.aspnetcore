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

        public static GameState DealFirstGame(this Lobby lobby)
        {
            var playerCount = lobby.Users.Length;
            int playerThatGotLowestCard;
            var deal = DealCards(playerCount, out playerThatGotLowestCard);

            var startPlayer = playerThatGotLowestCard;
            var players = lobby.Users
                .Select((pl, i) => new Player(pl, new Play[0], deal[i].AsArray(), new Play[0], Rank.Unknown, null))
                .Select((pl, i) => i == startPlayer?pl.WithStartOptions():pl)
                .AsArray();
            return new GameState(GamePhase.Playing, players, null);
        }

        public static GameState Redeal(this GameState game)
        {
            var playerCount = game.Players.Length;
            int ignorePlayerThatGotLowestCard;
            var deal = DealCards(playerCount, out ignorePlayerThatGotLowestCard);
            var players = game.Players.Select((pl,i) => pl.WithNewHand(deal[i]).WithCardExchangeOptions()).AsArray();
            return new GameState(GamePhase.SwappingCards, players, null);
        }

        private static Player WithNewHand(this Player player, IEnumerable<Card> newCardsInHand)
        {
            return new Player(player.User, new Play[0], newCardsInHand, new Play[0], player.NewRank, null);
        }

        private static Player WithCardExchangeOptions(this Player player)
        {
            var action = new Play(player.GetCardsToExchange());
            return new Player(player.User, new Play[0], player.CardsInHand, new[] {action}, player.NewRank, null);
        }

        private static IEnumerable<Card> GetCardsToExchange(this Player player)
        {
            switch (player.NewRank)
            {
                case Rank.President:
                    return player.CardsInHand.Take(2);
                case Rank.VicePresident:
                    return player.CardsInHand.Take(1);
                case Rank.Neutraal:
                    return Enumerable.Empty<Card>();
                case Rank.ViezeKlootzak:
                    return player.CardsInHand.TakeLast(1);
                case Rank.Klootzak:
                    return player.CardsInHand.TakeLast(2);
                case Rank.Unknown:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static List<Card>[] DealCards(int playerCount, out int playerForCard)
        {
            var deck = new Stack<Card>(TopDownDeck.Take(8 * playerCount).Reverse());
            var deal = new List<Card>[playerCount];
            for (int playerNo = 0; playerNo < playerCount; playerNo++)
                deal[playerNo] = new List<Card>();

            playerForCard = 0;
            while (deck.Count != 0)
            {
                var dealtCard = deck.Pop();
                while (true)
                {
                    playerForCard = Random(playerCount);
                    if (deal[playerForCard].Count == 8)
                        continue;
                    deal[playerForCard].Add(dealtCard);
                    break;
                }
            }
            return deal;
        }

        public static Rank NextRank(this GameState game)
        {
            var position = game.Players.Count(pl => pl.NewRank != Rank.Unknown) + 1;
            if (position == 1) return Rank.President;
            if (position == 2 && game.Players.Length >= 4) return Rank.VicePresident;
            if (position == game.Players.Length - 1 && game.Players.Length >= 4) return Rank.ViezeKlootzak;
            if (position == game.Players.Length) return Rank.Klootzak;
            return Rank.Neutraal;
        }

        public static GameState WhenPlaying(this GameState game, User player, Play play)
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

        public static int FindPlayer(this GameState game, User user) => game.Players.FindSingle(pl => Equals(pl.User, user));
        public static Player GetPlayer(this GameState game, User user) => game.Players.Single(pl => Equals(pl.User, user));

        public static IEnumerable<Player> ReplacePlayer(this IEnumerable<Player> players, Player replacement) => players.Select(pl => pl.User.Equals(replacement.User) ? replacement : pl);

        public static GameState WhenPlayingSwappingGame(this GameState game, User user, Play play)
        {
            var playerAfterPlay = game.GetPlayer(user).ThatSwapped();
            var newPlayers = game.Players.ReplacePlayer(playerAfterPlay).AsArray();
            var thisWasNotTheLastPlayerToSwap = game.Players.Count(pl => pl.PossibleActions.Length != 0) > 1;
            if (thisWasNotTheLastPlayerToSwap)
                return new GameState(GamePhase.SwappingCards, newPlayers, null);

            var startPlayers = newPlayers.Select(pl => pl.TakeSwappedCards(newPlayers)).AsArray();
            return new GameState(GamePhase.Playing, startPlayers, null);
        }

        public static Player TakeSwappedCards(this Player player, IEnumerable<Player> allPlayers)
        {
            var oppositeRank = GetOppositeRank(player.NewRank);
            var oppositePlayer = allPlayers.First(pl => pl.NewRank == oppositeRank);
            var newPlayer = new Player(player.User, new Play[0],
                player.CardsInHand.Concat(oppositePlayer.ExchangedCards.PlayedCards), new Play[0], Rank.Unknown, null);
            return player.NewRank == Rank.Klootzak ? newPlayer.WithStartOptions() : newPlayer;
        }

        private static Rank GetOppositeRank(Rank playerNewRank)
        {
            switch (playerNewRank)
            {
                case Rank.Klootzak: return Rank.President;
                case Rank.ViezeKlootzak: return Rank.VicePresident;
                case Rank.Neutraal: return Rank.Neutraal;
                case Rank.VicePresident: return Rank.ViezeKlootzak;
                case Rank.President: return Rank.Klootzak;
                case Rank.Unknown:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Player ThatSwapped(this Player player)
        {
            var swappedCards = player.PossibleActions[0];
            return new Player(player.User, new Play[0], player.CardsInHand.Except(swappedCards.PlayedCards), new Play[0], player.NewRank, swappedCards);
        }

        public static GameState WhenPlayingEndedGame(this GameState game, User user, Play play)
        {
            if (!play.IsPass)
                throw new InvalidOperationException();
            return game.Redeal();
        }

        public static Player WhenGameEnded(this Player player)
        {
            return new Player(player.User, player.PlaysThisRound, player.CardsInHand, Play.PassOnly, player.NewRank == Rank.Unknown? Rank.Klootzak : player.NewRank, null);
        }

        public static GameState WhenPlayingActiveGame(this GameState game, User user, Play play)
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
                return new GameState(GamePhase.Ended, finalPlayers, game.CenterCard);
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

                return new GameState(GamePhase.Playing, players, newTopCard);
            }
            else
            {
                var players = tempPlayers.Select((pl, i) => i == newActivePlayer ? pl.WithOptions(lastProperPlay) : pl)
                    .AsArray();

                return new GameState(GamePhase.Playing, players, game.CenterCard);
            }
        }

        public static Player WithNewRank(this Player player, Rank newRank)
        {
            return new Player(player.User, player.PlaysThisRound, player.CardsInHand, player.PossibleActions, newRank, null);
        }

        public static int WhoPutLastCardsDown(this Player[] players)
        {
            return players.Select(
                    (p, i) =>
                        new {PlayerNo = i, HighestCard = p.PlaysThisRound.LastOrDefault()?.PlayedCards?.FirstOrDefault()?.Value ?? 0})
                .OrderByDescending(x => x.HighestCard)
                .First()
                .PlayerNo;
        }

        public static Player ThatPlayed(this Player player, Play play)
        {
            return new Player(player.User, player.PlaysThisRound.Concat(play).AsArray(), player.CardsInHand.Except(play.PlayedCards).AsArray(), new Play[0], Rank.Unknown, null);
        }

        public static Player WithStartOptions(this Player player)
        {
            var possibleActions = player.CardsInHand.StartOptions();
            return new Player(player.User, new Play[0], player.CardsInHand, possibleActions, Rank.Unknown, null);
        }

        public static Player WithOptions(this Player player, Play lastProperPlay)
        {
            var possibleActions = player.CardsInHand.Options(lastProperPlay);
            return new Player(player.User, player.PlaysThisRound, player.CardsInHand, possibleActions, Rank.Unknown, null);
        }

        public static Player StartNewRound(this Player player)
        {
            return new Player(player.User, new Play[0], player.CardsInHand, new Play[0], Rank.Unknown, null);
        }

        public static Play[] StartOptions(this Card[] cardsInHand)
        {
            return cardsInHand
                .GroupBy( card => card.Value)
                .SelectMany( AllPermutations)
                .Select( cardArray => new Play(cardArray))
                .Distinct()
                .AsArray();
        }

        public static Play[] Options(this Card[] cardsInHand, Play lastProperPlay)
        {
            return cardsInHand
                .Where(card => card.Value > lastProperPlay.PlayedCards[0].Value)
                .GroupBy(card => card.Value)
                .SelectMany(AllPermutations)
                .Where(cardArray => cardArray.Length == lastProperPlay.PlayedCards.Length)
                .Select(cardArray => new Play(cardArray))
                .Distinct()
                .Concat(Play.PassOnly)
                .AsArray();
        }

        private static IEnumerable<Card[]> AllPermutations(IEnumerable<Card> cardsOfSameValue)
        {
            var cards = cardsOfSameValue.AsArray();

            // Individual Cards
            foreach (var card in cards)
                yield return new[] {card};

            if (cards.Length >= 2)
                // All Cards
                yield return cards;

            if (cards.Length >= 3)
                // Skip only one
                for (var q = 0; q < cards.Length; q++)
                    yield return cards.Take(q).Concat(cards.Skip(q + 1)).AsArray();

            if (cards.Length == 4)
                // Take Two out of Four
                for (var q = 0; q < cards.Length; q++)
                for (var w = q + 1; w < cards.Length; w++)
                    yield return cards.Skip(q).Take(1).Concat(cards.Skip(w).Take(1)).AsArray();
        }

        private static Random RandomGenerator { get; } = new Random();

        private static int Random(int playerCount)
        {
            return RandomGenerator.Next(playerCount);
        }
    }
}