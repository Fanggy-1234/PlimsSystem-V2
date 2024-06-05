using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Plims.Data;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();


// VAREEWAN : Add for connect database
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("TU1Connection"))
    );
//builder.Services.AddDbContext<AppDbContext_2>(
//    options => options.UseSqlServer(builder.Configuration.GetConnectionString("TU2Connection"))
//    );
//builder.Services.AddDbContext<AppDbContext_3>(
//    options => options.UseSqlServer(builder.Configuration.GetConnectionString("TU3Connection"))
//    );

//VAREEWAN : Add for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1200);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "MySesstionAndCokkie";
});



// In the ConfigureServices method
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


//VAREEWAN : Add for User Authen
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("Full");
    });

    options.AddPolicy("ReadOnly", policy =>
    {
        policy.RequireRole("Read");
    });
    options.AddPolicy("No", policy =>
    {
        policy.RequireRole("No");
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
//VAREEWAN : Add for session
app.UseSession();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();

