using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Persistence;
using System.Reflection;
using System.Text;

namespace SurveyBasket.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependency(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddControllers();
            services.AddDbContextConfig(configuration)
                    .AddAuthConfig();

            services.AddSwaggerConfig()
                    .AddMapsterConfig()
                    .AddFluentValidationConfig()
                    .AddServicesConfig();


            return services;
        }
        private static IServiceCollection AddServicesConfig(this IServiceCollection services)
        {
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
        private static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }
        private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
        {
            // Add Mapster Global Configration
            var mappConfig = TypeAdapterConfig.GlobalSettings;
            mappConfig.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton<IMapper>(new Mapper(mappConfig));
            return services;
        }
        private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
        {
            // Add FluentValidation
            services.AddFluentValidationAutoValidation()
                    .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }

        private static IServiceCollection AddDbContextConfig(this IServiceCollection services,IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection String Not Found");
            services.AddDbContext<ApplicationDbContext>(option =>
            option.UseSqlServer(connectionString));
            return services;
        }

        //Jwt
        private static IServiceCollection AddAuthConfig(this IServiceCollection services)
        {
            services.AddSingleton<IJwtProvider, JwtProvider>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("J7MfAb4WcAIMkkigVtIepIILOVJEjAcB")),
                    ValidIssuer = "SurveyBasketApp",
                    ValidAudience = "SurveyBasketApp users"
                };
            });

            return services;
        }
    }
}