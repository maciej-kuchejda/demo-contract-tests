using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Provider.cs.Repository;
using Provider.Tests.Middleware;
using Provider.Tests.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Provider.Tests
{
    internal class TestStartup
    {
        private readonly Startup inner;

        public TestStartup(IConfiguration configuration)
        {
            this.inner = new Startup(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddApplicationPart(Assembly.GetAssembly(typeof(Startup)));
            services.AddSingleton<IOrderRepository, FakeOrderRepository>();

            this.inner.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ProviderStateMiddleware>();

            this.inner.Configure(app, env);
        }
    }
}
