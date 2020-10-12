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

            services.AddHttpClient("Retry")
                .AddPolicyHandler
                (
                    HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync
                    (
                        3,
                        retryInterval => TimeSpan.FromMilliseconds
                        (
                            Math.Pow(2, retryInterval) * 100
                        ),
                        onRetry: (exception, retryInterval) =>
                        {
                            Console.WriteLine(@$"[{DateTimeOffset.Now}] - 
                                Uri呼叫異常: {exception.Result.RequestMessage.RequestUri}, 
                                重試間隔時間: {retryInterval} , 
                                StatusCode: {(int)exception.Result.StatusCode} {exception.Result.StatusCode},
                                Error: {exception.Exception}");
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
