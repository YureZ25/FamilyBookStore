using Data;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataLayer();
builder.Services.AddServiceLayer();

builder.Services
    .AddControllersWithViews()
    .AddMvcOptions(opt =>
    {
        opt.MaxModelValidationErrors = 20;
        opt.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "���� ����������� ��� ����������");
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
app.UseAuthorization();

app.MapDefaultControllerRoute();

await app.RunMigrateDbStartupTaskAsync(app.Environment);

app.Run();
