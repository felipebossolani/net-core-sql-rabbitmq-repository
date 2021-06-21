using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbMigration;
using Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Repository.Cache.Redis.Repositories;
using Repository.SQL;
using Repository.SQL.Repositories;
using WebApi.Workers;

namespace WebApi
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
            var storeDatabase = Configuration.GetSection("ConnectionStrings:StoreDatabase");
            var storeRedis = Configuration.GetSection("ConnectionStrings:StoreRedis");

            services.AddTransient<IMigrator, DbUpMigrator>();
            services.AddHostedService(s => new DbMigratorService(new DbUpMigrator(), storeDatabase.Value));

            services.AddDbContext<StoreContext>(options => options.UseSqlServer(storeDatabase.Value));
            services.AddStackExchangeRedisCache(options => options.Configuration = storeRedis.Value);

            //services.AddTransient<IProductWriteRepository, ProductWriteRepository>();
            //services.AddTransient<IProductReadRepository, ProductReadRepository>();

            services.AddTransient<ProductWriteRepository>();
            services.AddTransient<IProductWriteRepository, ProductWriteCachingDecorator>(
                provider => new ProductWriteCachingDecorator(
                    provider.GetService<ProductWriteRepository>(),
                    provider.GetService<IDistributedCache>()
                    ));

            services.AddTransient<ProductReadRepository>();
            services.AddTransient<IProductReadRepository, ProductReadCachingDecorator>(
                provider => new ProductReadCachingDecorator(
                    provider.GetService<ProductReadRepository>(),
                    provider.GetService<IDistributedCache>()
                    ));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
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
