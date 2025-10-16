using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaxiNT.Client.Services;
using TaxiNT.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenService>();
builder.Services.AddScoped<IAuthenService, AuthenService>();
builder.Services.AddCascadingAuthenticationState();

// UI: Register Client Services
builder.Services.AddScoped<ICheckerService, CheckerService>();
builder.Services.AddScoped<ICheckerDetailService, CheckerDetailService>();
builder.Services.AddScoped<ISalaryService, SalaryService>();

//For SQL Server
builder.Services.AddScoped<IBillCheckService, BillCheckService>();


builder.Services.AddScoped(
    sp => new HttpClient {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

await builder.Build().RunAsync();
