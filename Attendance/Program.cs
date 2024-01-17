using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Attendance
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = BuildWebHost(args);
            await host.RunAsync();
        }

        public static IHost BuildWebHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(Web =>
                {
                    Web.UseStartup<Startup>()
                    .ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.AllowSynchronousIO = true;
                    });
                })
                .Build();
    }
}
