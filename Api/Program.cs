using Api.SignalR;
using Domain;
using Domain.RequestsValidators;
using FluentValidation;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.MapperProfiles;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ChatApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:default"]);
            });

            builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!String.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat-hub"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["GoogleAuth:ClientID"];
                    options.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
                    options.CallbackPath = "/signin-google";
                });


            InfrastructureServices.Register((irepo, repo) => builder.Services.AddScoped(irepo, repo));
            builder.Services.AddAutoMapper(typeof(InfraMapperProfile).Assembly);
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<TokenProvider>();


            DomainServices.Register(type => builder.Services.AddScoped(type));
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();


            builder.Services.AddSignalR();

            var allowedOrigins = builder.Configuration
                                        .GetSection("Cors:AllowedOrigins")
                                        .Get<string[]>();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(allowedOrigins)
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ChatHub>("chat-hub");

            app.Run();
        }
    }
}
