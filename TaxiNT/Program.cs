using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using TaxiNT.Client.Services;
using TaxiNT.Client.Services.Interfaces;
using TaxiNT.Components;
using TaxiNT.Data;
using TaxiNT.Data.Models;
using TaxiNT.Services;
using TaxiNT.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// UI: Tăng kích thước bộ nhớ đệm
builder.Services.AddSignalR(e =>
{
    e.MaximumReceiveMessageSize = 102400000;
});


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

// API: Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// API: call HTTP client Hub để lấy dữ liệu API từ bên ngoài
builder.Services.AddHttpClient();

//Add identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
        .AddEntityFrameworkStores<taxiNTDBContext>()
        .AddDefaultTokenProviders();

//Add connection string
builder.Services.AddDbContext<taxiNTDBContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration["ConnectionStrings:Vps"] ?? throw new InvalidOperationException("Can't found [Secret Key] in appsettings.json !"));
    //opt.UseSqlServer(builder.Configuration["ConnectionStrings:Hosting"] ?? throw new InvalidOperationException("Can't found [Secret Key] in appsettings.json !"));
    //opt.UseSqlServer(builder.Configuration["ConnectionStrings:Default"] ?? throw new InvalidOperationException("Can't found [Secret Key] in appsettings.json !"));
});

// UI: Get httpClient API default
builder.Services.AddScoped(
    defaultClient => new HttpClient
    {
        //BaseAddress = new Uri(builder.Configuration["API:Default"] ?? throw new InvalidOperationException("Can't found [Secret Key] in appsettings.json !"))
        //BaseAddress = new Uri(builder.Configuration["API:Monsterasp"] ?? throw new InvalidOperationException("Can't found [Secret Key] in appsettings.json !"))
        
        BaseAddress = new Uri(builder.Configuration["API:Hosting"] ?? throw new InvalidOperationException("Can't found [Secret Key] in appsettings.json !"))
    });

// API: Add Jwt, Gooogle Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwtBearerOptions =>
{
    jwtBearerOptions.RequireHttpsMetadata = false;
    jwtBearerOptions.SaveToken = true;
    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("Missing JWT:Secret"))
        ),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"]
    };
});

// API: Add SwaggerGen (dotnet add package Swashbuckle.AspNetCore)
builder.Services.AddSwaggerGen(
    opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Blazor Server Core API", Version = "v1" });
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

#region Back-end Register serivces
//ASP.NET Core server – Web API, MVC controller : [Authorize]
builder.Services.AddAuthorization();
//For SQL Server
builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddScoped<IShiftWorkService, ShiftWorkService>();
builder.Services.AddScoped<IBlogServer, BlogServer>();

//Google Sheets API:
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISalaryServer, SalaryServer>();
#endregion

//Zalo Customer Service: dùng để nhận thông tin khách đặt xe
builder.Services.AddScoped<IZaloCustomerService, ZaloCustomerService>();

#region Font-end Register services
// Blazor (client-side or server-side UI): [Authorize], [AuthorizeView]
builder.Services.AddAuthorizationCore(); 
// Authentication
builder.Services.AddScoped<AuthenticationStateProvider, AuthenService>();
builder.Services.AddScoped<IAuthenService, AuthenService>();
builder.Services.AddCascadingAuthenticationState();
// UI: Register Client Services
builder.Services.AddScoped<ISalaryService, SalaryService>();
builder.Services.AddScoped<IBlogService, BlogService>();

//For SQL Server
builder.Services.AddScoped<IBillCheckService, BillCheckService>();
#endregion


builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "TaxiNT.AntiForgery.v2.1"; 
    options.Cookie.SameSite = SameSiteMode.Lax; // tránh bị chặn bởi Facebook WebView
    options.Cookie.HttpOnly = true;

    // Host free CHỈ HTTP → dùng SameAsRequest (HTTP thì không Secure, HTTPS thì có Secure)
    // AUTO:
    // - Nếu request là HTTP  → cookie không gắn Secure
    // - Nếu request là HTTPS → cookie gắn Secure
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    /*
        Thuần HTTPS chuẩn, chỉ cần đổi thành:
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    */
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;

    options.OnAppendCookie = ctx =>
    {
        if (ctx.CookieOptions.SameSite == SameSiteMode.None)
            ctx.CookieOptions.SameSite = SameSiteMode.Unspecified;
    };

    options.OnDeleteCookie = ctx =>
    {
        if (ctx.CookieOptions.SameSite == SameSiteMode.None)
            ctx.CookieOptions.SameSite = SameSiteMode.Unspecified;
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else // API: Add run Swagger UI: https://localhost:7154/swagger/index.html
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(
        opt =>
        {
            opt.SwaggerEndpoint($"/swagger/v1/swagger.json", "Manager Business API V1");
        }
    );
}

app.UseHttpsRedirection();   // nếu host free không có HTTPS thì có thể tắt dòng này
app.UseStaticFiles();

app.UseCookiePolicy();           // ⭐ thêm dòng này

app.UseRouting();

// API: Add Authoz and Authen
app.UseAuthentication();
app.UseAuthorization();

// Anti-forgery middleware – để SAU routing, TRƯỚC MapEndpoints
app.UseAntiforgery();

app.MapControllers();

// Xử lý header iframe, CSP
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.Remove("X-Frame-Options");
        context.Response.Headers.Remove("Content-Security-Policy");
        context.Response.Headers["Content-Security-Policy"] = "frame-ancestors *";
        return Task.CompletedTask;
    });

    await next();
});

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TaxiNT.Client._Imports).Assembly);

app.Run();
