using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using WebApplication1.Hubs;
using WebApplication1.Models;
using WebApplication1.Repo;
using WebApplication1.Repo.Service;

namespace WebApplication1
{

    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration )
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddSingleton<LineBotConfig, LineBotConfig>((s) => new LineBotConfig
            {
                channelSecret = Configuration["LineBot:channelSecret"],
                accessToken = Configuration["LineBot:accessToken"],
                user_ID = Configuration["LineBot:user_ID"]
            });
          
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICloudClient<FirestoreDb, Product, Product>,FireBaseProductRepository> ();
            services.AddSingleton<ICloudClient<CloudBlobContainer, Product, Product>, AzureBlobStorageRepository>();
            services.AddSingleton<ICloudClient<FirestoreDb, Order, OrderDetails>, FireBaseOrderRepository>();
            services.AddSingleton<IProduct, ProductService>();
            services.AddSingleton<IOrder, OrderService>();

            services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState);

                    var result = new BadRequestObjectResult(problemDetails);

                    result.ContentTypes.Add("application/problem+json");
                    result.ContentTypes.Add("application/problem+xml");

                    return result;
                };
            });
          
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });

            });
            services.AddControllersWithViews();
            services.AddMemoryCache();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "RestWebapi 串接各類雲端服務",
                    Description = "目前串接雲端服務: AzureBlobstorage, Google Cloud Vision, FireBase 和 Python 爬蟲(selenium web scraping) ",
                    
                });
            });
            services.AddSignalR();
            services.AddRazorPages();;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(url : "/swagger/v1/swagger.json", name : "All RestWebApi");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=mySignalr}/{id?}");
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chathub");
            });
        }
    }
}
