using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Security.Claims;
using System.Threading.Tasks;
using Klootzakken.Core.ApiModel;
using Klootzakken.Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Klootzakken.Server.InMemory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Klootzakken.Api.Controllers
{
    public static class HelperExtensions
    {
        public static User AsGameUser(this ClaimsPrincipal claim)
        {
            var userId = claim.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var nameClaim = (claim.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name) ?? claim.Claims.Single(c => c.Type == ClaimTypes.Email));
            var userName = nameClaim.Value;
            return new User(userId, userName);
        }
    }

    [Authorize]
    [Route("kzapi")]
    public class GameController : Controller
    {
        public static GameApi TheGameApi = new GameApi();

        public User GameUser => User.AsGameUser();

        [HttpPost]
        [Route("lobby/create/{name}")]
        public Task<IActionResult> CreateLobby(string name)
        {
            return WrapAsync(TheGameApi.CreateLobby(User.AsGameUser(), name, true).ToTask());
        }

        [HttpPost]
        [Route("lobby/{lobbyId}/inviteFriend/{userId}")]
        public Task<IActionResult> InviteFriend(string lobbyId, string userId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("lobby/{lobbyId}/inviteFriends")]
        public Task<IActionResult> InviteFriends(string lobbyId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("lobby/{lobbyId}/join")]
        public Task<IActionResult> JoinLobby(string lobbyId)
        {
            return WrapAsync(TheGameApi.JoinLobby(GameUser, lobbyId).ToTask());
        }

        [HttpGet]
        [Route("game/{gameId}")]
        public Task<IActionResult> GetGame(string gameId)
        {
            return WrapAsync(TheGameApi.GetGame(GameUser, gameId).ToTask());
        }

        [HttpPost]
        [Route("lobby/{lobbyId}/start")]
        public Task<IActionResult> StartGame(string lobbyId)
        {
            return WrapAsync(TheGameApi.StartGame(GameUser, lobbyId).ToTask());
        }

        [HttpPost]
        [Route("game/{gameId}/play")]
        public Task<IActionResult> Play(string gameId, [FromBody] PlayView play)
        {
            return WrapAsync(TheGameApi.Play(GameUser, gameId, play).ToTask());
        }

        [HttpGet]
        [Route("myGames")]
        public Task<IActionResult> MyGames()
        {
            return WrapAsync(TheGameApi.MyGames(GameUser).ToArray().ToTask());
        }

        [HttpGet]
        [Route("myLobbies")]
        public Task<IActionResult> MyLobbies()
        {
            return WrapAsync(TheGameApi.MyLobbies(GameUser).ToArray().ToTask());
        }

        [HttpGet]
        [Route("friendLobbies")]
        public Task<IActionResult> FriendLobbies()
        {
            return WrapAsync(TheGameApi.FriendLobbies(GameUser).ToArray().ToTask());
        }

        [HttpGet]
        [Route("lobbies")]
        public Task<IActionResult> Lobbies()
        {
            return WrapAsync(TheGameApi.Lobbies(GameUser).ToArray().ToTask());
        }

        [HttpGet]
        [Route("lobby/{lobbyId}")]
        public Task<IActionResult> GetLobby(string lobbyId)
        {
            return WrapAsync(TheGameApi.GetLobby(GameUser, lobbyId).ToTask());
        }

        private async Task<IActionResult> WrapAsync<T>(Task<T> task)
        {
            try
            {
                var retVal = await task;
                return Ok(retVal);
            }
            catch (ApiException ae)
            {
                return BadRequest(ae.Message);
            }
        }
    }
}
