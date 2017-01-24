using System;
using Klootzakken.Server.Model;

namespace Klootzakken.Server.ApiModel
{
    public interface IUserApi
    {
        IObservable<LobbyView> CreateLobby(string name);
        IObservable<bool> InviteFriend(string lobbyId, string userId);
        IObservable<bool> InviteFriends(string lobbyId);
        IObservable<LobbyView> JoinLobby(string lobbyId);
        IObservable<GameView> GetGame(string gameId);
        IObservable<GameView> StartGame(string lobbyId);
        IObservable<GameView> Play(string gameId, Play play);
        IObservable<GameView> MyGames();
        IObservable<LobbyView> MyLobbies();
        IObservable<LobbyView> FriendLobbies();
        IObservable<LobbyView> Lobbies();
        IObservable<LobbyView> GetLobby(string lobbyId);
    }
}
