// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using MHealth.BusinessEntities;
using MHealth.SharedDataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MHealth.IdentityProvider
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString));

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                    var admin = userMgr.FindByNameAsync("admin@mhealth.com").Result;
                    if (admin == null)
                    {

                        admin = new AppUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            FullName = "Administrator",
                            UserName = "admin@mhealth.com",
                            Email = "admin@mhealth.com",
                            EmailConfirmed = true,
                            City = "Metro",
                            Region = "Lampung",
                            AddressLine = "AH. Nasution Street",
                            Country = "Indonesia",
                            Gender = AppUserGender.Unspecified,
                            PhotoUrl = "https://pbs.twimg.com/profile_images/1447837839404638227/Pi4j-2Z9_400x400.jpg",
                            PhoneNumber = "6289616482113"
                        };
                        var result = userMgr.CreateAsync(admin, "@Future30").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(admin, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Administrator"),
                            new Claim(JwtClaimTypes.Role, "admin")
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("Administrator user created");
                    }
                    else
                    {
                        Log.Debug("Administrator user already exists");
                    }

                }
            }
        }
    }
}
