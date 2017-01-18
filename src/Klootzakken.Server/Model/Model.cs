using System;
using System.Collections.Generic;
using System.Linq;

namespace Klootzakken.Server.Model
{
    public enum CardSuit
    {
        Spades,
        Hearts,
        Diamonds,
        Clubs,
    }

    public enum CardValue
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14,
    }

    public class Card : IEquatable<Card>
    {
        public bool Equals(Card other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Suit == other.Suit && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Card) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public Card(CardSuit suit, CardValue value)
        {
            Suit = suit;
            Value = value;
        }

        public CardSuit Suit { get; }
        public CardValue Value { get; }
    }

    public class Play : IEquatable<Play>
    {
        public bool Equals(Play other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (PlayedCards.Length != other.PlayedCards.Length) return false;
            return PlayedCards.Union(other.PlayedCards).Count() == PlayedCards.Length;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Play) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public Play(IEnumerable<Card> playedCards)
        {
            PlayedCards = playedCards.ToArray();
        }

        public Card[] PlayedCards { get; }
        public bool IsPass => PlayedCards.Length == 0;

        public static Play Pass { get; } = new Play(new Card[0]);
        public static IEnumerable<Play> PassOnly { get; } = new[] {Pass};
    }

    public class PlayerBase
    {
        public PlayerBase(string name, IEnumerable<Play> playsThisRound, Rank newRank)
        {
            Name = name;
            PlaysThisRound = playsThisRound.ToArray();
            NewRank = newRank;
        }

        public string Name { get; }
        public Play[] PlaysThisRound { get; }
        public Rank NewRank { get; }
    }

    public enum Rank
    {
        Unknown,
        Klootzak,
        ViezeKlootzak,
        Neutraal,
        VicePresident,
        President
    }

    public class Player : PlayerBase
    {
        public Player(string name, IEnumerable<Play> playsThisRound, IEnumerable<Card> cardsInHand, IEnumerable<Play> possibleActions, Rank newRank, Play exchangedCards) : base(name, playsThisRound, newRank)
        {
            ExchangedCards = exchangedCards;
            CardsInHand = cardsInHand.OrderBy(c => c.Value).ToArray();
            PossibleActions = possibleActions.ToArray();
        }

        public Card[] CardsInHand { get; }
        public Play[] PossibleActions { get; }
        public Play ExchangedCards { get; }
    }

    public class OtherPlayer : PlayerBase
    {
        public OtherPlayer(string name, IEnumerable<Play> playsThisRound, int cardCount, Rank newRank, int exchangedCardsCount) : base(name, playsThisRound, newRank)
        {
            CardCount = cardCount;
            ExchangedCardsCount = exchangedCardsCount;
        }

        public int CardCount { get; }
        public int ExchangedCardsCount { get; }
    }

    public class GameState
    {
        public GameState(IEnumerable<Player> players, Card centerCard, int activePlayer)
        {
            Phase = GamePhase.Playing;
            Players = players.ToArray();

            if (Players.All(p => p.NewRank != Rank.Unknown))
                throw new ArgumentException("One player must not have a rank in an active game");

            if (Players.Any(p => p.NewRank != Rank.Unknown && p.CardsInHand.Length !=0 ))
                throw new ArgumentException("No player can have a rank and cards in hand in an active game");

            if (Players.Any(p => p.NewRank != Rank.Unknown && p.PossibleActions.Length != 0))
                throw new ArgumentException("No player can have a rank and actions in an active game");

            if (Players[activePlayer].PossibleActions.Length==0)
                throw new ArgumentException("Active player must have at least one action in an active game");

            if (Players.Count(p => p.PossibleActions.Length != 0) != 1)
                throw new ArgumentException("Only one player must have actions in an active game");

            CenterCard = centerCard;
            ActivePlayer = activePlayer;
        }

        public GameState(IEnumerable<Player> players, Card centerCard)
        {
            Phase = GamePhase.Ended;
            Players = players.ToArray();

            if (Players.Any( p => p.NewRank == Rank.Unknown))
                throw new ArgumentException("All players must have a NewRank in an ended game");

            if (!Players.All(p => p.PossibleActions.Length == 1 && Equals(p.PossibleActions[0], Play.Pass)))
                throw new ArgumentException("All players must have only the Pass option in an ended game");

            CenterCard = centerCard;
            ActivePlayer = Players.FindSingle(pl => pl.NewRank == Rank.Klootzak);
        }

        public GameState(IEnumerable<Player> players)
        {
            Phase = GamePhase.SwappingCards;
            Players = players.ToArray();

            if (Players.Any(p => p.NewRank == Rank.Unknown))
                throw new ArgumentException("All players must have a NewRank when Swapping Cards");

            if (Players.Any(p => p.PossibleActions.Length > 1))
                throw new ArgumentException("No player must have more than one action when Swapping Cards");

            if (Players.All(p => p.PossibleActions.Length == 0))
                throw new ArgumentException("At least one player must have an action when Swapping Cards");

            CenterCard = null;
            ActivePlayer = Players.FindSingle(pl => pl.NewRank == Rank.Klootzak);
        }

        public Player[] Players { get; }
        public Card CenterCard { get; }
        public int ActivePlayer { get; }

        public GamePhase Phase {get; }
    }

    public enum GamePhase
    {
        Playing,
        SwappingCards,
        Ended
    }

    public class PublicGameState
    {
        public PublicGameState(PlayerBase you, OtherPlayer[] otherPlayers, Card centerCard, string activePlayerName)
        {
            You = you;
            OtherPlayers = otherPlayers;
            CenterCard = centerCard;
            ActivePlayerName = activePlayerName;
        }

        public PlayerBase You { get; }
        public OtherPlayer[] OtherPlayers { get; }
        public Card CenterCard { get; }
        public string ActivePlayerName { get; }
    }

    public class Lobby
    {
        public Lobby(string[] players)
        {
            Players = players;
        }

        public string[] Players { get; }
    }
}
