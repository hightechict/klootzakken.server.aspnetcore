using Klootzakken.Server.Model;

namespace Klootzakken.Server.ApiModel
{
    public interface IUserApi
    {
        LobbyView CreateLobby(string name);
        void InviteFriend(string lobbyId, string userId);
        void InviteFriends(string lobbyId);
        LobbyView JoinLobby(string lobbyId);
        GameView GetGame(string gameId);
        GameView StartGame(string lobbyId);
        GameView Play(string gameId, Play play);
        GameView[] MyGames();
        LobbyView[] MyLobbies();
        LobbyView[] FriendLobbies();
        LobbyView[] Lobbies();
        LobbyView[] GetLobby(string lobbyId);
    }
}