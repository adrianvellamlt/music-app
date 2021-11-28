using Microsoft.AspNetCore.Components;
using API.Infrastructure;
using Newtonsoft.Json.Converters;

namespace API
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(
                    config => config.UseGlobalRoutePrefix("api")
                )
                .AddNewtonsoftJson(config =>
                {
                    config.SerializerSettings.Converters.Add(new StringEnumConverter());
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}