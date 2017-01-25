using System;
using System.Linq;
using Klootzakken.Server.Model;

namespace Klootzakken.Server.ApiModel
{
    public static class SerializationExtensions
    {
        public static Card ParseCard(this string cardString) => new Card(cardString.Substring(0, 1).ParseCardSuit(), cardString.Substring(1).ParseCardValue());

        public static CardSuit ParseCardSuit(this string character)
        {
            switch (character)
            {
                case "♠":
                    return CardSuit.Spades;
                case "♥":
                    return CardSuit.Hearts;
                case "♦":
                    return CardSuit.Diamonds;
                case "♣":
                    return CardSuit.Clubs;
                default:
                    throw new ArgumentOutOfRangeException(nameof(character), character, null);
            }
        }

        public static CardValue ParseCardValue(this string valueString)
        {
            switch (valueString)
            {
                case "A":
                    return CardValue.Ace;
                case "K":
                    return CardValue.King;
                case "Q":
                    return CardValue.Queen;
                case "J":
                    return CardValue.Jack;
                case "10":
                    return CardValue.Ten;
            }
            if (valueString.Length == 1 && int.TryParse(valueString, out int value) && value > 1)
                return (CardValue) value;
            throw new ArgumentOutOfRangeException(nameof(valueString), valueString, null);
        }

        public static string Serialized(this CardValue card)
        {
            switch (card)
            {
                case CardValue.Two:
                case CardValue.Three:
                case CardValue.Four:
                case CardValue.Five:
                case CardValue.Six:
                case CardValue.Seven:
                case CardValue.Eight:
                case CardValue.Nine:
                case CardValue.Ten:
                    return $"{(int) card}";
                case CardValue.Jack:
                    return "J";
                case CardValue.Queen:
                    return "Q";
                case CardValue.King:
                    return "K";
                case CardValue.Ace:
                    return "A";
                default:
                    throw new ArgumentOutOfRangeException(nameof(card), card, null);
            }

        }

        public static string Serialized(this Rank rank)
        {
            switch (rank)
            {
                case Rank.Unknown:
                    return null;
                case Rank.Klootzak:
                case Rank.ViezeKlootzak:
                case Rank.Neutraal:
                case Rank.VicePresident:
                case Rank.President:
                    return rank.ToString();
                default:
                    throw new ArgumentOutOfRangeException(nameof(rank), rank, null);
            }
        }

        public static string Serialized(this CardSuit suit)
        {
            switch (suit)
            {
                case CardSuit.Spades:
                    return "♠";
                case CardSuit.Hearts:
                    return "♥";
                case CardSuit.Diamonds:
                    return "♦";
                case CardSuit.Clubs:
                    return "♣";
                default:
                    throw new ArgumentOutOfRangeException(nameof(suit), suit, null);
            }
        }

        public static Play Deserialized(this PlayView playView)
        {
            return new Play( playView.Cards.Select(cardString => cardString.ParseCard()));
        }

        public static string Serialized(this Card card) => $"{card.Suit.Serialized()}{card.Value.Serialized()}";
        public static string[] Serialized(this Card[] cards) => cards.Select(c => Serialized((Card) c)).ToArray();
    }
}