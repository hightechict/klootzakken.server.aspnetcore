using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using Klootzakken.Server.ApiModel;
using Klootzakken.Server.Model;

namespace Klootzakken.Server.InMemory
{
    public class GameApi : IGameApi
    {
        private readonly ConcurrentDictionary<string, Lobby> _lobbies = new ConcurrentDictionary<string, Lobby>();
        private readonly ConcurrentDictionary<string, Game> _games = new ConcurrentDictionary<string, Game>();

        public IObservable<LobbyView> CreateLobby(User user, string name, bool isPublic)
        {
            var lobby = new Lobby(_lobbies.Count.ToString(), name, user, isPublic);
            return OnLobby(lobby).AsView();
        }

        public IObservable<bool> InviteFriend(User user, string lobbyId, string userId)
        {
            throw new NotImplementedException();
        }

        public IObservable<bool> InviteFriends(User user, string lobbyId)
        {
            throw new NotImplementedException();
        }

        public IObservable<LobbyView> JoinLobby(User user, string lobbyId)
        {
            try
            {
                return OnLobby(PrivateGetLobby(lobbyId).Join(user)).AsView();
            }
            catch (ArgumentException e)
            {
                return Observable.Throw<LobbyView>(new ApiException(e));
            }
        }

        public IObservable<GameView> GetGame(User user, string gameId)
        {
            try
            {
                return PrivateGetGame(gameId).AsViewFor(user);
            }
            catch (ArgumentException e)
            {
                return Observable.Throw<GameView>(new ApiException(e));
            }
        }

        public IObservable<GameView> StartGame(User user, string lobbyId)
        {
            try
            {
                return OnGame(PrivateGetLobby(lobbyId).DealFirstGame()).AsViewFor(user);
            }
            catch (ArgumentException e)
            {
                return Observable.Throw<GameView>(new ApiException(e));
            }
            catch (InvalidOperationException e)
            {
                return Observable.Throw<GameView>(new ApiException(e));
            }
        }

        public IObservable<GameView> Play(User user, string gameId, Play play)
        {
            try
            {
                return OnGame(PrivateGetGame(gameId).WhenPlaying(user, play)).AsViewFor(user);
            }
            catch (ArgumentException e)
            {
                return Observable.Throw<GameView>(new ApiException(e));
            }
            catch (InvalidOperationException e)
            {
                return Observable.Throw<GameView>(new ApiException(e));
            }
        }

        public IObservable<GameView> MyGames(User user)
        {
            return _games.Values.Where(g => g.Players.Any(p => p.User.Equals(user))).ToObservable().AsViewFor(user);
        }

        public IObservable<LobbyView> MyLobbies(User user)
        {
            return _lobbies.Values.Where(g => g.Users.Any(u => u.Equals(user))).ToObservable().AsView();
        }

        public IObservable<LobbyView> FriendLobbies(User user)
        {
            throw new NotImplementedException();
        }

        public IObservable<LobbyView> Lobbies(User user)
        {
            return _lobbies.Values.Where(g => g.IsListed || g.Users.Any(u => u.Equals(user))).ToObservable().AsView();
        }

        public IObservable<LobbyView> GetLobby(User user, string lobbyId)
        {
            return PrivateGetLobby(lobbyId).AsView();
        }

        private Lobby PrivateGetLobby(string lobbyId)
        {
            if (_lobbies.TryGetValue(lobbyId, out Lobby lobby))
                return lobby;
            throw new ArgumentException($"No such lobby {lobbyId}", nameof(lobbyId));
        }

        private Game PrivateGetGame(string gameId)
        {
            if (_games.TryGetValue(gameId, out Game game))
                return game;
            throw new ArgumentException($"No such game {gameId}", nameof(gameId));
        }

        private IObservable<Lobby> OnLobby(Lobby lobby)
        {
            _lobbies[lobby.Id] = lobby;
            //_lobbyStream.OnNext(lobby);
            return Observable.Return(lobby);
        }

        private IObservable<Game> OnGame(Game game)
        {
            _games[game.Id] = game;
            _lobbies.TryRemove(game.Id, out Lobby ignoreMe);
            //_gameStream.OnNext(game);
            return Observable.Return(game);
        }
    }
}
