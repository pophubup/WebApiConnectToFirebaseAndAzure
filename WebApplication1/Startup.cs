using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using WebApplication1.Models;
using WebApplication1.Repo;

namespace WebApplication1
{

    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration )
        {
            //var configurationRoot = new ConfigurationBuilder()
            //  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //  .AddEnvironmentVariables()
            //  .Build();
          

            Configuration = configuration;
        }

      

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<LineBotConfig, LineBotConfig>((s) => new LineBotConfig
            {
                channelSecret = Configuration["LineBot:channelSecret"],
                accessToken = Configuration["LineBot:accessToken"],
                user_ID = Configuration["LineBot:user_ID"]
            });
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IProducts, ProductsRepository>();
            services.AddSingleton<IOrders, OrdersRepository>();
            //services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
            //{
            //    options.InvalidModelStateResponseFactory = context =>
            //    {
            //        var problemDetails = new ValidationProblemDetails(context.ModelState);

            //        var result = new BadRequestObjectResult(problemDetails);

            //        result.ContentTypes.Add("application/problem+json");
            //        result.ContentTypes.Add("application/problem+xml");

            //        return result;
            //    };
            //});
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });

            });

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
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
