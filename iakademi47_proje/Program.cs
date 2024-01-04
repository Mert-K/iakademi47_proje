using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Session s�resi i�in
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(10);
});

//JavaScript kaynakl� t�rk�e karakter sorunu i�in.
builder.Services.AddWebEncoders(o=>
{
    o.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(UnicodeRanges.All);
});

builder.Services.AddHttpContextAccessor(); //View sayfas�nda HttpContext property'sini kullanmak istiyorum. HttpContext property'si IHttpContextAccessor interface'inin bir eleman�. Bu elemana ula�abilmek i�in IHttpContextAccessor servisini(=Interface'ini) burada register etmemiz laz�m. Servis denilen �ey bir class da olabilirdi. Ve bu class'�n alt�ndaki bir metodu kullanabilmek i�in bu class'�(=servisi) buraya register etmemiz laz�m.

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
