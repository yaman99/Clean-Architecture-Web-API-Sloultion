using Adsbility.Appilication.Common.Interfaces;
using Adsbility.Appilication.Interfaces;
using Adsbility.Infrastructure.Identity;
using Adsbility.Infrastructure.presistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adsbility.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppilicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));

            services.AddScoped<IAppilicatioDbContext>(provider => provider.GetService<AppilicationDbContext>());

            services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;

                options.SignIn.RequireConfirmedEmail = true; // not working!!
                options.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<AppilicationDbContext>()
                .AddDefaultTokenProviders();

            var key = Encoding.UTF8.GetBytes(configuration["ApplicationSettings:JWT_Secret"].ToString());
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // validate the security key during the token validation
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key), //get or set the SecurityKey to use for signature 
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme =  JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddSingleton<IJwtHandler, JwtHandler>();

            return services;
        }
    }
}
