using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Migrations;
using UserIdentitySample.ApiService.DbContext;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

//var connectionString = builder.Configuration.GetConnectionString("ConnectionString");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("users")));

builder.Services.AddHostedService<DbInitializer<ApplicationDbContext>>();

var app = builder.Build();

app.Run();