# klootzakken.server.aspnetcore
Use VisualStudio 2017 (RC) to develop.

It's spread over a couple of projects
* ASP.NET site for registration and login, and token generation (SQLite user storage)
* ASP.NET WebAPI (with bearer-token-authentication only) serving the API
* A netstandard1.2 library for an In-Memory games server (reboot = games gone)
* A netstandard1.2 library for the actual game logic and models

## Authentication
The Game API itself ('kzapi') only supports (stateless) bearer token authentication.
You use the Site (SSO) to sign in using username/password or Google signin, and use the SSO token api to obtain a bearer token.

For clients where using a webview to sign in is not feasible or desireable, there's the option of Pin Pairing to obtain a bearer token on the device/app.
See [Pin Pairing](PinPairing.md) for more details.

## Live version
* ASP.NET Site (Sign-On server): <http://www.glueware.nl/Klootzakken/kz/>
* WebAPI : <http://www.glueware.nl/Klootzakken/kzapi/>
* To get an API token: <http://www.glueware.nl/Klootzakken/kz/token>
* Swagger.IO interactive spec: <http://www.glueware.nl/Klootzakken/kzapi/swagger/>
* Swagger.IO spec: <http://www.glueware.nl/Klootzakken/kzapi/swagger/v1/swagger.json>

## Notes
* The Server is registered as a Google API user application in Divverence's GSuite account.
* Above live server does not (yet) auto-update.
