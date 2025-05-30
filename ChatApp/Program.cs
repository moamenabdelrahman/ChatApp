using Domain;
using Domain.RequestsValidators;
using FluentValidation;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Identity;
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

            InfrastructureServices.Register((irepo, repo) => builder.Services.AddScoped(irepo, repo));
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            DomainServices.Register(type => builder.Services.AddScoped(type));
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
