using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace Provider.Tests
{
    public class ProviderTests
    {
        private static Uri ProviderUri = new("http://localhost:9001");
        private  IHost _server;
        private  PactVerifier _verifier;


        public void Dispose()
        {
            _server.Dispose();
            _verifier.Dispose();
        }
        [SetUp]
        public void Setup()
        {
            _server = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  webBuilder.UseUrls(ProviderUri.ToString());
                                  webBuilder.UseStartup<TestStartup>();
                              })
                              .Build();

            _server.Start();

            _verifier = new PactVerifier(new PactVerifierConfig
            {
                LogLevel = PactLogLevel.Debug,
                Outputters = new List<IOutput>
                {
                    new ConsoleOutput()
                }
            });
        }

        [Test]
        public void Verify()
        {
            string pactPath = Path.Combine("..",
                                           "..",
                                           "..",
                                           "..",
                                           "Consumer.Tests.cs",
                                           "pacts",
                                           "Consumer API-Orders API.json");

            _verifier
                .ServiceProvider("Orders API", ProviderUri)
                .WithFileSource(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(ProviderUri, "/provider-states"))
                .WithRequestTimeout(TimeSpan.FromSeconds(120))
                .WithSslVerificationDisabled()
                .Verify();
        }
    }
}