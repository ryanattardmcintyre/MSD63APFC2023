using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Diagnostics.Common;
using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSD63AWebApp.DataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace MSD63AWebApp
{
    public class Startup
    {
        private ILogger _logger;
        public Startup(IConfiguration configuration, IWebHostEnvironment host )
        {
          

            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                host.ContentRootPath + "\\msd63a2023-19b02b290325.json"
             );


            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddGoogle(new LoggingServiceOptions { ProjectId = "msd63a2023" });
            });

            _logger = loggerFactory.CreateLogger<Startup>();

            Configuration = configuration;
           
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services )
        { 
            string project = Configuration["project"];
            //oauth_secretkey

            services.AddGoogleErrorReportingForAspNetCore(new ErrorReportingServiceOptions
            {
                // Replace ProjectId with your Google Cloud Project ID.
                ProjectId = project,
                // Replace Service with a name or identifier for the service.
                ServiceName = "MainWebApp",
                // Replace Version with a version for the service.
                Version = "1"
            });


            // Create the client.
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Build the resource name.
            SecretVersionName secretVersionName = new SecretVersionName(project,
                "oauth_secretkey", 
                "1");

            // Call the API.
            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

            // Convert the payload to a string. Payloads are bytes by default.
            String payload = result.Payload.Data.ToStringUtf8();
            var key = JObject.Parse(payload);
            string secretKey = key["Authentication:Google:ClientSecret"].ToString();

            services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = "1034457160308-bbnb2qv7alljklf0qulfmr85828gdgem.apps.googleusercontent.com";
                options.ClientSecret = secretKey; 
                
            });

            services.AddControllersWithViews();

        
         
            //Dependency Injection

            FirestoreBookRepository fbr = new FirestoreBookRepository(project);


            string cacheConnectionStringLocal = "127.0.0.1:6379";
            //string cacheConnectionStringRedisLabs = "redis-14410.c1.us-east1-2.gce.cloud.redislabs.com:14410,password=pc1feaAiYI2d1dGHc7JBak8YxnmJ4hBv";

            services.AddScoped<FirestoreBookRepository>(provider => fbr);
            services.AddScoped<FirestoreReservationsRepository>(provider
                => new FirestoreReservationsRepository(project, fbr));

            services.AddLogging(); 
            try
            {

                services.AddScoped<RedisCacheMenusRepository>(provider =>
                    new RedisCacheMenusRepository(cacheConnectionStringLocal, _logger));
            }
            catch(Exception ex)
            {
                //logged

            }
            services.AddScoped<PubsubEmailsRepository>(provider => new PubsubEmailsRepository(project));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
