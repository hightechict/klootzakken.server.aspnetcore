using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klootzakken.Server.ApiModel;
using Klootzakken.Server.Model;

namespace Klootzakken.Server.InMemory
{
    public class UserApi : IUserApi
    {
        public IObservable<LobbyView> CreateLobby(string name)
        {
            throw new NotImplementedException();
        }

        public IObservable<bool> InviteFriend(string lobbyId, string userId)
        {
            throw new NotImplementedException();
        }

        public IObservable<bool> InviteFriends(string lobbyId)
        {
            throw new NotImplementedException();
        }

        public IObservable<LobbyView> JoinLobby(string lobbyId)
        {
            throw new NotImplementedException();
        }

        public IObservable<GameView> GetGame(string gameId)
        {
            throw new NotImplementedException();
        }

        public IObservable<GameView> StartGame(string lobbyId)
        {
            throw new NotImplementedException();
        }

        public IObservable<GameView> Play(string gameId, Play play)
        {
            throw new NotImplementedException();
        }

        public IObservable<GameView> MyGames()
        {
            throw new NotImplementedException();
        }

        public IObservable<LobbyView> MyLobbies()
        {
            throw new NotImplementedException();
        }

        public IObservable<LobbyView> FriendLobbies()
        {
            throw new NotImplementedException();
        }

        public IObservable<LobbyView> Lobbies()
        {
            throw new NotImplementedException();
        }

        public IObservable<LobbyView> GetLobby(string lobbyId)
        {
            throw new NotImplementedException();
        }
    }
}
