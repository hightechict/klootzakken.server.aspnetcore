# klootzakken.server.aspnetcore
Use 
* Visual Studio 2017
or
* dotnet core cli/sdk 1.1.1 [Download](https://github.com/dotnet/core/blob/master/release-notes/download-archives/1.1.1-download.md)
  * Combined with your favorite editor (eg. the excellent OSS/X-platform Visual Studio Code ) 

It's spread over a couple of projects
* ASP.NET core site for registration and login, and token generation (SQLite user storage), the "SSO"
  * Klootzakken.Web
* ASP.NET Core WebAPI (with bearer-token-authentication only) serving the API
  * Klootzakken.Api
* A netstandard1.2 library for an In-Memory games server (reboot = games gone)
  * Klootzakken.Server.InMemory
* A netstandard1.2 library for the actual game logic and models
  * Klootzakken.Core
  
## Compiling/Running
Using VS2017 it's trivial, just do what you always do.

Both the Web (SSO) and Api (Game Server) projects are dotnet core applications, self-hosting using Kestrel.
Using the dotnet cli application, it's simple too:

After cloning the project, go into either folder and run:

`dotnet restore` to restore all used nuget packages

`dotnet run` to build and run

If you want to change the port number/listen address, you can do so by setting the following Environment Variable:
ASPNETCORE_URLS=http://+:8080/

If you want to start both SSO and API, you'll need to assign separate port numbers.

The default port number is 5000 when running from dotnet cli.

## Authentication
The Game API itself only supports (stateless) bearer token authentication.
You use the Web project (SSO) to sign in using username/password or Google signin, and use the SSO token api to obtain a bearer token.

For clients where using a webview to sign in is not feasible or desireable, there's the option of Pin Pairing to obtain a bearer token on the device/app.
See [Pin Pairing](PinPairing.md) for more details.

## Live version
* ASP.NET Site (Sign-On server): <http://www.glueware.nl/Klootzakken/kz/>
* WebAPI : <http://www.glueware.nl/Klootzakken/kzapi/>
* To get an API token: <http://www.glueware.nl/Klootzakken/kz/token>
* Swagger.IO interactive spec: <http://www.glueware.nl/Klootzakken/kzapi/swagger/>
* Swagger.IO spec: <http://www.glueware.nl/Klootzakken/kzapi/swagger/v1/swagger.json>

## Notes
* Above Live Server is registered as a Google API user application in Divverence's GSuite account.
* Above live server does auto-update (version info [here](http://www.glueware.nl/Klootzakken/kz/version.html) ) from the deploy/glueware branch.
* Above live server is able to share a port between the 2 applications by hosting the kestrel applications from IIS - effectively, IIS forwards each request to the correct 'locally listening' .Net core app.

## Rules of 'klootzakken', also called 'presidents'
You play with 4 players, a deck of 32 card, each player receives 8 cards.

In the first round, everybody is a 'civilian'. The player with the lowest card (7 of spades).

The one who starts plays out 1 or more cards of the same value. Now the other players follow.
Every other player has to play out the same number of cards the first player started with, if they cannot, the should pass.

Every other player has to play out a higher hand than the preceeding player.

Example:
Player 1 plays 1 card, a 7.
Then player 2 has to play also a 8 or higher, or pass.

If play 1 plays 2 cards, they have to have the same figure, so 2 times a 7.
Then player 2 also has to play 2 times a 8 or 2 times a higher card.

The order of the cards is as follows, ordered from high to low:
7, 8, 9, 10, jack, queen, king, ace

Obviously, when a player plays one or more aces, the round immediately stops.

The player who starts in the next round is:
- The one that played out the last card(s)

The player who wins one game is the one who firstly managed to play out its entire hand. This player becomes the president. Other player continue. The last one who played out its hand is called 'the asshole' (Klootzak). Player 2 and 3 become resp. the vice president and the vice asshole.

So, in the 2nd round, all players are not civilians anymore but become:
- President, vice president, vice asshole or asshole.

Also, the winner of a round gets 3 points, seconds best 2 points, third best 1 point and loser 0 points.

Now, at the start of the next round, before the game starts:
- The asshole offers the president its highest 2 cards. The president offers the asshole its lowest 2 cards. Exchange happens simultaneously. Note that even if the president holds for example 3 sevens, 1 eight and a nine before the swap, he *must* hand over the lowest cards (two sevens) - he does not have the right to keep the strong hand of sevens and hand out his eight and nine instead.
- The both vices (vice-president / vice-asshold) exchange 1 card.
- Other players do not see the exchange.

20 rounds are played. The winner is the one with the most points (which can be the asshole at the end of the game).
