﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace AspNetCoreTodo.IntegrationTests
{
    public class TestFixture : IDisposable
    {
          TestServer _server;

        public TestFixture()
        {
            
            var builder = new WebHostBuilder()
                .UseStartup<AspNetCoreTodo.Startup>()
                .ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.SetBasePath(Path.Combine(
                        Directory.GetCurrentDirectory(), "..\\..\\..\\..\\AspNetCoreTodo"));

                    configBuilder.AddJsonFile("appsettings.json");

                    configBuilder.AddInMemoryCollection(new Dictionary<string, string>()
                    {
                        ["Facebook:AppId"] = "fake-app-id",
                        ["Facebook:AppSecret"] = "fake-app-secret"
                    });
                });
            _server = new TestServer(builder);

            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost:5000");
        }
        public HttpClient Client { get; }
        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}
