using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ReservaVehiculos.API.Entities;
using ReservaVehiculos.API.Services;

namespace ReservaVehiculos.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            string domain = $"https://{Configuration["Auth0:Domain"]}/";

            //Config Autenticacion
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = Configuration["Auth0:Audience"];
            });

            ///Config Autorizacion
            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:reservas", policy => policy.Requirements.Add(new HasScopeRequirement("read:reservas", domain)));
                options.AddPolicy("write:reservas", policy => policy.Requirements.Add(new HasScopeRequirement("write:reservas", domain)));
                options.AddPolicy("read:paises", policy => policy.Requirements.Add(new HasScopeRequirement("read:paises", domain)));

            });

            services.AddControllers();

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            services.AddSingleton<IReservaService, ReservaService>();

            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddAWSService<Amazon.S3.IAmazonS3>();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AWS Serverless Asp.Net Core Web API", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = feature.Error;
                var result = "";
                if (exception is CustomException)
                {
                    var customException = exception as CustomException;
                    result = JsonConvert.SerializeObject(new { descripcion = exception.Message });
                    context.Response.StatusCode = customException.ErrorCode;

                }
                else
                {
                    result = JsonConvert.SerializeObject(new { descripcion = exception.Message });
                    context.Response.StatusCode = 500;
                }


                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            string envName = Configuration["Swagger:Enviroment"];
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(envName + "/swagger/v1/swagger.json", "AWS Serverless Asp.Net Core Web API");
                c.RoutePrefix = "swagger";
            });


        }

    }
}