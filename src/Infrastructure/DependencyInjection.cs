using System.Configuration;
using FullTemplate.Application.Common.Interfaces;
using FullTemplate.Infrastructure.Data;
using FullTemplate.Infrastructure.Data.Interceptors;
using FullTemplate.Infrastructure.Identity;
using FullTemplate.Infrastructure.Identity.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString);
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        builder.EnrichSqlServerDbContext<ApplicationDbContext>();

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());


        // .................................... for built-in ( Identity ) Auth  

        // builder.Services.AddAuthentication()
        //     .AddBearerToken(IdentityConstants.BearerScheme);


        builder.Services
            .AddIdentityCore<ApplicationUser>(o=>
            {
                o.User.RequireUniqueEmail = true;
                o.SignIn.RequireConfirmedEmail = true;     
                o.Lockout.MaxFailedAccessAttempts = 5;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                o.Password.RequiredLength = 8;
                o.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;   // 6 digit codes for reset

            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()          // needed for login, not added by AddIdentityCore
            .AddDefaultTokenProviders();   // needed for the 6 digit OTP/reset tokens
            // .AddApiEndpoints();  -> do not want built-in ( Identity ) Auth Endpoints
 
            //.......................................................... my custom Auth with jwt
            var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateIssuerSigningKey = true
                };
            });
        builder.Services.AddAuthorizationBuilder();


        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<TokenService>();
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
        builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.SectionName));
        builder.Services.Configure<AuthLinkOptions>(builder.Configuration.GetSection(AuthLinkOptions.SectionName));

        builder.Services.AddTransient<IEmailSender<ApplicationUser>, EmailSender>();
    }
}
