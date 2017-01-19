using System;
using System.Collections.Generic;
using System.Linq;

namespace Klootzakken.Server.Model
{
    public class GameState
    {
        public GameState(GamePhase phase, IEnumerable<Player> players, Card centerCard)
        {
            CenterCard = centerCard;
            Players = players.AsArray();
            Phase = phase;

            switch (Phase)
            {
                case GamePhase.Playing:
                    if (Players.All(p => p.NewRank != Rank.Unknown))
                        throw new ArgumentException("One player must not have a rank in an active game");

                    if (Players.Any(p => p.NewRank != Rank.Unknown && p.CardsInHand.Length != 0))
                        throw new ArgumentException("No player can have a rank and cards in hand in an active game");

                    if (Players.Any(p => p.NewRank != Rank.Unknown && p.PossibleActions.Length != 0))
                        throw new ArgumentException("No player can have a rank and actions in an active game");

                    if (Players.Count(p => p.PossibleActions.Length != 0) != 1)
                        throw new ArgumentException("Only one player must have actions in an active game");
                    break;
                case GamePhase.Ended:
                    if (Players.Any(p => p.NewRank == Rank.Unknown))
                        throw new ArgumentException("All players must have a NewRank in an ended game");

                    if (!Players.All(p => p.PossibleActions.Length == 1 && Equals(p.PossibleActions[0], Play.Pass)))
                        throw new ArgumentException("All players must have only the Pass option in an ended game");
                    break;
                case GamePhase.SwappingCards:
                    if (Players.Any(p => p.NewRank == Rank.Unknown))
                        throw new ArgumentException("All players must have a NewRank when Swapping Cards");

                    if (Players.Any(p => p.PossibleActions.Length > 1))
                        throw new ArgumentException("No player must have more than one action when Swapping Cards");

                    if (Players.All(p => p.PossibleActions.Length == 0))
                        throw new ArgumentException("At least one player must have an action when Swapping Cards");

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Player[] Players { get; }
        public Card CenterCard { get; }

        public int ActivePlayer
        {
            get
            {
                if (Phase != GamePhase.Playing)
                    throw new InvalidOperationException("Only games in Playing Phase have an Active Player");
                return Players.FindSingle(pl => pl.PossibleActions.Length != 0);
            }
        }

        public GamePhase Phase {get; }
    }
}