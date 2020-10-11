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
using Microsoft.Extensions.Logging;
using TCStudentRecordManagement.Auth;
using TCStudentRecordManagement.Auth.Authorization;
using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement
{
    public class Startup
    {
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
                        //c.Response.ContentType = "text/plain";
                        //c.Response.WriteAsync(c.Exception.ToString()).Wait();
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
            });

            // Add authorization policies
            // Ref: ASP.NET Core 3 and React, Carl Rippon, React Publishing 2019 (ISBN: 9781789950229) 
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }



}
