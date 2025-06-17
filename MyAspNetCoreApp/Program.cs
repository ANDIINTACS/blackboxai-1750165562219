using Microsoft.EntityFrameworkCore;
using MyAspNetCoreApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register EmailSender service
builder.Services.AddTransient<MyAspNetCoreApp.Services.IEmailSender, MyAspNetCoreApp.Services.EmailSender>();

// Register ApplicationDbContext with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register INTSystemDbContext with SQL Server
builder.Services.AddDbContext<MyAspNetCoreApp.Data.INTSystemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("INTSystemConnection")));

// Add authentication services (cookie-based)
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Configure Kestrel to listen on all network interfaces on port 8000
app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:8000");

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }

    // Initialize the INT System database connection
    try
    {
        var intContext = services.GetRequiredService<MyAspNetCoreApp.Data.INTSystemDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Successfully connected to the INT System database.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Failed to initialize the Clients database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
