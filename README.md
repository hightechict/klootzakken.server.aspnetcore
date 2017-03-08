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
