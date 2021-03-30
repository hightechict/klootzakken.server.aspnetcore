using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Klootzakken.Api.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYNAME = "playerId";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                if (!context.HttpContext.Request.Query.TryGetValue(APIKEYNAME, out extractedApiKey))
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = 401,
                        Content = "Api Key was not provided"
                    };
                    return;
                }
            }

            //var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            /*
            var apiKey = appSettings.GetValue<string>(APIKEYNAME);

            if (!apiKey.Equals(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Api Key is not valid"
                };
                return;
            }
            */
            var userId = extractedApiKey;
            var userName = "Naam";
            context.HttpContext.User = new ClaimsPrincipal(new[] { new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier,userId),
                        new Claim(ClaimTypes.Name,userName)}) });
            await next();
        }
    }
}
