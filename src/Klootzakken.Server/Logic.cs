using System;
using System.Collections.Generic;
using System.Linq;
using Klootzakken.Server.Model;

namespace Klootzakken.Server
{
    public static class Logic
    {
        public static Card[] TopDownDeck = new Card[0];
        public static GameState Deal(this Lobby lobby)
        {
            var playerCount = lobby.Players.Length;
            var deck = new Stack<Card>(TopDownDeck.Take(8*playerCount));
            var deal = new List<Card>[playerCount];
            while (deck.Count != 0)
            {
                var dealtCard = deck.Pop();
                while (true)
                {
                    var playerForCard = Random(playerCount);
                    if (deal[playerForCard].Count == 8)
                        continue;
                    deal[playerForCard].Add(dealtCard);
                    break;
                }
            }

            var players = lobby.Players.Select((pl, i) => new YourPlayer(pl, new Play[0], deal[i].ToArray(), new Play[0])).ToArray();
            return new GameState(players, null, 0);
        }

        private static Random RandomGenerator { get; } = new Random();

        private static int Random(int playerCount)
        {
            return RandomGenerator.Next(playerCount);
        }
    }
}