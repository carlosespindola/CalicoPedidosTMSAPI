using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using BrainSystem.Pedidos.DAL;
using BrainSystem.Pedidos.Model;
using BrainSystem.Auth.API.Identity;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;
using BrainSystem.Auth.API.Filters;
using BrainSystem.Auth.API.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics;
using System.Net.Mime;
using BrainSystem.Auth.API.Helpers;
using BrainSystem.Auth.API.Wrappers;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Hosting;
using BrainSystem.Pedidos.API.Auth;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;



namespace BrainSystem.Auth.API
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
            services
                .AddControllers(options => options.Filters.Add<ErrorHandlingFilterAttribute>())
                .AddJsonOptions(opts =>
                {
                    var enumConverter = new JsonStringEnumConverter();
                    opts.JsonSerializerOptions.Converters.Add(enumConverter);
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(string.Format(@"{0}\Brain-system.Pedidos.API.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Brain-system.Pedidos.API", Version = "v1" });

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



            // Add framework services.
            //BrainSystemDBContext is for SAAD database connection!!
            services.AddDbContext<BrainSystemDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(serviceProvider => new
                     IDBContextFactory(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<SaadisDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SaadisConnection")));

            services.AddScoped(serviceProvider => new
                     ISaadisDBContextFactory(Configuration.GetConnectionString("SaadisConnection")));



            services.AddScoped(serviceProvider => new BrainSystemDBContextFactory(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(serviceProvider => new SaadisDBContextFactory(Configuration.GetConnectionString("SaadisConnection")));

            services.AddIdentity<CLIENTESACCESOAPI, IdentityRole>()
                .AddEntityFrameworkStores<SaadisDBContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IUserStore<CLIENTESACCESOAPI>, UserStore>();


            //old
            //services.AddTransient<IRoleStore<UserRole>, RoleStore>();

            services.AddSingleton<IJwtFactory, JwtFactory>();

            #endregion

            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });


            //*************************************************************************** OLD
            /******************************* codigo para Core 2.1
             * 
             * 
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });

            services.AddSingleton(Configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            // Add framework services.
            //BrainSystemDBContext is for SAAD database connection!!
            services.AddDbContext<BrainSystemDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(serviceProvider => new
                     IDBContextFactory(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<SaadisDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SaadisConnection")));

            services.AddScoped(serviceProvider => new
                     ISaadisDBContextFactory(Configuration.GetConnectionString("SaadisConnection")));

            //remove AdminDB from conn string and set it as expected by DBContextFactory
            //string companyConnStringTemplate = Configuration.GetConnectionString("DefaultConnection");
            //companyConnStringTemplate = companyConnStringTemplate.Replace("TCMasterDB", "{companyDBName}");

            services.AddScoped(serviceProvider => new BrainSystemDBContextFactory(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(serviceProvider => new SaadisDBContextFactory(Configuration.GetConnectionString("SaadisConnection")));

            services.AddDefaultIdentity<CLIENTESACCESOAPI>()
                .AddDefaultTokenProviders();

            services.AddTransient<IUserStore<CLIENTESACCESOAPI>, UserStore>();
            //services.AddTransient<IRoleStore<UserRole>, RoleStore>();
            
            services.AddSingleton<IJwtFactory, JwtFactory>();

            //services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            });

            //services.AddAutoMapper();
            //services.AddMvc().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddOptions();
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings")); //to have application settings from appsettings.json available in controllers

            services.AddApiVersioning(o => {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion( 1, 0 );
                o.ReportApiVersions = true;
            });

            services.AddSwaggerGen(
                options =>
                {
                    // resolve the IApiVersionDescriptionProvider service
                    // note: that we have to build a temporary service provider here because one has not been created yet
                    var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                    // add a swagger document for each discovered API version
                    // note: you might choose to skip or document deprecated API versions differently
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                    }

                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();

                    // integrate xml comments
                    options.IncludeXmlComments(XmlCommentsFilePath);

                    options.DescribeAllEnumsAsStrings();
                    //JWT - token authentication by password
                    options.AddSecurityDefinition("oauth2", new ApiKeyScheme
                    {
                        Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                        In = "header",
                        Name = "Authorization",
                        Type = "apiKey"
                    });

                    options.OperationFilter<SecurityRequirementsOperationFilter>();
                });
            ***************/
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Brain-system.Pedidos.API v1"));


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


            /********************************************* codigo net core 2.1
             * 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"../swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }

                    options.InjectStylesheet($"../SwaggerExtensions/BrainSystemSwagger.css");
                });

            ************/
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
