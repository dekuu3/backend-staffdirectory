using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_staffdirectory {
    public class Program {
        public static void Main(string[] args) {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, builder) => {

                    if (hostContext.HostingEnvironment.IsDevelopment()) {
                        builder.AddUserSecrets<Program>();
                    }
                });
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>()
                        .UseUrls("http://localhost:4000");
                }).UseDefaultServiceProvider(options => options.ValidateScopes = false);
    }
}
