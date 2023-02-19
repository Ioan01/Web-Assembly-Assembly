using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using asm;
using MudBlazor.Services;
using BlazorMonaco.Editor;
using asm.Asm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");





builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


builder.Services.AddSingleton<InstructionEncoder>();
builder.Services.AddSingleton<InstructionDecoder>();
builder.Services.AddSingleton<CodeProcessor>();
builder.Services.AddScoped<Emulator>();



await builder.Build().RunAsync();