using System;
using System.Collections.Generic;
using System.Linq;
using Klootzakken.Core.Model;

namespace Klootzakken.Core
{
    public static class PlayerExtensions
    {
        public static Player WithNewHand(this Player player, IEnumerable<Card> newCardsInHand)
        {
            return new Player(player.User, new Play[0], newCardsInHand, new Play[0], player.NewRank, null);
        }

        public static Player WithCardExchangeOptions(this Player player)
        {
            var action = new Play(player.GetCardsToExchange());
            return new Player(player.User, new Play[0], player.CardsInHand, new[] { action }, player.NewRank, null);
        }

        public static Rank NextRank(this IEnumerable<Player> playersx)
        {
            var players = playersx.AsArray();
            var position = players.Count(pl => pl.NewRank != Rank.Unknown) + 1;
            if (position == 1) return Rank.President;
            if (position == 2 && players.Length >= 4) return Rank.VicePresident;
            if (position == players.Length - 1 && players.Length >= 4) return Rank.ViezeKlootzak;
            if (position == players.Length) return Rank.Klootzak;
            return Rank.Neutraal;
        }

        public static IEnumerable<Card> GetCardsToExchange(this Player player)
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


        public static Player WithNewRank(this Player player, Rank newRank)
        {
            return new Player(player.User, player.PlaysThisRound, player.CardsInHand, player.PossibleActions, newRank, null);
        }

        public static int WhoPutLastCardsDown(this Player[] players)
        {
            return players.Select(
                    (p, i) =>
                        new { PlayerNo = i, HighestCard = p.PlaysThisRound.LastOrDefault()?.PlayedCards?.FirstOrDefault()?.Value ?? 0 })
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
        public static IEnumerable<Player> ReplacePlayer(this IEnumerable<Player> players, Player replacement) => players.Select(pl => pl.User.Equals(replacement.User) ? replacement : pl);

        public static Player TakeSwappedCards(this Player player, IEnumerable<Player> allPlayers)
        {
            var oppositeRank = player.NewRank.GetOppositeRank();
            var oppositePlayer = allPlayers.First(pl => pl.NewRank == oppositeRank);
            var newPlayer = new Player(player.User, new Play[0],
                player.CardsInHand.Concat(oppositePlayer.ExchangedCards.PlayedCards), new Play[0], Rank.Unknown, null);
            return player.NewRank == Rank.Klootzak ? newPlayer.WithStartOptions() : newPlayer;
        }


        public static Player ThatSwapped(this Player player)
        {
            var swappedCards = player.PossibleActions[0];
            return new Player(player.User, new Play[0], player.CardsInHand.Except(swappedCards.PlayedCards), new Play[0], player.NewRank, swappedCards);
        }

        public static Player WhenGameEnded(this Player player)
        {
            return new Player(player.User, player.PlaysThisRound, player.CardsInHand, Play.PassOnly, player.NewRank == Rank.Unknown ? Rank.Klootzak : player.NewRank, null);
        }

        public static Rank GetOppositeRank(this Rank playerNewRank)
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

        public static IEnumerable<Card[]> AllPermutations(this IEnumerable<Card> cardsOfSameValue)
        {
            var cards = cardsOfSameValue.AsArray();

            // Individual Cards
            foreach (var card in cards)
                yield return new[] { card };

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
        public static Play[] StartOptions(this Card[] cardsInHand)
        {
            return cardsInHand
                .GroupBy(card => card.Value)
                .SelectMany(AllPermutations)
                .Select(cardArray => new Play(cardArray))
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




    }
}