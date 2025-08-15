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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSignalR();
            services.AddControllers();
            services.AddApplication();
            services.AddInfrastructure(Configuration);
            services.AddTokenAuthentication(Configuration);

            // services.AddHangfire(configuration =>
            //        {
            //            configuration.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"), new Hangfire.SqlServer.SqlServerStorageOptions
            //            {
            //                PrepareSchemaIfNecessary = true, // Prevents migration attempts
            //                SchemaName = "HangFire"           // Use the exact schema name in your DB
            //            });
            //        });
            //services.AddHangfireServer();
            services.AddRecurringJob(Configuration);



            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.Configure<EmailSettingViewModel>(Configuration.GetSection("MailSettings"));
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "InspireErpApi",
                    Version = "v1",
                    Description = "Services offered by inspire solutions",
                }); 

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
               {
                 new OpenApiSecurityScheme
                 {
                   Reference = new OpenApiReference
                   {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                   }
                  },
                  new string[] { }
                }
              });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
                      builder =>
                      {
                          builder.WithOrigins("*")
                                 .AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
            });
           
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddScoped<INotificationHub, NotificationHub>();

            services.AddControllers(options =>
            {
                // Register the filter globally
                options.Filters.Add<SpecialCharacterValidationFilter>();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<CustomExceptionHandlerMiddleware>();
            // This middleware serves generated Swagger document as a JSON endpoint
            app.UseSwagger();

            // This middleware serves the Swagger documentation UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inspire Solution Erp API V1");
                //c.RoutePrefix = string.Empty;
            });

            app.UseRouting();
            app.UseCors("AllowAllHeaders");
            // global cors policy            
            app.UseCors(options => options.AllowAnyOrigin());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notifications");
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Upload/Pdfs")),
                RequestPath = "/pdfs"
            });

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

            loggerFactory.AddFile($@"{Directory.GetCurrentDirectory()}\Logs\log.txt");

        }
    }
}
