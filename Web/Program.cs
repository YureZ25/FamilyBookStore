using Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using Services.ModelBinders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataLayer();
builder.Services.AddServiceLayer();

builder.Services.AddHttpContextAccessor();
builder.Services
    .AddControllersWithViews()
    .AddMvcOptions(opt =>
    {
        opt.MaxModelValidationErrors = 20;
        opt.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "Поле обязательно для заполнения");
        opt.ModelBinderProviders.Insert(0, new IsbnModelBinderProvider());
    })
    .AddViewOptions(opt =>
    {
        opt.HtmlHelperOptions.FormInputRenderMode = FormInputRenderMode.DetectCultureFromInputType;
    });

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Auth/Login";
        o.LogoutPath = "/Auth/Logout";
    });

builder.Services
    .AddIdentityCore<Data.Entities.User>(e =>
    {
        e.Password.RequireUppercase = false;
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
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

await app.RunMigrateDbStartupTask(app.Environment);

app.Run();
