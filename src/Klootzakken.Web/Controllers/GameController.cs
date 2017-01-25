using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Security.Claims;
using System.Threading.Tasks;
using Klootzakken.Server.ApiModel;
using Klootzakken.Server.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Klootzakken.Server.InMemory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Klootzakken.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        public static GameApi TheGameApi = new GameApi();

        private static User GetUser(ClaimsPrincipal claim)
        {
            var userId = claim.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var nameClaim = (claim.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name) ?? claim.Claims.Single(c => c.Type == ClaimTypes.Email));
            var userName = nameClaim.Value;
            return new User(userId, userName);
        }

        [HttpPost]
        [Route("lobby/create/{name}")]
        public Task<LobbyView> CreateLobby(string name)
        {
            return TheGameApi.CreateLobby(GetUser(User), name, true).ToTask();
        }

        [HttpPost]
        [Route("lobby/{lobbyId}/inviteFriend/{userId}")]
        public Task<bool> InviteFriend(string lobbyId, string userId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("lobby/{lobbyId}/inviteFriends")]
        public Task<bool> InviteFriends(string lobbyId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("lobby/{lobbyId}/join")]
        public Task<LobbyView> JoinLobby(string lobbyId)
        {
            return TheGameApi.JoinLobby(GetUser(User), lobbyId).ToTask();
        }

        [HttpGet]
        [Route("game/{gameId}")]
        public Task<GameView> GetGame(string gameId)
        {
            return TheGameApi.GetGame(GetUser(User), gameId).ToTask();
        }

        [HttpPost]
        [Route("lobby/{lobbyId}/start")]
        public Task<GameView> StartGame(string lobbyId)
        {
            return TheGameApi.StartGame(GetUser(User), lobbyId).ToTask();
        }

        [HttpPost]
        [Route("game/{gameId}/play")]
        public Task<GameView> Play(string gameId, [FromBody] Play play)
        {
            return TheGameApi.Play(GetUser(User), gameId, play).ToTask();
        }

        [HttpGet]
        [Route("myGames")]
        public Task<GameView[]> MyGames()
        {
            return TheGameApi.MyGames(GetUser(User)).ToArray().ToTask();
        }

        [HttpGet]
        [Route("myLobbies")]
        public Task<LobbyView[]> MyLobbies()
        {
            return TheGameApi.MyLobbies(GetUser(User)).ToArray().ToTask();
        }

        [HttpGet]
        [Route("friendLobbies")]
        public Task<LobbyView[]> FriendLobbies()
        {
            return TheGameApi.FriendLobbies(GetUser(User)).ToArray().ToTask();
        }

        [HttpGet]
        [Route("lobbies")]
        public Task<LobbyView[]> Lobbies()
        {
            return TheGameApi.Lobbies(GetUser(User)).ToArray().ToTask();
        }

        [HttpGet]
        [Route("lobby/{lobbyId}")]
        public Task<LobbyView> GetLobby(string lobbyId)
        {
            return TheGameApi.GetLobby(GetUser(User), lobbyId).ToTask();
        }
    }
}
