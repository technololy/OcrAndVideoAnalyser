using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tewr.Blazor.FileReader;
using Microsoft.Extensions.Http;

namespace ImageValidationUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddFileReaderService(o =>
            {
                o.UseWasmSharedBuffer = true;
            });

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddHttpClient("FacialrecogAPI", client =>
    client.BaseAddress = new Uri("https://pass.IconFlux.ng/FacialrecogAPI/"));

            await builder.Build().RunAsync();
        }
    }
}
