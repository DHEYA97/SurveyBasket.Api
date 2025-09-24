using FluentValidation.AspNetCore;
using Hangfire;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Health;
using SurveyBasket.Api.Persistence;
using SurveyBasket.Api.Settinges;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace SurveyBasket.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependency(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddControllers();
            
            services.AddDbContextConfig(configuration)
                    .AddAuthConfig(configuration)
                    .AddCorsConfig(configuration)
                    .AddMailConfig(configuration)
                    .AddHangFireConfig(configuration)
                    .AddHealthCheckConfig(configuration);

            services.AddSwaggerConfig()
                    .AddMapsterConfig()
                    .AddFluentValidationConfig()
                    .AddServicesConfig()
                    .AddCacheConfig()
                    .AddExceptionHandlerConfig()
                    .AddHttpContextAccessorConfig()
                    .AddRateLimitConfig();


            return services;
        }
        private static IServiceCollection AddServicesConfig(this IServiceCollection services)
        {
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IVoteService, VoteService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            //services.AddScoped<ICacheService, CacheService>();
            return services;
        }
        private static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "SurveyBasket API",
                    Version = "v1"
                });
            });
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
        private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddPermissionConfig();

            services.AddSingleton<IJwtProvider, JwtProvider>();

            //Set Configure og JwtOptions from Appsetting

            //before DataAnnotation
            //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            //After DataAnnotation
            services.AddOptions<JwtOptions>()
                    .BindConfiguration(JwtOptions.SectionName)
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            //Read into JwtOptions from Appsetting
            var jwtOption = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();


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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption!.Key)),
                    ValidIssuer = jwtOption!.Issuer,
                    ValidAudience = jwtOption!.Audience
                };
            });


            //Add Idintity configration
            services.AddIdentityConfig();
            return services;
        }
        private static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var allowOrigin = configuration.GetSection("AllowOrigin").Get<string[]>();

            
            services.AddCors(option => {
                                option.AddDefaultPolicy(bulder =>
                                                        bulder.AllowAnyOrigin()
                                                              .AllowAnyMethod()
                                                              .AllowAnyHeader()

                                //Add from AppSetting
                                //.WithOrigins(allowOrigin)
                                );
                                //More Than One Policy
                                //option.AddPolicy("MyPolicy02", bulder =>
                                //                                        bulder.AllowAnyOrigin()
                                //                                                .AllowAnyMethod()
                                //                                                .AllowAnyHeader()

                                                
                                //                );
            }
                             );
            return services;
        }
        private static IServiceCollection AddExceptionHandlerConfig(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>()
                    .AddProblemDetails();
            return services;
        }
        private static IServiceCollection AddCacheConfig(this IServiceCollection services)
        {
            services.AddHybridCache();
            return services;
        }
        private static IServiceCollection AddIdentityConfig(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(Options =>
            {
                Options.Password.RequiredLength = 8;
                Options.SignIn.RequireConfirmedEmail = true;
                Options.User.RequireUniqueEmail = true;
            });
            return services;
        }
        private static IServiceCollection AddMailConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSetting>(configuration.GetSection(MailSetting.SectionName));
            return services;
        }
        private static IServiceCollection AddHangFireConfig(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddHangfire(config => config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

            services.AddHangfireServer();
            return services;
        }
        private static IServiceCollection AddHttpContextAccessorConfig(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            return services; 
        }
        private static IServiceCollection AddPermissionConfig(this IServiceCollection services)
        {
            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            return services;
        }
        private static IServiceCollection AddHealthCheckConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection String Not Found");
            services.AddHealthChecks()
                    .AddDbContextCheck<ApplicationDbContext>(name: "DataBase From Entity FrameWork")
                    .AddSqlServer(name: "DataBase From Sql Server",connectionString: connectionString)
                    .AddHangfire(opttions =>
                    {
                        opttions.MinimumAvailableServers = 1;
                    },name: "HangFire")
                    .AddUrlGroup(name: "google Api", uri:new Uri("https://www.google.com"), tags: ["api"]) // Tag-Optional
                    .AddUrlGroup(name: "facebook Api", uri:new Uri("https://www.facebook.com"), tags: ["api"])
                    .AddCheck<MailHealthCheck>(name: "Mail Health Check");
            return services;
        }

        private static IServiceCollection AddRateLimitConfig(this IServiceCollection services)
        {
            services.AddRateLimiter(ratelimitConfig =>
            {
                ratelimitConfig.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                // 1- Concurrency
                ratelimitConfig.AddConcurrencyLimiter(RateLimitConst.concurrency, option =>
                {
                    option.PermitLimit = 2;
                    option.QueueLimit = 1;
                    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                // 2- Fixed Window
                ratelimitConfig.AddFixedWindowLimiter(RateLimitConst.fixedWindow, option =>
                {
                    option.PermitLimit = 2;
                    option.QueueLimit = 1;
                    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    option.Window = TimeSpan.FromSeconds(20);
                });

                // 3- Token Bucket
                ratelimitConfig.AddTokenBucketLimiter(RateLimitConst.tokenBucket, option =>
                {
                    option.TokenLimit = 2;
                    option.QueueLimit = 1;
                    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    option.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
                    option.TokensPerPeriod = 2;
                    option.AutoReplenishment = true;
                });

                // 4- Fixed Window
                ratelimitConfig.AddSlidingWindowLimiter(RateLimitConst.slidingWindow, option =>
                {
                    option.PermitLimit = 2;
                    option.QueueLimit = 1;
                    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    option.Window = TimeSpan.FromSeconds(20);
                    option.SegmentsPerWindow = 2;
                });


                // 5- Ip Adress
                ratelimitConfig.AddPolicy(RateLimitConst.ipAddress, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey:httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromSeconds(20)
                        }
                    )
                );

                // 6- User
                ratelimitConfig.AddPolicy(RateLimitConst.userLimit, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromSeconds(20)
                        }
                    )
                );

            });
            return services;
        }
    }
}



















