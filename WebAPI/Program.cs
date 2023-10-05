using WebAPI.Data;
using Microsoft.EntityFrameworkCore;
using WebAPI.Middlewares;
using Microsoft.AspNetCore.Identity;
using WebAPI.Entities;
using WebAPI.Configs;
using WebAPI.Extensions;
using WebAPI.Services;
using WebAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseNpgsql(connectionString);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<INodeService, NodeService>();

builder.Services
    .AddIdentityCore<User>()   
    .AddEntityFrameworkStores<ApplicationDbContext>();
    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();    
    await db.Database.MigrateAsync();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var defaultUser = builder.Configuration
               .GetSection(nameof(DefaultUser))
               .Get<DefaultUser>();
    if(defaultUser != null)
    {
        await userManager.SeedDefaultUser(defaultUser);
    }
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
