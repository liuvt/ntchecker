﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.Filters;
using System.Text;
using MudBlazor.Services;

using ntchecker.Components;
using ntchecker.Data;
using ntchecker.Data.Models;
using ntchecker.Services;
using ntchecker.Repositories;
using ntchecker.Services.Interfaces;
using ntchecker.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add identity
builder.Services.AddDbContext<ntcheckerDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Hosting")));
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ntcheckerDBContext>()
.AddDefaultTokenProviders();

// API: Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Cấu hình lại cho kết nối SingleIR
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromMinutes(2); // Khoảng thời gian server chờ tín hiệu từ client (ping).
        options.HandshakeTimeout = TimeSpan.FromSeconds(30); // Khoảng thời gian client phải hoàn tất bắt tay ban đầu với server
        options.KeepAliveInterval = TimeSpan.FromSeconds(15); // Giữ kết nối sống lâu hơn với KeepAlive
        options.MaximumReceiveMessageSize = 100 * 1024 * 1024; // 100MB Tăng kích thước nhận thông tin

    });

// API: Add SwaggerGen (dotnet add package Swashbuckle.AspNetCore)
builder.Services.AddSwaggerGen(
    opt =>
    {
        //Init project: CRUD category,order,orderdetail,..., AugCenterModel
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Manager API", Version = "v1" });
        opt.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            In = ParameterLocation.Header,
            Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")"
        });

        //Add filter to block case authorize: Swashbuckle.AspNetCore.Filters
        opt.OperationFilter<SecurityRequirementsOperationFilter>();
    }
);

// API: Add Jwt, Gooogle Authentication
builder.Services.AddAuthentication(authenticationOptions =>
{
    authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    authenticationOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    // Once a user is authenticated, the OAuth2 token info is stored in cookies.
    authenticationOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
                .AddCookie()
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.RequireHttpsMetadata = false;
                    jwtBearerOptions.SaveToken = true;
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                        ValidAudience = builder.Configuration["JWT:ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]
                                                ?? throw new InvalidOperationException("Can't found [Secret Key] in appsettings.json !"))
                            ),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

// UI: Register Client Factory
builder.Services.AddHttpClient("ntChecker", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["API:Hosting"] ??
                            throw new InvalidOperationException("Can't found [Secret Key] in appsettings.json !"));
    httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "HttpRequestNTChecker");
});

// UI: Get httpClient API default
builder.Services.AddScoped(
    defaultClient => new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration["API:Hosting"] ??
                                throw new InvalidOperationException("Can't found [Secret Key] in appsettings.json !"))
    });

// UI: Register Repository
builder.Services.AddScoped<IAuthenRepository, AuthenRepository>();

// UI: Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenService>();
builder.Services.AddScoped<IAuthenService, AuthenService>();
builder.Services.AddCascadingAuthenticationState();

// UI: Register Services Revenue
builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddScoped<IOrderService, OrderService>(); 
builder.Services.AddScoped<IOrderByHistoryService, OrderByHistoryService>();
builder.Services.AddScoped<ISalaryService, SalaryService>();

var app = builder.Build();

//Chỉ gọi lần đầu khi chưa có database để tạo identity seeding
/*using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    Console.WriteLine("Seeding Identity Data...");
    await DataSeeding.SeedIdentityDataAsync(services);
    Console.WriteLine("Seeding Done!");
}*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else // API: Add run Swagger UI: http://localhost:5097/swagger/index.html
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(
        opt =>
        {
            opt.SwaggerEndpoint($"/swagger/v1/swagger.json", "Manager API V1");
        }
    );
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAntiforgery();

// API: Add Authoz and Authen
app.UseAuthentication();
app.UseAuthorization();

// 👇 Quan trọng cho API Controllers
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


