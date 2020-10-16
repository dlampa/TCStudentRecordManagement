using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using TCStudentRecordManagement.Auth;
using TCStudentRecordManagement.Auth.Authorization;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Utils;

namespace TCStudentRecordManagement
{
    public class Startup
    {
        readonly string CORSAllowedOrigins = "_CORSAllowedOrigins";
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // Copy configuration to a static class (see APIConfig for details)
            APIConfig.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Context with SQL Server connection with parameters from appsettings.json
            // Ref: https://elanderson.net/2019/11/entity-framework-core-no-database-provider-has-been-configured-for-this-dbcontext/
            services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration["sqldb:ConnectionString"]));

            // CORS Configuration - will have to be adapted for the specific configuration on deployment
            // Ref: https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1

            services.AddCors(options =>
            {
                options.AddPolicy(CORSAllowedOrigins,
                                  builder =>
                                  {
                                      // Add host address from localsettings.json - Ref: https://stackoverflow.com/a/41330941/12802214
                                      builder.WithOrigins(Configuration.GetSection("CORSAllowedOrigins").GetChildren().ToArray().Select(x => x.Value).ToArray())
                                                          .AllowAnyHeader()
                                                          .AllowAnyMethod();
                                      /*builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); */
                                  });
            });

            // Add authentication service using JWT token validation
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SecurityTokenValidators.Clear();
                options.SecurityTokenValidators.Add(new GoogleTokenValidator());
                options.Events = new JwtBearerEvents()
                {

                    OnAuthenticationFailed = c =>
                    {
                        // Allows for clean processing of authentication failure response (normally returns the exception)
                        // Ref: https://stackoverflow.com/a/50451116/12802214
                        c.NoResult();
                        c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
            });

            // Add authorization policies
            // Ref: ASP.NET Core 3 and React, Carl Rippon, Pakt Publishing 2019 (ISBN: 9781789950229) 
            services.AddAuthorization(options =>
            {
                options.AddPolicy("StaffMember", policy => policy.Requirements.Add(new StaffAuthCheck()));
                options.AddPolicy("SuperAdmin", policy => policy.Requirements.Add(new SuperAuthCheck()));
            });

            // Register handler for dependency injection
            services.AddScoped<IAuthorizationHandler, StaffAuthCheckHandler>();
            services.AddScoped<IAuthorizationHandler, SuperAuthCheckHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applife)
        {
            // Application shutdown handling
            // Ref: https://thinkrethink.net/2017/03/09/application-shutdown-in-asp-net-core/
            applife.ApplicationStopping.Register(new ShutdownHandler().ShutdownCleanup);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(CORSAllowedOrigins);

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }




}
