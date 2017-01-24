using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Klootzakken.Server.ApiModel;
using Klootzakken.Server.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Klootzakken.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        private static User GetUser(ClaimsPrincipal claim)
        {
            var userId = claim.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var nameClaim = (claim.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name) ?? claim.Claims.Single(c => c.Type == ClaimTypes.Email));
            var userName = nameClaim.Value;
            return new User(userId, userName);
        }

        [HttpPost]
        [Route("lobby/create/{name}")]
        public LobbyView CreateLobby(string name)
        {
            var user = GetUser(User);
            var gameId = Guid.NewGuid().ToString();
            var lobby = new Lobby(gameId, $"{user.Name}'s Game", user, true);
            return new LobbyView(lobby);
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
