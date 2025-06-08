using Api.SignalR;
using Domain;
using Domain.RequestsValidators;
using FluentValidation;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.MapperProfiles;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                options.UseSqlServer(builder.Configuration.GetConnectionString("default"));
            });

            builder.Services.AddIdentity<AppUser, IdentityRole<int>>()
                            .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration.GetSection("GoogleAuth")["ClientID"];
                options.ClientSecret = builder.Configuration.GetSection("GoogleAuth")["ClientSecret"];
                options.CallbackPath = "/signin-google";
            });


            InfrastructureServices.Register((irepo, repo) => builder.Services.AddScoped(irepo, repo));
            builder.Services.AddAutoMapper(typeof(InfraMapperProfile).Assembly);


            DomainServices.Register(type => builder.Services.AddScoped(type));
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();


            builder.Services.AddSignalR();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ChatHub>("chat-hub");

            app.Run();
        }
    }
}
