using System;
using System.Linq;
using System.Reflection;
using Backend.DatabaseAccessLayer;
using Backend.DatabaseAccessLayer.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Backend
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
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend", Version = "v1" }); });
            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = Configuration.GetConnectionString("redis");
            }); 
            services.AddDbContext<UsersSqlServerContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlConnection"))); 
            
            services.Add(ServiceDescriptor.Singleton<IDistributedCache, RedisCache>());

            AddScopedRepositories(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void AddScopedRepositories(IServiceCollection services)
        {
            var result = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => type.Name.Contains("Repository") && type.IsClass)
                .All(type =>
                {
                    var typeInterface = type.GetInterfaces().FirstOrDefault(inter => inter.Name.Contains("Repository"));
                    if (typeInterface is null)
                    {
                        return false;
                    }

                    services.AddScoped(typeInterface, type);
                    return true;
                });
            
            if (!result)
            {
                throw new Exception("Not all repositories were registered");
            }
        }

        private void AddScopedManagers(IServiceCollection services)
        {
            var result = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => type.Name.Contains("Manager") && type.IsClass)
                .All(type =>
                {
                    var typeInterface = type.GetInterfaces().FirstOrDefault(inter => inter.Name.Contains("Manager"));
                    if (typeInterface is null)
                    {
                        return false;
                    }

                    services.AddScoped(typeInterface, type);
                    return true;
                });
            
            if (!result)
            {
                throw new Exception("Not all managers were registered");
            }
        }
    }
}