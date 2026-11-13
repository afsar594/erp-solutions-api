using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Inspire.Erp.Application;
using Inspire.Erp.Domain.Models.Common;
using Inspire.Erp.Infrastructure;
using Inspire.Erp.Infrastructure.CustomExceptionHandler;
using Inspire.Erp.NotificationBus;
using Inspire.Erp.Web.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace Inspire.Erp.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSignalR();
            services.AddControllers();
            services.AddApplication();
            services.AddInfrastructure(Configuration);

            // ✅ Use your existing JWT setup from DependencyInjection.cs
            services.AddTokenAuthentication(Configuration);

            // ✅ CORS for Angular + SignalR
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularClient",
                    builder =>
                    {
                        builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(_ => true)
                        .AllowCredentials();
                    });
            });

            // AutoMapper config
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Email settings
            services.Configure<EmailSettingViewModel>(Configuration.GetSection("MailSettings"));

            // Swagger config
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "InspireErpApi",
                    Version = "v1",
                    Description = "Services offered by Inspire Solutions",
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // PDF Converter
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            // Notification hub service
            services.AddScoped<INotificationHub, NotificationHub>();

            // Global filters
            services.AddControllers(options =>
            {
                options.Filters.Add<SpecialCharacterValidationFilter>();
            });

            // Recurring jobs
            //services.AddRecurringJob(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<CustomExceptionHandlerMiddleware>();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inspire Solution Erp API V1");
            });

            app.UseRouting();

            // ✅ Apply CORS before authentication
            app.UseCors("AllowAngularClient");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notifications");
            });

            // Static files
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Upload/Pdfs")),
                RequestPath = "/pdfs"
            });

            // Logging
            loggerFactory.AddFile($@"{Directory.GetCurrentDirectory()}\Logs\log.txt");


            //this is only for Water bird project
            //app.UseHangfireDashboard("/EmailScheduler");
            //using (var scope = app.ApplicationServices.CreateScope())
            //{
            //    var emailScheduler = scope.ServiceProvider.GetRequiredService<IEmailSchedulerService>();

            //    // Schedule both email jobs to run daily
            //    Hangfire.RecurringJob.AddOrUpdate("jobscops-email-job", () =>
            //         emailScheduler.CheckAndSendEmailForJobScops(), Cron.Daily);

            //    Hangfire.RecurringJob.AddOrUpdate("bankguarantees-email-job", () =>
            //        emailScheduler.CheckAndSendEmailForJobBankGuarantees(), Cron.Daily);
            //}
        }
    }
}
