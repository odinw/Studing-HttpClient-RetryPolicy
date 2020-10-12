using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using System;

namespace Studing_HttpClient_RetryPolicy
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
            services.AddControllers();

            services.AddHttpClient("a")
                //.SetHandlerLifetime(Timeout.InfiniteTimeSpan)
                .AddPolicyHandler
                (
                    HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync
                    (
                        3,
                        retryAttempt => TimeSpan.FromSeconds
                        (
                            Math.Pow(2, retryAttempt)
                        ),
                        onRetry: (exception, retryCount) =>
                        {
                            Console.WriteLine($"[{DateTimeOffset.Now}] : 呼叫 API 異常, 進行第 {retryCount} 次重試, Error :{exception.Result.StatusCode}");
                        }
                    )
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
