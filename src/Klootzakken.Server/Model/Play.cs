using System;
using System.Collections.Generic;
using System.Linq;

namespace Klootzakken.Server.Model
{
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
}