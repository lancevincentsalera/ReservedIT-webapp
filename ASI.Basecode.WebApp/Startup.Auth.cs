﻿using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Extensions.Configuration;
using ASI.Basecode.Resources.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Security.Claims;

namespace ASI.Basecode.WebApp
{
    // Authorization configuration
    internal partial class StartupConfigurer
    {
        private readonly SymmetricSecurityKey _signingKey;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly TokenProviderOptions _tokenProviderOptions;

        /// <summary>
        /// Configure authorization
        /// </summary>
        private void ConfigureAuthorization()
        {
            var token = Configuration.GetTokenAuthentication();
            var tokenProviderOptionsFactory = this._services.BuildServiceProvider().GetService<TokenProviderOptionsFactory>();
            var tokenValidationParametersFactory = this._services.BuildServiceProvider().GetService<TokenValidationParametersFactory>();
            var tokenValidationParameters = tokenValidationParametersFactory.Create();

            this._services.AddAuthentication(Const.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            })
            .AddCookie(Const.AuthenticationScheme, options =>
            {
                options.Cookie = new CookieBuilder()
                {
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest,
                    Name = $"{this._environment.ApplicationName}_{token.CookieName}"
                };
                options.LoginPath = new PathString("/Account/Login");
                options.AccessDeniedPath = new PathString("/ErrorPage/Forbidden");
                options.ReturnUrlParameter = "ReturnUrl";
                options.TicketDataFormat = new CustomJwtDataFormat(SecurityAlgorithms.HmacSha256, _tokenValidationParameters, Configuration, tokenProviderOptionsFactory);
            });

            this._services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAuthenticatedUser", policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, Enums.UserRoleManager.ROLE_ADMIN.ToString(), Enums.UserRoleManager.ROLE_SUPER.ToString());
                });
                options.AddPolicy("ManagerRegular", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, Enums.UserRoleManager.ROLE_MANAGER.ToString(), Enums.UserRoleManager.ROLE_REGULAR.ToString());
                });
                options.AddPolicy("ManagerOnly", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, Enums.UserRoleManager.ROLE_MANAGER.ToString());
                });
                options.AddPolicy("RegularUserOnly", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, Enums.UserRoleManager.ROLE_REGULAR.ToString());
                });
            });

            this._services.AddMvc(options =>
            {
                options.Filters.Add(new AuthorizeFilter("RequireAuthenticatedUser"));
            });
        }
    }
}
