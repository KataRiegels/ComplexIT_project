using Microsoft.AspNetCore.Hosting;
using SignalRChat.Hubs;


using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Chat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}


