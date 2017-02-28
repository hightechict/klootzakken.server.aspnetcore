# Device / App sign-in
Because of security reasons, the klootzakken SSO does not support Basic login (and the user/pass login should be removed too), but only more secure logins that require (for all practical purposes) a web browser view to be used, with cookie sharing between the application's http client and such - very tedious.

For these scenarios, the SSO offers an alternative sign-in method: PIN pairing. Those of you who use Plex Home Theater or Plex on Smart TV will find this familiar:

* The App shows a PIN number
* The user uses his browser (on any device) to sign in normally
* The user browses to a special PIN page
* The user enters the displayed PIN number
* The device is signed in as the user

So how does this work, technically?

Device:
* POST or GET /kz/pin/create
* Display the received PIN code
* Poll /kz/pin/<pin>/token
  * until a token instead of a 404 is received
* Optionally, call /kz/token using the received bearer token for one that has a longer validity.
Browser (logged in session):
* POST /kz/pin/<pin>/pair


## Details
If the client (app, AI) does not have a bearer token, or the token is expired (401 response from kzapi), it requests a new pairing PIN:

```
GET http://www.glueware.nl/klootzakken/kz/pin/create HTTP/1.1
User-Agent: Fiddlers
Host: www.glueware.nl
```


```
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Server: Kestrel
X-Powered-By: ASP.NET
Date: Tue, 28 Feb 2017 19:46:48 GMT

{"pin":"0902"}
```
It shows this PIN number to the end user and ask him to go his browser and log on, and pick 'PIN pairing' option (page is TODO).
The client meanwhile starts polling the pin-token api.

Any of 3 responses are possible:
* 404 => Not paired yet, keep trying
* 400 => Invalid PIN or Expired (lifetime of PIN is only a couple of minutes)
* 200 => token JSON response.
```
GET http://www.glueware.nl/klootzakken/kz/pin/0902/token HTTP/1.1
User-Agent: Fiddler
Host: www.glueware.nl
```
```
HTTP/1.1 404 Not Found
Content-Type: application/json; charset=utf-8
Server: Kestrel
X-Powered-By: ASP.NET
Date: Tue, 28 Feb 2017 19:47:07 GMT

{"error":"PIN not paired yet. try again later."}
```
So the client repeats the above...

This is where the Logged-in user (PC Browser) calls
```
POST http://www.glueware.nl/klootzakken/kz/pin/0902/pair
```
And gets '202 Accepted' in return.

After this the client gets a token, but this token may have a limited validity
```
GET http://www.glueware.nl/klootzakken/kz/pin/0902/token HTTP/1.1
User-Agent: Fiddler
Host: www.glueware.nl
```

```
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Server: Kestrel
X-Powered-By: ASP.NET
Date: Tue, 28 Feb 2017 19:47:50 GMT
Content-Length: 510

{"access_token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhNGE5MGM0NS01YWJmLTQ5ZWQtYmQ3Mi0xMTFlY2Y0MjU0ZjEiLCJ1bmlxdWVfbmFtZSI6ImJhcnQuZGUuYm9lckBkaXZ2ZXJlbmNlLmNvbSIsIkFzcE5ldC5JZGVudGl0eS5TZWN1cml0eVN0YW1wIjoiMzNjYmIzZGItZmU0Yy00NzdlLWE4ZDAtYzUxYWFjMDIyZDEyIiwiYXV0aG1ldGhvZCI6Ikdvb2dsZSIsIm5iZiI6MTQ4ODMxMTI2NCwiZXhwIjoxNDg4Mzk3NjY0LCJpYXQiOjE0ODgzMTEyNjQsImlzcyI6IkRpdnZlcmVuY2UuY29tIEtsb290emFra2VuIiwiYXVkIjoiRGVtb0F1ZGllbmNlIn0.dcvT_urhmrBi1hzfEkkLicEnZ_LP1UfT_yCkqTbPJHg","expires_in":86400}
```
But it can use this token to use the normal 'token' api to get a token that's valid for longer (31 days currently).
It's good practice to always refresh the token on app startup.
```
GET http://www.glueware.nl/klootzakken/kz/token HTTP/1.1
User-Agent: Fiddler
Host: www.glueware.nl
Authorization: bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhNGE5MGM0NS01YWJmLTQ5ZWQtYmQ3Mi0xMTFlY2Y0MjU0ZjEiLCJ1bmlxdWVfbmFtZSI6ImJhcnQuZGUuYm9lckBkaXZ2ZXJlbmNlLmNvbSIsIkFzcE5ldC5JZGVudGl0eS5TZWN1cml0eVN0YW1wIjoiMzNjYmIzZGItZmU0Yy00NzdlLWE4ZDAtYzUxYWFjMDIyZDEyIiwiYXV0aG1ldGhvZCI6Ikdvb2dsZSIsIm5iZiI6MTQ4ODMxMTI2NCwiZXhwIjoxNDg4Mzk3NjY0LCJpYXQiOjE0ODgzMTEyNjQsImlzcyI6IkRpdnZlcmVuY2UuY29tIEtsb290emFra2VuIiwiYXVkIjoiRGVtb0F1ZGllbmNlIn0.dcvT_urhmrBi1hzfEkkLicEnZ_LP1UfT_yCkqTbPJHg
```
```
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Server: Kestrel
X-Powered-By: ASP.NET
Date: Tue, 28 Feb 2017 19:48:34 GMT

{"access_token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhNGE5MGM0NS01YWJmLTQ5ZWQtYmQ3Mi0xMTFlY2Y0MjU0ZjEiLCJ1bmlxdWVfbmFtZSI6ImJhcnQuZGUuYm9lckBkaXZ2ZXJlbmNlLmNvbSIsIkFzcE5ldC5JZGVudGl0eS5TZWN1cml0eVN0YW1wIjoiMzNjYmIzZGItZmU0Yy00NzdlLWE4ZDAtYzUxYWFjMDIyZDEyIiwiYXV0aG1ldGhvZCI6Ikdvb2dsZSIsIm5iZiI6MTQ4ODMxMTMxNCwiZXhwIjoxNDkwOTg5NzE0LCJpYXQiOjE0ODgzMTEzMTQsImlzcyI6IkRpdnZlcmVuY2UuY29tIEtsb290emFra2VuIiwiYXVkIjpbIkRlbW9BdWRpZW5jZSIsIkRlbW9BdWRpZW5jZSJdfQ.Z0cEkeDM4TFEj2irYZ104YTtLO__hDOgaQ0a1FqV3fk","expires_in":2678400}
```

The client can now use the klootzakken API using the above token as bearer token.

