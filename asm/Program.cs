using Frontend;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Shared.Asm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddMudServices();
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
    

builder.Services.AddSingleton<InstructionEncoder>();
builder.Services.AddSingleton<InstructionDecoder>();
builder.Services.AddSingleton<CodeProcessor>();
builder.Services.addsing
builder.Services.AddScoped<Emulator>();


await builder.Build().RunAsync();