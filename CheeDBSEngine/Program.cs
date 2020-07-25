using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using RocksDbSharp;

namespace CheeDBSEngine
{
    class Program
    {
        public static RocksDb DB = RocksDb.Open(new DbOptions()
            .SetCreateIfMissing().SetCreateMissingColumnFamilies(), "my.db");

        static void Main(string[] args)
        {

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(AppDomain.CurrentDomain.SetupInformation.ApplicationBase)
                .ConfigureServices(services => services.AddRouting())
                .ConfigureKestrel(options =>
                {
                    options.Listen(new IPEndPoint(IPAddress.Loopback, 2022), listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        //if (true) listenOptions.UseHttps();
                    });
                })
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
