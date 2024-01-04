using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Session süresi için
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(10);
});

//JavaScript kaynaklý türkçe karakter sorunu için.
builder.Services.AddWebEncoders(o=>
{
    o.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(UnicodeRanges.All);
});

builder.Services.AddHttpContextAccessor(); //View sayfasýnda HttpContext property'sini kullanmak istiyorum. HttpContext property'si IHttpContextAccessor interface'inin bir elemaný. Bu elemana ulaþabilmek için IHttpContextAccessor servisini(=Interface'ini) burada register etmemiz lazým. Servis denilen þey bir class da olabilirdi. Ve bu class'ýn altýndaki bir metodu kullanabilmek için bu class'ý(=servisi) buraya register etmemiz lazým.

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
app.UseSession(); //to automatically enable session state
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
