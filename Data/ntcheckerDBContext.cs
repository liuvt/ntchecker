using ntchecker.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ntchecker.Data;

public partial class ntcheckerDBContext : IdentityDbContext<AppUser>
{
    //Get config in appsetting
    private readonly IConfiguration configuration;
    //Default constructor
    public ntcheckerDBContext()
    {
    }

    public ntcheckerDBContext(DbContextOptions<ntcheckerDBContext> options, IConfiguration _configuration) : base(options)
    {
        //Models - Etityties
        configuration = _configuration;
    }


    //Config to connection sql server
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                configuration["ConnectionStrings:Hosting"] ??
                    throw new InvalidOperationException("Can't find [ConnectionStrings:Default] in appsettings.json!")
            );
        }
    }
}

//Create mirations: dotnet ef migrations add Init -o Data/Migrations
//Create database: dotnet ef database update
//Publish project: dotnet publish -c Release --output ./Publish ntchecker.csproj