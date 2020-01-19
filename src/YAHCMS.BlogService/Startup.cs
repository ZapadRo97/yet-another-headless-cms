using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using YAHCMS.BlogService.Persistence;

namespace YAHCMS.BlogService
{
    public class Startup
    {

        private ILogger logger;
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            HostEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment {get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            var serviceProvider = services.BuildServiceProvider();
            logger = serviceProvider.GetService<ILogger<Startup>>();
            services.AddSingleton(typeof(ILogger), logger);

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;

                options.Audience = "blogservice";
            });
            

            //not working in tests
            var transient = true;
            if(Configuration["transient"] != null) {
                transient = Boolean.Parse(Configuration["transient"]);
            }

            if(transient)
            {
                logger.LogInformation("Using transient local repository");
                services.AddScoped<IBlogRepository, MemoryBlogRepository>();
            }
            else
            {
                logger.LogInformation("Using SQL Server repository");
                services.AddScoped<IBlogRepository, BlogRepository>();
                services.AddEntityFrameworkSqlServer()
                .AddDbContext<BlogDbContext>(options 
                    => options.UseSqlServer(Configuration["ConnectionString"]));
            
                logger.LogInformation(Configuration["ConnectionString"]);
            }

        
            
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
