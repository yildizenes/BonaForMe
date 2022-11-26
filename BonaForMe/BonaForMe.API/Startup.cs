using AutoMapper;
using BonaForMe.API.Infrastucture;
using BonaForMe.DataAccessCore;
using BonaForMe.DomainCommonCore.Constants;
using BonaForMe.ServicesCore.JwtService;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace BonaForMe.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(Opt =>
            {
                Opt.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            });

            services.AddMvcCore().AddAuthorization();

            services.AddMemoryCache();
            services.AddControllers();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            Configuration.GetConnectionString("BonaForMeDBContext");
            services.AddDbContext<BonaForMeDBContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("BonaForMeDBContext")), ServiceLifetime.Transient);

            JwtSettings.Audience = Configuration["JwtSettings:Audience"];
            JwtSettings.Encryptkey = Configuration["JwtSettings:Encryptkey"];
            JwtSettings.ExpirationMinutes = int.Parse(Configuration["JwtSettings:ExpirationMinutes"]);
            JwtSettings.Issuer = Configuration["JwtSettings:Issuer"];
            JwtSettings.SecretKey = Configuration["JwtSettings:SecretKey"];
            JwtSettings.NotBeforeMinutes = int.Parse(Configuration["JwtSettings:NotBeforeMinutes"]);

            services.AddLamar(new LamarMainRegistry(Configuration));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped(typeof(IJwtService), typeof(JwtService));

            //jwt start
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme   /*JwtBearerDefaults.AuthenticationScheme*/).AddCookie(
                options =>
                {

                    options.LoginPath = new PathString("/Account/Login/");

                })
               .AddJwtBearer(options =>
               {
                   var secretkey = Encoding.UTF8.GetBytes(JwtSettings.SecretKey);
                   var encryptionkey = Encoding.UTF8.GetBytes(JwtSettings.Encryptkey);

                   var validationParameters = new TokenValidationParameters
                   {
                       ClockSkew = TimeSpan.Zero, //default value 5 minute
                       RequireSignedTokens = true,

                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(secretkey),

                       RequireExpirationTime = true,
                       ValidateLifetime = true,

                       ValidateAudience = true, //default : false
                       ValidAudience = JwtSettings.Audience,

                       ValidateIssuer = true, //default : false
                       ValidIssuer = JwtSettings.Issuer,

                       TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
                   };

                   options.RequireHttpsMetadata = false;
                   options.SaveToken = true;
                   options.TokenValidationParameters = validationParameters;
               });
            //jwt end
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BonaForMeDBContext dbcontext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseRouting();


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}");
            });

            SeedDB.Initialize(dbcontext);
        }
    }
}
