using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace kudvenkat {
    public class Program {
        public static void Main(string[] args) {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try {
                CreateWebHostBuilder(args).Build().Run();
            } catch (Exception ex) {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            } finally {
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(log => {
                    log.ClearProviders();
                    log.AddConsole();
                    log.SetMinimumLevel(LogLevel.Trace);
                }).UseNLog();
        }
    }
}
