using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcctOpeningImageValidationAPI.Helpers;
using AcctOpeningImageValidationAPI.Logger;
using AcctOpeningImageValidationAPI.Repository;
using AcctOpeningImageValidationAPI.Repository.Abstraction;
using AcctOpeningImageValidationAPI.Repository.Services.Implementation;
using AcctOpeningImageValidationAPI.Services;
using FluentScheduler;
using IdentificationValidationLib;
using IdentificationValidationLib.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AcctOpeningImageValidationAPI
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
            var connection = Configuration.GetConnectionString("IconFluxOnebankIDCards");
            services.AddDbContext<Models.IconFluxOnebankIDCardsContext>(options => options.UseSqlServer(connection));

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            services.AddControllers()
               .AddNewtonsoftJson(options =>
               options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("EnableCORS", builder =>
            //    {
            //        builder.AllowAnyOrigin()
            //        .AllowAnyHeader()
            //        .AllowAnyMethod();
            //    });
            //});

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<IComputerVision, ComputerVision>();
            services.AddScoped<IAPI, API>();
            services.AddScoped<IExternalImageValidationService, ExternalImageValidationService>();
            services.AddScoped<ReadAttributesFromFacialImage.IFaceValidation, ReadAttributesFromFacialImage.FaceValidation>();
            services.AddScoped<IOCRRepository, OCRRepository>();
            services.AddScoped<IFaceRepository, FaceRepository>();
            services.AddScoped<RestClientService, RestClientService>();
            services.AddScoped<INetworkService, NetworkService>();
            services.AddScoped<IVerifyMeService, VerifyMeService>();
            // Register the Swagger generator, defining 1 or more Swagger documents

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = 1000000000;
                options.MultipartBodyLengthLimit = 10000000000; // if don't set default value is: 128 MB
                options.MultipartHeadersLengthLimit = 1000000000;
            });

            services.AddSwaggerGen();
            services.AddSignalR();
            JobManager.Initialize();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Models.IconFluxOnebankIDCardsContext context, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddLog4Net();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

#if DEBUG
                // For Debug in Kestrel
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
#else
               // To deploy on IIS
               c.SwaggerEndpoint("/FacialrecogAPI/swagger/v1/swagger.json", "My API V1");
#endif
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            if (!env.IsDevelopment())
            {
                context.Database.Migrate();
            }

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/hub/messages");
            });
        }
    }
}
