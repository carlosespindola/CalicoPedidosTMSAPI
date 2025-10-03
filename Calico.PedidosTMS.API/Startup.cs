using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Calico.PedidosTMS.API.Auth;
using Calico.PedidosTMS.Auth.API.Filters;
using Calico.PedidosTMS.Auth.API.Helpers;
using Calico.PedidosTMS.Auth.API.Models;
using Calico.PedidosTMS.Auth.API.Wrappers;
using Calico.PedidosTMS.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;



namespace Calico.PedidosTMS.Auth.API
{
    public class Startup
    {
        private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Fix for Swagger/ApiExplorer compatibility with .NET 9
            AppContext.SetSwitch("Microsoft.AspNetCore.Mvc.ApiExplorer.IsEnhancedModelMetadataSupported", true);

            services
                .AddControllers(options => options.Filters.Add<ErrorHandlingFilterAttribute>())
                .AddJsonOptions(opts =>
                {
                    var enumConverter = new JsonStringEnumConverter();
                    opts.JsonSerializerOptions.Converters.Add(enumConverter);
                    // Enable reflection-based serialization for anonymous types and complex objects
                    opts.JsonSerializerOptions.TypeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver();
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(string.Format(@"{0}\Calico.PedidosTMS.API.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Calico.PedidosTMS.API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer"
                });

                c.CustomSchemaIds(type => type.ToString());

                // NOTE! THIS IS NEEDED FOR THE FCKING FILTER FOR AUTHORIZE. CLASS IS DEFINED AT THE BOTTOM OF THE FILE
                c.OperationFilter<AuthenticationRequirementsOperationFilter>();

                //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                //c.IgnoreObsoleteActions();
                //c.IgnoreObsoleteProperties();
                //c.CustomSchemaIds(type => type.FullName);
            });

            services.Configure<MvcOptions>(options =>
            {
                options.EnableEndpointRouting = true;
            });


            services.AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });
            
            //services.AddApiVersioning(o => {
            //    o.AssumeDefaultVersionWhenUnspecified = true;
            //    o.DefaultApiVersion = new ApiVersion(1, 0);
            //    o.ReportApiVersions = true;
            //});


            #region To Implement
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        // .AllowCredentials()
                        );
            });

            var jwtAppSettingOptions = Configuration.GetSection(nameof(Jwt));

            // Configure Jwt
            services.Configure<Jwt>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(Jwt.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(Jwt.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    //ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = _signingKey //new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

    //        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //.AddJwtBearer(options =>
    //{
    //    options.Authority = "https://your-auth-server.com"; // Replace with your identity provider
    //    options.Audience = "your-api";
    //});

            // Add end point for health checks
            services.AddHealthChecks()
                .AddSqlServer(Configuration.GetConnectionString("DefaultConnection"), name: "SQL Server Check", timeout: TimeSpan.FromSeconds(3), tags: new[] { "ready" });


            // Register Dapper Contexts
            services.AddScoped(sp => new Calico.PedidosTMS.DAL.DapperContexts.CalicoInterfazDapperContext(
                Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(sp => new Calico.PedidosTMS.DAL.DapperContexts.CalicoMasterDapperContext(
                Configuration.GetConnectionString("DBMasterConnection")));

            services.AddScoped(sp => new Calico.PedidosTMS.DAL.DapperContexts.CalicoApiTmsDapperContext(
                Configuration.GetConnectionString("Calico_API_TMS_Connection")));

            // Register Repositories
            services.AddScoped<Calico.PedidosTMS.DAL.Repositories.IClienteAccesoApiRepository, Calico.PedidosTMS.DAL.Repositories.ClienteAccesoApiRepository>();
            services.AddScoped<Calico.PedidosTMS.DAL.Repositories.IRefreshTokenRepository, Calico.PedidosTMS.DAL.Repositories.RefreshTokenRepository>();

            // Register Services
            services.AddScoped<Calico.PedidosTMS.API.Services.PedidosTMSService>();
            services.AddScoped<Calico.PedidosTMS.API.Services.RotuloService>();

            services.AddSingleton<IJwtFactory, JwtFactory>();

            #endregion

            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");

            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                    ResponseWriter = async (context, report) =>
                    {
                        var result = JsonSerializer.Serialize(
                            new
                            {
                                status = report.Status.ToString(),
                                checks = report.Entries.Select(entry => new
                                {
                                    name = entry.Key,
                                    status = entry.Value.Status.ToString(),
                                    exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                                    duration = entry.Value.Duration.ToString()
                                })
                            }
                        );
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                }); // database connection is good
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = (_) => false }); // service up n running
            });

            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features
                    .Get<IExceptionHandlerPathFeature>()
                    .Error;

                // Gets the level of detail to return
                var errorLevelDetail = (Configuration.GetValue<string>("TRSettings:ErrorDetailsLevel") ?? "low").ToUpper();
                string content;

                // If exception is an token validation then return a 550, else return a 500 with details
                if (exception.GetType() == typeof(InvalidToken))
                {
                    if (errorLevelDetail == "HIGH")
                        content = JsonSerializer.Serialize(new ServerErrorExceptionResponse { Error = exception.Message, Trace = exception.StackTrace, InnerException = exception.InnerException?.Message ?? string.Empty });
                    else
                        content = JsonSerializer.Serialize(new ServerErrorExceptionResponse { Error = exception.Message });

                    context.Response.StatusCode = 550;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(content);
                }
                else
                {
                    if (errorLevelDetail == "HIGH")
                        content = JsonSerializer.Serialize(new ServerErrorExceptionResponse { Error = exception.Message, Trace = exception.StackTrace, InnerException = exception.InnerException?.Message ?? string.Empty });
                    else
                        content = JsonSerializer.Serialize(new ServerErrorExceptionResponse { Error = exception.Message });

                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(content);
                }
            }));

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Calico.PedidosTMS.API v1"));


            //app.UseSwaggerUI(options =>
            //{
            //    var descriptions = "Brain-system.Pedidos.API v1";
            //    foreach (var description in descriptions)
            //    {
            //        var url = $"/swagger/{description.GroupName}/swagger.json";
            //        var name = description.GroupName.ToUpperInvariant();
            //        options.SwaggerEndpoint(url, name);
            //    }
            //});


        }

        /********************************************* codigo net core 2.1
             *
        static string XmlCommentsFilePath
        {
            get
            {
                var fileName = typeof( Startup ).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine( AppContext.BaseDirectory, fileName );
            }
        }

        static Info CreateInfoForApiVersion( ApiVersionDescription description )
        {
            var info = new Info()
            {
                Title = $"Translog - API de Pedidos {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "Translog - API de Pedidos.",
                Contact = new Contact() { Name = "Soporte Translog", Email = "wfamulari@brain-system.com.ar" },
                TermsOfService = "None"
                //, License = new License() { Name = "MIT", Url = "https://opensource.org/licenses/MIT" }
            };

            if ( description.IsDeprecated )
            {
                info.Description += " Esta version de API ha sido deprecada.";
            }

            return info;
        }    
        **********/
    }



    /// <summary>
    /// Class used by swagger to filter which operations have to use authorization and which doesn't
    /// </summary>
    public class AuthenticationRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authRequired = context.ApiDescription.CustomAttributes().Any(attr => attr.GetType() == typeof(AuthorizeAttribute));

            if (authRequired == false) return;

            if (operation.Security == null)
                operation.Security = new List<OpenApiSecurityRequirement>();

            operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }, new List<string>()
                    }
                });
        }
    }
}
