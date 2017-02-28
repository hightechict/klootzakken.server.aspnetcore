using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Klootzakken.Web.Controllers
{
    [Authorize]
    [Route("pin")]
    public class PinController : Controller
    {
        private object CreateToken(TimeSpan expiration)
        {
            var firstIdentity = User.Identity as ClaimsIdentity ?? User.Identities.First();
            var response = TokenController.CreateTokenFor(expiration, firstIdentity);
            return response;
        }

        [AllowAnonymous]
        [HttpGet, HttpPost]
        [Route("create")]
        public IActionResult GetPairingPin()
        {
            if (User.Identity.IsAuthenticated)
                return Forbid();

            return Ok(new {pin = PinStorage.RequestPin()});
        }

        [HttpGet, HttpPost]
        [Route("{pin}/pair")]
        public IActionResult PairWithPin( string pin)
        {
            if (!PinStorage.IsPinKnown(pin))
                return Forbid();

            var token = CreateToken(TimeSpan.FromDays(1));
            if (!PinStorage.SetToken(pin, token))
                return Forbid();

            return Accepted();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{pin}/token")]
        public IActionResult TryGetToken(string pin)
        {
            if (User.Identity.IsAuthenticated)
                return Forbid();

            if (!PinStorage.IsPinKnown(pin))
                return BadRequest(new { error = "unknown or expired PIN"});

            var token = PinStorage.TryGetToken(pin);
            if (token == null)
                return NotFound(new {error = "PIN not paired yet. try again later."});

            return Ok(token);
        }

        class PinData
        {
            public DateTimeOffset Expiration { get; set; }
            public object TokenResponse { get; set; }
        }

        class PendingPinHandler
        {
            private readonly ConcurrentDictionary<string, PinData> _dictionary = new ConcurrentDictionary<string, PinData>();
            public string RequestPin()
            {
                while (true)
                {
                    Cleanup();
                    var newPin = GeneratePin();
                    var expiration = DateTimeOffset.UtcNow.AddMinutes(2);
                    if (_dictionary.TryAdd(newPin, new PinData {Expiration = expiration}))
                        return newPin;
                }
            }

            private void Cleanup()
            {
                var now = DateTimeOffset.UtcNow;
                
                foreach (var kvp in _dictionary.Where( kv => kv.Value.Expiration<now).ToArray())
                {
                    PinData ignore;
                    _dictionary.TryRemove(kvp.Key, out ignore);
                }
            }

            private string GeneratePin()
            {
                return new Random().Next(9999).ToString("D4");
            }

            public object TryGetToken(string pin)
            {
                PinData value;
                if (!_dictionary.TryGetValue(pin, out value))
                    return null;

                if (value.Expiration < DateTimeOffset.UtcNow)
                    return null;

                if (value.TokenResponse == null )
                    return null;

                return !_dictionary.TryRemove(pin, out value) ? null : value.TokenResponse;
            }

            public bool IsPinKnown(string pin)
            {
                PinData value;
                if (!_dictionary.TryGetValue(pin, out value))
                    return false;

                return value.Expiration >= DateTimeOffset.UtcNow;
            }

            public bool SetToken(string pin, object token)
            {
                PinData value;
                if (!_dictionary.TryGetValue(pin, out value))
                    return false;

                if (value.Expiration < DateTimeOffset.UtcNow)
                {
                    _dictionary.TryRemove(pin, out value);
                    return false;
                }

                value.TokenResponse = token;
                return true;
            }

        }
        private static readonly PendingPinHandler PinStorage = new PendingPinHandler();
    }
}