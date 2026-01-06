using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Data;
using WebAPI.Interfaces;
using WebAPI.Repositories;
using WebAPI.Services;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography.Xml;
using Microsoft.Extensions.FileProviders;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using StackExchange.Redis;
using WebAPI.Middleware;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            var dbContext = builder.Services.BuildServiceProvider().GetService<DataContext>();

            builder.Services.AddSingleton<IRefreshTokenGenerator>(provider => new RefreshTokenGeneratorRepository(dbContext));
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            
            // Configure Redis (if enabled)
            var redisEnabled = builder.Configuration.GetValue<bool>("Redis:Enabled", false);
            var useRedisForRateLimiting = builder.Configuration.GetValue<bool>("RateLimiting:UseRedis", false);
            
            if (redisEnabled && useRedisForRateLimiting)
            {
                var redisConfiguration = builder.Configuration.GetValue<string>("Redis:Configuration");
                var redis = ConnectionMultiplexer.Connect(redisConfiguration);
                builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
                
                var instanceName = builder.Configuration.GetValue<string>("Redis:InstanceName", "RequestSystem_");
                builder.Services.AddSingleton(new RedisRateLimitStorage(redis, instanceName));
            }
            
            // Configure CORS with specific origins, methods, and headers
            var corsAllowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" };
            var corsAllowedMethods = builder.Configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ?? new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
            var corsAllowedHeaders = builder.Configuration.GetSection("Cors:AllowedHeaders").Get<string[]>() ?? new[] { "Content-Type", "Authorization", "Accept" };
            var corsAllowCredentials = builder.Configuration.GetValue<bool>("Cors:AllowCredentials", true);
            var corsMaxAge = builder.Configuration.GetValue<int>("Cors:MaxAge", 3600);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("RequestSystemCorsPolicy", policy =>
                {
                    policy.WithOrigins(corsAllowedOrigins)
                          .WithMethods(corsAllowedMethods)
                          .WithHeaders(corsAllowedHeaders)
                          .SetPreflightMaxAge(TimeSpan.FromSeconds(corsMaxAge));

                    if (corsAllowCredentials)
                    {
                        policy.AllowCredentials();
                    }
                });
            });
            
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Configure Rate Limiting
            var rateLimitingEnabled = builder.Configuration.GetValue<bool>("RateLimiting:EnableRateLimiting", true);
            
            if (rateLimitingEnabled)
            {
                builder.Services.AddRateLimiter(options =>
                {
                    // Global policy - applies to all endpoints by default
                    var globalPermitLimit = builder.Configuration.GetValue<int>("RateLimiting:GlobalPolicy:PermitLimit", 100);
                    var globalWindow = builder.Configuration.GetValue<int>("RateLimiting:GlobalPolicy:Window", 60);
                    
                    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    {
                        var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
                        
                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: clientId,
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = globalPermitLimit,
                                Window = TimeSpan.FromSeconds(globalWindow),
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = 0
                            });
                    });

                    // Login endpoint policy - stricter limits for authentication
                    var loginPermitLimit = builder.Configuration.GetValue<int>("RateLimiting:LoginPolicy:PermitLimit", 5);
                    var loginWindow = builder.Configuration.GetValue<int>("RateLimiting:LoginPolicy:Window", 60);
                    
                    options.AddPolicy("LoginPolicy", context =>
                    {
                        var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
                        
                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: $"login:{clientId}",
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = loginPermitLimit,
                                Window = TimeSpan.FromSeconds(loginWindow),
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = 0
                            });
                    });

                    // API endpoint policy - moderate limits
                    var apiPermitLimit = builder.Configuration.GetValue<int>("RateLimiting:ApiPolicy:PermitLimit", 30);
                    var apiWindow = builder.Configuration.GetValue<int>("RateLimiting:ApiPolicy:Window", 60);
                    
                    options.AddPolicy("ApiPolicy", context =>
                    {
                        var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
                        
                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: $"api:{clientId}",
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = apiPermitLimit,
                                Window = TimeSpan.FromSeconds(apiWindow),
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = 0
                            });
                    });

                    // Configure rejection response
                    options.OnRejected = async (context, token) =>
                    {
                        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        
                        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                        {
                            context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
                        }

                        await context.HttpContext.Response.WriteAsJsonAsync(new
                        {
                            error = "Too many requests",
                            message = "Rate limit exceeded. Please try again later.",
                            retryAfter = retryAfter?.TotalSeconds
                        }, cancellationToken: token);
                    };
                });
            }

            var secretKey = builder.Configuration.GetSection("AppSettings:SecretKey").Value;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(opt =>
                    {
                        opt.RequireHttpsMetadata = false;
                        opt.SaveToken = false;
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = key,
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };
                    });

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JWT Token",
                    Version = "v1",
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using Bearer scheme \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Use the configured CORS policy
            app.UseCors("RequestSystemCorsPolicy");

            // Enable rate limiting
            if (rateLimitingEnabled)
            {
                app.UseRateLimiter();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(builder.Environment.ContentRootPath, "Resources", "images")),
                RequestPath = "/Resources"
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}