using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ForeignExchange
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        //main
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}