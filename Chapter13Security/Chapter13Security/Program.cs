using Chapter13Security.Client.Pages;
using Chapter13Security.Components;
using Chapter13Security.Models;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7110/") });


var en = new Encript();
en.EncriptFile();


var provider = DataProtectionProvider.Create("MyApp");
var protector = provider.CreateProtector("ConfigurationProtector");
var encryptedConfig = File.ReadAllText("appsettings.encrypted.json");
var decryptedConfig = protector.Unprotect(encryptedConfig);
builder.Configuration.AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(decryptedConfig)));
builder.Services.AddDataProtection();
builder.Services.AddControllersWithViews();



var Configuration = JsonSerializer.Deserialize<Configuration>(decryptedConfig, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});


builder.Services.AddSingleton(Configuration);






// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Chapter13Security.Client._Imports).Assembly);


app.MapGet("/get-cookie-content", async context =>
  {
      var protector = app.Services.GetDataProtector("CookieProtector");
      if (!context.Request.Cookies.TryGetValue
              ("MyProtectedCookie", out var encryptedCookie))
      {
          // Encrypting the data
          var sensitiveData = "Sensitive Cookie Data";
          var encryptedData = protector.Protect(sensitiveData);
          // Setting encrypted cookie
          context.Response.Cookies.Append
                       ("MyProtectedCookie", encryptedData);
          await context.Response.WriteAsync
               (@"Cookie has been set. Click on button again to see the decrypted data.");
      }
      else
      {
          // Retrieving and decrypting the data
          var decryptedData = protector.Unprotect(encryptedCookie);
          await context.Response.WriteAsync
                       ($"Decrypted Cookie Data: {decryptedData}");
      }
  });

app.Run();



