using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaxiNT.Data.Models;
using TaxiNT.Libraries.Models;

namespace TaxiNT.Data;

public partial class taxiNTDBContext : IdentityDbContext<AppUser>
{
    //Get config in appsetting
    private readonly IConfiguration configuration;

    public taxiNTDBContext(DbContextOptions<taxiNTDBContext> options, IConfiguration _configuration) : base(options)
    {
        //Models - Etityties
        configuration = _configuration;
    }

    //Call Model to create table in database
    public virtual DbSet<ModelBank> Banks { get; set; } = null!;
    public virtual DbSet<TripDetail> Trips { get; set; } = null!;
    public virtual DbSet<ContractDetail> Contracts { get; set; } = null!;
    public virtual DbSet<ShiftWork> ShiftWorks { get; set; } = null!;

    //Config to connection sql server
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                //configuration["ConnectionStrings:Hosting"] ??
                configuration["ConnectionStrings:Default"] ??
                    throw new InvalidOperationException("Can't find ConnectionStrings in appsettings.json!")
            );
        }

    }
}

//Update tool: dotnet tool update --global dotnet-ef

//Create mirations: dotnet ef migrations add Init -o Data/Migrations
//Create database: dotnet ef database update

//Publish project: dotnet publish -c Release --output ./Publish TaxiNT.csproj
/* 
npx @tailwindcss/cli -i ./TaxiNT/TailwindImport/input.css -o ./TaxiNT/wwwroot/css/tailwindcss.css --watch 
*/

