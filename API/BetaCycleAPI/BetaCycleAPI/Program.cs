using BetaCycleAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BetaCycleAPI.Contexts;
using System.Text;
using Microsoft.OpenApi.Models;
using BetaCycleAPI.BLogic;

namespace BetaCycleAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHostedService<RetrainingService>();
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // JWT Authentication
            JwtSettings jwtSettings = new();
            jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            builder.Services.AddSingleton(jwtSettings);

            string url = builder.Configuration.GetSection("BaseSiteUrl").Get<string>();
            builder.Services.AddSingleton(url);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        RequireExpirationTime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                    };
                });

            //builder.Services.AddAuthentication()
            //    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", opt => { });

            //builder.Services.AddAuthorization(opts =>
            //{
            //    opts.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication")
            //        .RequireAuthenticatedUser().Build());
            //});

            Connectionstrings.AdventureWorks = builder.Configuration.GetConnectionString("AdventureWorks");
            Connectionstrings.Credentials = builder.Configuration.GetConnectionString("Credentials");

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BetaCycleAPI",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            builder.Services.AddDbContext<AdventureWorksLt2019Context>(opt =>
                    opt.UseSqlServer(builder.Configuration.GetConnectionString("AdventureWorks")));
            builder.Services.AddDbContext<AdventureWorks2019CredentialsContext>(opt =>
                    opt.UseSqlServer(builder.Configuration.GetConnectionString("Credentials")));
            builder.Services.AddResponseCaching();

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
            app.UseResponseCaching();


            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
