# klootzakken.server.aspnetcore
Use VisualStudio 2017 (RC) to develop.

It's spread over a couple of projects
* ASP.NET site for registration and login, and token generation (SQLite user storage)
* ASP.NET WebAPI (with bearer-token-authentication only) serving the API
* A netstandard1.2 library for an In-Memory games server (reboot = games gone)
* A netstandard1.2 library for the actual game logic and models

## Live version
* ASP.NET Site: http://www.glueware.nl/Klootzakken/kz/
* WebAPI : http://www.glueware.nl/Klootzakken/kzapi/
* To get an API token: http://www.glueware.nl/Klootzakken/kz/token
* Swagger.IO interactive spec: http://www.glueware.nl/Klootzakken/kzapi/swagger/
* Swagger.IO spec: http://www.glueware.nl/Klootzakken/kzapi/swagger/v1/swagger.json

###Notes
* The Server is registered as a Google API user application in Divverence's GSuite account.
* Above server does not auto-update
