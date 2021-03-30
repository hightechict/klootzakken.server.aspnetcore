using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Klootzakken.Core.ApiModel;
using Klootzakken.Core.Model;
using Microsoft.AspNetCore.Authorization;
using Klootzakken.Server.InMemory;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Klootzakken.Api.Attributes;

namespace Klootzakken.Api.Controllers
{
    /// <summary>
    /// Controller for the Game
    /// </summary>
    [ApiKey]
    [Route("")]
    public class GameController : Controller
    {
        internal static GameApi TheGameApi = new GameApi();

        private User GameUser => User.AsGameUser();

        /// <summary>
        /// Create a new lobby
        /// </summary>
        /// <param name="name">The name of the future game</param>
        /// <param name="isPublic">Indicates whether the lobby is visible to everyone</param>
        /// <returns></returns>
        [HttpPost]
        [Route("lobby/create/{name}")]
        [ProducesResponseType(typeof(LobbyView), 200)]
        [SwaggerOperation("CreateLobby")]
        public Task<IActionResult> CreateLobby(string name, bool isPublic = true)
        {
            return WrapAsync(TheGameApi.CreateLobby(GameUser, name, isPublic).ToTask());
        }
        /*
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
        */

        /// <summary>
        /// Join a Lobby
        /// </summary>
        /// <param name="lobbyId">Id of the lobby to join</param>
        /// <returns>The new state of the lobby</returns>
        [HttpPost]
        [Route("lobby/{lobbyId}/join")]
        [ProducesResponseType(typeof(LobbyView), 200)]
        [SwaggerOperation("JoinLobby")]
        public Task<IActionResult> JoinLobby(string lobbyId)
        {
            return WrapAsync(TheGameApi.JoinLobby(GameUser, lobbyId).ToTask());
        }

        /// <summary>
        /// Retrieve the state of a game
        /// </summary>
        /// <param name="gameId">Id of the game</param>
        /// <returns>The game state</returns>
        [HttpGet]
        [Route("game/{gameId}")]
        [ProducesResponseType(typeof(GameView), 200)]
        [SwaggerOperation("GetGame")]
        public Task<IActionResult> GetGame(string gameId)
        {
            return WrapAsync(TheGameApi.GetGame(GameUser, gameId).ToTask());
        }

        /// <summary>
        /// Start the game for a lobby. The lobby will be converted to a game.
        /// </summary>
        /// <param name="lobbyId">Id of the lobby</param>
        /// <returns>The state of the new game</returns>
        [HttpPost]
        [ProducesResponseType(typeof(GameView), 200)]
        [Route("lobby/{lobbyId}/start")]
        [SwaggerOperation("StartGame")]
        public Task<IActionResult> StartGame(string lobbyId)
        {
            return WrapAsync(TheGameApi.StartGame(GameUser, lobbyId).ToTask());
        }

        /// <summary>
        /// Play on of your possible actions in the game
        /// </summary>
        /// <param name="gameId">Id of the game</param>
        /// <param name="play">The details of your action - must be one of the possiblePlays from the game state</param>
        /// <returns>The new state of the game</returns>
        [HttpPost]
        [ProducesResponseType(typeof(GameView), 200)]
        [Route("game/{gameId}/play")]
        [SwaggerOperation("GameAction")]
        public Task<IActionResult> Play(string gameId, [FromBody] PlayView play)
        {
            return WrapAsync(TheGameApi.Play(GameUser, gameId, play).ToTask());
        }

        /// <summary>
        /// Return the games you participate in
        /// </summary>
        /// <returns>The full state of the games</returns>
        [HttpGet]
        [Route("myGames")]
        [ProducesResponseType(typeof(GameView[]), 200)]
        [SwaggerOperation("ListMyGames")]
        public Task<IActionResult> MyGames()
        {
            return WrapAsync(TheGameApi.MyGames(GameUser).ToArray().ToTask());
        }

        /// <summary>
        /// Return the lobbies you've created or joined
        /// </summary>
        /// <returns>The full state of the games</returns>
        [HttpGet]
        [Route("myLobbies")]
        [ProducesResponseType(typeof(LobbyView[]), 200)]
        [SwaggerOperation("ListMyLobbies")]
        public Task<IActionResult> MyLobbies()
        {
            return WrapAsync(TheGameApi.MyLobbies(GameUser).ToArray().ToTask());
        }
/*
        [HttpGet]
        [Route("friendLobbies")]
        [ProducesResponseType(typeof(LobbyView[]), 200)]
        public Task<IActionResult> FriendLobbies()
        {
            return WrapAsync(TheGameApi.FriendLobbies(GameUser).ToArray().ToTask());
        }
*/

        /// <summary>
        /// Get all your and public lobbies
        /// </summary>
        /// <returns>The details of the lobbies</returns>
        [HttpGet]
        [Route("lobbies")]
        [ProducesResponseType(typeof(LobbyView[]), 200)]
        [SwaggerOperation("ListLobbies")]
        public Task<IActionResult> Lobbies()
        {
            return WrapAsync(TheGameApi.Lobbies(GameUser).ToArray().ToTask());
        }

        /// <summary>
        /// Get the state of a particular lobby
        /// </summary>
        /// <param name="lobbyId"></param>
        /// <returns>The details of the lobby</returns>
        [HttpGet]
        [Route("lobby/{lobbyId}")]
        [ProducesResponseType(typeof(LobbyView), 200)]
        [SwaggerOperation("GetLobby")]
        public Task<IActionResult> GetLobby(string lobbyId)
        {
            return WrapAsync(TheGameApi.GetLobby(GameUser, lobbyId).ToTask());
        }

        private async Task<IActionResult> WrapAsync<T>(Task<T> task)
        {
            try
            {
                var retVal = await task;
                var srespon =  Ok(retVal);
                return srespon;
            }
            catch (ApiException ae)
            {
                return BadRequest(ae.Message);
            }
        }
    }
}
