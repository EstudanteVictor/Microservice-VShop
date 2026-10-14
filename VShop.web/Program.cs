using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using VShop.web.Services;
using VShop.web.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("ProductAPI", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ServiceUri:ProductApi"]);
});

builder.Services.AddHttpClient("CartApi", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ServiceUri:CartApi"]);
});

builder.Services.AddHttpClient<ICouponService,CouponService>("DiscountApi", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ServiceUri:DiscountApi"]);
});

builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
}).AddCookie("Cookies", c =>
{
    c.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    c.Events = new CookieAuthenticationEvents()
    {
        OnRedirectToAccessDenied = (context) =>
        {
            context.HttpContext.Response.Redirect(builder.Configuration["ServiceUri:IdentityServer"] + "/Pages/Account/AccessDenied");
            return Task.CompletedTask;
        }
    };
})
    .AddOpenIdConnect("oidc", options =>
    {
        options.Events.OnRemoteFailure = context =>
        {
            context.Response.Redirect("/");
            context.HandleResponse();

            return Task.FromResult(0);
        };

        options.Authority = builder.Configuration["ServiceUri:IdentityServer"];
        options.GetClaimsFromUserInfoEndpoint = true;
        options.ClientId = "vshop";
        options.ClientSecret = builder.Configuration["Client:Secret"];
        options.ResponseType = "code";
        options.ClaimActions.MapJsonKey("role", "role", "role");
        options.ClaimActions.MapJsonKey("sub", "sub", "sub");
        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
        options.Scope.Add("vshop");
        options.SaveTokens = true;
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
