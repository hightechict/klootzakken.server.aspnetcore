using System;
using Klootzakken.Server.Model;

namespace Klootzakken.Server.ApiModel
{
    public interface IGameApi
    {
        IObservable<LobbyView> CreateLobby(User user, string name, bool isPublic);
        IObservable<bool> InviteFriend(User user, string lobbyId, string userId);
        IObservable<bool> InviteFriends(User user, string lobbyId);
        IObservable<LobbyView> JoinLobby(User user, string lobbyId);
        IObservable<GameView> GetGame(User user, string gameId);
        IObservable<GameView> StartGame(User user, string lobbyId);
        IObservable<GameView> Play(User user, string gameId, PlayView play);
        IObservable<GameView> MyGames(User user);
        IObservable<LobbyView> MyLobbies(User user);
        IObservable<LobbyView> FriendLobbies(User user);
        IObservable<LobbyView> Lobbies(User user);
        IObservable<LobbyView> GetLobby(User user, string lobbyId);
    }
}