using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using backend_staffdirectory.Helpers;
using backend_staffdirectory.Services;

namespace backend_staffdirectory {
    public class Startup {
        public IConfiguration Configuration { get; }

        //    public Startup(IConfiguration configuration) {
        //        Configuration = configuration;
        //    }

        //    // add services to the DI container
        //    public void ConfigureServices(IServiceCollection services) {
        //        services.AddCors();
        //        services.AddControllers();

        //        // configure strongly typed settings object
        //        services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

        //        // configure DI for application services
        //        services.AddScoped<IUserService, UserService>();
        //    }

        //    // configure the HTTP request pipeline
        //    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        //        app.UseRouting();

        //        // global cors policy
        //        app.UseCors(x => x
        //            .AllowAnyOrigin()
        //            .AllowAnyMethod()
        //            .AllowAnyHeader());

        //        // custom jwt auth middleware
        //        app.UseMiddleware<JwtMiddleware>();

        //        app.UseEndpoints(x => x.MapControllers());
        //    }
        //}

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddCors(opt => {
                opt.AddDefaultPolicy(build => build.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "backend_staffdirectory", Version = "v1" });
            });

            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // configure Dependency Injection for application services
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "backend_staffdirectory v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors();

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
