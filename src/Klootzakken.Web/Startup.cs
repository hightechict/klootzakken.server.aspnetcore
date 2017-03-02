using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Klootzakken.Web.Data;
using Klootzakken.Web.Models;
using Klootzakken.Web.Services;
using Microsoft.IdentityModel.Tokens;
using Klootzakken.Web.Controllers;

namespace Klootzakken.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets("aspnet-Klootzakken.Web-cfe4f083-d1ad-4900-ada5-dc1fded80250");
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
                );

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(options =>
            {
                options.SslPort = 44376;
                //options.Filters.Add(new RequireHttpsAttribute());
            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
			var googleClientId = Configuration["Authentication:Google:ClientId"];
			if (!string.IsNullOrEmpty(googleClientId))
			{
			    app.UseGoogleAuthentication(new GoogleOptions()
			    {
                    ClientId = googleClientId,
                    ClientSecret = Configuration["Authentication:Google:ClientSecret"]
                });
			}
			else
			{
			    	
			}

            ConfigureAuth(app);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureAuth(IApplicationBuilder app)
        {
            var tokenSecretKey = Configuration["Authentication:Token:SecretKey"];
            if (string.IsNullOrEmpty(tokenSecretKey))
                throw new Exception("Authentication:Token:SecretKey not configured");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSecretKey));

            TokenController.Options.Audience = Configuration["Authentication:Token:Audience"];
            TokenController.Options.Issuer = Configuration["Authentication:Token:Issuer"];
            TokenController.Options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = Configuration["Authentication:Token:Issuer"],
                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = Configuration["Authentication:Token:Audience"],
                // Validate the token expiry
                ValidateLifetime = true,
                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });
        }
    }

}
