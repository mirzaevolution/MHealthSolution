using MHealth.Abstracts;
using MHealth.Services;
using MHealth.SharedDataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHealth.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MHealth.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            });
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddControllers();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
               {
                   options.Authority = Configuration["IdentityServer:Authority"];
                   options.Audience = Configuration["IdentityServer:Audience"];
                   options.TokenValidationParameters.NameClaimType = "name";
                   options.TokenValidationParameters.RoleClaimType = "role";
               });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TM.Api", Version = "v1" });
                c.AddSecurityDefinition(GlobalConstants.SwaggerSecurityDefinitionKey, new OpenApiSecurityScheme
                {
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{Configuration["IdentityServer:Authority"]}/connect/authorize"),
                            TokenUrl = new Uri($"{Configuration["IdentityServer:Authority"]}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "MHealth.Api:Read","MHealth.Api read access" },
                                { "openid", "OpenId Connect scope" },
                                { "profile", "Profile scope" },
                                { "email", "Email scope" },
                                { "user_roles", "User roles scope" }
                            }
                        }
                    },
                    Type = SecuritySchemeType.OAuth2
                });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });
            services.AddHttpClient("IdentityServer", o =>
            {
                o.BaseAddress = new Uri(Configuration["IdentityServer:Authority"]);

            });
            services.AddCors(options =>
            {
                options.AddPolicy("Default", policy =>
                {
                    policy.WithOrigins(Configuration["SelfHostAddress"])
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MHealth.API v1");
                c.OAuthClientId(Configuration["IdentityServer:ClientId"]);
                c.OAuthClientSecret(Configuration["IdentityServer:ClientSecret"]);
                c.OAuthScopes(new string[]
                            {
                                "openid",
                                "profile",
                                "email",
                                "user_roles",
                                "MHealth.Api:Read"
                            });
                c.OAuthAppName("MHealth API");
                c.OAuthUsePkce();
            });

            app.UseRouting();
            app.UseCors("Default");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
