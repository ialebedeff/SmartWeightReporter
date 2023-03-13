using Communication.Hubs;
using Database;
using Database.Repositories;
using Entities;
using EntitiesMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting.Internal;
using Narochno.Jenkins;
using SmartWeight.Admin.Client.Pages.Factories;
using SmartWeight.RemoteStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
//builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.LogTo(action => Console.WriteLine(action), LogLevel.Error, Microsoft.EntityFrameworkCore.Diagnostics.DbContextLoggerOptions.SingleLine);
    options.UseNpgsql(builder.Configuration.GetConnectionString("SmartWeightDatabase"));
});

builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddAutoMapper(typeof(MapProfile));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 1024 * 1024 * 32;
    options.MaximumParallelInvocationsPerClient = 100;
});
builder.Services.AddScoped<FactoryManager>();
builder.Services.AddScoped<FactoryRepository>();
builder.Services.AddScoped<JenkinsClient>(conf =>
{
    var jenkinsConfiguration = new JenkinsConfig()
    {
        ApiKey = builder.Configuration
            .GetRequiredSection("Jenkins")
            .GetRequiredSection("ApiKey")
            .Get<string>(),
        Username = builder.Configuration
            .GetRequiredSection("Jenkins")
            .GetRequiredSection("UserName")
            .Get<string>(),
        JenkinsUrl = builder.Configuration
            .GetRequiredSection("Jenkins")
            .GetRequiredSection("Host")
            .Get<string>(),
    };

    var jenkinsClient = new JenkinsClient(jenkinsConfiguration);

    return jenkinsClient;
});
builder.Services.AddScoped<BuildRepository>();
builder.Services.AddScoped<NoteManager>();
builder.Services.AddScoped<NoteRepository>();
builder.Services.AddScoped<BuildManager>();
//builder.Services.AddDirectoryBrowser();
builder.Services.AddScoped<RemoteStorageProvider>();

builder.Services.AddScoped<RemoteBuilds>(build => builder.Configuration
                .GetRequiredSection("RemoteServer")
                .Get<RemoteBuilds>());
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "ngrokCrossOrigin",
        policy =>
        {
            policy.WithOrigins("https://*.ngrok.io")
                .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
});

var app = builder.Build();


#region инициализация юзеров и ролей

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
{
    UserManager<User> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
    RoleManager<Role> roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

    await ApplicationDbInitializer.SeedRoles(roleManager,
        app.Configuration.GetRequiredSection("Roles").Get<IList<string>>());
    await ApplicationDbInitializer.SeedUsers(userManager,
        app.Configuration.GetRequiredSection("Admin").GetRequiredSection("Email").Get<string>(),
        app.Configuration.GetRequiredSection("Admin").GetRequiredSection("Login").Get<string>(),
        app.Configuration.GetRequiredSection("Admin").GetRequiredSection("Password").Get<string>(),
        app.Configuration.GetRequiredSection("Admin").GetRequiredSection("Roles").Get<IList<string>>());

    ApplicationFolderInitializer.SeedFolders();
}
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    //app.UseSwagger();
    //app.UseSwaggerUI();
    //app.UseSwaggerUI(options =>
    //{
    //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    //    options.RoutePrefix = string.Empty;
    //});
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseRouting();







app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.UseAuthentication();

var buildPath = Path.Combine(builder.Environment.ContentRootPath, "builds");

app.MapHub<MessageHub>("/hub/smartweight");
app.UseStaticFiles();
app.UseAuthorization();
//new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(buildPath),
//    RequestPath = "/builds",
//}
app.Run();
