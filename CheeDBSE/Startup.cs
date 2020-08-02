using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
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
            }).UseEndpoints(KeysRoutes).UseEndpoints(CacheRoutes);
        }

        private void CacheRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.Map("/cache/keys", async context =>
            {
                context.Response.Headers.Add("X-Powered-By", "CheeDBSE/ONE");
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(string.Join(Environment.NewLine,
                    MemoryCache.Default.Select(item => $"{item.Key}:{item.Value}").ToList()));
            });
        }

        private void KeysRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.Map("/rocks/keys", async context =>
            {
                var keyList = new List<string>();
                using (var iterator = DB.NewIterator())
                {
                    var seek = context.Request.Query["seek"].ToString();
                    iterator.Seek(seek);
                    while (iterator.Valid() && iterator.StringKey().StartsWith(seek))
                    {
                        //if (!iterator.StringKey().StartsWith(seek)) break;
                        keyList.Add(iterator.StringKey() + ":" + iterator.StringValue());
                        iterator.Next();
                    }
                }
                await context.Response.WriteAsync(string.Join(Environment.NewLine, keyList));
            });
            endpoints.Map("/rocks/keys/{keyName}", async context =>
            {
                context.Response.Headers.Add("X-Powered-By", "CheeDBSE/ONE");
                context.Response.ContentType = "text/plain";
                await MethodCases(context, context.Request.Method);
            });

            endpoints.Map("/rocks/keys/{keyName}/{keyMethod}", async context =>
            {
                var keyMethod = context.GetRouteValue("keyMethod").ToString();
                context.Response.Headers.Add("X-Powered-By", "CheeDBSE/ONE");
                context.Response.ContentType = "text/plain";
                await MethodCases(context, keyMethod.ToUpper());
            });
        }

        private static async Task MethodCases(HttpContext context, string method)
        {
            var queryDict = context.Request.Query;
            var keyName = context.GetRouteValue("keyName").ToString();
            switch (method)
            {
                case "GET":
                    if (MCache.TryGet(keyName, out var tVal))
                        await context.Response.WriteAsync(tVal.ToString());
                    else
                    {
                        var val = DB.Get(keyName);
                        if (string.IsNullOrEmpty(val)) await context.Response.WriteAsync("not found");
                        else
                        {
                            MCache.Put(keyName, val);
                            await context.Response.WriteAsync(val);
                        }
                    }

                    break;
                case "PUT" when queryDict.TryGetValue("value", out var keyValue) ||
                                context.Request.Method != "GET" &&
                                context.Request.Form.TryGetValue("value", out keyValue) &&
                                !string.IsNullOrWhiteSpace(keyValue):
                    await Task.Run(() =>
                    {
                        DB.Put(keyName, keyValue.ToString());
                        MCache.Put(keyName, keyValue.ToString());
                    });
                    await context.Response.WriteAsync("OK");
                    break;
                case "PUT" when queryDict.TryGetValue("val", out var keyValue) ||
                                context.Request.Method != "GET" &&
                                context.Request.Form.TryGetValue("value", out keyValue) &&
                                !string.IsNullOrWhiteSpace(keyValue):
                    await Task.Run(() =>
                    {
                        DB.Put(keyName, keyValue.ToString());
                        MCache.Put(keyName, keyValue.ToString());
                    });
                    await context.Response.WriteAsync("OK");
                    break;
                case "PUT":
                    await context.Response.WriteAsync("need value");
                    break;
                case "DELETE":
                    await Task.Run(() =>
                    {
                        MCache.Del(keyName);
                        DB.Remove(keyName);
                    });
                    await context.Response.WriteAsync("OK");
                    break;
                case "DEL":
                    await Task.Run(() =>
                    {
                        MCache.Del(keyName);
                        DB.Remove(keyName);
                    });
                    await context.Response.WriteAsync("OK");
                    break;
            }
        }
    }
}
