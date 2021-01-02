using JHStatsWebsite.Configuration;
using JHStatsWebsite.Data;
using JHStatsWebsite.Data.Interfaces;
using JHStatsWebsite.MessageHandlers;
using JHStatsWebsite.Processor;
using JHStatsWebsite.Processor.Interfaces;
using JHStatsWebsite.Utility;
using JHStatsWebsite.Utility.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace JHStatsWebsite
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
            services.AddApplicationInsightsTelemetry();

            services.AddControllers();

            ImageDomainConfiguration imageDomainConfiguration = Configuration.GetSection("ImageDomainConfiguration").Get<ImageDomainConfiguration>();
            services.AddSingleton(imageDomainConfiguration);
            TwitterConfiguration twitterConfiguration = Configuration.GetSection("TwitterConfiguration").Get<TwitterConfiguration>();
            services.AddSingleton(twitterConfiguration);

            services.AddTransient<TwitterAuthHandler>();
            services.AddHttpClient<ITwitterStreamProcessor, TwitterStreamProcessor>((httpClient) =>
            {
                httpClient.BaseAddress = new Uri(twitterConfiguration.BaseUrl);
            })
                .AddHttpMessageHandler<TwitterAuthHandler>();

            services.AddSingleton<ITwitterDataStorage, TwitterDataInMemoryStorage>();
            services.AddSingleton<ITwitterDataCapturer, TwitterDataCapturer>();
            services.AddSingleton<ITwitterSampleDataRetriever, TwitterSampleDataRetriever>();
            services.AddSingleton<ITwitterEstimatedLiveDataRetriever, TwitterEstimatedLiveDataRetriever>();

            services.AddSingleton<ITweetProcessor, TweetProcessor>();
            services.AddSingleton<IUrlHandler, UrlHandler>();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
