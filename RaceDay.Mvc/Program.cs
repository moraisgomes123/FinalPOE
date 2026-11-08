var builder = WebApplication.CreateBuilder(args);

// ---------- Services ----------
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddHttpClient("RaceDayApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7271/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<RaceDay.Mvc.Services.RaceDayApiClient>();


// ---------- App ----------
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    var isProtected = path.StartsWith("/Organiser", StringComparison.OrdinalIgnoreCase)
                   || path.StartsWith("/Participant", StringComparison.OrdinalIgnoreCase);

    if (isProtected && string.IsNullOrEmpty(context.Session.GetString("Token")))
    {
        context.Response.Redirect("/Account/Login");
        return;
    }

    if (path.StartsWith("/Organiser", StringComparison.OrdinalIgnoreCase) && context.Session.GetString("Role") != "Organiser")
    {
        context.Response.Redirect("/Home/AccessDenied");
        return;
    }

    if (path.StartsWith("/Participant", StringComparison.OrdinalIgnoreCase) && context.Session.GetString("Role") != "Participant")
    {
        context.Response.Redirect("/Home/AccessDenied");
        return;
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();