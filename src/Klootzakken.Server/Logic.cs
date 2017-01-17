using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Klootzakken.Server.Model;

namespace Klootzakken.Server
{
    public static class Logic
    {
        public static Card[] TopDownDeck { get; } = CreateTopDownDeck().ToArray();

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

        public static GameState Deal(this Lobby lobby)
        {
            var playerCount = lobby.Players.Length;
            var deck = new Stack<Card>(TopDownDeck.Take(8*playerCount).Reverse());
            var deal = new List<Card>[playerCount];
            for(int playerNo = 0; playerNo<playerCount; playerNo++)
                deal[playerNo] = new List<Card>();

            int playerForCard = 0;
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

            var startPlayer = playerForCard;
            var players = lobby.Players
                .Select((pl, i) => new YourPlayer(pl, new Play[0], deal[i].ToArray(), new Play[0], Rank.Unknown))
                .Select((pl, i) => i == startPlayer?pl.WithStartOptions():pl)
                .ToArray();
            return new GameState(players, null, startPlayer);
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

        public static GameState WhenPlaying(this GameState game, Play play)
        {
            var playingPlayerNo = game.ActivePlayer;
            var playingPlayer = game.Players[playingPlayerNo];
            if (!playingPlayer.PossibleActions.Contains(play))
                throw new InvalidOperationException();

            var temp = playingPlayer.ThatPlayed(play);
            if (!temp.CardsInHand.Any())
            {
                temp = temp.WithNewRank(game.NextRank());
            }

            var tempPlayers = game.Players.Select((pl, i) => i == playingPlayerNo ? temp : pl).ToArray();

            var stillPlaying = tempPlayers.Where(pl => pl.NewRank == Rank.Unknown).ToList();
            if (stillPlaying.Count == 1)
            {
                var finalPlayers = tempPlayers.Select((pl, i) => pl.NewRank == Rank.Unknown? pl.WithNewRank(Rank.Klootzak):pl).ToArray();
                return new GameState(finalPlayers, game.CenterCard, 0);
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
                    .ToArray();
                var newTopCard = lastProperPlay.PlayedCards.Last();

                return new GameState(players, newTopCard, newActivePlayer);
            }
            else
            {
                var players = tempPlayers.Select((pl, i) => i == newActivePlayer ? pl.WithOptions(lastProperPlay) : pl)
                    .ToArray();

                return new GameState(players, game.CenterCard, newActivePlayer);
            }
        }

        public static YourPlayer WithNewRank(this YourPlayer player, Rank newRank)
        {
            return new YourPlayer(player.Name, player.PlaysThisRound, player.CardsInHand, player.PossibleActions, newRank);
        }

        public static int WhoPutLastCardsDown(this YourPlayer[] players)
        {
            return players.Select(
                    (p, i) =>
                        new {PlayerNo = i, HighestCard = p.PlaysThisRound.LastOrDefault()?.PlayedCards?.FirstOrDefault()?.Value ?? 0})
                .OrderByDescending(x => x.HighestCard)
                .First()
                .PlayerNo;
        }

        public static YourPlayer ThatPlayed(this YourPlayer player, Play play)
        {
            return new YourPlayer(player.Name, player.PlaysThisRound.Concat(play).ToArray(), player.CardsInHand.Except(play.PlayedCards).ToArray(), new Play[0], Rank.Unknown);
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> src, T item)
        {
            return src.Concat(Enumerable.Repeat(item, 1));
        }

        public static YourPlayer WithStartOptions(this YourPlayer player)
        {
            var possibleActions = player.CardsInHand.StartOptions();
            return new YourPlayer(player.Name, new Play[0], player.CardsInHand, possibleActions, Rank.Unknown );
        }

        public static YourPlayer WithOptions(this YourPlayer player, Play lastProperPlay)
        {
            var possibleActions = player.CardsInHand.Options(lastProperPlay);
            return new YourPlayer(player.Name, player.PlaysThisRound, player.CardsInHand, possibleActions, Rank.Unknown);
        }

        public static YourPlayer StartNewRound(this YourPlayer player)
        {
            return new YourPlayer(player.Name, new Play[0], player.CardsInHand, new Play[0], Rank.Unknown);
        }

        public static Play[] StartOptions(this Card[] cardsInHand)
        {
            return cardsInHand
                .GroupBy( card => card.Value)
                .SelectMany( AllPermutations)
                .Select( cardArray => new Play(cardArray))
                .Distinct()
                .ToArray();
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
                .Concat(Enumerable.Repeat(Play.Pass, 1))
                .ToArray();
        }

        private static IEnumerable<Card[]> AllPermutations(IEnumerable<Card> cardsOfSameValue)
        {
            var cards = cardsOfSameValue.ToArray();
            // Individual Cards
            foreach (var card in cards)
                yield return new[] {card};

            if (cards.Length != 1)
                // All Cards
                yield return cards;

            if (cards.Length >= 3)
                // Skip only one
                for (int q = 0; q < cards.Length; q++)
                    yield return cards.Take(q).Concat(cards.Skip(q + 1)).ToArray();

            if (cards.Length == 4)
                // Take Two out of Four
                for (int q = 0; q < cards.Length; q++)
                for (int w = q + 1; w < cards.Length; w++)
                    yield return cards.Skip(q).Take(1).Concat(cards.Skip(w).Take(1)).ToArray();
        }

        private static Random RandomGenerator { get; } = new Random();

        private static int Random(int playerCount)
        {
            return RandomGenerator.Next(playerCount);
        }
    }
}