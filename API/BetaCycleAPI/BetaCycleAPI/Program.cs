
using BetaCycleAPI.BLogic.Authentication.Basic;
using BetaCycleAPI.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BetaCycleAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", opt => { });

            builder.Services.AddAuthorization(opts =>
            {
                opts.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication")
                    .RequireAuthenticatedUser().Build());
            });


            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AdventureWorksLt2019Context>(opt =>
                    opt.UseSqlServer(builder.Configuration.GetConnectionString("AdventureWorks")));
            builder.Services.AddDbContext<AdventureWorks2019CredentialsContext>(opt =>
                    opt.UseSqlServer(builder.Configuration.GetConnectionString("Credentials")));

            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((hosts) => true));
            });
            builder.Services.AddControllersWithViews()
                    .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();


            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
