// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace MHealth.IdentityProvider
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile(),
                        new IdentityResources.Email(),
                        new IdentityResource("user_roles", new[]
                        {
                            "role"
                        })
                   };

        public static IEnumerable<ApiResource> ApiResources => new[]
        {
            new ApiResource("MHealth.Api")
            {
                Scopes = new[]
                {
                    "MHealth.Api:Read"
                },
                UserClaims = new[]
                {
                    "name",
                    "role"
                }
            }
        };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("MHealth.Api:Read","MHealth Api General Read Access")
            };

        public static IEnumerable<Client> Clients(IConfiguration configuration) =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "mhealth.api",
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowOfflineAccess = true,
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Profile,
                        "user_roles",
                        "MHealth.Api:Read"
                    },
                    RedirectUris = new[]
                    {
                        $"{configuration["Api:BaseAddress"]}/swagger/oauth2-redirect.html"
                    },
                    AllowedCorsOrigins = new[]
                    {
                        configuration["Api:BaseAddress"]
                    }
                }
            };
    }
}