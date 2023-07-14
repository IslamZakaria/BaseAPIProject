using Core.Settings;
using DTOs.Shared.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.RateLimiting;

namespace Services.Configuration
{
    public static class CoreDependencies
    {
        /// <summary>
        /// Adds Basic Setup for Controllers with NewtonsoftJson, Adding Cors, and HttpContextAccessor
        /// </summary>
        /// <param name="services">IServiceCollection to Extend</param>
        /// <returns>Extended IServiceCollection</returns>
        public static IServiceCollection AddEndpointsSetup(this IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver =
                                             new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddCors();

            services.AddHttpContextAccessor();

            return services;
        }

        /// <summary>
        /// Add Swagger Setup with Docs and option of adding Security Definition
        /// </summary>
        /// <param name="services">IServiceCollection to Extend</param>
        /// <param name="apiTitle">Title of the OpenApiInfo</param>
        /// <param name="apiVersion">API Version of the OpenApiInfo</param>
        /// <param name="addSecurity">Flag to choose whether to add Security Definition or not</param>
        /// <param name="addCrm">Flag to choose whether to add Security Definition or not</param>
        /// <param name="addAdmin">Flag to choose whether to add Security Definition or not</param>
        /// <param name="addMobile">Flag to choose whether to add Security Definition or not</param>
        /// <returns>Extended IServiceCollection</returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services,
                                                    IConfiguration configuration,
                                                    string apiTitle,
                                                    string apiVersion,
                                                    string xmlPath,
                                                    bool addSecurity = false,
                                                    bool addCrm = false,
                                                    bool addAdmin = false,
                                                    bool addMobile = false)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(apiVersion, new OpenApiInfo { Title = apiTitle, Version = apiVersion });

                // Enables Swagger Documentation
                c.IncludeXmlComments(xmlPath);

                if (addSecurity)
                {
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                                Scheme = "Bearer",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                            }, new List<string>()
                        },
                    });
                }

                if (addCrm)
                {
                    c.SwaggerDoc("CRM", new OpenApiInfo { Title = "Content CRM", Version = apiVersion });
                }
                if (addAdmin)
                {
                    c.SwaggerDoc("Admin", new OpenApiInfo { Title = "Admin Portal", Version = apiVersion });
                }
                if (addMobile)
                {
                    c.SwaggerDoc("Mobile", new OpenApiInfo { Title = "Content Mobile", Version = apiVersion });
                }
            });

            services.Configure<SwaggerSettings>(configuration.GetSection("SwaggerSettings"));
            return services;
        }

        /// <summary>
        /// Add Api Supported Versions
        /// </summary>
        /// <param name="services">IServiceCollection to Extend</param>
        /// <param name="defaultApiVersion">ApiVersion class to set the Default Api Version</param>
        /// <param name="assumeDefaultVersionWhenUnspecified">boolean</param>
        /// <param name="reportApiVersions">boolean</param>
        /// <returns>Extended IServiceCollection</returns>
        public static IServiceCollection AddApiVersioningSetup(this IServiceCollection services,
                                                               ApiVersion defaultApiVersion,
                                                               bool assumeDefaultVersionWhenUnspecified = true,
                                                               bool reportApiVersions = true)
        {
            services.AddApiVersioning(config =>
            {
                // Specify the default API Version
                config.DefaultApiVersion = defaultApiVersion;
                // If the client hasn't specified the API version in the request, use the default API version number 
                config.AssumeDefaultVersionWhenUnspecified = assumeDefaultVersionWhenUnspecified;
                // Advertise the API versions supported for the particular endpoint
                config.ReportApiVersions = reportApiVersions;
            });

            return services;
        }

        /// <summary>
        /// Add JWT Authentication to the API
        /// </summary>
        /// <param name="services">IServiceCollection to Extend</param>
        /// <param name="configuration">IConfiguration to access jwt settings</param>
        /// <returns>Extended IServiceCollection</returns>
        public static IServiceCollection AddJWTAuthentication(this IServiceCollection services,
                                                              IConfiguration configuration)
        {
            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                };
                o.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.Response.OnStarting(async () =>
                        {
                            c.NoResult();
                            c.Response.StatusCode = 401;
                            c.Response.ContentType = "text/plain";
                            await c.Response.WriteAsync(c.Exception.ToString());
                        });
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.Response.OnStarting(async () =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new Response<string>("You are not Authorized"));
                            await context.Response.WriteAsync(result);
                        });
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.OnStarting(async () =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new Response<string>("You are not authorized to access this resource"));
                            await context.Response.WriteAsync(result);
                        });
                        return Task.CompletedTask;
                    },
                };
            });

            return services;
        }

        public static IServiceCollection AddNativeRateLimiting(this IServiceCollection services,
                                                               IConfiguration configuration)
        {
            services.Configure<RateLimitingSettings>(
                configuration.GetSection(RateLimitingSettings.RateLimitSection));

            var rateLimitingSettings = new RateLimitingSettings();
            configuration.GetSection(RateLimitingSettings.RateLimitSection).Bind(rateLimitingSettings);

            services.AddRateLimiter(_ =>
            {
                _.OnRejected = (context, _) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                    }

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    var response = System.Text.Json.JsonSerializer.Serialize(new Response<string>(rateLimitingSettings.RateLimitMessage));

                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.WriteAsync(response);

                    return new ValueTask();
                };

                _.AddPolicy(rateLimitingSettings.ThirdPartiesPolicy, context =>
                {
                    IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;

                    if (!IPAddress.IsLoopback(remoteIpAddress!))
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(remoteIpAddress!,
                         _ => new FixedWindowRateLimiterOptions
                         {
                             PermitLimit = rateLimitingSettings.ThirdPartyWindowLimit,
                             Window = TimeSpan.FromDays(rateLimitingSettings.ThirdPartyWindowPeriod)
                         });
                    }

                    return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
                });

                _.AddPolicy("foodicsPolicy", context =>
                {
                    var routes = context.Request.RouteValues;

                    long restaurantId = 0;

                    if (routes.TryGetValue("restaurantId", out var id))
                    {
                        restaurantId = long.Parse((string)id!);

                        return RateLimitPartition.GetFixedWindowLimiter(restaurantId.ToString(),
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = rateLimitingSettings.ThirdPartyWindowLimit,
                            Window = TimeSpan.FromDays(rateLimitingSettings.ThirdPartyWindowPeriod)
                        });

                    }

                    return RateLimitPartition.GetNoLimiter(restaurantId.ToString());
                });

                _.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                     PartitionedRateLimiter.Create<HttpContext, string>(context =>
                     {
                         var accessToken = context.Request.Headers[rateLimitingSettings.GlobalTokenBucketHeaderName].ToString().Replace("Bearer ", "");

                         if (StringValues.IsNullOrEmpty(accessToken))
                         {
                             accessToken = rateLimitingSettings.DefaultGlobalTokenBucketKey;
                         }

                         return RateLimitPartition.GetTokenBucketLimiter(accessToken,
                           _ => new TokenBucketRateLimiterOptions
                           {
                               ReplenishmentPeriod = TimeSpan.FromMinutes(rateLimitingSettings.GlobalTokenReplenishmentPeriod),
                               TokenLimit = rateLimitingSettings.GlobalTokensLimit,
                               TokensPerPeriod = rateLimitingSettings.GlobalTokensPerPeriod
                           });
                     })
                    );
            });

            return services;
        }
    }
}
