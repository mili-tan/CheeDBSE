using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static CheeDBSEngine.Program;

namespace CheeDBSEngine
{
    public class Startup
    {
        private static string SetupBasePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        private static string IndexStr = File.Exists(SetupBasePath + "index.html")
            ? File.ReadAllText(SetupBasePath + "index.html")
            : "Welcome to CheeDBS";

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseRouting().UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(IndexStr);
                });
            }).UseEndpoints(KeysRoutes);
        }

        private void KeysRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.Map("/keys/{keyName}", async context =>
            {
                var queryDictionary = context.Request.Query;
                var keyName = context.GetRouteValue("keyName").ToString();
                context.Response.Headers.Add("X-Powered-By", "CheeDBS/ONE");
                context.Response.ContentType = "text/plain";

                switch (context.Request.Method)
                {
                    case "GET":
                        var val = DB.Get(keyName);
                        await context.Response.WriteAsync(string.IsNullOrEmpty(val) ? "not found" : val);
                        break;
                    case "PUT" when queryDictionary.TryGetValue("value", out var keyValue) &&
                                    !string.IsNullOrWhiteSpace(keyValue):
                        DB.Put(keyName, keyValue.ToString());
                        await context.Response.WriteAsync("OK");
                        break;
                    case "PUT" when queryDictionary.TryGetValue("val", out var keyValue) &&
                                    !string.IsNullOrWhiteSpace(keyValue):
                        DB.Put(keyName, keyValue.ToString());
                        await context.Response.WriteAsync("OK");
                        break;
                    case "PUT":
                        await context.Response.WriteAsync("need value");
                        break;
                    case "DELETE":
                        await Task.Run(() => { DB.Remove(keyName); });
                        await context.Response.WriteAsync("OK");
                        break;
                }
            });

            endpoints.Map("/keys/{keyName}/{keyMethod}", async context =>
            {
                var queryDictionary = context.Request.Query;
                var keyName = context.GetRouteValue("keyName").ToString();
                var keyMethod = context.GetRouteValue("keyMethod").ToString();
                context.Response.Headers.Add("X-Powered-By", "CheeDBS/ONE");
                context.Response.ContentType = "text/plain";
                switch (keyMethod.ToUpper())
                {
                    case "GET":
                        var val = DB.Get(keyName);
                        await context.Response.WriteAsync(string.IsNullOrEmpty(val) ? "not found" : val);
                        break;
                    case "PUT" when queryDictionary.TryGetValue("value", out var keyValue) &&
                                    !string.IsNullOrWhiteSpace(keyValue):
                        DB.Put(keyName, keyValue.ToString());
                        await context.Response.WriteAsync("OK");
                        break;
                    case "PUT" when queryDictionary.TryGetValue("val", out var keyValue) &&
                                    !string.IsNullOrWhiteSpace(keyValue):
                        DB.Put(keyName, keyValue.ToString());
                        await context.Response.WriteAsync("OK");
                        break;
                    case "PUT":
                        await context.Response.WriteAsync("need value");
                        break;
                    case "DELETE":
                        await Task.Run(() => { DB.Remove(keyName); });
                        await context.Response.WriteAsync("OK");
                        break;
                }
            });
        }
    }
}
