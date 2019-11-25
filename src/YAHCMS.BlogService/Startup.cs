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

        private readonly ILogger logger;
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            this.logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
