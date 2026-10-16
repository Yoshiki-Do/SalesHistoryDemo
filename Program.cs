using SalesHistoryDemo.Data;
using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OracleDb");

using (var connection = new OracleConnection(connectionString))
{
    connection.Open();
    Console.WriteLine("Oracle Database connected successfully.");
}

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DatabaseManager>(serviceProvider =>
    new DatabaseManager(connectionString!));

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
