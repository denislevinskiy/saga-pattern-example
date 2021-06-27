using System;
using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Saga.Tests.Integration.Fixtures
{
    [CollectionDefinition("WebServerFixture")]
    public class WebServerCollection : ICollectionFixture<WebServerFixture> { }
    
    public sealed class WebServerFixture : IDisposable
    {
        private readonly TestServer _testServer;
        private bool _disposed;

        public WebServerFixture()
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                .UseContentRoot("..//..//..//..//Facade.Api")
                .UseStartup<Startup>();
            _testServer = new TestServer(webHostBuilder);
            HttpClient = _testServer.CreateClient();
        }

        public HttpClient HttpClient { get; }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            HttpClient.Dispose();
            _testServer.Dispose();
            _disposed = true;
        }
    }
}