// https://localhost:5001/
// user, user123
// admin, admin123


using MyApp.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Dodanie obsługi stron Razor
// builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<MyApp.Services.DatabaseService>();

//Dodanie obsługo sesji
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(300);
    options.Cookie.HttpOnly = true;//plik cookie jest niedostępny przez skrypt po stronie klienta
    options.Cookie.IsEssential = true;//pliki cookie sesji będą zapisywane dzięki czemu sesje będzie mogła być śledzona podczas nawigacji lub przeładowania strony
});

var app = builder.Build();
app.Services.GetService<MyApp.Services.DatabaseService>();

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



//Dodanie obsługo sesji
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseLoginRedirect("/Account/Login");


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//Dodanie obsługi stron Razor
//app.MapRazorPages();

app.Run();
